#=====================================================================================================================================
# Args
ARG VERSION=8.0

#=====================================================================================================================================
# Compile Stage
FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS build
WORKDIR /src

# Args
ARG LINUXARQ=linux-x64
ARG PROJECT_NAME="CustomerPlatform.Worker"

# Copy build configuration files
COPY /nuget.config /config/Nuget.config

# Copy source code structure
COPY /src/ /src/

# Restore dependencies
WORKDIR "/src/${PROJECT_NAME}"
RUN dotnet restore --runtime ${LINUXARQ} --configfile "/config/Nuget.config" "${PROJECT_NAME}.csproj"

# Build application
RUN dotnet build "${PROJECT_NAME}.csproj" -c Release -o /app/build

#=====================================================================================================================================
# Publish Stage
FROM build AS publish
RUN dotnet publish "${PROJECT_NAME}.csproj" -c Release -r ${LINUXARQ} -o /app/publish --self-contained false

#=====================================================================================================================================
# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS base
ADD https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb /

# Environment and dependencies
ARG VERSION_CORE
ENV APP_VERSION=$VERSION_CORE
RUN dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get upgrade -y \
    && apt-get install -y libmsquic=1.9* \
    && rm -rf /var/lib/apt/lists/* \
    && rm packages-microsoft-prod.deb

WORKDIR /app

#=====================================================================================================================================
# Runtime Image
FROM base AS final

# Create non-root user for security (NetToolsKit pattern)
RUN addgroup --group gnet --gid 2000 \
    && adduser --uid 1000 --gid 2000 unet \
    && chown unet:gnet /app

# Switch to non-root user
USER unet:gnet

# Copy published application
COPY --chown=unet:gnet --from=publish /app/publish .

# Environment configuration
ENV DOTNET_EnableDiagnostics=0
ENV DOTNET_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "CustomerPlatform.Worker.dll"]