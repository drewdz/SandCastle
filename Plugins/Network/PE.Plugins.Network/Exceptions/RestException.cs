﻿using System;
using System.Net;

namespace PE.Plugins.Network.Exceptions
{
    public class RestException : Exception
    {
        #region Constructors

        public RestException()
            : base()
        {
        }

        public RestException(string message)
            : base(message)
        {
        }

        public RestException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public RestException(string message, Exception exception)
            : base(message, exception)
        {
        }

        public RestException(string message, HttpStatusCode statusCode, Exception exception)
            : base(message, exception)
        {
            StatusCode = statusCode;
        }

        #endregion Constructors

        #region Properties

        public HttpStatusCode StatusCode { get; set; }

        #endregion Properties
    }
}
