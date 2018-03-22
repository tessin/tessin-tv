const puppeteer = require("puppeteer");

const path = require("path");

const panicHtmlFile = path.resolve("./test/panic.html");

// configure using a TXT record for tv.tessin.local otherwise fallback to host.txt

async function main() {
  const browser = await puppeteer.launch({
    headless: false,
    args: [
      "--disable-infobars" // hide "Chrome is being controlled by ..."
    ]
  });

  const page = await browser.newPage();

  page.on("error", err => {
    console.error("page error", err);
  });

  // must clear interval, otherwise node will not exit
  setInterval(async () => {
    console.log(await page.metrics()); // monitor page health
  }, 3000);

  await page.goto(panicHtmlFile);
}

main(); // go!
