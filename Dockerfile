FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./*sln ./

COPY ./src/OpenFTTH.NotificationServer/*.csproj ./src/OpenFTTH.NotificationServer/

RUN dotnet restore --packages ./packages

COPY . ./
WORKDIR /app/src/OpenFTTH.NotificationServer
RUN dotnet publish -c Release -o out --packages ./packages

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app

COPY --from=build-env /app/src/OpenFTTH.NotificationServer/out .
ENTRYPOINT ["dotnet", "OpenFTTH.NotificationServer.dll"]

EXPOSE 8000