using System;

namespace Chinchon.API
{
    public class Result<T> where T : class
    {
        private readonly T? _item;
        private readonly string? _error;

        private Result(in T item)
        {
            _item = item;
        }

        private Result(string error)
        {
            _error = error;
        }

        public static Result<T> Ok(in T item)
        {
            return new Result<T>(item);
        }

        public static Result<T> Error(string error)
        {
            return new Result<T>(error);
        }

        public U Match<U>(Func<T, U> success, Func<string, U> error)
        {
            return string.IsNullOrEmpty(_error)
                ? success(_item!)
                : error(_error);
        }
    }
}
