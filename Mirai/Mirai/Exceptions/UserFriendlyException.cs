using Mirai.Domain.Enum;

namespace Mirai.Exceptions
{
    public class UserFriendlyException : Exception
    {
        public int StatusCode { get; set; }
        public string MessageCode { get; set; }
        public string UserFriendlyMessage { get; set; }
        public ErrorCode ErrorCode { get; set; }

        public UserFriendlyException(
            ErrorCode errorCode,
            string userFriendlyMessage,
            string messageCode,
            int statusCode = 400,
            Exception? innerException = null
        ) : base(userFriendlyMessage, innerException)
        {
            ErrorCode = errorCode;
            UserFriendlyMessage = userFriendlyMessage;
            MessageCode = messageCode;
            StatusCode = statusCode;
        }
    }
}
