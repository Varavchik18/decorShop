using System;
using System.Collections.Generic;
using System.Linq;

namespace DecorStore.Domain.Exceptions
{
    public class DomainValidationException : Exception
    {
        public IEnumerable<DomainErrorCodes> ErrorCodes { get; }

        public DomainValidationException()
        {
        }

        public DomainValidationException(string message) : base(message)
        {
        }

        public DomainValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DomainValidationException(IEnumerable<DomainErrorCodes> errorCodes) : base(CreateMessage(errorCodes))
        {
            ErrorCodes = errorCodes;
        }

        public DomainValidationException(string message, IEnumerable<DomainErrorCodes> errorCodes) : base(message)
        {
            ErrorCodes = errorCodes;
        }

        private static string CreateMessage(IEnumerable<DomainErrorCodes> errorCodes)
        {
            return $"One or more validation errors occurred: {string.Join(", ", errorCodes.Select(e => e.ToString()))}";
        }
    }
}
