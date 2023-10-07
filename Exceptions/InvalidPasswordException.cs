using System;

namespace mongodb_dotnet_example.Exceptions
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException(string message) : base(message)
        {
        }
    }
    public class InvalidUserException : Exception
    {
        public InvalidUserException(string message) : base(message)
        {
        }
    }
    public class InvalidTrainException : Exception
    {
        public InvalidTrainException(string message) : base(message)
        {
        }
    }
    public class InvalidDateException : Exception
    {
        public InvalidDateException(string message) : base(message)
        {
        }
    }
}
