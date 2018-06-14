#!/usr/bin/env node

// @ts-check

const puppeteer = require("puppeteer");
const Page = require("puppeteer/lib/Page");
const ElementHandle = require("puppeteer/lib/ElementHandle");
const fs = require("fs");
const os = require("os");
const { parse, format } = require("url");

const utils = require("./utils");
const request = require("./http");

let splashHtmlFile = require.resolve("./static/splash.html");

if (os.platform() === "linux") {
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
      page.on("close", () => clearInterval(interval_id));
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

function set_status_text(textContent) {
  document.getElementById("div-1").textContent = textContent;
}

/**
 * @type {{id:string, name:string, getCommandUrl:string}}
 */
let hello_payload = null;

async function newPage(browser) {
  /** @type {Page} */
  let page = await browser.newPage();

  for (const event of ["close", "console", "error", "pageerror"]) {
    page.on(event, function() {
      console.debug("page", event, ...arguments);
    });
  }

  // native Raspberry Pi resolution
  await page.setViewport({ width: 1920, height: 1080 });

  await page.goto(splashHtmlFile);

  await page.waitFor("#div-1");

  try {
    await page.evaluate(
      set_status_text,
      JSON.stringify({
        name: hello_payload && hello_payload.name,
        ...(await utils.getHostID())
      })
    );
  } catch (err) {
    await page.evaluate(set_status_text, `error: ${err.message}`);
  }

  return page;
}

function addSecret(requestUrl) {
  const parsed = parse(requestUrl);

  parsed.query = Object.assign({}, parsed.query, {
    code: process.env.TESSIN_TV_SECRET
  });

  return format(parsed);
}

async function hello() {
  let protocol = "https";
  let port;

  const hostname = process.env.TESSIN_TV_HOST || "localhost";
  const secret = process.env.TESSIN_TV_SECRET;

  if (hostname == "localhost") {
    protocol = "http";
    port = 7071;
  }

  const url = format({
    protocol,
    hostname,
    port,
    pathname: "api/hello",
    query: { code: secret }
  });

  console.debug(">>>>", url);

  const res = await request({
    method: "POST",
    url,
    content: { hostID: await utils.getHostID() }
  });

  console.debug("hello", res.content);

  if (res.content.success) {
    hello_payload = res.content.payload;
  }
}

async function main() {
  await hello();

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

  let page = await newPage(browser);

  for (;;) {
    await watch_dog(page);

    console.error("page crashed...");

    await timeout(5000);

    const page2 = await newPage(browser);
    await page2.goto(splashHtmlFile);

    page.close();
    page = page2;
  }
}

function timeout(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

main(); // go!