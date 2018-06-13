using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TessinTelevisionServer
{
    public static class Async
    {
        public static AsyncLazy<T> Lazy<T>(Func<Task<T>> taskFactory)
        {
            return new AsyncLazy<T>(taskFactory);
        }
    }

    public class AsyncLazy<T>
    {
        private readonly Lazy<Task<T>> _lazy;

        public bool IsValueCreated => _lazy.IsValueCreated;

        public AsyncLazy(Func<Task<T>> taskFactory)
        {
            _lazy = new Lazy<Task<T>>(() => Task.Factory.StartNew(() => taskFactory()).Unwrap());
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return _lazy.Value.GetAwaiter();
        }
    }
}
