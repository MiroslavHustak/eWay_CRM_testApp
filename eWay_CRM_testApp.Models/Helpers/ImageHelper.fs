module ImageHelper

open System
open System.IO

open IO_MonadSimulation

let internal saveBase64ImageToFile (base64String: string) (email: string) =

    IO ( fun () ->
        try
            let imageBytes = Convert.FromBase64String base64String
            
            let tempDir = Path.Combine(Path.GetTempPath(), "eWayCRM_Photos")
            Directory.CreateDirectory(tempDir) |> ignore
            
            let uniqueEmail = 
                email
                |> Seq.filter (fun c -> Char.IsLetterOrDigit(c) || c = '@' || c = '.')
                |> Seq.toArray
                |> String
            
            let fileName = sprintf "%s_%s.png" uniqueEmail (DateTime.Now.Ticks.ToString())
            let filePath = Path.Combine(tempDir, fileName)
            
            File.WriteAllBytes(filePath, imageBytes)
            
            Some filePath
        with
        | ex -> 
            //System.Diagnostics.Debug.WriteLine(sprintf "Error saving image: %s" ex.Message)
            None
    )