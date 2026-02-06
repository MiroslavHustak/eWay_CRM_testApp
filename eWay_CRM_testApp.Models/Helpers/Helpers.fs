module Helpers

open FsToolkit.ErrorHandling

open System.Text.RegularExpressions

//Only domains with 2-3 letter TLDs are allowed 
let private emailRegex =
    try
        Regex(
            @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,3}$",
            RegexOptions.IgnoreCase ||| RegexOptions.Compiled
        ) |> Option.ofNull
    with
    | _ -> None

let internal isValidEmail (email: string) =
    option
        {
            let! emailRegex = emailRegex
            let! _ = email |> Option.ofNullEmptySpace
            let! _ = emailRegex.IsMatch email |> Option.ofBool

            return! email
        }