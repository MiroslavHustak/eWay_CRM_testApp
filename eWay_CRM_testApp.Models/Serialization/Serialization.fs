module Serialization
  
open System.IO

open Thoth.Json.Net
open FsToolkit.ErrorHandling

open ErrorTypes
open IO_MonadSimulation

//********************************************************

let private safeFullPathResult path =

    IO (fun () ->   
        try
            Path.GetFullPath path
            |> Option.ofNullEmpty 
            |> Option.toResult FileNotExisting  
        with
        | _ -> Error FileNotExisting
    )
      
let internal serializeWithThothAsync (emails: string list) (path : string) =

    IO (fun () ->   
        try   
            let json: string =
                emails
                |> List.map Encode.string
                |> Encode.list
                |> Encode.toString 4

            asyncResult 
                {
                    let! path = safeFullPathResult >> runIO <| path                                
                    use writer = new StreamWriter(path, append = false)
                    return! writer.WriteAsync json |> Async.AwaitTask
                }
        with
        | _ -> async { return Error SerializationError }
    )

let internal deserializeWithThothAsync (path: string) =

    IO (fun () ->   
        try 
            asyncResult
                {
                    let! fullPath = safeFullPathResult >> runIO <| path
        
                    // TODO: Verify TOCTOU effect
                    do! 
                        File.Exists fullPath 
                        |> Result.fromBool fullPath FileNotExisting
                        |> Result.ignore<string, Errors>
        
                    use reader = new StreamReader(fullPath)
                    let! json = reader.ReadToEndAsync() |> Async.AwaitTask
        
                    let! emails = 
                        Decode.fromString (Decode.list Decode.string) json
                        |> Result.mapError (fun _ -> DeserializationError)
        
                    return emails
                }
        with
        | _ -> async { return Error DeserializationError }
    )