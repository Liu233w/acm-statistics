FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:a2a8f968b043349b8faa0625c5405ac33da70b3274ff9e17109430f16aa9a3ee

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
