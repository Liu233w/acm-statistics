FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:16d0bf147784031d85040de10bd4b8c9dca2f634a7e19a85fd7bca8a63c987e1

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
