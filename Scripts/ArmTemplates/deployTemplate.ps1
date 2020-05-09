$resourceGroupName = "TestTemplates"
$templatePath = "./template.json"
$location = "EastUs"

#Connect-AzAccount

$resourceGroup = Get-AzResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue

if (!$resourceGroup)
{    
    New-AzResourceGroup -Name $resourceGroupName -Location $location
}

New-AzResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templatePath