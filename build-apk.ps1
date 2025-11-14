Write-Host "=== SIMPLE APK BUILD ===" -ForegroundColor Green

# Clean
Write-Host "1. Cleaning project..." -ForegroundColor Yellow
dotnet clean

# Restore
Write-Host "2. Restoring packages..." -ForegroundColor Yellow
dotnet restore

# Build APK (самый надежный способ)
Write-Host "3. Building APK..." -ForegroundColor Green
dotnet build -c Release -f net9.0-android -p:AndroidPackageFormat=apk

# Find APK
$ApkFiles = Get-ChildItem -Path "bin\Release\net9.0-android" -Filter "*.apk" -Recurse

if ($ApkFiles) {
    Write-Host "✅ APK successfully created!" -ForegroundColor Green
    foreach ($ApkFile in $ApkFiles) {
        Write-Host "📱 APK: $($ApkFile.FullName)" -ForegroundColor Cyan
        Write-Host "📊 Size: $([math]::Round($ApkFile.Length/1MB, 2)) MB" -ForegroundColor Cyan
    }
    
    # Open folder
    Invoke-Item "bin\Release\net9.0-android"
} else {
    Write-Host "❌ APK not found!" -ForegroundColor Red
    Write-Host "Trying alternative build method..." -ForegroundColor Yellow
    
    # Альтернативный способ
    dotnet publish -c Release -f net9.0-android -r android-arm64 -p:AndroidPackageFormat=apk
    
    $ApkFiles = Get-ChildItem -Path "bin\Release\net9.0-android" -Filter "*.apk" -Recurse
    if ($ApkFiles) {
        Write-Host "✅ APK created with publish method!" -ForegroundColor Green
        foreach ($ApkFile in $ApkFiles) {
            Write-Host "📱 APK: $($ApkFile.FullName)" -ForegroundColor Cyan
        }
    } else {
        Write-Host "❌ APK creation failed completely!" -ForegroundColor Red
    }
}