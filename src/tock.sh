#!/bin/bash

FUNC_HOST=tessin-raspi.azurewebsites.net
FUNC_CODE=wt1nf81dnShiaZ2DznD1iR44a64VikIdB%2FyWPzRYpzlByQwNg2DVpQ%3D%3D
FUNC_HEARTBEAT="https://$FUNC_HOST/api/pi/hosts/$(hostname)/heartbeat?code=$FUNC_CODE"
FUNC_AUTOSTART="https://$FUNC_HOST/api/pi/hosts/$(hostname)/autostart?code=$FUNC_CODE"

# heartbeat

/opt/vc/bin/vcgencmd measure_temp | curl --silent -X POST -d @- $FUNC_HEARTBEAT

# autostart

curl --fail --silent -F "autostart=@.config/lxsession/LXDE-pi/autostart" $FUNC_AUTOSTART -o /tmp/autostart
if [ $? -eq 0 ]; then
	cp /tmp/autostart .config/lxsession/LXDE-pi/autostart
	sudo reboot
fi
