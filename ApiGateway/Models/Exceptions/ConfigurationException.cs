namespace ApiGateway.Models.Exceptions;

/// <summary>
/// Exception thrown when there is an issue with the configuration.
/// </summary>
public class ConfigurationException(string message) : Exception(message)
{
}
