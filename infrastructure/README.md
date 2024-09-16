# Infrastructure Deployment

This folder contains the Bicep template for deploying the Azure infrastructure required for the two-region web application architecture.

## Components Deployed

- **Azure Front Door**: Global load balancing and failover.
- **Azure App Service**: Hosts the web application and APIs.
- **Azure Functions**: Executes background tasks and handles failover scenarios.
- **Azure Cosmos DB**: Globally distributed database with multi-region replication.
- **Azure Cache for Redis**: Caches data to optimize performance.
- **Azure Queue Storage**: Manages background tasks via queue messages.

## Deployment Instructions

1. Make sure you have the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) installed and logged in.
2. To deploy the infrastructure manually, run:

    ```bash
    az deployment group create --resource-group <your-resource-group> --template-file azure-resources.bicep
    ```

3. Alternatively, trigger the `Deploy Infrastructure` GitHub Action by pushing changes to the `main` branch.

## Required GitHub Secrets

- **`AZURE_CREDENTIALS`**: Azure service principal credentials in JSON format.
- **`AZURE_RESOURCE_GROUP`**: The Azure resource group name.
- **`AZURE_LOCATION`**: The primary region for deployment.
- **`AZURE_LOCATION_SECONDARY`**: The secondary region for deployment.

## License

This project is licensed under the MIT License.
