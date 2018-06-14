# Tessin TV from Scratch

This document outlines how to prepare a Raspbian NOOBS gold master image.

## Recommended hardware

- Raspberry Pi 3 Model B
- Raspberry Pi 3 Model B+
- Micro USB B Power Supply 2.5A, 5.1V <sup>[1]</sup>
- OmniPi, Raspberry Pi (Black) unofficial case <sup>[2]</sup>

<sup>[1]</sup> note the amperage requirement! while you can boot a Raspberry Pi from almost any USB power source it won't be stable and you'll see a lightning indicator ⚡ in the top-right corner. This can also happen if you are running an old version of Raspbian NOOBS (i.e. SD cards with pre-installed Raspbian NOOBS can be out of date).

<sup>[2]</sup> this case is good. Has better airflow and enough space to install a small fan.

## Recommended software

- PiBakery
- Win32 Disk Imager

## Installation instructions

Estimated time 30 minutes - 1 hour.

Use PiBakery to create a initial image with some default configuration, for example, a Wi-Fi configuration (unless you want to connect your Pi's by wired ethernet). Do not use the lite installation, use the desktop installation of Raspbian NOOBS.

```sh
sudo apt-get update
sudo apt-get upgrade chromium-browser
```

This should upgrade your Raspberry Pi `chromium-browser` to version 65 (or later). You can verify this by running the command:

```sh
chromium-browser --version
```

While Node.js is pre-installed, the version is old. You need at least version 7. We recommend the latest LTS release and as of version 8, right now, this is:

- [https://nodejs.org/dist/v8.11.3/node-v8.11.3-linux-armv6l.tar.gz](https://nodejs.org/dist/v8.11.3/node-v8.11.3-linux-armv6l.tar.gz)
- [https://nodejs.org/dist/v8.11.3/node-v8.11.3-linux-armv7l.tar.gz](https://nodejs.org/dist/v8.11.3/node-v8.11.3-linux-armv7l.tar.gz)

You can use the command `uname -m` to check if your Raspberry Pi supports `armv6l` or `armv7l`. You can run `armv6l` on top of `armv7l` but not the other way around.

## `sudo tv`

```
#tessin-tv
tessin-tv ALL=(ALL) NOPASSWD: /sbin/shutdown -r +1
tessin-tv ALL=(ALL) NOPASSWD: /usr/bin/vcgencmd display_power 0
tessin-tv ALL=(ALL) NOPASSWD: /usr/bin/vcgencmd display_power 1
```
