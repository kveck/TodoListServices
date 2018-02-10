namespace ToDoListServices.Common.ErrorHandling
{
    using System;
    using Microsoft.Extensions.Options;

    public static class Guard
    {
        public static void NotNull<T>(T value, string name) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }

        public static void NotNull<T>(IOptions<T> value, string name) where T : class, new()
        {
            if (value == null)
                throw new ArgumentNullException(name);

            if (value.Value == null)
                throw new ArgumentNullException(name);
        }

        public static void NotNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Parameter {name} cannot be null or empty");
        }

        public static void NotEmpty(Guid value, string name)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"Parameter {name} cannot be an empty guid");
        }
    }
}
