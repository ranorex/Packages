# Copyright © 2018 Ranorex All rights reserved

param($installPath, $toolsPath, $package, $project)

Write-Host 'Started adding constant for current Ranorex version to compilation symbols...'

$rxVersion = $project.Properties.Item("RanorexVersion")
$rxVersion = $rxVersion.Value -replace '\.'
if (!$rxVersion)
{
    Write-Warning('Could not find Ranorex version property in project.')
    exit
}
$rxVersion = "RX$rxVersion"
if ($rxVersion -ne "RX72" -and $rxVersion -ne "RX80")
{
    Write-Warning('Current Ranorex version is fully supported by this package.')
    exit
}

$defineConstants = $project.Properties.Item("DefineConstants")
[array]$symbols = $defineConstants.Value.Split(';')
Write-Host "Found: $symbols"
$symbols = $symbols | Where {$_[0] -ne 'R' -and $_[1] -ne 'X'}

$symbols += $rxVersion

$symbolString = $symbols -join ';'
Write-Host "Replace with: $symbolString"
$defineConstants.Value = $symbolString

Write-Host 'Finished adding Ranorex version to compilation symbols.'