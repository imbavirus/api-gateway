namespace ApiGateway.Helper;

public static class Helper
{
    public static string getStatusColor (int StatusCode)
    {
        switch (StatusCode.ToString()[0]) {
            case '1':
                return "table-info"; // Blue for informational responses
            case '2':
                return "table-success"; // Green for success
            case '3':
                return "table-warning"; // Light blue for redirection
            case '4':
                return "table-danger"; // Red for error
            case '5':
                return "table-danger"; // Red for server error
            default:
                return "table-secondary"; 
        }
    }
}
