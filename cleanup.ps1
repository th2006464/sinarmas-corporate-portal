# 清理调试残留进程 — 解决卡死问题
Write-Host "正在清理残留进程..."

$killed = 0

# 杀掉所有 Portal 进程
Get-Process -Name "Portal" -ErrorAction SilentlyContinue | ForEach-Object {
    Stop-Process -Id $_.Id -Force
    Write-Host "  已终止 Portal (PID: $($_.Id), 内存: $([math]::Round($_.WorkingSet64/1MB,0))MB)"
    $killed++
}

# 杀掉所有 dotnet 进程
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | ForEach-Object {
    Stop-Process -Id $_.Id -Force
    Write-Host "  已终止 dotnet (PID: $($_.Id), 内存: $([math]::Round($_.WorkingSet64/1MB,0))MB)"
    $killed++
}

if ($killed -eq 0) {
    Write-Host "  没有残留进程，系统干净 ✓"
} else {
    Write-Host "  共清理 $killed 个残留进程 ✓"
}
