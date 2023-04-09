
#画像をコピーする
$webpFolder = "画像/"
$pngFolder = "画像png/"
$mapperdir = "出力json"
$mappers = @("filemapperパイロット.json","filemapperスキル.json","filemapperパーツ.json","filemapper戦艦艦載砲.json","filemapper戦艦バフ.json")
$waifu2x="waifu2x-caffe\waifu2x-caffe-cui.exe"
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process

foreach( $name in $mappers){
  $path = "$mapperdir/$name"
  if(Test-Path $path){
    mytools/CopyMapperFiles.ps1 $path
  }
}
Write-Host "convert png to webp start."
Start-Process -FilePath $waifu2x -ArgumentList "-i $pngFolder -o $webpFolder -s 1 -e webp -q 80" -NoNewWindow -Wait
