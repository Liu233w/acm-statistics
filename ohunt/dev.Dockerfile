FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:0fece15a102530aa2dad9d247bc0d05db6790917696377fc56a8465604ef1aff AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
