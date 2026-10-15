using ChurchAttendance.API.Controllers;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Domain.Entities;
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
    public class AttendanceDaysControllerTests
    {
        [Fact]
        public async Task GetAttendanceDays_ReturnsPaged()
        {
            var svcMock = new Mock<IAttendanceDayService>();
            var paged = new PagedResult<AttendanceDayResponseDTO>
            {
                Items = new List<AttendanceDayResponseDTO> {
                    new AttendanceDayResponseDTO { AttendanceDayId = Guid.NewGuid(), Date = DateTime.UtcNow, IsOpen = true }
                },
                Page = 1,
                PageSize = 10,
                TotalCount = 1
            };

            svcMock.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(paged);

            var controller = new AttendanceDaysController(svcMock.Object);

            var result = await controller.GetAttendanceDays(1, 10);

            var ok = Assert.IsType<ActionResult<PagedResult<AttendanceDayResponseDTO>>>(result);
        }

        [Fact]
        public async Task GetAttendanceDay_ReturnsNotFound_WhenNull()
        {
            var svcMock = new Mock<IAttendanceDayService>();
            svcMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((AttendanceDayResponseDTO?)null);

            var controller = new AttendanceDaysController(svcMock.Object);

            var result = await controller.GetAttendanceDay(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
