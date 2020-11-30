FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:429afb9737e249e01278b49e0aec112e7af89f71355545cedcaf86475a48998c AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
