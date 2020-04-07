using System;
using System.Runtime.Serialization;

namespace BBaB.Service.CrudExceptions.Exceptions
{
    [Serializable]
    public class RecordNotCreatedException : Exception
    {
        public RecordNotCreatedException()
        {
        }

        public RecordNotCreatedException(string message) : base(message)
        {
        }

        public RecordNotCreatedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecordNotCreatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}