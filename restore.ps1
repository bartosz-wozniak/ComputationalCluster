if (-not (Test-Path ".\.paket\paket.exe")) {
    & ".\.paket\paket.bootstrapper.exe"
}
& ".\.paket\paket.exe" restore