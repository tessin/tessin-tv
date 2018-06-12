// @ts-check

const { parse } = require("url");

/**
 * @returns {Promise<{statusCode: number, statusMessage: string, headers: {[key: string]: string}, content: any}>}
 */
function requestAsync({ method, url, headers, content, ...rest }) {
  const { protocol, hostname, port, path } = parse(url);

  let request;

  switch (protocol) {
    case "http:": {
      request = require("http").request;
      break;
    }
    case "https:": {
      request = require("https").request;
      break;
    }
    default: {
      throw new RangeError(`unsupported protocol '${protocol}'`);
    }
  }

  const headers2 = Object.assign(
    {
      ["Accept"]: "application/json"
    },
    headers,
    content
      ? {
          ["Content-Length"]: content.length
        }
      : null
  );

  return new Promise((resolve, reject) => {
    const options = {
      hostname,
      port,
      method,
      path,
      headers: headers2,
      ...rest
    };
    // console.error("request", options);
    const req = request(options, res => {
      res.setEncoding("utf8");

      let content2 = "";

      res.on("data", chunk => {
        content2 += chunk;
      });

      res.on("end", () => {
        resolve({
          statusCode: res.statusCode,
          statusMessage: res.statusMessage,
          headers: res.headers,
          content:
            res.headers["content-type"] === "application/json" ||
            res.headers["content-type"] === "application/json; charset=utf-8"
              ? JSON.parse(content2)
              : content2
        });
      });
    });

    req.on("socket", socket => {
      socket.on("timeout", () => {
        req.abort();
      });
    });

    req.on("error", err => reject(err));

    if (content) {
      req.write(content);
    }

    req.end();
  });
}

/**
 * @param {number} x
 */
function variance(x) {
  var y = Math.random();
  var z = x + (y * (x - 2 * x)) / 2;
  return z;
}

const retry_policy = [1, 3, 9, 27, 81, 243]; // 3^1, 3^2, 3^n...

/**
 * @param {number} seconds
 */
function timeoutAsync(seconds) {
  return new Promise(resolve => {
    setTimeout(resolve, 1000 * seconds);
  });
}

/**
 * @param {{method: string, url: string, headers: {[key: string]: string}, content: any}} req
 * @returns {Promise<{statusCode: number, statusMessage: string, headers: {[key: string]: string}, content: any}>}
 */
async function requestWithRetryAsync(req) {
  let res;
  for (let i = 0; i < retry_policy.length; i++) {
    // console.error("request with retry", i);
    if (0 < i) {
      await timeoutAsync(variance(retry_policy[i]));
    }
    try {
      res = await requestAsync(req);
      if (res.statusCode === 503) {
        continue;
      }
      break;
    } catch (err) {
      // console.error("request with retry", err.code);
      if (err.code === "ECONNRESET" && i + 1 < retry_policy.length) {
        continue;
      }
      throw err;
    }
  }
  return res;
}

module.exports = requestWithRetryAsync;
