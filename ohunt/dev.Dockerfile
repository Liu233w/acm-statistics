FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:15c22c170650b8db2f6250547a2dc5341978b0647c6b21ef67768e628de614f3

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
