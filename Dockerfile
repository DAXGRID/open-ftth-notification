FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./*sln ./

COPY ./src/OpenFTTH.Notification/*.csproj ./src/OpenFTTH.Notification/

RUN dotnet restore --packages ./packages

COPY . ./
WORKDIR /app/src/OpenFTTH.Notification
RUN dotnet publish -c Release -o out --packages ./packages

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app

COPY --from=build-env /app/src/OpenFTTH.Notification/out .
ENTRYPOINT ["dotnet", "OpenFTTH.Notification.dll"]

EXPOSE 8000