cls

echo "Terminating known orphanged VB / Rider processes..."

$msb = Get-Process -Name "MSBuild" -ea silentlycontinue
if ($msb) { Stop-Process -InputObject $msb -force }
Get-Process | Where-Object {$_.HasExited}

$vbcsc = Get-Process -Name "VBCSCompiler" -ea silentlycontinue
if ($vbcsc) { Stop-Process -InputObject $vbcsc -force }
Get-Process | Where-Object {$_.HasExited}

$w3wp = Get-Process -Name "w3wp" -ea silentlycontinue
if ($w3wp) { Stop-Process -InputObject $w3wp -force }
Get-Process | Where-Object {$_.HasExited}

if ((!$msb) -and (!$vbcsc) -and (!$w3wp)) { echo "No known orphanded processes found!" }

echo "Deleting all bin and obj content..."
$paths = "."

foreach ($path in $paths)
{
    $directories = Get-ChildItem -Path $path -Directory -Recurse

    foreach ($directory in $directories)
    {
        $binFolder = Join-Path -Path $directory.FullName -ChildPath "bin"
        $objFolder = Join-Path -Path $directory.FullName -ChildPath "obj"

        if (Test-Path -Path $binFolder)
        {
            Write-Output "Deleting folder: $binFolder"
            Remove-Item -Path $binFolder -Recurse -ErrorAction SilentlyContinue -Force
        }

        if (Test-Path -Path $objFolder)
        {
            Write-Output "Deleting folder: $objFolder"
            Remove-Item -Path $objFolder -Recurse -ErrorAction SilentlyContinue -Force
        }
    }
}

echo "Deleting all garbage from user Temp folder..."
Remove-Item -path $env:userprofile\AppData\Local\Temp -recurse -force -ea silentlycontinue

echo "Deleting all SQL garbage..."
Remove-Item -path C:\Windows\ServiceProfiles\SSISScaleOutMaster140\AppData\Local\SSIS\ScaleOut\Master -recurse -force -ea silentlycontinue
Remove-Item -path C:\Windows\ServiceProfiles\SSISScaleOutMaster160\AppData\Local\SSIS\ScaleOut\16\Master -recurse -force -ea silentlycontinue
