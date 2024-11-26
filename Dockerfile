FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

WORKDIR /app

COPY . .

RUN dotnet publish ./ehr-csharp.sln -v q -c Release -r linux-x64 -o ./dist

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runner

WORKDIR /app

COPY --from=base ./app/dist .

EXPOSE 8080

USER 65534:65534 

CMD [ "dotnet", "./ehr-csharp.dll" ]
