using System;

namespace TspResponder.Core.Internal
{
    /// <summary>
    /// Custom exception
    /// </summary>
    internal class TspException : Exception
    {
        internal TspException(string message) : base(message)
        {
        }

        internal TspException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}