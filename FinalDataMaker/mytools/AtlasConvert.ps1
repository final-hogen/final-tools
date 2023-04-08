Param(
  [String]$inFile,
  [String]$outFile
)
#スパインのatlasのサイズを変換
$targetSize = 1024
$PSDefaultParameterValues['*:Encoding'] = 'utf8'

if(!(Test-Path $inFile)){
  Write-Host "file not found "+$inFile
  exit
}

New-Item -Type File $outFile -Force
$file = (Get-Content -Encoding utf8 $inFile) -as [string[]]
$wScale = 0
$hScale = 0
foreach ($line in $file) {
    if($line.EndsWith(".png")){
      $line = "spine.webp"
    }else{
      $split = $line.Split(":")
      if($split.Count -gt 1){
        switch ($split[0]) {
          "size" {
            $splitRight = $split[1].Split(",")
            $wScale = $targetSize / [int]$splitRight[0]
            $hScale = $targetSize / [int]$splitRight[1]
            $line = $split[0]+": "+$targetSize+","+$targetSize
          }
          ({$_ -in ("  xy","  size","  orig","  offse")})
          {
            $splitRight = $split[1].Split(",")
            $x = $wScale * [int]$splitRight[0]
            $y = $hScale * [int]$splitRight[1]
            $line = $split[0]+": "+[Math]::Round($x,0)+","+[Math]::Round($y,0)
          }
          Default {}
        }
      }
    }
    $line = [Text.Encoding]::UTF8.GetBytes($line+"`r`n")
    Add-Content -Path $outFile -Value $line -Encoding Byte
}
