# build application
FROM microsoft/aspnetcore-build:2.0 AS build-env

WORKDIR /app

COPY ./app/ ./

RUN dotnet test tests \
 && dotnet publish -c Release -o out

# build runtime image
FROM microsoft/aspnetcore:2.0

ENV ASPNETCORE_URLS=http://+:5000
ENV PING=http://localhost:5000/api/health
ENV USER=langbot
ENV GROUP=langbot
ENV UID=2000
ENV GID=2000
ENV HOME=/home/langbot

WORKDIR /app

RUN echo "add ${USER} user" \
 && groupadd --system --gid "${GID}" "${GROUP}" \
 && useradd --system --uid "${UID}" --home-dir "${HOME}" --create-home --gid "${GID}" "${USER}" \
 && echo "install packages" \
 && apt-get --quiet update \
 && apt-get install --yes --no-install-recommends \
      gosu \
 && rm -rf /var/lib/apt/lists/* \
 && echo "install tini" \
 && curl --silent --show-error --location --output /usr/local/bin/tini "https://github.com/krallin/tini/releases/download/v0.17.0/tini-amd64" \
 && echo "2ad381b0ff2ebd7a5b161c0d2d1a730ce419ed048b89d50acf1e9059a822961c /usr/local/bin/tini" | sha256sum --check - \
 && chmod +x /usr/local/bin/tini \
 && tini -s true

COPY --from=build-env /app/web/out ./
COPY scripts /usr/local/bin/

EXPOSE 5000

ENTRYPOINT ["/usr/local/bin/tini", "--"]

CMD ["run-root.sh"]

HEALTHCHECK --interval=30s --timeout=1s CMD run-health.sh || exit 1
