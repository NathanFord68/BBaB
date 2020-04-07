using System;
using System.Runtime.Serialization;

namespace BBaB.Service.CrudExceptions.Exceptions
{
    [Serializable]
    public class RecordNotDeletedException : Exception
    {
        public RecordNotDeletedException()
        {
        }

        public RecordNotDeletedException(string message) : base(message)
        {
        }

        public RecordNotDeletedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecordNotDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}