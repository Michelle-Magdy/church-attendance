# ChurchAttendance – what changed

Copy the files in this folder over the same paths in your solution, then apply the steps below.

## 1. Delete these files (replaced or no longer needed)

ChurchAttendance.API/Controllers/AttendanceDaysController.cs
ChurchAttendance.API/Controllers/AttendancesController.cs
ChurchAttendance.API/Validators/AddAttendanceDTOValidator.cs
ChurchAttendance.API/Validators/RegisterDTOValidator.cs
ChurchAttendance.Core/DTOs/AddAttendanceDTO.cs
ChurchAttendance.Core/DTOs/AttendanceDayResponseWithUserStatusDTO.cs
ChurchAttendance.Core/DTOs/RegisterDTO.cs

## 2. Secrets (do not keep them in appsettings.json)

    cd ChurchAttendance.API
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your connection string>"
    dotnet user-secrets set "JWTSettings:Key" "<random string, 32+ characters>"
    dotnet user-secrets set "SeedAdmin:PhoneNumber" "01012345678"
    dotnet user-secrets set "SeedAdmin:Password" "<a strong password>"
    dotnet user-secrets set "SeedAdmin:PersonName" "Administrator"

On the server use environment variables instead, e.g. `ConnectionStrings__DefaultConnection`, `JWTSettings__Key`, `SeedAdmin__PhoneNumber`.

## 3. Packages

    dotnet restore

The Core project now references QuestPDF (printable QR page).

## 4. Database

No model changes, so no new migration is needed.

## 5. Tests

ChurchAttendance.Tests is not part of the solution and already referenced a controller that does not exist
(AccountController), so it is untouched and will not compile against the new controllers. It needs rewriting.
