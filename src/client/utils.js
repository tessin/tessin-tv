// @ts-check

function exec(command, options) {
  const child_process = require("child_process");
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

function stat(fn) {
  const { statSync } = require("fs");
  try {
    return statSync(fn);
  } catch (err) {
    if (err.code === "ENOENT") {
      return false;
    }
    throw err;
  }
}

function timeout(milliseconds) {
  return new Promise(resolve => setTimeout(resolve, milliseconds));
}

module.exports = { exec, stat, timeout };
