module ExternalDataModelling

open System

open Thoth.Json.Net

//=============================================================================
// Raw eWay CRM <-> My app contact data 
//=============================================================================

// DTO
//*********************************************

type ContactDto =
    { 
        Name2: string option
        Email1Address: string option
        Phone: string option
        CompanyName: string option 
    }

let contactDtoDecoder : Decoder<ContactDto> =
    Decode.object
        (fun get 
            ->
            { 
                Name2 = get.Optional.Field "Name2" Decode.string
                Email1Address = get.Optional.Field "Email1Address" Decode.string
                Phone = get.Optional.Field "Phone" Decode.string
                CompanyName = get.Optional.Field "CompanyName" Decode.string 
            }
        )


// Domain model
//*********************************************

type Contact =
    { 
        Name2: string
        Email: string
        Phone: string
        Company: string 
    }


// Transformation Layer 
//*********************************************

module ContactTransform =
    
    let internal  toDomain (dto: ContactDto) : Contact =
        { 
            Name2 = dto.Name2 |> Option.defaultValue String.Empty
            Email = dto.Email1Address |> Option.defaultValue String.Empty
            Phone = dto.Phone |> Option.defaultValue String.Empty
            Company = dto.CompanyName |> Option.defaultValue String.Empty
        }
    
    let internal toJson (contacts: Contact list) : string =
        Encode.toString 2
            (
                Encode.object 
                    [
                        "Data",
                        Encode.list
                            (
                                contacts 
                                |> List.map 
                                    (fun c
                                      ->
                                        Encode.object 
                                            [
                                                "Name2", Encode.string c.Name2
                                                "Email", Encode.string c.Email
                                                "Phone", Encode.string c.Phone
                                                "Company", Encode.string c.Company
                                            ]
                                )
                            )
                    ]
            )