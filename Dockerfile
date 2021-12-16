FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app
COPY . .

WORKDIR /app/src
RUN dotnet build -c Release

WORKDIR /app/src/GitVersion.App
RUN dotnet publish -c Release -f net6.0 --no-build

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS final
RUN apt-get -y update
RUN apt-get -y install git
RUN mkdir -p /opt/gitversion
ENV PATH="/opt/gitversion:${PATH}"
COPY --from=build /app/src/GitVersion.App/bin/Release/net6.0/publish /opt/gitversion