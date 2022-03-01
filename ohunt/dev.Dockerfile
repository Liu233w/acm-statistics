FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:70b890cd12f73f8ad80061d242081b61da666bda7ec2d729113855a8b9410e1e

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
