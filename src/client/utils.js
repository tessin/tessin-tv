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

if (platform() === "linux") {
  getHostname = () => exec("hostname -I");
  getCpuInfoRaw = () => exec("cat /proc/cpuinfo");
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
}

async function parseSerialNumber() {
  const cpu_info = await getCpuInfoRaw();
  const m = /[Ss]erial\s*:\s*([0-9A-Fa-f]+)/.exec(cpu_info);
  if (m) {
    return m[1].toLowerCase();
  }
  return null;
}

class Utils {
  static async getHostID() {
    return {
      hostname: await getHostname(),
      serialNumber: await parseSerialNumber()
    };
  }
}

module.exports = Utils;
