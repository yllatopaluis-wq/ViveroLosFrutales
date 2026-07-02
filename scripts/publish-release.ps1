[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$root = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot "..")).Path
$deployRoot = Join-Path $root "DeployHosting"
$publicOutput = Join-Path $deployRoot "PublicWeb"
$erpOutput = Join-Path $deployRoot "ERPApp"

function Remove-DeploymentOutput {
    param([Parameter(Mandatory)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        return
    }

    $resolved = (Resolve-Path -LiteralPath $Path).Path
    if (-not $resolved.StartsWith($deployRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "No se puede limpiar una ruta fuera de DeployHosting: $resolved"
    }

    Remove-Item -LiteralPath $resolved -Recurse -Force
}


function Set-ErpHostingDefaults {
    param([Parameter(Mandatory)][string]$Path)

    New-Item -ItemType Directory -Path (Join-Path $Path "logs") -Force | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $Path "Logs") -Force | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $Path ".data-protection-keys") -Force | Out-Null

    $hostingSettingsPath = Join-Path $deployRoot "appsettings.json"
    if (Test-Path -LiteralPath $hostingSettingsPath) {
        Copy-Item -LiteralPath $hostingSettingsPath -Destination (Join-Path $Path "appsettings.json") -Force
    }

    $settingsPath = Join-Path $Path "appsettings.json"
    $settings = Get-Content -Raw -LiteralPath $settingsPath | ConvertFrom-Json
    $settings.Seed = [pscustomobject]@{ RunOnStartup = $false }
    $settings | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $settingsPath -Encoding UTF8

    $webConfigPath = Join-Path $Path "web.config"
    [xml]$webConfig = Get-Content -Raw -LiteralPath $webConfigPath
    $aspNetCore = $webConfig.configuration.location.'system.webServer'.aspNetCore
    $aspNetCore.stdoutLogEnabled = "true"
    $aspNetCore.stdoutLogFile = ".\logs\stdout"
    $webConfig.Save($webConfigPath)
}

New-Item -ItemType Directory -Path $deployRoot -Force | Out-Null
Remove-DeploymentOutput -Path $publicOutput
Remove-DeploymentOutput -Path $erpOutput

Push-Location $root
try {
    dotnet publish "src/ViveroLosFrutales.PublicWeb/ViveroLosFrutales.PublicWeb.csproj" -c Release -o $publicOutput
    if ($LASTEXITCODE -ne 0) { throw "Fallo la publicación de PublicWeb." }

    dotnet publish "src/ViveroLosFrutales.Web/ViveroLosFrutales.Web.csproj" -c Release -o $erpOutput
    if ($LASTEXITCODE -ne 0) { throw "Fallo la publicación del ERP." }
    Set-ErpHostingDefaults -Path $erpOutput

    Compress-Archive -Path "$publicOutput/*" -DestinationPath (Join-Path $deployRoot "ViveroLosFrutales-PublicWeb.zip") -CompressionLevel Optimal -Force
    Compress-Archive -Path "$erpOutput/*" -DestinationPath (Join-Path $deployRoot "ViveroLosFrutales-ERPApp.zip") -CompressionLevel Optimal -Force
}
finally {
    Pop-Location
}

Write-Host "Release generado únicamente en: $deployRoot" -ForegroundColor Green

