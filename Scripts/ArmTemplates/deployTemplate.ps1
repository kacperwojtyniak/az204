$resourceGroupName = "TestTemplates"
$templatePath = "./template.json"
$logicApptemplatePath = "./logicApp.json"
$logicAppParameterPath = "./logicApp.parameters.json"
$templateParameterPath = "./template.parameters.json"
$location = "EastUs"
$functionsPath = "../../az204api/az204functions"
$publishPath = $functionsPath + '/bin/debug/netcoreapp3.1/*'

az login

$resourceGroup = az group exists --name $resourceGroupName

if (!$resourceGroup) {    
  az group create --name $resourceGroupName --location $location
}

az group deployment create --resource-group $resourceGroupName --template-file $templatePath --parameters $templateParameterPath

dotnet publish $functionsPath
Compress-Archive -Path $publishPath -DestinationPath ./function.zip

az functionapp deployment source config-zip -g $resourceGroupName -n 'az204functions123' --src './function.zip'

az group deployment create --resource-group $resourceGroupName --template-file $logicApptemplatePath --parameters $logicAppParameterPath

