using System;

namespace FollowUP.Core.Domain
{
    public abstract class FollowUPException : Exception
    {
        public string Code { get; }

        protected FollowUPException()
        {
        }

        protected FollowUPException(string code)
        {
            Code = code;
        }

        protected FollowUPException(string message, params object[] args) : this(string.Empty, message, args)
        {
        }

        protected FollowUPException(string code, string message, params object[] args) : this(null, code, message, args)
        {
        }

        protected FollowUPException(Exception innerException, string message, params object[] args)
            : this(innerException, string.Empty, message, args)
        {
        }

        protected FollowUPException(Exception innerException, string code, string message, params object[] args)
            : base(string.Format(message, args), innerException)
        {
            Code = code;
        }
    }
}
