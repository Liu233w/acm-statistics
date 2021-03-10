FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:02530633fdaa704e7739e24df0570a7c046acf7a3acfff262e7772c01e115a54 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
