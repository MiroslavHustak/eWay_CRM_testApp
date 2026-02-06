module Serialization
  
open System.IO

open Thoth.Json.Net
open FsToolkit.ErrorHandling
 
//open Types.Haskell_IO_Monad_Simulation

let internal safeFullPathResult path =
        
    try
        Path.GetFullPath path
        |> Option.ofNullEmpty 
        |> Option.toResult "Failed getting path"  
    with
    | ex -> Error <| sprintf "Path is invalid: %s" (string ex.Message)
      
let internal serializeWithThothAsync (emails: string list) (path : string) : Async<Result<unit, string>> =
       
    try   
        let json: string =
            emails
            |> List.map Encode.string
            |> Encode.list
            |> Encode.toString 4

        asyncResult 
            {
                let! path = safeFullPathResult path                                
                use writer = new StreamWriter(path, append = false)
                return! writer.WriteAsync json |> Async.AwaitTask
            }
    with
    | ex -> async { return Error <| string ex.Message }


let internal deserializeWithThothAsync (path: string) : Async<Result<string list, string>> =

    asyncResult
        {
            let! fullPath = safeFullPathResult path
        
            // TODO: Verify TOCTOU effect
            do! File.Exists fullPath 
                |> Result.fromBool fullPath "File does not exist" 
                |> Result.ignore
        
            use reader = new StreamReader(fullPath)
            let! json = reader.ReadToEndAsync() |> Async.AwaitTask
        
            let! emails = 
                Decode.fromString (Decode.list Decode.string) json
                |> Result.mapError (sprintf "Failed to decode: %s")
        
            return emails
        }