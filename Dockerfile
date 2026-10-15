# ── Stage 1: restore & build ──────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first — this layer is cached unless a .csproj changes
COPY ChurchAttendance.slnx                                                      ./
COPY ChurchAttendance.API/ChurchAttendance.API.csproj                           ChurchAttendance.API/
COPY ChurchAttendance.Core/ChurchAttendance.Core.csproj                         ChurchAttendance.Core/
COPY ChurchAttendance.Infrastructure/ChurchAttendance.Infrastructure.csproj     ChurchAttendance.Infrastructure/

RUN dotnet restore ChurchAttendance.API/ChurchAttendance.API.csproj

# Copy the full source then publish
COPY ChurchAttendance.API/            ChurchAttendance.API/
COPY ChurchAttendance.Core/           ChurchAttendance.Core/
COPY ChurchAttendance.Infrastructure/ ChurchAttendance.Infrastructure/

RUN dotnet publish ChurchAttendance.API/ChurchAttendance.API.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

# ── Stage 2: runtime image ────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Non-root user for security
RUN addgroup --system appgroup \
 && adduser  --system --ingroup appgroup appuser

COPY --from=build /app/publish .
RUN chown -R appuser:appgroup /app

USER appuser

# Listen on 8080 (non-privileged port, no HTTPS inside container — terminate TLS at the proxy)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ChurchAttendance.API.dll"]
