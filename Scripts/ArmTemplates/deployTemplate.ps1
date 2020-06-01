$resourceGroupName = "TestTemplates"
$acrResourceGroupName = "AcrResourceGroup"
$acrTemplatePath = "./acrTemplate.json"
$templatePath = "./template.json"
$logicApptemplatePath = "./logicApp.json"
$logicAppParameterPath = "./logicApp.parameters.json"
$templateParameterPath = "./template.parameters.json"
$location = "EastUs"
$functionsPath = "../../az204api/az204functions"
$publishPath = $functionsPath + '/bin/debug/netcoreapp3.1/*'

Connect-AzAccount

if (!(Get-AzResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue)) {    
  New-AzResourceGroup -Name $resourceGroupName -Location $location
}

if (!(Get-AzResourceGroup -Name $acrResourceGroupName -ErrorAction SilentlyContinue)) {
  New-AzResourceGroup -Name $acrResourceGroupName -Location $location
}

#Deploy acr
$registryName = (New-AzResourceGroupDeployment -ResourceGroupName $acrResourceGroupName -TemplateFile $acrTemplatePath).Outputs.acrName.Value

#Push image to registry
$registry = Get-AzContainerRegistry -ResourceGroupName $acrResourceGroupName -Name $registryName
$creds = Get-AzContainerRegistryCredential -Registry $registry
$creds.Password | docker login $registry.LoginServer -u $creds.Username --password-stdin
$imageName = "$($registry.LoginServer)/az204api:latest"
docker build -f ..\..\az204api\az204api\Dockerfile -t $imageName ..\..\az204api
docker push $imageName


$setOrderStatusFunction = Get-Content '.\setOrderStatusFunction.js' -Raw
#Deploy main resource group
New-AzResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templatePath -TemplateParameterFile $templateParameterPath `
  -acrUserName $creds.Username `
  -acrPassword $creds.Password `
  -imageName $imageName `
  -acrServerName $registry.LoginServer `
  -setOrderStatusFunction $setOrderStatusFunction

#Build azure functions, zip and deploy
dotnet publish $functionsPath
Compress-Archive -Path $publishPath -DestinationPath ./function.zip -Force

az functionapp deployment source config-zip -g $resourceGroupName -n 'az204functions123' --src './function.zip'

#Deploy logic apps
New-AzResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $logicApptemplatePath -TemplateParameterFile $logicAppParameterPath
