// Add this to your Helpers module or create a new ImageHelpers module
module ImageHelper

open System
open System.IO
open System.Windows.Media.Imaging

let base64ToBitmapImage (base64String: string) : BitmapImage option =
    try
        // Convert base64 string to byte array
        let imageBytes = Convert.FromBase64String(base64String)
            
        // Create bitmap from bytes
        use ms = new MemoryStream(imageBytes)
        let bitmap = BitmapImage()
        bitmap.BeginInit()
        bitmap.CacheOption <- BitmapCacheOption.OnLoad
        bitmap.StreamSource <- ms
        bitmap.EndInit()
        bitmap.Freeze()  // Important for cross-thread access in WPF
            
        Some bitmap
    with
    | ex -> 
        System.Diagnostics.Debug.WriteLine(sprintf "Error converting base64 to image: %s" ex.Message)
        None

// Save base64 image to a temporary file and return the path
let saveBase64ImageToFile (base64String: string) (email: string) : string option =
    try
        let imageBytes = Convert.FromBase64String(base64String)
            
        // Create temp directory if it doesn't exist
        let tempDir = Path.Combine(Path.GetTempPath(), "eWayCRM_Photos")
        Directory.CreateDirectory(tempDir) |> ignore
            
        // Create unique filename based on email
        let sanitizedEmail = 
            email
            |> Seq.filter (fun c -> Char.IsLetterOrDigit(c) || c = '@' || c = '.')
            |> Seq.toArray
            |> String
            
        let fileName = sprintf "%s_%s.png" sanitizedEmail (DateTime.Now.Ticks.ToString())
        let filePath = Path.Combine(tempDir, fileName)
            
        // Save to file
        File.WriteAllBytes(filePath, imageBytes)
            
        Some filePath
    with
    | ex -> 
        System.Diagnostics.Debug.WriteLine(sprintf "Error saving image: %s" ex.Message)
        None