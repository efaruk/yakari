#!/bin/bash

docker start redis
sleep 5
docker start yakariweb0 yakariweb1 yakariweb2 yakariweb3 yakariweb4 yakariweb5
