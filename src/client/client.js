// @ts-check

const puppeteer = require("puppeteer");
const { Browser } = require("puppeteer/lib/Browser");
const Page = require("puppeteer/lib/Page");

let splashHtmlFile = require.resolve("./static/splash.html");

if (require("os").platform() === "linux") {
  splashHtmlFile = "file://" + splashHtmlFile;
}

const hostID = require("./host-id");
const hello = require("./hello");
const { getCommand, completeCommand } = require("./commands");

const { stat, timeout } = require("./utils");

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
  }

  async newPage() {
    if (this._page) {
      await this._page.close();
      this._page = null;
    }

    const page = await this._browser.newPage();



    page.on("close", () => {
      // clean up
    });

    // native Raspberry Pi resolution
    await page.setViewport({ width: 1920, height: 1080 });

    await page.goto(splashHtmlFile);

    await page.waitFor("#div-1");

    try {
      await page.evaluate(
        textContent =>
          (document.getElementById("div-1").textContent = textContent),
        JSON.stringify(
          Object.assign(
            {},
            {
              name: this._hello && this._hello.name
            },
            this._hostID
          )
        )
      );
    } catch (err) {
      await page.evaluate(set_status_text, `error: ${err.message}`);
    }

    this._page = page;
  }

  async messageLoop() {
    for (;;) {
      const msg = await getCommand(this._hello.getCommandUrl);
      if (msg == null) {
        console.debug("command idle");
        await timeout(5 * 60 * 1000);
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
    }
  }
}

module.exports = Client;
