FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:8a628b0b3b26e8827dca4113c36d9594a0a5bc2214fd7bae46eb5b48f7b677ba AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
