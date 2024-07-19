FROM mcr.microsoft.com/dotnet/runtime-deps

WORKDIR /app
COPY bin/Kooboo .

RUN chmod +x /app/Kooboo.Web
RUN mkdir -p /data

ENTRYPOINT ["/app/Kooboo.Web", "--appdata", "/data"]

LABEL description="In a few minutes, you can have your own website, whether it is a business website, an e-commerce site, or various internal applications within your organization."
VOLUME /data

EXPOSE 80
EXPOSE 443
