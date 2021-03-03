FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY MessageConsumer/ MessageConsumer/
COPY RabbitMQ.QueueCommunication/ RabbitMQ.QueueCommunication/
COPY Utilities/ Utilities/
WORKDIR /src/MessageConsumer
RUN dotnet restore  MessageConsumer.csproj

FROM build AS publish
RUN dotnet publish MessageConsumer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
EXPOSE 5020
ENTRYPOINT ["dotnet", "MessageConsumer.dll"]