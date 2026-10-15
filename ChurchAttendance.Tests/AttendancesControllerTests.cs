using ChurchAttendance.API.Controllers;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.ServiceContracts;
using ChurchAttendance.Core.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ChurchAttendance.Tests
{
    public class AttendancesControllerTests
    {
        [Fact]
        public async Task GetAttendances_ReturnsPagedResult()
        {
            // Arrange
            var attendanceServiceMock = new Mock<IAttendanceService>();
            var currentUserMock = new Mock<ICurrentUserService>();

            var paged = new PagedResult<AttendanceResponseDTO>
            {
                Items = new List<AttendanceResponseDTO>
                {
                    new AttendanceResponseDTO { AttendanceId = Guid.NewGuid(), UserId = Guid.NewGuid(), AttendanceDayId = Guid.NewGuid(), MarkedAt = DateTime.UtcNow }
                },
                Page = 1,
                PageSize = 10,
                TotalCount = 1
            };

            attendanceServiceMock.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(paged);

            var controller = new AttendancesController(attendanceServiceMock.Object, currentUserMock.Object);

            // Act
            var actionResult = await controller.GetAttendances(1, 10);

            // Assert
            var ok = Assert.IsType<ActionResult<PagedResult<AttendanceResponseDTO>>>(actionResult);
        }

        [Fact]
        public async Task GetAttendance_Forbid_When_NotOwnerOrAdmin()
        {
            // Arrange
            var attendanceServiceMock = new Mock<IAttendanceService>();
            var currentUserMock = new Mock<ICurrentUserService>();

            var attendance = new AttendanceResponseDTO { AttendanceId = Guid.NewGuid(), UserId = Guid.NewGuid(), AttendanceDayId = Guid.NewGuid() };

            attendanceServiceMock.Setup(s => s.GetByIdAsync(attendance.AttendanceId)).ReturnsAsync(attendance);
            currentUserMock.SetupGet(c => c.userId).Returns(Guid.NewGuid()); // different user

            var controller = new AttendancesController(attendanceServiceMock.Object, currentUserMock.Object);

            // Act
            var result = await controller.GetAttendance(attendance.AttendanceId);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }
}
