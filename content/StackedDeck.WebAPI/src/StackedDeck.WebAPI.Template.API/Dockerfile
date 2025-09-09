FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apk add --no-cache icu-data-full icu-libs

USER $APP_UID
COPY ["/publish", "."]
ENTRYPOINT ["dotnet", "StackedDeck.WebAPI.Template.API.dll"]
