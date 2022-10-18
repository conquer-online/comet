FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /usr/src/comet/

# Set argument defaults
ENV COMET_BUILD_CONFIG "release"
ARG COMET_BUILD_CONFIG=$COMET_BUILD_CONFIG

# Stage 1: Copy and build servers and dependencies. Builds all servers into a single server
# image, which the server containers use to run specific or multiple servers.
COPY . ./

RUN dotnet restore
RUN dotnet publish ./src/Comet.Account -c $COMET_BUILD_CONFIG -o out/Comet.Account
RUN dotnet publish ./src/Comet.Game -c $COMET_BUILD_CONFIG -o out/Comet.Game

# Stage 2: Copy the compiled server binaries from the previous stage and prepare an image to
# be tagged as a new release of Comet.
FROM mcr.microsoft.com/dotnet/runtime:6.0

WORKDIR /usr/bin/comet/
COPY --from=builder /usr/src/comet/out .
ENTRYPOINT ["dotnet"]
