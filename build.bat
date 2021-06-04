@echo off

dotnet build yakari-all.sln -c Release

dotnet publish .\src\Yakari.Demo.Web\Yakari.Demo.Web.csproj -c Release --verbosity minimal

@echo on