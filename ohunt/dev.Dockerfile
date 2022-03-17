FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:8dcc61c6c6f42d4c38c07dadb58a78c6c2f729842846c39e9e978c1fb06f51b3

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
