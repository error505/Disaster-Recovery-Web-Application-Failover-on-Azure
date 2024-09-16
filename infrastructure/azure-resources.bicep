// Parameters
@description('Primary location for the Azure resources.')
param location string = resourceGroup().location
@description('Secondary location for failover.')
param locationSecondary string = 'eastus2'
@description('Name for the primary Azure Cosmos DB account.')
param cosmosDbAccountName string = 'twoRegionCosmosDb'
param cosmosDbPrimaryRegionName string = 'twoRegionCosmosDbPrimary'
param appServicePlanName string = 'twoRegionAppServicePlan'
param apiAppServicePrimaryName string = 'twoRegionAppServicePrimary'
param apiAppServiceSecondaryName string = 'twoRegionAppServiceSecondary'
param functionAppPrimaryName string = 'twoRegionFunctionAppPrimary'
param functionAppSecondaryName string = 'twoRegionFunctionAppSecondary'
param redisCachePrimaryName string = 'twoRegionRedisPrimary'
param redisCacheSecondaryName string = 'twoRegionRedisSecondary'
param queueStoragePrimaryName string = 'twoRegionQueuePrimary'
param queueStorageSecondaryName string = 'twoRegionQueueSecondary'
param appInsightsName string = 'twoRegionAppInsights'
param frontDoorName string = 'twoRegionFrontDoor'
@description('Name for the primary Storage account.')
param storageAccountPrimaryName string = 'tworegionstorageprimary'
@description('Name for the secondary Storage account.')
param storageAccountSecondaryName string = 'tworegionstoragesecondary'

// Azure Cosmos DB Account
resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' = {
  name: cosmosDbAccountName
  location: location
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
      {
        locationName: locationSecondary
        failoverPriority: 1
        isZoneRedundant: false
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session' // Adjust based on your consistency requirements
    }
    enableAutomaticFailover: true
    enableMultipleWriteLocations: true // Enables multi-region writes
  }
}

// Cosmos DB Database and Container (Primary and Secondary)
resource cosmosDbPrimary 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-05-15' = {
  parent: cosmosDbAccount
  name: cosmosDbPrimaryRegionName
  properties: {
    resource: {
      id: 'twoRegionDatabase'
    }
  }
}

resource cosmosDbContainerPrimary 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15' = {
  parent: cosmosDbPrimary
  name: 'twoRegionContainer'
  properties: {
    resource: {
      id: 'twoRegionContainer'
      partitionKey: {
        paths: ['/partitionKey']
        kind: 'Hash'
      }
    }
    options: {
      throughput: 400
    }
  }
}

// Azure App Service Plan (Primary and Secondary)
resource appServicePlanPrimary 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'S1'
    tier: 'Standard'
  }
}

resource appServicePlanSecondary 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${appServicePlanName}Secondary'
  location: locationSecondary
  sku: {
    name: 'S1'
    tier: 'Standard'
  }
}

// API App Services (Primary and Secondary)
resource apiAppServicePrimary 'Microsoft.Web/sites@2022-03-01' = {
  name: apiAppServicePrimaryName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: appServicePlanPrimary.id
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'CosmosDBConnectionString'
          value: cosmosDbAccount.properties.documentEndpoint
        }
      ]
    }
  }
}

resource apiAppServiceSecondary 'Microsoft.Web/sites@2022-03-01' = {
  name: apiAppServiceSecondaryName
  location: locationSecondary
  kind: 'app'
  properties: {
    serverFarmId: appServicePlanSecondary.id
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'CosmosDBConnectionString'
          value: cosmosDbAccount.properties.documentEndpoint
        }
      ]
    }
  }
}

// Azure Function Apps (Primary and Secondary)
resource functionAppPrimary 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppPrimaryName
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: appServicePlanPrimary.id
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'CosmosDBConnectionString'
          value: cosmosDbAccount.properties.documentEndpoint
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
      ]
    }
  }
}

resource functionAppSecondary 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppSecondaryName
  location: locationSecondary
  kind: 'functionapp'
  properties: {
    serverFarmId: appServicePlanSecondary.id
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'CosmosDBConnectionString'
          value: cosmosDbAccount.properties.documentEndpoint
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
      ]
    }
  }
}

// Azure Cache for Redis (Primary and Secondary)
resource redisCachePrimary 'Microsoft.Cache/redis@2024-04-01-preview' = {
  name: redisCachePrimaryName
  location: location
  properties:{
    sku: {
      name: 'Standard'
      family: 'C'
      capacity: 1
    }
    enableNonSslPort: false
    minimumTlsVersion: '1.2'
  }
}

resource redisCacheSecondary 'Microsoft.Cache/redis@2024-04-01-preview' = {
  name: redisCacheSecondaryName
  location: locationSecondary
  properties:{
    sku: {
      name: 'Standard'
      family: 'C'
      capacity: 1
    }
    enableNonSslPort: false
    minimumTlsVersion: '1.2'
  }
}

// Azure Storage Account (Primary)
resource storageAccountPrimary 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountPrimaryName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    supportsHttpsTrafficOnly: true
  }
}

// Azure Storage Account (Secondary)
resource storageAccountSecondary 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountSecondaryName
  location: locationSecondary
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    supportsHttpsTrafficOnly: true
  }
}

// Azure Queue Service (Primary)
resource queueServicePrimary 'Microsoft.Storage/storageAccounts/queueServices@2022-09-01' = {
  parent: storageAccountPrimary
  name: 'default' // Default name for queue service
}

// Azure Queue Service (Secondary)
resource queueServiceSecondary 'Microsoft.Storage/storageAccounts/queueServices@2022-09-01' = {
  parent: storageAccountSecondary
  name: 'default' // Default name for queue service
}

// Azure Queue Storage (Primary)
resource queueStoragePrimary 'Microsoft.Storage/storageAccounts/queueServices/queues@2022-09-01' = {
  parent: queueServicePrimary
  name: queueStoragePrimaryName
}

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    RetentionInDays: 30
  }
}

// Azure Front Door
resource frontDoor 'Microsoft.Network/frontDoors@2021-06-01' = {
  name: frontDoorName
  location: location
  properties: {
    frontendEndpoints: [
      {
        name: 'default'
      }
    ]
  }
}
