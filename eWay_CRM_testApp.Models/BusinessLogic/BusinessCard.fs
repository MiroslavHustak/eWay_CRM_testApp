module BusinessLogic

open eWayCRM.API
open Newtonsoft.Json.Linq

open Thoth.Json.Net
open FsToolkit.ErrorHandling

open Types
open Helpers
open ErrorTypes
open Connection
open CoreDataModelling
open IO_MonadSimulation
open ExternalDataModelling

//***************************************************************
 
let private searchContactsByEmail (email: string) =

    withConnection
        (fun (conn: Connection)
            -> 
            option
                {
                    let! email = isValidEmail >> runIO <| email
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

    IO (fun () ->    
        result 
            {
                let! json = 
                    searchContactsByEmail email
                    |> runIO
                    |> Option.toResult ConnectionError
                
                let! contacts = 
                    Decode.fromString contactsDecoder json
                    |> Result.mapError (fun _ -> DeserializationError)
                
                return!  
                    contacts
                    |> List.map toBusinessCard
                    |> List.tryFind 
                        (fun card ->
                            let (Email e) = card.Email
                            e = email
                        )
                    |> Option.toResult CRMError
            }
    )