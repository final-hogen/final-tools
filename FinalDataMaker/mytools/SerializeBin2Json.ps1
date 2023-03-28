
#バイナリフォーマッタで作られたデータをJSONに取り出す
$PSDefaultParameterValues['*:Encoding'] = 'utf8'
$targetFolder = "TextAsset/"
$filenames = @("datass2.txt","JAP_LanguageData.txt","SC_LanguageData.txt","JAP_storyJsonDatas.txt","datass.txt");
$paths = @()
foreach ( $file in $filenames )
{
  $paths += $targetFolder+$file
}
mytools/FinalSerialBinToJson.exe $paths
