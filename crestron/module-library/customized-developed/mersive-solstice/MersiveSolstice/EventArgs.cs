using System;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Http;

namespace MersiveSolstice
{
    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs()
        {
        }

        public CustomEventArgs(Solstice _object)
        {
            _display = _object;
        }

        public Solstice _display;
    }

    public class CustomExceptionArgs : EventArgs
    {
        public CustomExceptionArgs()
        {
        }

        public CustomExceptionArgs(UrlParserException _object)
        {
            _error = _object;
        }

        public UrlParserException _error;
    }
}