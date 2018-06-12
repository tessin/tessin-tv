const { parse } = require("url");
const http = require("http");

const http_server = http.createServer((req, res) => {
  const { path, query } = parse(req.url);

  console.log("<<<<", path);

  switch (path) {
    case "/503":
      res.statusCode = 503;
      res.write("service unavailable");
      res.end();
      return;
    case "/timeout":
      setTimeout(() => res.end(), 5 * 60 * 1000); // 5 min
      return;
    case "/destroy":
      res.destroy();
      return;
  }

  res.statusCode = 404;
  res.write("not found");
  res.end();
});

http_server.listen(8080);
