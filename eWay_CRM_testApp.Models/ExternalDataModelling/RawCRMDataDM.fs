module ExternalDataModelling

open System
open Thoth.Json.Net

open IO_MonadSimulation

//=============================================================================
// Raw eWay CRM <-> My app contact data 
//=============================================================================

// DTO
//*********************************************
type ContactDto =
    { 
        FirstName: string option
        LastName: string option
        Email1Address: string option
        TelephoneNumber1: string option
        BusinessAddressStreet: string option
        BusinessAddressCity: string option
        BusinessAddressState: string option
        BusinessAddressPostalCode: string option
        Company: string option
        ProfilePicture: string option  
        ProfilePictureWidth: int option
        ProfilePictureHeight: int option
    }

let contactDtoDecoder : Decoder<ContactDto> =
    Decode.object
        (fun get 
            ->
            { 
                FirstName = get.Optional.Field "FirstName" Decode.string
                LastName = get.Optional.Field "LastName" Decode.string
                Company = get.Optional.Field "Company" Decode.string
                Email1Address = get.Optional.Field "Email1Address" Decode.string
                TelephoneNumber1 = get.Optional.Field "TelephoneNumber1" Decode.string
                BusinessAddressStreet = get.Optional.Field "BusinessAddressStreet" Decode.string
                BusinessAddressCity = get.Optional.Field "BusinessAddressCity" Decode.string
                BusinessAddressState = get.Optional.Field "BusinessAddressState" Decode.string
                BusinessAddressPostalCode = get.Optional.Field "BusinessAddressPostalCode" Decode.string
                ProfilePicture = get.Optional.Field "ProfilePicture" Decode.string
                ProfilePictureWidth = get.Optional.Field "ProfilePictureWidth" Decode.int
                ProfilePictureHeight = get.Optional.Field "ProfilePictureHeight" Decode.int
            }
        )

// Domain model
//*********************************************
type Contact =
    { 
        FirstName: string
        LastName: string
        FullName: string
        Company: string
        Email: string
        Phone: string
        Street: string
        City: string
        State: string
        PostalCode: string
        FullAddress: string
        ProfilePicture: string option 
    }

// Transformation Layer 
//*********************************************
module ContactTransform =

    open ImageHelper  
          
    let internal toDomain (dto: ContactDto) : Contact =        
    
        let firstName = dto.FirstName |> Option.defaultValue String.Empty
        let lastName = dto.LastName |> Option.defaultValue String.Empty
        let email = dto.Email1Address |> Option.defaultValue String.Empty
        let company = dto.Company |> Option.defaultValue String.Empty
        let street = dto.BusinessAddressStreet |> Option.defaultValue String.Empty
        let city = dto.BusinessAddressCity |> Option.defaultValue String.Empty
        let state = dto.BusinessAddressState |> Option.defaultValue String.Empty
        let postalCode = dto.BusinessAddressPostalCode |> Option.defaultValue String.Empty
        let phone = dto.TelephoneNumber1 |> Option.defaultValue String.Empty 
        
        let fullName = 
            match firstName.Trim(), lastName.Trim() with
            | s1, s2 
                when s1 = String.Empty && s2 = String.Empty
                -> "N/A"
            | first, s 
                when s = String.Empty
                -> first
            | s, last 
                when s = String.Empty
                 -> last               
            | first, last
                -> sprintf "%s %s" first last        
       
        let fullAddress =
            [ street; city; state; postalCode ]
            |> List.choose Option.ofNullEmptySpace
            |> String.concat ", "
            |> function s when s = String.Empty -> "N/A" | s -> s
        
        let photoPath =
            dto.ProfilePicture
            |> Option.bind (fun base64 -> runIO <| saveBase64ImageToFile base64 email)
        
        { 
            FirstName = firstName
            LastName = lastName
            FullName = fullName
            Company = company
            Email = email
            Phone = phone
            Street = street
            City = city
            State = state
            PostalCode = postalCode
            FullAddress = fullAddress
            ProfilePicture = photoPath  // file path option
        }