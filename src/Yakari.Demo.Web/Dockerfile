FROM microsoft/aspnetcore

# LABEL
LABEL author="efaruk" \
      version="1.0.0" \
      description="Yakari Demo image based microsoft/aspnetcore"

# Copy
COPY ./bin/Debug/netcoreapp1.0/publish /app

# Define
ENV ASPNETCORE_URLS http://+:5000 \
    ASPNETCORE_ENVIRONMENT="Development"

WORKDIR /app
EXPOSE 5000
ENTRYPOINT ["dotnet", "Yakari.Demo.Web.dll"]

