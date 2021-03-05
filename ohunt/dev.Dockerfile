FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:0aabc099920afb17b14995cb0499b84116f1f4bee8f953f79cb4192d7912e71c AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
