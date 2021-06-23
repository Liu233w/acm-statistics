FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:b9a35bf953e470d58bc28fb689020033bc0bf60c93fe4d45a1664096f2c6fbdc AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
