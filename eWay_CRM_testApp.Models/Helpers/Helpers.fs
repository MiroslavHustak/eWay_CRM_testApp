module Helpers

open FsToolkit.ErrorHandling
open System.Text.RegularExpressions

open IO_MonadSimulation

//Only domains with 2-3 letter TLDs are allowed 
let private emailRegex =
    IO (fun () ->   
        try
            Regex(
                @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,3}$",
                RegexOptions.IgnoreCase ||| RegexOptions.Compiled
            ) |> Option.ofNull
        with
        | _ -> None
    )

let internal isValidEmail (email: string) =
    IO (fun () ->
        option
            {
                let! emailRegex = runIO emailRegex
                let! _ = email |> Option.ofNullEmptySpace
                let! _ = emailRegex.IsMatch email |> Option.ofBool

                return! email
            }
    )

let internal randomPlaceholderPhotoPath () = 
    IO (fun () ->
        let placeholders = [| "placeholder1.jpg"; "placeholder2.jpg" |]
        let random = System.Random()
        placeholders.[random.Next(placeholders.Length)]
    )