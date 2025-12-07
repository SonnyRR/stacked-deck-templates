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

# You might get some SAST warnings if you use cloud security scanning
# tools like 'Orca Security' for detected usage of 'wget' with unencrypted
# URls. You can safely ignore these warnings, since they are false positives.
# This instruction runs from inside of the container engine & it's a
# common practice to use 'HTTP' for these checks. Otherwise you'll have
# to import/trust DEV SSL certificates as part of this document, which
# I heavily discourage.
#
# - Vasil Kotsev
HEALTHCHECK --interval=34s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --spider --tries=1 http://localhost:8080/sd-api-route-prefix/health

USER $APP_UID
COPY ["/publish", "."]
ENTRYPOINT ["dotnet", "StackedDeck.WebAPI.Template.API.dll"]
