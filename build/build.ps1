param(
    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release",
    [string]$OutputPath = (Join-Path (Get-Location) "PlayniteServices\")
)

$global:ErrorActionPreference = "Stop"
& .\common.ps1

if (Test-Path $OutputPath)
{
    Remove-Item $OutputPath -Recurse
}

New-Folder $OutputPath
Push-Location
Set-Location "..\source\PlayniteServices\"

try
{
    Write-OperationLog "Building..."
    $compiler = StartAndWait "dotnet" ("publish -c {0} -o {1}" -f $Configuration, $OutputPath)
    if ($compiler -ne 0)
    {
        Write-Host "Build failed." -ForegroundColor "Red"
    }
}
finally
{
    Pop-Location 
}