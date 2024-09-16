# Static Web App (Admin Dashboard)

This folder contains the React TypeScript code for the front-end dashboard used to manage inventory items and monitor the health of the application.

## Features

- **Add Inventory Items**: Allows administrators to add new inventory items.
- **View Inventory Items**: Displays the current inventory items.
- **Monitor Health**: Shows the health status of the primary and secondary regions.

## Deployment Instructions

1. Make sure you have [Node.js](https://nodejs.org/) and [npm](https://www.npmjs.com/) installed.
2. Install dependencies:

    ```bash
    npm install
    ```

3. Build the project:

    ```bash
    npm run build
    ```

4. Deploy the app using the `Deploy Static Web App` GitHub Action by pushing changes to the `main` branch.

## Required GitHub Secrets

- **`AZURE_STATIC_WEB_APP_NAME`**: The name of the Azure Static Web App.
- **`AZURE_STATIC_WEB_APP_PUBLISH_PROFILE`**: Publish profile of the Azure Static Web App.

## License

This project is licensed under the MIT License.
