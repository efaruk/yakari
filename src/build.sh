#!/bin/bash

echo "Restoring packages"
echo "..................................................................................."
dotnet restore **/project.json
echo "Building Projects"
echo "..................................................................................."
dotnet build **/project.json
echo "Testing"
echo "..................................................................................."
dotnet test Yakari.Tests/
