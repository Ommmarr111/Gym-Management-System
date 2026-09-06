using FluentValidation;
using FluentValidation.AspNetCore;
using GymManagementSystem.Api.Middleware;
using GymManagementSystem.Application.BackgroundJobs;
using GymManagementSystem.Application.BackgroundJobs.Interfaces;
using GymManagementSystem.Application.BackgroundJobs.Jobs;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Application.Mappings;
using GymManagementSystem.Application.Services;
using GymManagementSystem.Application.Validators;
using GymManagementSystem.Domain.Entities;
using GymManagementSystem.Infrastructure.Persistence;
using GymManagementSystem.Infrastructure.Repositories;
using GymManagementSystem.Infrastructure.Seeding;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace GymManagementSystem.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .WriteTo.Console(outputTemplate:
               "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog as the logging provider
            builder.Host.UseSerilog();


            Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

            if (!builder.Environment.IsEnvironment("Testing"))
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            // 2. JWT AUTHENTICATION CONFIGURATION
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });
            builder.Services.AddCors(options =>
            {

                options.AddPolicy("MyPolicy", policy =>
                {
                    //policy.WithOrigins("http://localhost:4200") for angular 
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();

                });
            });

            // Add Redis Cache for distributed caching

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
            });

            //  Rate Limiting Service
            builder.Services.AddRateLimiter(options =>
            {
                // Return standard 429 Too Many Requests when limits are hit
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // The callback triggered when a request is blocked
                options.OnRejected = async (context, cancellationToken) =>
                {
                    // 1. Set the content type to JSON
                    context.HttpContext.Response.ContentType = "application/json";

                    // 2. Attempt to read the Retry-After value if the limiter provides it
                    var retryAfter = TimeSpan.Zero;
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue))
                    {
                        retryAfter = retryAfterValue;
                    }

                    // 3. Construct a standardized error response object
                    var response = new
                    {
                        StatusCode = 429,
                        Error = "Too Many Requests",
                        Message = "You have exceeded the allowed request limit.",

                        // Calculate exact seconds if available, otherwise default to a message

                        RetryAfterSeconds = retryAfter.TotalSeconds > 0 ? (int)retryAfter.TotalSeconds : (int?)null

                    };

                    // 4. Serialize and write the response body
                    await context.HttpContext.Response.WriteAsync(
                        JsonSerializer.Serialize(response),
                        cancellationToken);
                };
                // GLOBAL POLICY: Token Bucket 
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 100,             // Maximum burst size
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0,               // Do not queue, reject immediately
                            ReplenishmentPeriod = TimeSpan.FromMinutes(1), // Refill every minute
                            TokensPerPeriod = 100,        // Add 100 tokens per minute
                            AutoReplenishment = true
                        }));

                // NAMED POLICY : Sliding Window for Auth endpoints
                options.AddSlidingWindowLimiter("StrictAuth", config =>
                {
                    config.PermitLimit = 5;               // Max 5 attempts
                    config.Window = TimeSpan.FromMinutes(1); // Per minute
                    config.SegmentsPerWindow = 3;         // Granularity of the slide
                    config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    config.QueueLimit = 0;
                });
            });

            // UNIT OF WORK CONFIGURATION
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add services to the container.
            builder.Services.AddScoped<IMembershipPlanService, MembershipPlanService>();
            builder.Services.AddScoped<IGymService, GymService>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Add the subscription expiry job to the DI container
            builder.Services.AddScoped<ISubscriptionExpiryJob, SubscriptionExpiryJob>();

            // Add Memory Cache for caching frequently accessed data
            builder.Services.AddMemoryCache();


            // EXCEPTION HANDLING
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            // ProblemDetails middleware for standardized error responses
            builder.Services.AddProblemDetails();

            // VALIDATION
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateMemberDtoValidator>();

            // HANGFIRE CONFIGURATION
            builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Add the Hangfire server to process jobs
            builder.Services.AddHangfireServer();


            // EMAIL SERVICE CONFIGURATION
            builder.Services.AddScoped<IEmailServiceJob, EmailServiceJob>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddAutoMapper(config =>
            {
                config.AddMaps(typeof(MappingProfile).Assembly);
            });
            builder.Services.AddSwaggerGen();
            var app = builder.Build();


            // Use Serilog request logging middleware to log HTTP requests and responses
            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("MyPolicy");

            app.UseRateLimiter();

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await RoleSeeder.SeedAsync(roleManager, userManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Error occured during processing");
                }
            }

            app.MapControllers();

            app.UseHangfireDashboard("/hangfire");
            RecurringJob.AddOrUpdate<ISubscriptionExpiryJob>("expire-overdue-subscriptions",
                job => job.ExpireOverdueSubscriptionsAsync(),
                Cron.Hourly);

            app.Run();
        }
    }
}
