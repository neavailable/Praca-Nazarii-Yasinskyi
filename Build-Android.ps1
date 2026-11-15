param(
    [string]$UnityPath = 'C:\Program Files\Unity\Hub\Editor\2022.3.60f1\Editor\Unity.exe'
)

$ErrorActionPreference = 'Stop'
if (-not (Test-Path -LiteralPath $UnityPath)) {
    throw "Unity executable not found: $UnityPath"
}

# An isolated copy permits a build while the source project is open in Unity.
$buildRoot = Join-Path $PSScriptRoot 'Builds\Android'
$workspace = Join-Path $PSScriptRoot ('Builds\AndroidWorkspace-' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $buildRoot, $workspace -Force | Out-Null

foreach ($directory in @('Assets', 'Packages', 'ProjectSettings', 'Library\PackageCache')) {
    $source = Join-Path $PSScriptRoot $directory
    if (Test-Path -LiteralPath $source) {
        & robocopy $source (Join-Path $workspace $directory) /E /NFL /NDL /NJH /NJS /NP
        if ($LASTEXITCODE -gt 7) { throw "Could not copy $directory (robocopy exit $LASTEXITCODE)" }
    }
}

$apk = Join-Path $buildRoot 'BallFinisher.apk'
$log = Join-Path $buildRoot 'build.log'
Write-Output "Build workspace: $workspace"
Write-Output "Build log: $log"
$buildArguments = @('-batchmode', '-nographics', '-quit', '-projectPath', ('"' + $workspace + '"'),
    '-buildTarget', 'Android', '-executeMethod', 'BallFinisher.Editor.AndroidBuild.BuildApk',
    '-androidOutput', ('"' + $apk + '"'), '-logFile', ('"' + $log + '"'))
$process = Start-Process -FilePath $UnityPath -ArgumentList $buildArguments -WindowStyle Hidden -PassThru -Wait
if ($process.ExitCode -ne 0) { throw "Android build failed. See $log (exit $($process.ExitCode))" }
Write-Output "APK: $apk"
