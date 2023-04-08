
#画像をコピーする
$webpFolder = "画像/"
$pngFolder = "画像png/"
$waifu2x="waifu2x-caffe\waifu2x-caffe-cui.exe"
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process

mytools/CopyMapperFiles.ps1 "出力json/filemapperパイロット.json"
mytools/CopyMapperFiles.ps1 "出力json/filemapperスキル.json"
mytools/CopyMapperFiles.ps1 "出力json/filemapperパーツ.json"
mytools/CopyMapperFiles.ps1 "出力json/filemapper戦艦艦載砲.json"
mytools/CopyMapperFiles.ps1 "出力json/filemapper戦艦バフ.json"

Write-Host "convert png to webp start."
Start-Process -FilePath $waifu2x -ArgumentList "-i $pngFolder -o $webpFolder -s 1 -e webp -q 80" -NoNewWindow -Wait
