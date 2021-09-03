FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:3f97be4c34f9133dfc6f61c418e7141b40f45815bb5c5573f52bce5ffc4842ab AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
