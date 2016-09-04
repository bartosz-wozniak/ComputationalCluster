#r @"packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.TeamCityHelper
open Fake.FixieHelper
open Fake.TargetHelper

let isTeamCityBuild = TeamCityVersion.IsSome

let on = "on" :> obj
let off = "off" :> obj

let fixieConfig (p : FixieParams) =
    { p with
        ToolPath = findToolInSubPath "Fixie.Console.exe" (currentDirectory @@ "packages" @@ "Fixie")
        CustomOptions = [ "TeamCity", if isTeamCityBuild then on else off ]
    }

[<Literal>]
let BuildDir = "./build"

Target "Clean" (fun _ ->
    trace "Cleaning build directory.."
    CleanDir BuildDir
)

Target "Build" (fun _ ->
    trace "Building projects..."
    !! "src/**/*.csproj"
    |> MSBuildRelease BuildDir "Build"
    |> Log "Build:"
)

Target "StandaloneTest" (fun _ ->
    trace "Running unit tests"
    !! (BuildDir @@ "**/*.Tests.dll")
    |> Fixie fixieConfig
)

Target "Test" (fun _ ->
    run "StandaloneTest"
)

Target "Default" DoNothing

"Clean" ==> "Build" ==> "Test" ==> "Default"

RunTargetOrDefault "Default"