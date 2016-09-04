$target = $args[0]
$rest = $args[1..$args.Length]
if ([String]::IsNullOrEmpty($target)) {
    $target = "Default"
}

& ".\packages\FAKE\tools\Fake.exe" "build.fsx" ("target=" + $target) $rest