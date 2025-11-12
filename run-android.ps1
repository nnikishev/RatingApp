Write-Host "=== ЗАПУСК ANDROID ПРИЛОЖЕНИЯ ===" -ForegroundColor Green

# Настройка путей
$env:ANDROID_HOME = "C:\Program Files (x86)\Android\android-sdk"
$env:PATH = "$env:PATH;$env:ANDROID_HOME\emulator;$env:ANDROID_HOME\platform-tools"

Write-Host "1. Проверка эмуляторов..." -ForegroundColor Yellow
$avds = & "$env:ANDROID_HOME\emulator\emulator.exe" -list-avds

if ($avds.Count -eq 0) {
    Write-Host "Создаем эмулятор..." -ForegroundColor Yellow
    & "$env:ANDROID_HOME\cmdline-tools\latest\bin\avdmanager.bat" create avd -n "maui_android" -k "system-images;android-33;google_apis;x86_64" -d "pixel_4"
    $avdName = "maui_android"
} else {
    $avdName = $avds[0]
    Write-Host "Используем эмулятор: $avdName" -ForegroundColor Green
}

Write-Host "2. Запуск эмулятора..." -ForegroundColor Yellow
Start-Process -FilePath "$env:ANDROID_HOME\emulator\emulator.exe" -ArgumentList "-avd", $avdName, "-no-audio", "-gpu", "swiftshader"

Write-Host "3. Ждем запуска эмулятора (30 сек)..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

Write-Host "4. Проверка подключения..." -ForegroundColor Yellow
& "$env:ANDROID_HOME\platform-tools\adb.exe" devices

Write-Host "5. Запуск приложения..." -ForegroundColor Green
dotnet build -t:Run -f net9.0-android

Write-Host "Готово! Приложение должно запуститься на эмуляторе." -ForegroundColor Cyan