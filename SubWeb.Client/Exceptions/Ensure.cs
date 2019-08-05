using System;

namespace SubWeb.Client.Exceptions
{
    public class Ensure
    {
        public static void ArgumentNotEmpty(string value, string propName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException($"{nameof(propName)} cannot be empty");
        }
    }
}
