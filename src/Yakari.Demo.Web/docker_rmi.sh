#!/bin/bash

docker rmi $(docker images -a -f since=microsoft/dotnet --format {{.ID}})
