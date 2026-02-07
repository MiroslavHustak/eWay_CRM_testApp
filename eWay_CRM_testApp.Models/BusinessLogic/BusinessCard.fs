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
                    request.Add("includeProfilePictures", JValue true)
                    
                    let! response = conn.CallMethod("SearchContacts", request) |> Option.ofNull
                    
                    (*
                    System.Diagnostics.Debug.WriteLine("=== FULL RESPONSE ===")
                    System.Diagnostics.Debug.WriteLine(response.ToString())
                    System.Diagnostics.Debug.WriteLine("=== END RESPONSE ===")
                    *)
                    
                    let! data = response.["Data"] |> Option.ofNull
                    
                    (*
                    System.Diagnostics.Debug.WriteLine("=== DATA ARRAY ===")
                    System.Diagnostics.Debug.WriteLine(data.ToString())
                  
                    let!_ = 
                        data.HasValues 
                        |> Option.fromBool
                            (                                
                                let firstContact = data.First :?> JObject
                                System.Diagnostics.Debug.WriteLine("=== AVAILABLE FIELDS ===")
                               
                                firstContact.Properties()
                                |> Seq.iter 
                                    (fun prop 
                                        ->
                                        prop.Value.ToString()
                                        |> fun v -> match v.Length > 100 with true -> v.Substring(0, 100) + "..." | false -> v
                                        |> sprintf "Field: %s = %s" prop.Name
                                        |> System.Diagnostics.Debug.WriteLine
                                )
                                
                                System.Diagnostics.Debug.WriteLine("=== END FIELDS ===")
                            )
                    *)

                    let! dataStr = data.ToString() |> Option.ofNull
                    let! dtos = Decode.fromString (Decode.list contactDtoDecoder) dataStr |> Result.toOption
            
                    return 
                        dtos
                        |> List.map ContactTransform.toDomain
                }
        )

let internal getUniqueData email =

    IO (fun () ->    
        result 
            {
                let! contacts = 
                    searchContactsByEmail email
                    |> runIO
                    |> Option.toResult ConnectionError
                
                return!  
                    contacts
                    |> List.map toBusinessCard
                    |> List.tryFind 
                        (fun card 
                            ->
                            let (Email e) = card.Email
                            e = email
                        )
                    |> Option.toResult CRMError
            }
    )