// @ts-check

const request = require("./request");

function authorize(url) {
  const { parse, format } = require("url");

  const parsed = parse(url, true);

  // console.debug("authorize", "url", url);
  // console.debug("authorize", "parsed.query", parsed.query);
  // console.debug("authorize", "TESSIN_TV_SECRET", process.env.TESSIN_TV_SECRET);

  Object.assign(parsed.query, {
    code: process.env.TESSIN_TV_SECRET
  });

  delete parsed.search; // otherwise the original 'search' value is used

  const authorized = format(parsed);

  // console.debug("authorize", "authorized", authorized);

  return authorized;
}

/**
 * @param {string} url
 * @returns {Promise<{ command: any, completeUrl: string }>}
 */
async function getCommand(url) {
  const res = await request({
    method: "POST",
    url: authorize(url)
  });

  const content = res.content;

  if (content.success) {
    return content.payload;
  } else if (content.errorCode === "TV_COMMAND_QUEUE_IS_EMPTY") {
    return null;
  }

  console.error("<<<<", content);

  return null;
}

async function completeCommand(url) {
  const res = await request({
    method: "POST",
    url: authorize(url)
  });

  if (res.statusCode !== 200) {
    console.error("<<<<", res);
    return;
  }

  const content = res.content;

  if (!content.success) {
    console.error("<<<<", content);
    return;
  }
}

module.exports = { getCommand, completeCommand };
