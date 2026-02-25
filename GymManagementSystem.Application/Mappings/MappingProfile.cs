using AutoMapper;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Member Mappings 
            CreateMap<Member, MemberDetailsDto>()
                .ForMember(dest => dest.GymName,
                    opt => opt.MapFrom(src => src.Gym != null ? src.Gym.Name : "No Gym Assigned"));

            CreateMap<Member, MemberDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.GymName,
                    opt => opt.MapFrom(src => src.Gym != null ? src.Gym.Name : "No Gym Assigned"));

            CreateMap<CreateMemberDto, Member>()
                .ForMember(dest => dest.JoinDate, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Gym, opt => opt.Ignore());

            // Gym Mappings
            CreateMap<Gym, GymDto>();

            CreateMap<CreateGymDto, Gym>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Plans, opt => opt.Ignore());

            CreateMap<UpdateGymDto, Gym>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Plans, opt => opt.Ignore());

            // Subscription Mappings
            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(dest => dest.MemberName,
                    opt => opt.MapFrom(src => src.Member != null
                        ? $"{src.Member.FirstName} {src.Member.LastName}"
                        : "Unknown"))
                .ForMember(dest => dest.PlanName,
                    opt => opt.MapFrom(src => src.MembershipPlan != null ? src.MembershipPlan.Name : "Unknown"))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.AmountPaid))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ToString("yyyy-MM-dd")));

            CreateMap<CreateSubscriptionDto, Subscription>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.StartDate, opt => opt.Ignore())    // Set in service
                .ForMember(dest => dest.EndDate, opt => opt.Ignore())      // Calculated in service
                .ForMember(dest => dest.AmountPaid, opt => opt.Ignore())   // Set from plan price
                .ForMember(dest => dest.Status, opt => opt.Ignore())       // Set in service
                .ForMember(dest => dest.Member, opt => opt.Ignore())
                .ForMember(dest => dest.MembershipPlan, opt => opt.Ignore());

            // MembershipPlan Mappings
            CreateMap<MembershipPlan, MembershipPlanDto>()
                .ForMember(dest => dest.GymName,
                    opt => opt.MapFrom(src => src.Gym != null ? src.Gym.Name : "No Gym Assigned"));

            CreateMap<CreateMembershipPlanDto, MembershipPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Gym, opt => opt.Ignore());

            CreateMap<UpdateMembershipPlanDto, MembershipPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.GymId, opt => opt.Ignore())        // Don't change gym
                .ForMember(dest => dest.Gym, opt => opt.Ignore());

            // Attendance Mappings
            CreateMap<Attendance, AttendanceDto>()
                .ForMember(dest => dest.MemberName,
                    opt => opt.MapFrom(src => src.Member != null
                        ? $"{src.Member.FirstName} {src.Member.LastName}"
                        : "Unknown"))
                .ForMember(dest => dest.GymName,
                    opt => opt.MapFrom(src => src.Gym != null ? src.Gym.Name : "Unknown"))
                .ForMember(dest => dest.CheckInTime,
                    opt => opt.MapFrom(src => src.CheckInTime.ToString("yyyy-MM-dd hh:mm tt")));

            CreateMap<CheckInDto, Attendance>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CheckInTime, opt => opt.Ignore())  // Set in service
                .ForMember(dest => dest.Member, opt => opt.Ignore())
                .ForMember(dest => dest.Gym, opt => opt.Ignore());

            // ====================== Payment Mappings ======================
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.PaymentDate,
                    opt => opt.MapFrom(src => src.PaymentDate.ToString("yyyy-MM-dd")));
        }

    }
}