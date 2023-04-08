
#変換ファイルができてるかチェック
$PSDefaultParameterValues['*:Encoding'] = 'utf8'
$filemap = "出力json/filemapperパイロット.json"

$jsonData =(Get-Content $filemap | ConvertFrom-Json )

$jsonData.psobject.Properties | ForEach-Object {
  $key = $_.Name
  if(!($key.EndsWith("顔"))){
    return;
  }
  $targetFolder = $key.Replace("顔","")
  $img = "画像/"+$targetFolder+"spine/sd/spine.webp"
  $spinedir = "画像/"+$targetFolder+"spine/sd/"
  if(!(Test-Path $img)){
    Write-Host "ファイルが見つかりません. $img"
    throw
  }
  $file = $spinedir+"spine.atlas"
  if(!(Test-Path $file)){
    Write-Host "ファイルが見つかりません. $file"
    throw
  }
  $file = $spinedir+"spine.json"
  if(!(Test-Path $file)){
    Write-Host "ファイルが見つかりません. $file"
    throw
  }
}
