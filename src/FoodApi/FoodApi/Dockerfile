FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

# 👇 CSProj joyiga moslab yozing:
RUN dotnet publish src/FoodApi/FoodApi/FoodApi.csproj -c Release -o /out

FROM base
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet","FoodApi.dll"]
