
# cec-client

By default, Raspbian does not come with cec-client, so you need to install it first:

```
$ apt-get install libcec3 cec-utils
```

Turning your monitor off:

```
$ echo 'standby 0' | cec-client -s -d 1
```

Turning your monitor on:

```
$ echo 'on 0' | cec-client -s -d 1
```

Set the Raspberry Pi as input as active (i.e. toggle the TV to switch input):

```
$ echo 'as' | cec-client -s -d 1
```

Set the Raspberry Pi input as an inactive input:

```
$ echo 'is' | cec-client -s -d 1
```

You can learn more about what you can do with CEC by running:

```
$ echo h | cec-client -s -d 1
[...]
```
    
You can also find more at CEC-O-Matic and in the libcec faq.

## References

- https://www.screenly.io/blog/2017/07/02/how-to-automatically-turn-off-and-on-your-monitor-from-your-raspberry-pi/
- http://www.cec-o-matic.com/
- http://libcec.pulse-eight.com/faq