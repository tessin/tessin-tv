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
        resolve(stdout);
      }
    });
  });
}

class Utils {
  /** @returns Promise<string> */
  static getHostIPAddressList() {
    if (is_linux) {
      return exec("hostname -I"); // list of uplink IP addresses
    } else {
      return exec("hostname"); // Windows fallback
    }
  }
}

module.exports = Utils;
