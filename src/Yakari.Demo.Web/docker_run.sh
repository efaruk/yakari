#!/bin/bash

docker run --name yakariweb0 -d -p 5000:5000 --link redis:redis efaruk/yakariweb
docker run --name yakariweb1 -d -p 5001:5000 --link redis:redis efaruk/yakariweb
docker run --name yakariweb2 -d -p 5002:5000 --link redis:redis efaruk/yakariweb
docker run --name yakariweb3 -d -p 5003:5000 --link redis:redis efaruk/yakariweb
docker run --name yakariweb4 -d -p 5004:5000 --link redis:redis efaruk/yakariweb
docker run --name yakariweb5 -d -p 5005:5000 --link redis:redis efaruk/yakariweb
docker ps
