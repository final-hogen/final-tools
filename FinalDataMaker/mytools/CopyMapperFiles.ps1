
#画像をコピーする
Param( [string]$mapFilename)
$PSDefaultParameterValues['*:Encoding'] = 'utf8'
$targetFolder = "画像png/"
$sourceFoler = "Texture2D/"
$ErrorActionPreference = "Stop"

$jsonData =(Get-Content $mapFilename | ConvertFrom-Json )
$jsonData.psobject.Properties | ForEach-Object {
  $key = $_.Name
  $value = $jsonData.$key
  $sourcepath = $sourceFoler+$value+".png"
  $targetpath = $targetFolder + $key+".png"
  $targetdir = Split-Path $targetpath -Parent
  New-Item -Path $targetdir -ItemType Directory -Force
  Copy-Item $sourcepath -Destination $targetpath -Recurse -Force -ErrorAction Stop
}
