Write-Output "Removing any folder bin & obj"
Get-ChildItem .\ -include bin,obj -Recurse | ForEach-Object ($_) { remove-item $_.fullname -recurse -force }
dotnet restore
Write-Output "Done"