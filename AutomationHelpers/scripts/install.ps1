# Copyright © 2018 Ranorex All rights reserved

param($installPath, $toolsPath, $package, $project)

Write-Host 'Started adding constant for current Ranorex version to compilation symbols...'

if ($project.Name -ne 'Ranorex Automation Helpers')
{
    Write-Information('Ignore the non-helper project ' + $project.Name)
    exit
}

$rxVersion = GET-VARIABLE RanorexVersion -ErrorAction 'SilentlyContinue'
$rxVersion = $rxVersion.Value -replace '\.'
if (!$rxVersion)
{
    Write-Warning('Could not find Ranorex version variable.')
    exit
}
$rxVersion = "RX$rxVersion"

$defineConstants = $project.Properties.Item("DefineConstants")
[array]$symbols = $defineConstants.Value.Split(';')
Write-Host "Found: $symbols"
$symbols = $symbols | Where {$_[0] -ne 'R' -and $_[1] -ne 'X'}

$symbols += $rxVersion

$symbolString = $symbols -join ';'
Write-Host "Replace with: $symbolString"
$defineConstants.Value = $symbolString

Write-Host 'Finished adding Ranorex version to compilation symbols.'
