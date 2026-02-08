module ImageHelper

open System
open System.IO

open FsToolkit.ErrorHandling

//********************************************************

open Helpers
open Settings
open IO_MonadSimulation

//********************************************************

let internal saveBase64ImageToFile (base64String: string) (email: string) =
    IO (fun () ->
        try
            option
                {
                    let! validBase64 = base64String |> Option.ofNullEmptySpace                
                    let! validEmail = isValidEmail email |> runIO
                
                    // Decode base64
                    let! imageBytes =
                        try
                            let bytes = Convert.FromBase64String <| validBase64.Trim()

                            match bytes.Length with
                            | 0 -> None
                            | _ -> Some bytes
                        with
                        | _  -> None
                
                    let! dirInfo =
                        try
                            let tempDir = Path.Combine(Path.GetTempPath(), pathToCRM_Photos)
                            let dir = Directory.CreateDirectory tempDir

                            match dir.Exists with  //TODO: verify TOCTOU effect
                            | true  -> Some dir
                            | false -> None
                        with
                        | _  -> None
                
                    let sanitizedEmail = 
                        validEmail
                        |> Seq.filter (fun c -> Char.IsLetterOrDigit(c) || c = '@' || c = '.')
                        |> Seq.toArray
                        |> String
                
                    let fileBaseName = 
                        match sanitizedEmail with
                        | s when s = String.Empty                        
                            -> "unknown"
                        | s -> s
                
                    let fileName = sprintf "%s_%s.png" fileBaseName (Guid.NewGuid().ToString("N").Substring(0, 8))
                    let filePath = Path.Combine(dirInfo.FullName, fileName)
                
                    let! finalPath =
                        try
                            match filePath.Length with
                            | len 
                                when len > pathTooLongLimit 
                                ->
                                let shortFileName = sprintf "%s.png" (Guid.NewGuid().ToString("N"))
                                let shortPath = Path.Combine(dirInfo.FullName, shortFileName)
                                File.WriteAllBytes(shortPath, imageBytes)
                                Some shortPath
                            | _ ->
                                File.WriteAllBytes(filePath, imageBytes)
                                Some filePath
                        with
                        | _ -> None             
                                
                    return finalPath
                }
            with
            | _ -> None
    )