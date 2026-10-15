using System;
using System.Collections.Generic;
using System.Text;

namespace ChurchAttendance.Core.ServiceContracts
{
    public interface ICurrentUserService
    {
        Guid? userId { get; }
    }
}
