$global:ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$servicesDir = "/home/playnite/PlayniteServices"
$tempDir = "/home/playnite/PlayniteServicesNew"

ssh -t playnite@api2.playnite.link "rm -rf $tempDir"
scp -r .\PlayniteServices\ playnite@api2.playnite.link:$tempDir
ssh -t playnite@api2.playnite.link "sudo systemctl stop playnite-backend; rm -rf $servicesDir; mv $tempDir $servicesDir; sudo systemctl start playnite-backend"

Start-Sleep -s 2
curl "https://api2.playnite.link/api/playnite/diag/test"