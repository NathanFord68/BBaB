using System;
using System.Runtime.Serialization;

namespace BBaB.Service.CrudExceptions.Exceptions
{
    [Serializable]
    public class RecordNotUpdatedException : Exception
    {
        public RecordNotUpdatedException()
        {
        }

        public RecordNotUpdatedException(string message) : base(message)
        {
        }

        public RecordNotUpdatedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecordNotUpdatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}