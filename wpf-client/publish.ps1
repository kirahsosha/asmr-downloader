<#
Simple packaging helper: publish self-contained single-file for Windows x64
Usage: .\publish.ps1 -Configuration Release -Runtime win-x64
#>
param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

Write-Host "Publishing project..."
dotnet publish AsmrDownloader.Client.csproj -c $Configuration -r $Runtime /p:PublishSingleFile=true /p:PublishTrimmed=false --self-contained true -o ../dist/wpf-client
if ($LASTEXITCODE -ne 0) { Write-Error "Publish failed"; exit $LASTEXITCODE }
Write-Host "Published to ../dist/wpf-client"
