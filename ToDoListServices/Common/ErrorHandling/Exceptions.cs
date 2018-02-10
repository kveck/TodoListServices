namespace ToDoListServices.Common.ErrorHandling
{
    using System;

    public class TodoServicesException : Exception
    {
        public TodoServicesException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        public TodoServicesException(string msg)
            : base(msg)
        {
        }
    }

    public class ItemNotExistsException : TodoServicesException
    {
        public ItemNotExistsException(int itemId, Exception innerException, string msg="")
            : base(msg + $" Item [id={itemId}] does not exist", innerException)
        {
        }

        public ItemNotExistsException(int itemId, string msg="")
            : base(msg + $" Item [id={itemId}] does not exist")
        {
        }
    }

    public class InvalidStatusException : TodoServicesException
    {
        public InvalidStatusException(string invalidValue, string msg="")
            : base(msg + $" [invalidValue={invalidValue}]")
        {
        }

        public InvalidStatusException(string invalidValue, Exception innerException, string msg = "")
            :base(msg + $" [invalidValue={invalidValue}]", innerException)
        {
        }
    }
}
