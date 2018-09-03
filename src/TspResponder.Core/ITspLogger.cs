namespace TspResponder.Core
{
    /// <summary>
    /// A simple logging for the <see cref="ITimeStampResponder"/>
    /// </summary>
    public interface ITimeStampLogger
    {
        /// <summary>
        /// Log a message for Debug level
        /// </summary>
        /// <param name="message">message</param>
        void Debug(string message);

        /// <summary>
        /// Log a message for Warn level
        /// </summary>
        /// <param name="message">message</param>
        void Warn(string message);

        /// <summary>
        /// Log a message for Error level
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);
    }
}
