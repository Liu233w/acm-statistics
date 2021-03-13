FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:505e8c67f5f1c23cd156b38798e744d8bdd5b9ed98e40b489e45c5512eb4841b AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
