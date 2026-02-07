module EmailHistoryModelling

open Settings
open Serialization
open IO_MonadSimulation

//=============================================================================
// DTO 
//=============================================================================

type internal EmailHistoryDto =
    {
        Emails : string list
    }

//=============================================================================
// Domain Model
//=============================================================================

type internal EmailHistoryDm =
    {
        TruncatedEmailList : string list
    }

//=============================================================================
// Transformation Layer 
//=============================================================================

let internal fromDto () =
    IO (fun () ->
        async
            {
                let! emailList = deserializeWithThothAsync >> runIO <| pathToJson   

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

let internal toDto newEmails =
    IO (fun () ->
        let newEmails = { Emails = newEmails }     
        runIO <| serializeWithThothAsync newEmails.Emails pathToJson    
    )