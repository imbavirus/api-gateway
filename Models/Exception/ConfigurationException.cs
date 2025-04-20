using System;

namespace ApiGateway.Models.Exceptions
{
    /// <summary>
    /// Exception thrown when there is an issue with the configuration.
    /// </summary>
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message) { }
    }
}
