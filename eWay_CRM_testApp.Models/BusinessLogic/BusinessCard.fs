module BusinessLogic

open Newtonsoft.Json.Linq

open Thoth.Json.Net
open FsToolkit.ErrorHandling

open Types
open Helpers
open Connection
open CoreDataModelling
open ExternalDataModelling

//***************************************************************
 
let private searchContactsByEmail (email: string) =
         
    withConnection 
        (fun conn
            ->
            option
                {
                    let! email = isValidEmail email

                    let transmitObject = JObject()
                    transmitObject.Add("Email1Address", JValue email)
 
                    let request = JObject()
                    request.Add("transmitObject", transmitObject)
                    request.Add("includeProfilePictures", JValue false)

                    let! response = conn.CallMethod("SearchContacts", request) |> Option.ofNull
                    let! data = response.["Data"] |> Option.ofNull
                    let! dataStr = data.ToString() |> Option.ofNull
                    let! dtos = Decode.fromString (Decode.list contactDtoDecoder) dataStr |> Result.toOption
            
                    return 
                        dtos
                        |> List.map ContactTransform.toDomain
                        |> ContactTransform.toJson
                }
        )

let internal getUniqueData email =

    result 
        {
            let! json = searchContactsByEmail email |> Option.toResult "Nebylo možno získat data. Ověř připojení k CRM."//"""{ "Data": [] }"""
            let! contacts = Decode.fromString contactsDecoder json

            return!  
                contacts
                |> List.map toBusinessCard
                |> List.tryFind 
                    (fun card
                        ->
                        let (Email e) = card.Email
                        e = email
                    )
                |> Option.toResult "Nebylo možno získat data z CRM na základě příslušného emailu."
        }