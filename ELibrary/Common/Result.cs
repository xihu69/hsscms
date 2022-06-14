using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Common
{
    public class Result<T>:Result
    {
        public T Data { get; set; }
       
    }
    public class Result {
        public int Code { get; set; }
        public string Message { get; set; }

        public static Result<T> To<T>(string err = null, T data = default(T)) {
            if (string.IsNullOrEmpty(err))
                return new Result<T>() { Code = 1, Data = data };
            return new Result<T>() { Code = 0, Message = err, Data = data };
        }
        public static Result<T> To<T>(T data)
        {
            return new Result<T>() { Code = 1, Data = data };
        }
            public static Result To(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return new Result() { Code = 1 };
            return new Result() { Code = 0, Message = msg };
        }
    }
}
