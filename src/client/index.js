#!/usr/bin/env node

// @ts-check

const puppeteer = require("puppeteer");

const panicHtmlFile = require.resolve("./test/panic.html");
const splashHtmlFile = require.resolve("./test/splash.html");

// configure using a TXT record for tv.tessin.local otherwise fallback to host.txt

function watch_dog_process(page) {
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

async function main() {
  const browser = await puppeteer.launch({
    headless: false,
    args: [
      "--no-default-browser-check",
      "--no-first-run",
      "--disable-infobars", // hide "Chrome is being controlled by  ..."
      // "--incognito",
      // process.env.NODE_ENV !== "development" ? "--start-fullscreen" : null,
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

  console.log("viewport", await page.viewport());
  console.log(
    "viewport",
    await page.setViewport({ width: 1920, height: 1080 }) // native resolution for Raspberry Pi
  );

  await page.goto(splashHtmlFile);

  for (;;) {
    const events = ["close", "console", "error", "pageerror"];

    for (const event of events) {
      page.on(event, function() {
        console.log("page", event);
      });
    }

    await watch_dog_process(page);

    console.error("crashed...");

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
