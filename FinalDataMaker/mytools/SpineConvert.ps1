
#スパイン関係を変換
$PSDefaultParameterValues['*:Encoding'] = 'utf8'
$waifu2x="waifu2x-caffe/waifu2x-caffe-cui.exe"
$filemap = "出力json/filemapperパイロット.json"
$assetDir = "TextAsset/"
$textureDir = "Texture2D/"
$outputFolder = "画像"
$pngTexFolder = "画像texpng"
$ErrorActionPreference = "Stop"
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process

if(!(Test-Path $assetDir)){
  Write-Host "アセットディレクトリがありません"
  throw
}
if(!(Test-Path $textureDir)){
  Write-Host "テクスチャディレクトリがありません"
  throw
}

$jsonData =(Get-Content $filemap | ConvertFrom-Json )

$jsonData.psobject.Properties | ForEach-Object {
  $key = $_.Name
  if(!($key.EndsWith("顔"))){
    return;
  }
  $targetFolder = $key.Replace("顔","")
  $value = $jsonData.$key
  $splits = $value.Split("_")
  $fname = $splits[1]+"_"+$splits[2];
  $texpath = $textureDir+$fname +".png"
  $atlaspath = $assetDir+$fname+".atlas.asset"
  $skelpath = $assetDir+$fname+".skel.asset"
  $imgdir = "$pngTexFolder/"+$targetFolder+"spine/sd/"
  New-Item -Path $imgdir -ItemType Directory -Force
  Copy-Item $texpath -Destination ($imgdir+"spine.png") -Recurse -Force -ErrorAction Stop
  $spinedir = "$outputFolder/"+$targetFolder+"spine/sd/"
  New-Item -Path $spinedir -ItemType Directory -Force
  $targetatlas = $spinedir+"spine.atlas"
  $arg = "-command .\mytools\AtlasConvert.ps1 '$atlaspath' '$targetatlas'"
  Start-Process -FilePath powershell.exe -ArgumentList $arg -NoNewWindow -ErrorAction Stop
  $outpath = $spinedir+"spine.json"
  Start-Process -FilePath .\mytools\FinalSpineBinToJson.exe -ArgumentList  `"$skelpath`",`"$outpath`" -NoNewWindow  -ErrorAction Stop
}

Start-Process -FilePath $waifu2x -ArgumentList "-i $pngTexFolder -o $outputFolder -w 1024 -e webp -q 80" -NoNewWindow -Wait

powershell.exe -command .\mytools\CheckConvert.ps1
Write-Host "全て投げました"
