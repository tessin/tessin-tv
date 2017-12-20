#!/bin/bash

FUNC_HOST=tessin-raspi.azurewebsites.net
FUNC_CODE=wt1nf81dnShiaZ2DznD1iR44a64VikIdB%2FyWPzRYpzlByQwNg2DVpQ%3D%3D
FUNC_TICK="https://$FUNC_HOST/api/pi/hosts/$(hostname)/tick?code=$FUNC_CODE"

curl $FUNC_TICK -o tock.sh
chmod u+x tock.sh
./tock.sh
