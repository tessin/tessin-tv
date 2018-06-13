#!/usr/bin/env node

// @ts-check

const Client = require("./client");

async function main() {
  const client = new Client();

  const hostID = await client.hostID();

  console.debug("hostID", hostID);

  const hello = await client.hello();

  console.debug("hello", hello);

  if (hello) {
    await client.launch();
    await client.newPage();
    await client._page.waitFor(5000); // keep splash screen open for 5 seconds...
    await client.messageLoop();
  } else {
    process.exitCode = 1;
  }
}

main();
