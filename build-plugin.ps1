$version = "2.0.0"
$project = "Weather"
$dllName = "WeatherPlugin.dll"
$dllPath = "$($env:LOCALAPPDATA)\Loupedeck\Plugins\$project"
$buildPath = ".builds"
$outputFileName = "$project"
$zipPath = "$buildPath\$outputFileName.zip"
$pluginName = "$outputFileName.lplug4"
$loupedeckYaml = "LoupedeckPackage.yaml"
$cwd = Get-Location

if( Test-Path $buildPath){
	Remove-Item -Path "$buildPath" -Force -Recurse 
}

New-Item -Path "$buildPath" -Force -Name "bin" -ItemType "directory" > $null
New-Item -Path "$buildPath" -Force -Name "metadata" -ItemType "directory" > $null

Copy-Item "$loupedeckYaml" -Force -Destination "$buildPath\metadata\$loupedeckYaml" > $null
((Get-Content -Path "$buildPath\metadata\$loupedeckYaml" -Raw) -replace 'x.x.x', $version) | Set-Content "$buildPath\metadata\$loupedeckYaml"

Copy-Item "$($project)Plugin\Resources\256.png" -Force -Destination "$buildPath\metadata\256.png" > $null
Copy-Item "$dllPath\$dllName" -Force -Destination "$buildPath\bin\$dllName" > $null

$compress = @{
	Path = "$buildPath\*"
	CompressionLevel = "Fastest"
	DestinationPath = $zipPath
}
Compress-Archive @Compress > $null

Rename-Item $zipPath -Force -NewName $pluginName > $null