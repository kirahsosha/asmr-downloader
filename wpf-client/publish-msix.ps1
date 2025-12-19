<#
Simple MSIX packaging helper.
Requires `MakeAppx.exe` and `SignTool.exe` from Windows SDK, and a code signing certificate (pfx).

This script will publish the project, create an MSIX package and sign it.
Adjust `$CertificatePath` and `$CertificatePassword` before use.
#>
param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$CertificatePath = "",
    [string]$CertificatePassword = ""
)

$out = "..\dist\wpf-client-msix"
Write-Host "Publishing self-contained app to temporary folder..."
dotnet publish AsmrDownloader.Client.csproj -c $Configuration -r $Runtime --self-contained true -o ../dist/wpf-client-pub
if ($LASTEXITCODE -ne 0) { Write-Error "Publish failed"; exit $LASTEXITCODE }

New-Item -ItemType Directory -Force -Path $out | Out-Null

Write-Host "Copying files to package layout..."
Copy-Item -Path ../dist/wpf-client-pub/* -Destination $out -Recurse -Force

# Create AppxManifest minimal
$manifest = @"
<Package xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10" xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10">
  <Identity Name="AsmrDownloader.Client" Publisher="CN=LocalDev" Version="1.0.0.0" />
  <Properties>
    <DisplayName>ASMR Downloader</DisplayName>
    <PublisherDisplayName>LocalDev</PublisherDisplayName>
  </Properties>
  <Dependencies>
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17763.0" MaxVersionTested="10.0.19041.0" />
  </Dependencies>
  <Resources>
    <Resource Language="en-us" />
  </Resources>
  <Applications>
    <Application Id="App" Executable="AsmrDownloader.Client.exe" EntryPoint="Windows.FullTrustApplication">
      <uap:VisualElements DisplayName="ASMR Downloader" Square150x150Logo="Assets\Logo.png" />
    </Application>
  </Applications>
</Package>
"@

$manifestPath = Join-Path $out "AppxManifest.xml"
Set-Content -Path $manifestPath -Value $manifest -Encoding UTF8

Write-Host "Creating MSIX package (MakeAppx)..."
$msix = "AsmrDownloader.Client.msix"
& "MakeAppx.exe" pack /d $out /p $msix
if ($LASTEXITCODE -ne 0) { Write-Error "MakeAppx failed"; exit $LASTEXITCODE }

if ($CertificatePath -ne "") {
    Write-Host "Signing package..."
    & "SignTool.exe" sign /fd SHA256 /a /f $CertificatePath /p $CertificatePassword $msix
    if ($LASTEXITCODE -ne 0) { Write-Error "SignTool failed"; exit $LASTEXITCODE }
}

Write-Host "MSIX package created: $msix"
