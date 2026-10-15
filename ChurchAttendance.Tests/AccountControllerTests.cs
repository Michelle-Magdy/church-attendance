using ChurchAttendance.API.Controllers;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Services;
using ChurchAttendance.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ChurchAttendance.Tests
{
    public class AccountControllerTests
    {
        [Fact]
        public async Task PostRegister_ReturnsOk_OnSuccess()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(MockBehavior.Loose,
                null, null, null, null, null, null, null, null);
            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(MockBehavior.Loose,
                userManagerMock.Object, null, null, null, null, null);
            var roleManagerMock = new Mock<RoleManager<ApplicationRole>>(MockBehavior.Loose,
                null, null, null, null, null);
            var jwtMock = new Mock<IJWTService>();

            var registerDto = new RegisterDTO
            {
                PersonName = "Test",
                PhoneNumber = "0912345678",
                Password = "Password@1"
            };

            userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            signInManagerMock.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), false, null))
                .Returns(Task.CompletedTask);
            jwtMock.Setup(j => j.CreateJWTTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new AuthenticationResponseDTO { Token = "t", RefreshToken = "r" });

            var controller = new AccountController(userManagerMock.Object, signInManagerMock.Object, roleManagerMock.Object, jwtMock.Object);

            // Act
            var result = await controller.PostRegister(registerDto);

            // Assert
            var ok = Assert.IsType<ActionResult<AuthenticationResponseDTO>>(result);
        }
    }
}
