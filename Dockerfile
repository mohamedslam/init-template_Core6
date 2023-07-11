FROM mcr.microsoft.com/dotnet/sdk:6.0 AS sdk
WORKDIR /build

ARG PACKAGE_REGISTRY_HOST=""
ARG PACKAGE_REGISTRY_USER=""
ARG PACKAGE_REGISTRY_PASSWORD=""

RUN if test -n "$PACKAGE_REGISTRY_HOST" && \
       test -n "$PACKAGE_REGISTRY_USER" && \
       test -n "$PACKAGE_REGISTRY_PASSWORD"; then \
        dotnet nuget add source "$PACKAGE_REGISTRY_HOST" \
            --name gitlab \
            --username ${PACKAGE_REGISTRY_USER} \
            --password ${PACKAGE_REGISTRY_PASSWORD} \
            --store-password-in-clear-text; \
    fi;

# Copy csproj and restore as distinct layers
COPY ./**/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore Fab.Web

# Copy everything else and build
COPY . ./
RUN dotnet publish Finance.Web -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
ARG RELEASE=""
ENV SENTRY__RELEASE=${RELEASE}

WORKDIR /app
COPY --from=sdk /build/out .

RUN apt-get update && apt-get install -y curl

HEALTHCHECK --interval=1m --timeout=10s --start-period=5s \
    CMD curl -sf http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "Fab.Web.dll"]
