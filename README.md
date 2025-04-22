# API Gateway (.NET 9)

A simple yet effective API Gateway built with .NET 9, designed primarily to create contacts on the Less Annoying CRM (LACRM) API from call data provided to the API Gateway and add a note to the contact with call information.

## Features

*   Creates a contact on LACRM from phonecall data provided to the API Gateway.
*   Adds a note to the contact with call information.
*   Swagger page for easily making api calls.
*   Web page that lists all calls made to the LACRM api.

## Getting Started

Follow these instructions to get the project up and running on your local machine for development and testing.

### Prerequisites

*   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
*   A suitable IDE (e.g., [Visual Studio 2022](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), [JetBrains Rider](https://www.jetbrains.com/rider/))
*   [Git](https://git-scm.com/)
*   Access to a Less Annoying CRM (LACRM) account and generated API credentials (User Code and API Token).

### Installation & Configuration

1.  **Clone the repository:**
    ```bash
    git clone <your-repository-url>
    cd ApiGateway # Or your specific project directory name
    ```

2.  **Configure Application Settings:**
    *   This project uses `appsettings.json`, environment variables, and user secrets for configuration.
    *   Rename or copy `.env.example` to `.env`.
    *   Update `.env` with your api key from LACRM:
    ```
    LACRM_API_KEY=YOUR_API_KEY_HERE
    ```

3.  **Restore Dependencies:**
    (Usually happens automatically with `dotnet run` or when opening in an IDE, but can be done manually)
    ```bash
    dotnet restore
    ```

### Running the Application

1.  **Using the .NET CLI:**
    ```bash
    dotnet run --project .\ApiGateway
    ```
    (This will typically build and run the project using the `Development` environment configuration)

2.  **Using an IDE:**
    *   Open the solution (`.sln`) or project (`.csproj`) file in Visual Studio or Rider.
    *   Press the "Run" button (often F5).
    *   For VS Code, ensure you have the C# Dev Kit extension installed and use the Run and Debug panel.

The API Gateway should now be running, typically on `http://localhost:5132` as configured in `Properties/launchSettings.json`. Check the console output for the exact URLs.
The Swagger page can be found at `http://localhost:5132/swagger/index.html`. (Assuming you use 5132 as your port)

### Running Tests

   ```bash
    dotnet test
   ```

## Usage

Explain how to interact with the API Gateway endpoints.

*   **Example Endpoint:** `GET /gateway/contacts`
    *   **Description:** Retrieves a list of contacts by forwarding the request to the appropriate LACRM endpoint.
    *   **Example Request:**
        ```json        
        {
            "eventName": "telephone_call",
            "callStart": "16-Apr-25 12:57:48",
            "callId": "c49a1899-4d43-41f7-8d63-4e75ef7f1ca3",
            "callersName": "Alice",
            "callersTelephoneNumber": "01527306999"
        }
        ```
    *   **Responses:** (Describe the expected JSON response format, likely mirroring or transforming the LACRM response)
        New Contact created:
        ```json
        {
            "message": "Contact created successfully!"
        }
        ```
        Contact exists:
        ```json
        {
            "message": "Contact already exists!"
        }
        ```

*   **[Add documentation for other gateway endpoints, including required headers, parameters, and example request/response formats]**

## Technology Stack

*   .NET 9
*   ASP.NET Core
*   Less Annoying CRM API

