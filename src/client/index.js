#!/usr/bin/env node

// @ts-check

const puppeteer = require("puppeteer");
const fs = require("fs");
const os = require("os");

let panicHtmlFile = require.resolve("./test/panic.html");
let splashHtmlFile = require.resolve("./test/splash.html");

if (os.platform() === "linux") {
  panicHtmlFile = "file://" + panicHtmlFile;
  splashHtmlFile = "file://" + splashHtmlFile;
}

// configure using a TXT record for tv.tessin.local otherwise fallback to host.txt

function watch_dog(page) {
  return new Promise(async (resolve, reject) => {
    let heart_freq = 2500;
    let heart_beat = Date.now();
    let heart_stopped = false;

    function callback() {
      const heart_skip = Date.now() - heart_beat;
      if (heart_freq < heart_skip) {
        resolve(true);
      }
    }

    let interval_id = setInterval(callback, 2 * heart_freq);
    try {
      for (;;) {
        await page.waitFor(heart_freq / 2);
        await page.metrics(); //(await page.metrics()).JSHeapUsedSize;
        heart_beat = Date.now();
      }
    } catch (err) {
      reject(err);
    } finally {
      clearInterval(interval_id);
    }
  });
}

function stat(fn) {
  try {
    return fs.statSync(fn);
  } catch (err) {
    if (err.code === "ENOENT") {
      return false;
    }
    throw err;
  }
}

async function main() {
  let executablePath;

  if (stat("/usr/bin/chromium-browser")) {
    executablePath = "/usr/bin/chromium-browser";
  }

  const browser = await puppeteer.launch({
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

  for (const event of [
    "disconnected",
    "targetchanged",
    "targetcreated",
    "targetdestroyed"
  ]) {
    browser.on(event, function() {
      console.log("browser", event);
    });
  }

  let page = await browser.newPage();

  await page.setViewport({ width: 1920, height: 1080 }); // native resolution for Raspberry Pi

  await page.goto(splashHtmlFile);

  for (;;) {
    for (const event of ["close", "console", "error", "pageerror"]) {
      page.on(event, function() {
        console.log("page", event);
      });
    }

    await watch_dog(page);

    console.error("page crashed...");

    await timeout(5000);

    const page2 = await browser.newPage();
    await page2.goto(splashHtmlFile);

    page.close();
    page = page2;
  }
}

function timeout(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

main(); // go!
