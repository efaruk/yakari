#!/bin/bash

docker run --name yakariweb0 -d -p 8880:80 efaruk/yakari:web
docker run --name yakariweb1 -d -p 8881:80 efaruk/yakari:web
docker run --name yakariweb2 -d -p 8882:80 efaruk/yakari:web
docker run --name yakariweb3 -d -p 8883:80 efaruk/yakari:web
docker run --name yakariweb4 -d -p 8884:80 efaruk/yakari:web
docker run --name yakariweb6 -d -p 8885:80 efaruk/yakari:web

