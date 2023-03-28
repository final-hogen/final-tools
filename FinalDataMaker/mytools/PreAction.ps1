
#タスク実行のための前処理
$PSDefaultParameterValues['*:Encoding'] = 'utf8'
Function CheckFile($name){
  if(Test-Path $name){return}
  Write-Host "ファイルが見つかりません" $name
  throw $name
}
Function CheckExec($path,$name){
  if(Test-Path ($path+$name)){return}
  $zip = $path+$name.Replace(".exe",".zip")
  Expand-Archive -Path $zip -DestinationPath $path -Force
  if(Test-Path ($path+$name)){return}
  throw $name
}

$toolfolder = "mytools/"
$execfiles = @("FinalSerialBinToJson.exe","FinalSpineBinToJson.exe");
$RequiredFiles = @("TextAsset","Texture2D")
foreach ( $file in $RequiredFiles )
{
  CheckFile $file
}
foreach ( $file in $execfiles )
{
  CheckExec $toolfolder $file
}

