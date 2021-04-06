FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:84cf7fdb35c9d2498db2292528af42bfdc4a34add689086abaed8dac4623d5d5 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
