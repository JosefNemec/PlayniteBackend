$global:ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$servicesDir = "/home/playnite/PlayniteServices"
$tempDir = "/home/playnite/PlayniteServicesNew"

ssh -t playnite@api.playnite.link "rm -rf $tempDir"
scp -r .\PlayniteServices\ playnite@api.playnite.link:$tempDir
ssh -t playnite@api.playnite.link "sudo systemctl stop playnite-backend; rm -rf $servicesDir; mv $tempDir $servicesDir; sudo systemctl start playnite-backend"

Start-Sleep -s 2
curl "https://api.playnite.link/api/playnite/diag/test"