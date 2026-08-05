using GymManagementSystem.Api.Controllers;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GymManagementSystem.Tests.Controllers
{
    public class MembersControllerTests
    {
        [Fact]
        public async Task GetAll_Should_Return_Ok()
        {
            // Arrange

            var mock = new Mock<IMemberService>();
            var expectedMembers = new List<MemberDto>
            {
                new MemberDto
                {
                    FullName = "John Doe",
                    GymName = "Fitness Center",
                },
                new MemberDto
                {
                    FullName = "Jane Smith",
                    GymName = "Health Club",
                }
            };
            mock.Setup(c => c.GetAllMembersAsync()).ReturnsAsync(expectedMembers);


            var controller = new MembersController(mock.Object);

            // Act

            var result = await controller.GetAll();

            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result); // Check if the result is of type OkObjectResult

            mock.Verify(c => c.GetAllMembersAsync(), Times.Once()); // Verify that the GetAllMembersAsync method was called exactly once

            var returnedMembers = Assert.IsType<List<MemberDto>>(okResult.Value);
            Assert.Same(expectedMembers, returnedMembers); // Check if the returned value is the same as the expected members
        }



        [Fact]

        public async Task GetById_Should_Return_Ok()
        {
            // Arrange

            var mock = new Mock<IMemberService>();

            var expected = new MemberDetailsDto
            {
                Id = 1,
                FirstName = "John Doe",
                GymName = "Fitness Center",
            };
            mock.Setup(c => c.GetMemberByIdAsync(1)).ReturnsAsync(expected);
            var controller = new MembersController(mock.Object);


            // Act

            var result = await controller.GetById(1);

            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result); // Check if the result is of type OkObjectResult

            var returnedMember = Assert.IsType<MemberDetailsDto>(okResult.Value);

            Assert.Same(expected, returnedMember); // Check if the returned value is the same as the expected member

            mock.Verify(c => c.GetMemberByIdAsync(1), Times.Once()); // Verify that the GetMemberByIdAsync method was called exactly once
        }


        [Fact]
        public async Task Create_Should_Return_CreatedAtAction()
        {
            // Arrange
            var dto = new CreateMemberDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoe.gmail.com",
                PhoneNumber = "1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                GymId = 1
            };

            var createdMember = new MemberDetailsDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoe.gmail.com",
                PhoneNumber = "1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                JoinDate = DateTime.Now,
                GymId = 1,
                GymName = "Fitness Center"
            };

            var mock = new Mock<IMemberService>();
            mock.Setup(c => c.CreateMemberAsync(dto)).ReturnsAsync(createdMember);

            var MembersController = new MembersController(mock.Object);

            // Act

            var result = await MembersController.Create(dto);


            // Assert
            var CreatedResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedMember = Assert.IsType<MemberDetailsDto>(CreatedResult.Value);
            var routeValues = CreatedResult.RouteValues;
            var actionName = CreatedResult.ActionName;

            Assert.Same(createdMember, returnedMember);
            Assert.NotNull(routeValues);
            Assert.True(routeValues.ContainsKey("id"));
            Assert.Equal(createdMember.Id, routeValues["id"]);
            Assert.Equal(nameof(MembersController.GetById), actionName);
            mock.Verify(c => c.CreateMemberAsync(dto), Times.Once());



        }

        [Fact]
        public async Task Update_Should_Return_NoContent()
        {
            // Arrange
            var dto = new CreateMemberDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoe.gmail.com",
                PhoneNumber = "1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                GymId = 1
            };

            var mock = new Mock<IMemberService>();

            var controller = new MembersController(mock.Object);
            // Act

            var result = await controller.Update(1, dto);


            // Assert

            var noContent = Assert.IsType<NoContentResult>(result);
            mock.Verify(c => c.UpdateMemberAsync(1, dto), Times.Once());


        }

        [Fact]

        public async Task Delete_Should_Return_NoContent()
        {


            var mock = new Mock<IMemberService>();

            var controller = new MembersController(mock.Object);

            // Act

            var result = await controller.Delete(1);

            // Assert

            var noContent = Assert.IsType<NoContentResult>(result);
            mock.Verify(c => c.DeleteMemberAsync(1), Times.Once());


        }
    }
}
