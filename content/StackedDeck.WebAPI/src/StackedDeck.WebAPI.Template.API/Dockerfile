FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apk add --no-cache icu-data-full=76.1-r1 icu-libs=76.1-r1

USER $APP_UID
COPY ["/publish", "."]
ENTRYPOINT ["dotnet", "StackedDeck.WebAPI.Template.API.dll"]
