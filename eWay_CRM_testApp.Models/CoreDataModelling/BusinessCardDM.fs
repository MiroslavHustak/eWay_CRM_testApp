module CoreDataModelling

open Types
open Helpers
open IO_MonadSimulation
open ExternalDataModelling 

//=============================================================================
// eWay CRM (Transformation -> My business card <-> transformed eWay CRM contact data)
//=============================================================================

// DTO
//*********************************************
type internal ContactDto =
    {
        FirstName: string
        LastName: string
        FullName: string
        Email: string
        Phone: string
        Street: string
        City: string
        State: string
        PostalCode: string
        FullAddress: string
        Photo: string option
    }

// Domain Model
//*********************************************
type internal BusinessCard =
    {
        Name: Name
        Position: Position
        CompanyName: CompanyName
        Address: Address
        Phone: Phone
        Email: Email
        Photo: PhotoPath 
    }

let internal businessCardDefault =
    { 
        Name = Name "N/A"
        Position = Position "N/A"
        CompanyName = CompanyName "N/A"
        Address = Address "N/A"
        Phone = Phone "N/A"
        Email = Email "N/A"
        Photo = PhotoPath (randomPlaceholderPhotoPath >> runIO <| ()) 
    }

// Transformation Layer (I know, not actually necessary now, but may come in handy in the future)
//*********************************************
let private toDto (contact: ExternalDataModelling.Contact) : ContactDto =
    {
        FirstName = contact.FirstName
        LastName = contact.LastName
        FullName = contact.FullName
        Email = contact.Email
        Phone = contact.Phone
        Street = contact.Street
        City = contact.City
        State = contact.State
        PostalCode = contact.PostalCode
        FullAddress = contact.FullAddress
        Photo = contact.ProfilePicture
    }

let internal toBusinessCard (contact: ExternalDataModelling.Contact) : BusinessCard =

    let dto = toDto contact 
    
    { 
        Name = dto.FullName |> Name
        Position = Position "N/A"
        CompanyName = CompanyName "N/A"
        Address = dto.FullAddress |> Address
        Phone = dto.Phone |> Phone
        Email = dto.Email |> Email
        Photo = 
            dto.Photo 
            |> Option.defaultWith (fun () -> randomPlaceholderPhotoPath >> runIO <| ())
            |> PhotoPath
    }