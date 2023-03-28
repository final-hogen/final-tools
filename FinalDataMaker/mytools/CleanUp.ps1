
#作成ファイルをぜんぶ抹殺
$PSDefaultParameterValues['*:Encoding'] = 'utf8'
Remove-item *.JsonData 	-Recurse -Force
Remove-item mytools/*.exe 	-Recurse -Force
Remove-item mytools/*.dll 	-Recurse -Force
