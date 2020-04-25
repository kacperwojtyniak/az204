$resourceGroupName = "az204"
$accountName = "az204db"
$databaseName = "Az204"
$databaseRUs = 400
$containerName = "Coffees"
$partitionKeyPath = "/roastery"

Connect-AzAccount

# Create DB
Set-AzCosmosDBSqlDatabase `
    -ResourceGroupName $resourceGroupName `
    -AccountName $accountName `
    -Name $databaseName `
    -Throughput $databaseRUs

#Create container
    Set-AzCosmosDBSqlContainer `
    -ResourceGroupName $resourceGroupName `
    -AccountName $accountName `
    -DatabaseName $databaseName `
    -Name $containerName `
    -PartitionKeyKind Hash `
    -PartitionKeyPath $partitionKeyPath


    #Create container for coffees partitioned by brewingMethod
    Set-AzCosmosDBSqlContainer `
    -ResourceGroupName $resourceGroupName `
    -AccountName $accountName `
    -DatabaseName $databaseName `
    -Name 'CoffeesBrewing' `
    -PartitionKeyKind Hash `
    -PartitionKeyPath '/brewingMethod'

    #Create container for coffees partitioned by  origin
    Set-AzCosmosDBSqlContainer `
    -ResourceGroupName $resourceGroupName `
    -AccountName $accountName `
    -DatabaseName $databaseName `
    -Name 'CoffeesOrigin' `
    -PartitionKeyKind Hash `
    -PartitionKeyPath '/origin'

    #Create container for orders partitioned by date
    Set-AzCosmosDBSqlContainer `
    -ResourceGroupName $resourceGroupName `
    -AccountName $accountName `
    -DatabaseName $databaseName `
    -Name 'Orders' `
    -PartitionKeyKind Hash `
    -PartitionKeyPath '/date'