FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copia todos os arquivos do projeto para o container
COPY . .

# Restaura, compila e publica a aplicação diretamente
RUN dotnet publish HelpDeskApi.csproj -c Release -o /out

# Imagem final leve de execução
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "HelpDeskApi.dll"]