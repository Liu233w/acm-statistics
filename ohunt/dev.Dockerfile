FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:129e2b121141804a84797d3eefb4d5eb9d28f42df040dc098d788f91302322ab AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
