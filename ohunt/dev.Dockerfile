FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:694931c049d7391d8a8b82e779693e52608a6869756c639dead867a7ac1b272f

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
