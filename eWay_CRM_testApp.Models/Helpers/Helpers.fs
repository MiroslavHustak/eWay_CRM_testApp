module Helpers

open System

open FsToolkit.ErrorHandling
open System.Text.RegularExpressions

open ImpureWrapper

//Only domains with 2-3 letter TLDs are allowed 
let private emailRegex =
    Impure (fun () ->   
        try
            Regex
                (
                    @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,3}$",
                        RegexOptions.IgnoreCase ||| RegexOptions.Compiled
                )
            |> Option.ofNull'
        with
        | _ -> None
    )

let internal isValidEmail (email: string) =
    Impure (fun () ->
        try
            option
                {
                    let! emailRegex = runImpure emailRegex
                    let! _ = email |> Option.ofNullEmptySpace
                    let! _ = emailRegex.IsMatch email |> Option.ofBool

                    return! email
                }
        with
        | _ -> None
    )

let internal randomPlaceholderPhotoPath () =
    let placeholders =
        [|
            @"Resources/placeholder1.jpg"
            @"Resources/placeholder2.jpg" 
        |]
    
    Impure (fun () ->    
        try
            placeholders 
            |> Array.item (Random().Next (placeholders |> Array.length)) 
        with
        | _ -> @"Resources/placeholder1.jpg"   
    )