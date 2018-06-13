const Utils = require("./utils");

async function main() {
  console.debug(await Utils.getHostID());
}

main();
