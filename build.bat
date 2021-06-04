@echo off

dotnet build yakari-all.sln

dotnet publish .\src\QuickFit.Server\QuickFit.Server.csproj -c $BUILD_CONFIGURATION --verbosity minimal -o .\docker\publish



@echo on