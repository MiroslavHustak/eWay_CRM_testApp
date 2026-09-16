module EmailHistoryModelling

open Settings
open Serialization
open ImpureWrapper

// DTM
//*********************************************
type internal EmailHistoryDtm =
    {
        Emails : string list
    }

// Domain Model
//*********************************************
type internal EmailHistoryDm =
    {
        TruncatedEmailList : string list
    }

// Transformation Layer 
//*********************************************
let internal fromDtm () =
    Impure (fun () ->
        async
            {
                let! emailList = deserializeWithThothAsync >> runImpure <| pathToJson   

                match emailList with
                | Ok emails
                    ->                             
                    let truncatedEmailList = { TruncatedEmailList = emails |> List.truncate maxListBoxItems } 
                    return Ok truncatedEmailList.TruncatedEmailList  
                                      
                | Error err 
                    ->
                    return Error err
            } 
    )

let internal toDtm newEmails =
    Impure (fun () ->
        let newEmails = { Emails = newEmails }     
        runImpure <| serializeWithThothAsync newEmails.Emails pathToJson    
    )