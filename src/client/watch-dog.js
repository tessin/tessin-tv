// @ts-check

const Page = require("puppeteer/lib/Page");

const Client = require("./client");
const { deferred, timeout } = require("./utils");

function t() {
  const [seconds, _] = process.hrtime();
  return seconds;
}

const WATCH_DOG_TARGET = 5000;
const WATCH_DOG_SAMPLE_RATE = WATCH_DOG_TARGET / 2;
const WATCH_DOG_LIMIT = 2 * WATCH_DOG_TARGET;

/**
 * @param {Client} client
 * @param {Page} page
 * @param {deferred} cancellation_token
 */
async function watchDog(client, page, cancellation_token) {
  let tm1 = process.hrtime();

  let interval = null;
  interval = setInterval(() => {
    const [seconds, nanoseconds] = process.hrtime(tm1);
    const dt = 1000 * seconds + nanoseconds / (1000 * 1000);
    // console.debug("watch-dog", "heartbeat", dt);
    if (!(dt <= WATCH_DOG_LIMIT)) {
      console.debug("watch-dog", "crash");
      if (interval) {
        clearInterval(interval);
        interval = null;
      }
      client.newPage(cancellation_token); // this will close page and clear interval
    }
  }, WATCH_DOG_TARGET);

  try {
    for (; !page.isClosed(); ) {
      await page.metrics();
      tm1 = process.hrtime();
      await timeout(WATCH_DOG_SAMPLE_RATE, cancellation_token);
    }
  } catch (err) {
    console.error("watch-dog", err.message);
  } finally {
    if (interval) {
      clearInterval(interval);
      interval = null;
    }
  }
}

module.exports = watchDog;
