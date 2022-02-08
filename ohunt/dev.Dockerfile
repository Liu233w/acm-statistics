FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:329f54fde64e1ce6ea4dd3a17192cc0c97aee394836c5a322751a6d78db511e4

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
