$resourceGroupName = 'Az204'
$registryName = 'az204acr'

Connect-AzAccount

$registry = Get-AzContainerRegistry -ResourceGroupName $resourceGroupName -Name $registryName

$creds = Get-AzContainerRegistryCredential -Registry $registry

$creds.Password | docker login $registry.LoginServer -u $creds.Username --password-stdin

$imageName = "$($registry.LoginServer)/az204api:latest"

docker build -f .\az204api\az204api\Dockerfile -t $imageName .\az204api

docker push $imageName

az container create --resource-group $resourceGroupName --file ./Scripts/containerInstance.yaml

# Create service principal with password. Store password in key vault and get passworid during deployment https://docs.microsoft.com/en-us/azure/container-registry/container-registry-auth-service-principal
# https://docs.microsoft.com/en-us/azure/container-registry/container-registry-auth-aci
# https://github.com/Azure/azure-docs-powershell-samples/tree/master/container-registry
New-AzContainerGroup `
  -ResourceGroupName $resourceGroupName `
  -Name az204aci `
  -Image "$($registryName).azurecr.io/$($imageName)" `
  -RegistryServer "$($registryName).azurecr.io"
