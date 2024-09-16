# Azure Functions (Background Task and Failover)

This folder contains the C# code for the Azure Functions used in the two-region web application architecture.

## Functions

- **BackgroundTaskFunction**: Processes background tasks from the Azure Queue Storage and updates Cosmos DB.
- **FailoverFunction**: Periodically checks the health of the primary region and initiates failover if necessary.

## Deployment Instructions

1. Make sure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed.
2. Build and publish the project:

    ```bash
    dotnet build --configuration Release
    dotnet publish --configuration Release -o ./publish
    ```

3. Deploy the functions using the `Deploy Azure Functions` GitHub Action by pushing changes to the `main` branch.

## Required GitHub Secrets

- **`AZURE_FUNCTION_APP_NAME_RETRY`**: The name of the Azure Function App handling retry logic.
- **`AZURE_FUNCTION_APP_NAME_FAILOVER`**: The name of the Azure Function App handling failover logic.
- **`AZURE_FUNCTION_APP_PUBLISH_PROFILE_RETRY`**: Publish profile for the retry Azure Function App.
- **`AZURE_FUNCTION_APP_PUBLISH_PROFILE_FAILOVER`**: Publish profile for the failover Azure Function App.

## License

This project is licensed under the MIT License.
