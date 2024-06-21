FROM defradigital/dotnetcore:dotnet8.0 AS base
ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080

FROM defradigital/dotnetcore-development:dotnet8.0 AS build
COPY --chown=dotnet:dotnet ["EPR.RegulatorService.Facade.API/EPR.RegulatorService.Facade.API.csproj", "EPR.RegulatorService.Facade.API/"]
COPY --chown=dotnet:dotnet ["EPR.RegulatorService.Facade.Core/EPR.RegulatorService.Facade.Core.csproj", "EPR.RegulatorService.Facade.Core/"]
RUN dotnet restore "EPR.RegulatorService.Facade.API/EPR.RegulatorService.Facade.API.csproj"

COPY --chown=dotnet:dotnet EPR.RegulatorService.Facade.API/. ./EPR.RegulatorService.Facade.API/.
COPY --chown=dotnet:dotnet EPR.RegulatorService.Facade.Core/. ./EPR.RegulatorService.Facade.Core/.

WORKDIR "/home/dotnet/EPR.RegulatorService.Facade.API"
RUN dotnet build "EPR.RegulatorService.Facade.API.csproj" -c Release -o /home/dotnet/build

FROM build AS publish
RUN dotnet publish "EPR.RegulatorService.Facade.API.csproj" -c Release -o /home/dotnet/publish

FROM base AS final

# Switch to the non-root user
USER dotnet

COPY --from=publish /home/dotnet/publish .
ENTRYPOINT ["dotnet", "EPR.RegulatorService.Facade.API.dll"]
