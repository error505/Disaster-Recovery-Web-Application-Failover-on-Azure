# App Service (Inventory Management API)

This folder contains the C# code for the Azure App Service, which serves as the main web API for managing inventory in the two-region web application architecture.

## API Endpoints

- **POST /api/inventory/add-item**: Adds a new inventory item.
- **GET /api/inventory/get-item/{id}**: Retrieves an inventory item by ID.

## Deployment Instructions

1. Make sure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed.
2. Build and publish the project:

    ```bash
    dotnet build --configuration Release
    dotnet publish --configuration Release -o ./publish
    ```

3. Deploy the app using the `Deploy App Service` GitHub Action by pushing changes to the `main` branch.

## Required GitHub Secrets

- **`AZURE_APP_SERVICE_NAME`**: The name of the Azure App Service.
- **`AZURE_APP_SERVICE_PUBLISH_PROFILE`**: Publish profile of the Azure App Service.

## License

This project is licensed under the MIT License.
