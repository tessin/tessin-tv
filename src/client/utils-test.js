const { execAsync } = require("./utils");

execAsync("ipconfig").then(console.log);
