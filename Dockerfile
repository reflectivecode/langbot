# build application
FROM microsoft/dotnet:2.2-sdk-alpine AS build-env

WORKDIR /app

COPY ./app/ ./

RUN dotnet test tests \
 && dotnet publish -c Release -o out

# build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime-alpine

ENV ASPNETCORE_URLS=http://+:5000
ENV PING=http://localhost:5000/api/health
ENV USER=langbot
ENV GROUP=langbot
ENV UID=2000
ENV GID=2000
ENV HOME=/home/langbot

WORKDIR /app

RUN apk add --no-cache \
      su-exec \
      tini \
 && addgroup -g "${GID}" -S "${GROUP}" \
 && adduser -u "${UID}" -D -S -G "${GROUP}" "${USER}"

COPY --from=build-env /app/web/out ./
COPY scripts /usr/local/bin/

EXPOSE 5000

ENTRYPOINT ["/sbin/tini", "--"]

CMD ["run-root.sh"]

HEALTHCHECK --interval=60s --timeout=1s CMD run-health.sh || exit 1
