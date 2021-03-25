FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:5fe5fbee989f30fef0024024856bccf2abee4745ecd301bc06df315493c9bb55 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
