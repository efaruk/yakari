FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80

COPY \
    ./publish ./

ENTRYPOINT ["/app/Yakari.Demo.Web"]

EXPOSE 80
