$url = 'https://localhost:5001/Coffee'
$testData = Get-Content '../Data/addCoffeeExample.json' | ConvertFrom-Json


ForEach($coffee in $testData)
{
$coffeeJson = $coffee | ConvertTo-Json
Invoke-WebRequest -Uri $url -Method POST -Body $coffeeJson -ContentType "application/json"
}

