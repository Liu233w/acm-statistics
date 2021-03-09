FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:61c2c92dbe1ac953d2b89dcf4bc0034f3bdc1c54e8f724c8603b00c9dce546be AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
