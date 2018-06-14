#!/usr/bin/env node

// @ts-check

const Client = require("./client");

const { deferred } = require("./utils");

process.env.npm_package_version = require("./package.json").version;

async function main() {
  console.debug(process.env.npm_package_version);

  const client = new Client();

  const hostID = await client.hostID();

  console.debug("hostID", hostID);

  const hello = await client.hello();

  console.debug("hello", hello);

  if (hello) {
    const cancellation_token = new deferred();

    await client.launch();

    client._browser.on("disconnected", () => {
      // Chromium is closed or crashed or the browser.disconnect method was called
      cancellation_token.resolve();
    });

    await client.newPage(cancellation_token);

    // Keep splash screen open for 5 seconds...
    await client._page.waitFor(5000);

    await client.messageLoop(cancellation_token);
  } else {
    process.exitCode = 1;
  }
}

console.log(process.argv);

main();
