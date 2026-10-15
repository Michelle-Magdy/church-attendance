using ChurchAttendance.Core.DTOs.V1;

namespace ChurchAttendance.Core.ServiceContracts.V1;

public interface IV1AuthService
{
    Task<V1AuthResponseDto> RegisterAsync(V1RegisterDto dto);
    Task<V1AuthResponseDto> LoginAsync(V1LoginDto dto);
}
