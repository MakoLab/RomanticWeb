using System;
using System.IO;
using System.Net;

namespace RomanticWeb.Net
{
    /// <summary>Mimics a <see cref="System.Net.WebResponse"/> like access to assembly embedded resources.</summary>
    public class StreamWebResponse:WebResponse
    {
        private Stream _fileStream;

        internal StreamWebResponse(Stream fileStream):base()
        {
            if (fileStream==null)
            {
                throw new ArgumentNullException("fileStream");
            }

            _fileStream=fileStream;
        }

        /// <summary>Gets a response stream with an embedded resource stream.</summary>
        /// <returns>An embedded resource stream.</returns>
        public override Stream GetResponseStream()
        {
             return _fileStream;
        }
    }
}