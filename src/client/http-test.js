const requestAsync = require("./request");

async function main() {
  console.log(
    "requestAsync",
    await requestAsync({ url: "http://localhost:8080/timeout", timeout: 5000 })
  );
}

main();
