FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:a3dd4dee05cd1369014244d03b28b602e6a2e1650210dd8633322e00379471ec

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
