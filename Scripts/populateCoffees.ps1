$url = 'https://localhost:5001/Coffee'
$testData = Get-Content '../Data/addCoffeeExample.json' | ConvertFrom-Json


ForEach($coffee in $testData)
{
$coffeeJson = $coffee | ConvertTo-Json
Invoke-WebRequest -Uri $url -Method POST -Body $coffeeJson -ContentType "application/json"
}

$origins = "Ethiopia", "Peru", "Guatemala", "Brazil", "Salvador", "Columbia","Nicaragua","Burundi"

ForEach($origin in $origins)
{
    For ($i=0; $i -le 500; $i++) {
        $coffee = @{
            name = "test$($i)";
            roastery = "Roastery$($i)";
            origin = $origin;
            region = "$($origin)Region$($i)";
            altitudeMin = Get-Random -Minimum 1000 -Maximum 1200;
            altitudeMax = Get-Random -Minimum 1201 -Maximum 1850;
            process = "Washed", "Natural" | Get-Random;
            brewingMethod = "filter", "espresso", "all" | Get-Random;
            unitPrice = Get-Random -Minimum 40 - -Maximum 60
        }
         $json = $coffee | ConvertTo-Json
         Invoke-WebRequest -Uri $url -Method POST -Body $json -ContentType "application/json"
        }
}

