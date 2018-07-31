# Copyright © 2018 Ranorex All rights reserved

param($installPath, $toolsPath, $package, $project)

Write-Host 'Started removing constant for current Ranorex version from compilation symbols...'

$rxVersion = $project.Properties.Item("RanorexVersion")
$rxVersion = $rxVersion.Value -replace '\.'
if (!$rxVersion)
{
    Write-Information('Could not find Ranorex version property, ignore removing constant for this project.')
    exit
}

$defineConstants = $project.Properties.Item("DefineConstants")
[array]$symbols = $defineConstants.Value.Split(';')
Write-Host "Found: $symbols"
$symbols = $symbols | Where {$_[0] -ne 'R' -and $_[1] -ne 'X'}

$symbolString = $symbols -join ';'
Write-Host "Replace with: $symbolString"
$defineConstants.Value = $symbolString

Write-Host 'Finished removing Ranorex specific compilation symbols.'