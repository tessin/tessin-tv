// @ts-check

const request = require("./http");

function authorize(url) {
  const { parse, format } = require("url");

  const parsed = parse(url);

  parsed.query = Object.assign({}, parsed.query, {
    code: process.env.TESSIN_TV_SECRET
  });

  return format(parsed);
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

  if (res.content.success) {
    return res.content.payload;
  }

  console.error("<<<<", res.content);

  return null;
}

function completeCommand(url) {
  return request({
    method: "POST",
    url: authorize(url)
  });
}

module.exports = { getCommand, completeCommand };
