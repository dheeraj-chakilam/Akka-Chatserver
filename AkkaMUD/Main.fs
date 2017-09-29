module AkkaMUD.Main

open Akka.FSharp
open World
open Network

let beatrate = 100.

let config =
    (Akka.Configuration.ConfigurationFactory.ParseString
        @"
        akka {
        suppress-json-serializer-warning = on
        }").WithFallback(Configuration.defaultConfig())

[<EntryPoint>]
let main argv =
    match argv with
    | [|id; n; port|] ->
        let system = System.create "system" config
        let roomRef = spawn system "room" room
        let serverRef = spawn system "server" (server roomRef ChatServer (20000 + int id) (int id) (int n))
        let mServerRef = spawn system "master-server" (server roomRef MasterServer (int port) (int id) (int n))
        
        system.Scheduler.ScheduleTellRepeatedly(System.TimeSpan.FromMilliseconds <| 0.,
                                                    System.TimeSpan.FromMilliseconds <| beatrate,
                                                    roomRef,
                                                    SendHeartbeat id)

    | _ -> printfn "Incorrect arguments (%A)" argv
    
    while true do
        ()
    0