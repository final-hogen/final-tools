
#ダウンロードしたファイルからフォルダ構成に戻すやつ
$PSDefaultParameterValues['*:Encoding'] = 'utf8'

$directory = "downloaded"
$files = Get-ChildItem -Name -Filter ("$directory/*.webp")

foreach($file in $files)
{
  $splitnames = $file.Split("_")
  $filename = $splitnames[$splitnames.Length-1]
  $dir = $splitnames[0..($splitnames.Length-2)] -join "/"
  New-Item -Path $dir -ItemType Directory -Force
  Copy-Item "$directory/$file" -Destination ($dir+"/"+$filename) -Recurse -Force
}

