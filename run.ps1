param([switch]$Initialize,[switch]$Seed)
$ErrorActionPreference='Stop'
$env:DOTNET_CLI_HOME=Join-Path $PSScriptRoot '.dotnet'
$env:NUGET_PACKAGES=Join-Path $PSScriptRoot '.nuget/packages'
Push-Location $PSScriptRoot
try {
    if($Initialize) {
        $setupArgs=@('run','--project','Schoolnet.WEBUI','--','--initialize')
        if($Seed) { $setupArgs+='--seed' }
        & dotnet @setupArgs
        if($LASTEXITCODE -ne 0) { throw 'Database initialization failed' }
    } else {
        & dotnet run --project Schoolnet.WEBUI
    }
} finally { Pop-Location }
