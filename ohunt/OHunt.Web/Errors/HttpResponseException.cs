using System;

namespace OHunt.Web.Errors
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(object value, int status = 500)
        {
            Value = value;
            Status = status;
        }

        public int Status { get; }

        public object Value { get; }
    }
}
