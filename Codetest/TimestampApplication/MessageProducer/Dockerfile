FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY MessageProducer/ MessageProducer/
COPY RabbitMQ.QueueCommunication/ RabbitMQ.QueueCommunication/
COPY Utilities/ Utilities/
WORKDIR /src/MessageProducer
RUN dotnet restore  MessageProducer.csproj

FROM build AS publish
RUN dotnet publish MessageProducer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
EXPOSE 5021
ENTRYPOINT ["dotnet", "MessageProducer.dll"]