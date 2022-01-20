FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:e656fe19bc303356acdbfec4695cb2bdd42c7a30677a8775a15eed7fd88edf60

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
