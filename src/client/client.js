// @ts-check

const puppeteer = require("puppeteer");
const { Browser } = require("puppeteer/lib/Browser");
const Page = require("puppeteer/lib/Page");

let bootHtmlFile = require.resolve(process.argv[2] || "./static/splash.html");

if (require("os").platform() === "linux") {
  bootHtmlFile = "file://" + bootHtmlFile;
}

const hostID = require("./host-id");
const hello = require("./hello");
const { getCommand, completeCommand } = require("./commands");
const watchDog = require("./watch-dog");

const { exec, stat, deferred, timeout } = require("./utils");

function setStatusText(textContent) {
  const el = document.getElementById("status-text");
  if (el) {
    el.textContent = textContent;
  }
}

class Client {
  constructor() {
    /** @type {{ hostname:string, serialNumber: string }} */
    this._hostID = null;
    /** @type {{ id: string, name:string, getCommandUrl: string }} */
    this._hello = null;
    /** @type {Browser} */
    this._browser = null;
    /** @type {Page} */
    this._page = null;
  }

  async hostID() {
    return (this._hostID = await hostID());
  }

  async hello() {
    return (this._hello = await hello(this._hostID));
  }

  async launch() {
    let executablePath;

    if (stat("/usr/bin/chromium-browser")) {
      executablePath = "/usr/bin/chromium-browser";
    }

    // @ts-ignore: false positive
    this._browser = await puppeteer.launch({
      executablePath,
      headless: false,
      args: [
        "--no-default-browser-check",
        "--no-first-run",
        "--disable-infobars", // hide "Chrome is being controlled by  ..."
        // "--incognito", // incognito mode is not support by puppeteer
        process.env.NODE_ENV !== "development" ? "--kiosk" : null
      ].filter(x => x)
    });

    // debug:
    for (const eventName of [
      // "disconnected",
      "targetchanged",
      "targetcreated",
      "targetdestroyed"
    ]) {
      this._browser.on(eventName, () => {
        console.debug("browser", eventName);
      });
    }
  }

  /**
   * @param {deferred} cancellation_token
   */
  async newPage(cancellation_token) {
    if (this._page) {
      await this._page.close();
      this._page = null;
    }

    /** @type {Page} */
    const page = await this._browser.newPage();

    // native Raspberry Pi resolution
    await page.setViewport({ width: 1920, height: 1080 });

    await page.goto(bootHtmlFile);

    try {
      await page.evaluate(
        setStatusText,
        JSON.stringify(
          Object.assign(
            {},
            {
              name: this._hello && this._hello.name
            },
            this._hostID,
            { version: process.env.npm_package_version }
          )
        )
      );
    } catch (err) {
      console.error("cannot set status text", err.message);
    }

    watchDog(this, page, cancellation_token); // fire and forget!

    this._page = page;
  }

  /**
   * @param {deferred=} cancellation_token
   */
  async messageLoop(cancellation_token) {
    for (;;) {
      const msg = await getCommand(this._hello.getCommandUrl);
      if (msg == null) {
        console.debug("command idle");
        try {
          await timeout(5 * 60 * 1000, cancellation_token);
        } catch (err) {
          return; // canceled
        }
        continue;
      }
      console.debug("command", msg.command);
      try {
        await this._processCommand(msg.command);
        completeCommand(msg.completeUrl);
        console.debug("command done");
      } catch (err) {
        console.error("command error", err.message);
      }
    }
  }

  async _processCommand(command) {
    switch (command.type) {
      case "goto": {
        await this._page.goto(command.url);
        break;
      }
      case "sudo": {
        await exec(
          `echo ${command.password} | sudo -u pi -S ${command.command}`
        );
        break;
      }
    }
  }
}

module.exports = Client;
