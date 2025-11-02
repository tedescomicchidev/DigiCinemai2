param location string = resourceGroup().location
param environment string = 'dev'

resource serviceBus 'Microsoft.ServiceBus/namespaces@2023-01-01-preview' = {
  name: 'newsroom-${environment}-sb'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}

resource containerAppEnv 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: 'newsroom-${environment}-cae'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: '00000000-0000-0000-0000-000000000000'
        sharedKey: 'PLACEHOLDER'
      }
    }
  }
}

resource cosmos 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: 'newsroom-${environment}-cosmos'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    databaseAccountOfferType: 'Standard'
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'newsroom${environment}storage'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

output serviceBusNamespace string = serviceBus.name
output containerAppEnvironment string = containerAppEnv.name
output cosmosAccount string = cosmos.name
output storageAccount string = storage.name
