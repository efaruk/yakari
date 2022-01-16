rm -r ./docker/publish

dotnet build yakari-all.sln -c Release

dotnet publish ./src/Yakari.Demo.Web/Yakari.Demo.Web.csproj -c Release --verbosity minimal -o ./docker/publish

cd docker

docker build -t efaruk/yakariweb .

cd ..
