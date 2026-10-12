$ErrorActionPreference='Stop'
$taskRoot=Split-Path $PSScriptRoot -Parent
$config=Get-Content -Raw -LiteralPath (Join-Path $taskRoot 'Schoolnet.WEBUI/appsettings.Local.json') | ConvertFrom-Json
$connection=$config.ConnectionStrings.Training
$env:PGPASSWORD=[regex]::Match($connection,'(?:^|;)Password=([^;]*)').Groups[1].Value
$pgHost=[regex]::Match($connection,'(?:^|;)Host=([^;]*)').Groups[1].Value
$pgUser=[regex]::Match($connection,'(?:^|;)Username=([^;]*)').Groups[1].Value
$pgDb=[regex]::Match($connection,'(?:^|;)Database=([^;]*)').Groups[1].Value
try {
 & 'C:/Program Files/PostgreSQL/18/bin/psql.exe' -h $pgHost -U $pgUser -d $pgDb -w -f (Join-Path $PSScriptRoot 'verify-database.sql')
 if($LASTEXITCODE -ne 0) { throw 'Database acceptance tests failed' }
} finally { Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue }
