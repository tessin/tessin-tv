// @ts-check

const child_process = require("child_process");

const { platform } = require("os");

const is_linux = platform() === "linux";

function exec(command, options) {
  return new Promise((resolve, reject) => {
    child_process.exec(command, options, (err, stdout, stderr) => {
      process.stderr.write(stderr);
      if (err) {
        reject(err);
      } else {
        resolve(stdout.toString().trim());
      }
    });
  });
}

let getHostname;
let getCpuInfoRaw;
let getIfConfigRaw;

if (platform() === "linux") {
  getHostname = () => exec("hostname -I");
  getCpuInfoRaw = () => exec("cat /proc/cpuinfo");
  getIfConfigRaw = () => exec("ifconfig");
} else {
  // Windows fallback
  getHostname = () => exec("hostname");
  getCpuInfoRaw = () =>
    Promise.resolve(`processor       : 0
  model name      : ARMv7 Processor rev 4 (v7l)
  BogoMIPS        : 38.40
  Features        : half thumb fastmult vfp edsp neon vfpv3 tls vfpv4 idiva idivt vfpd32 lpae evtstrm crc32
  CPU implementer : 0x41
  CPU architecture: 7
  CPU variant     : 0x0
  CPU part        : 0xd03
  CPU revision    : 4
  
  processor       : 1
  model name      : ARMv7 Processor rev 4 (v7l)
  BogoMIPS        : 38.40
  Features        : half thumb fastmult vfp edsp neon vfpv3 tls vfpv4 idiva idivt vfpd32 lpae evtstrm crc32
  CPU implementer : 0x41
  CPU architecture: 7
  CPU variant     : 0x0
  CPU part        : 0xd03
  CPU revision    : 4
  
  processor       : 2
  model name      : ARMv7 Processor rev 4 (v7l)
  BogoMIPS        : 38.40
  Features        : half thumb fastmult vfp edsp neon vfpv3 tls vfpv4 idiva idivt vfpd32 lpae evtstrm crc32
  CPU implementer : 0x41
  CPU architecture: 7
  CPU variant     : 0x0
  CPU part        : 0xd03
  CPU revision    : 4
  
  processor       : 3
  model name      : ARMv7 Processor rev 4 (v7l)
  BogoMIPS        : 38.40
  Features        : half thumb fastmult vfp edsp neon vfpv3 tls vfpv4 idiva idivt vfpd32 lpae evtstrm crc32
  CPU implementer : 0x41
  CPU architecture: 7
  CPU variant     : 0x0
  CPU part        : 0xd03
  CPU revision    : 4
  
  Hardware        : BCM2709
  Revision        : a02082
  Serial          : 00000000cc7a99c9`);
  getIfConfigRaw = () =>
    Promise.resolve(`eth0      Link encap:Ethernet  HWaddr b8:27:eb:7a:99:c9
  inet6 addr: fe80::bc7f:6afb:1bba:7798/64 Scope:Link
  UP BROADCAST MULTICAST  MTU:1500  Metric:1
  RX packets:0 errors:0 dropped:0 overruns:0 frame:0
  TX packets:0 errors:0 dropped:0 overruns:0 carrier:0
  collisions:0 txqueuelen:1000
  RX bytes:0 (0.0 B)  TX bytes:0 (0.0 B)

lo        Link encap:Local Loopback
  inet addr:127.0.0.1  Mask:255.0.0.0
  inet6 addr: ::1/128 Scope:Host
  UP LOOPBACK RUNNING  MTU:65536  Metric:1
  RX packets:209 errors:0 dropped:0 overruns:0 frame:0
  TX packets:209 errors:0 dropped:0 overruns:0 carrier:0
  collisions:0 txqueuelen:1
  RX bytes:17176 (16.7 KiB)  TX bytes:17176 (16.7 KiB)

wlan0     Link encap:Ethernet  HWaddr b8:27:eb:2f:cc:9c
  inet addr:192.168.1.93  Bcast:192.168.1.255  Mask:255.255.255.0
  inet6 addr: fe80::d2b4:d1fc:da8a:1cf/64 Scope:Link
  UP BROADCAST RUNNING MULTICAST  MTU:1500  Metric:1
  RX packets:31757 errors:0 dropped:14940 overruns:0 frame:0
  TX packets:3058 errors:0 dropped:0 overruns:0 carrier:0
  collisions:0 txqueuelen:1000
  RX bytes:8362668 (7.9 MiB)  TX bytes:344017 (335.9 KiB)`);
}

class Utils {
  static async getHostID() {
    return {
      hostname: await getHostname()
    };
  }
}

module.exports = Utils;
