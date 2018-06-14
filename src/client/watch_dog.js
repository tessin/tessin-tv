// @ts-check

const Client = require("./client");

const { timeout } = require("./utils");

function t() {
  const [seconds, _] = process.hrtime();
  return seconds;
}

class WatchDog {
  /**
   * @param {Client} client
   */
  constructor(client) {
    this._client = client;
  }

  async watch() {
    let tm1 = t();
    const interval = setInterval(() => {
      const dt = t() - tm1;
      console.error("watch-dog", "heartbeat", dt);
      if (!(dt <= 5000)) {
        this._client.newPage(); // this will close page and clear interval
      }
    });
    try {
      for (; !this._client._page.isClosed(); ) {
        await this._client._page.metrics();
        tm1 = t();
        await timeout(2500);
      }
    } catch (err) {
      console.error("watch-dog", err.message);
    } finally {
      clearInterval(interval);
    }
  }
}

module.exports = WatchDog;
