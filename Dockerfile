FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["AuthenticationServer.API.csproj", "."]
RUN dotnet restore "./AuthenticationServer.API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AuthenticationServer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthenticationServer.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY *.p8 ./
COPY wait-for.sh ./
RUN chmod 755 ./wait-for.sh
RUN sed -i 's/\r$//' ./wait-for.sh
RUN apt update && \
    apt install netcat -y
ENTRYPOINT ["dotnet", "AuthenticationServer.API.dll"]
