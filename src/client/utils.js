// @ts-check

function exec(command, options) {
  const child_process = require("child_process");
  return new Promise((resolve, reject) => {
    child_process.exec(command, options, (err, stdout, stderr) => {
      process.stderr.write(stderr);
      if (err) {
        reject(err);
      } else {
        resolve(stdout.toString().trim());
      }
    });
  });
}

function stat(fn) {
  const { statSync } = require("fs");
  try {
    return statSync(fn);
  } catch (err) {
    if (err.code === "ENOENT") {
      return false;
    }
    throw err;
  }
}

function deferred() {
  this._task = new Promise((resolve, reject) => {
    this.resolve = resolve;
    this.reject = reject;
  });

  const subscribers = new Set();

  this._task.then(
    function() {
      for (const [onfulfilled, _] of subscribers) {
        onfulfilled && onfulfilled(...arguments);
      }
    },
    function() {
      for (const [_, onrejected] of subscribers) {
        onrejected && onrejected(...arguments);
      }
    }
  );

  this._subscribers = subscribers;
}

deferred.prototype.task = function() {
  return this._task;
};

deferred.prototype.subscribe = function(onfulfilled, onrejected) {
  const subscription = [onfulfilled, onrejected];
  this._subscribers.add(subscription);
  return () => this._subscribers.delete(subscription);
};

/**
 * @param {number} milliseconds
 * @param {deferred=} cancellationToken
 */
function timeout(milliseconds, cancellationToken) {
  return new Promise((resolve, reject) => {
    let cancellationRegistration;
    const handle = setTimeout(() => {
      cancellationRegistration && cancellationRegistration();
      resolve();
    }, milliseconds); // resolve if timed out
    if (cancellationToken) {
      // this is a memory leak
      cancellationRegistration = cancellationToken.subscribe(() => {
        console.debug("timeout canceled");
        clearTimeout(handle);
        reject(new Error("operation was canceled")); // reject if canceled
      });
    }
  });
}

module.exports = { exec, stat, timeout, deferred };
