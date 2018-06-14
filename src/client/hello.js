//@ts-check

const { format } = require("url");

const request = require("./request");

/**
 * @param {{ hostname: string, serialNumber: string }} hostID
 */
async function hello(hostID) {
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
    content: { hostID }
  });

  if (res.content.success) {
    return res.content.payload;
  }

  console.error("<<<<", res.content);

  return null;
}

module.exports = hello;
