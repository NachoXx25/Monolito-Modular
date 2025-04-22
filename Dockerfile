FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /App
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Monolito-Modular.csproj", "./"]
RUN dotnet restore "Monolito-Modular.csproj"
COPY . .
RUN dotnet build "Monolito-Modular.csproj" -c Release -o /App/build


RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"


COPY .env .
RUN dotnet ef database update -c UserContext
RUN dotnet ef database update -c AuthContext
RUN dotnet ef database update -c BillContext

FROM build AS publish
RUN dotnet publish "Monolito-Modular.csproj" -c Release -o /App/publish

FROM base AS final
WORKDIR /App
COPY --from=publish /App/publish .
COPY .env .
ENTRYPOINT ["dotnet", "Monolito-Modular.dll"]