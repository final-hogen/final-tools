
#コンパイル実行
Param( [string]$ProjectName)
$PSDefaultParameterValues['*:Encoding'] = 'utf8'
if($ProjectName -eq $null){
  throw "BildAndRun param not found."
}
$projectpath = "./$ProjectName/$ProjectName.csproj"
dotnet build $projectpath
dotnet run --project $projectpath
