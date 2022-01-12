FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:90b566b141a8e2747f2805d9e4b2935ce09040a2926a1591c94108a83ba10309

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
