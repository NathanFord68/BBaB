using System;
using System.Runtime.Serialization;

namespace BBaB.Service.Exceptions
{
    [Serializable]
    public class TransactionFailedException : Exception
    {
        public TransactionFailedException()
        {
        }

        public TransactionFailedException(string message) : base(message)
        {
        }

        public TransactionFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TransactionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}