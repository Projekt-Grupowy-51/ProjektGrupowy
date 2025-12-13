using System.Net;

namespace VidMark.Application.Exceptions
{
    public class ForbiddenException : Exception
    {
        public HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

        public ForbiddenException()
            : base("You do not have permission to perform this action.") { }

        public ForbiddenException(string message)
            : base(message) { }

        public ForbiddenException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
