# Etapa 1 — Build + Test
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia a solução e os projetos para restaurar dependências
COPY *.sln ./
COPY UsersVehicles-Api/*.csproj ./UsersVehicles-Api/
COPY Teste-UsersVehicles-Api/*.csproj ./Teste-UsersVehicles-Api/
RUN dotnet restore
# Baixa os pacotes para compilar

COPY . .
# Copia todo o código do projeto

# Rodar testes antes de publicar
RUN dotnet build Teste-UsersVehicles-Api/Teste-UsersVehicles-Api.csproj -c Release
RUN dotnet test Teste-UsersVehicles-Api/Teste-UsersVehicles-Api.csproj -c Release --no-build --verbosity normal
# Garante que todos os testes passem antes de continuar
# Pode usar --filter Category!=Integration se quiser pular testes de integração

WORKDIR /src/UsersVehicles-Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false /p:TrimUnusedDependencies=true
# Publish prepara tudo para deployar, o -o PATH indica a localização onde ficar os arquivos
# com UseAppHost false ele gera apenas a DLL, sem .exe
# TrimUnusedDependencies remove assemblies não usados para reduzir tamanho da imagem

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
# Copia apenas os arquivos publicados, sem incluir projetos de teste ou SDK

ENV ASPNETCORE_URLS=http://+:5027
EXPOSE 5025
ENTRYPOINT ["dotnet", "UsersVehicles-Api.dll"]
#Executa o código compilado