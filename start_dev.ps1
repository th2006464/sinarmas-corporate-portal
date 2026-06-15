# 企业门户 - 本地开发启动脚本
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:PORT = '5000'

Write-Host "正在启动企业门户..."
Start-Process -FilePath ".\release\PortalStarter.exe" -NoNewWindow -WorkingDirectory (Get-Location)

Write-Host "应用已启动，浏览器将自动打开 http://localhost:5000"
