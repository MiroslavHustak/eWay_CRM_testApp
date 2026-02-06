module CoreDataModelling

open Thoth.Json.Net

open Types
open Helpers
open IO_MonadSimulation

//=============================================================================
// eWay CRM (Transformation -> My business card <-> transformed eWay CRM contact data )
//=============================================================================

 // DTO
//*********************************************

type internal ContactDto =
    {
        Name: string option
        Email: string option
        Phone: string option
        Company: string option
        Title: string option
        Address: string option
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

// Transformation Layer 
//*********************************************

let private contactDecoder : Decoder<ContactDto> =
    Decode.object
        (fun get
            ->
            {
                Name = get.Optional.Field "Name" Decode.string
                Email = get.Optional.Field "Email" Decode.string
                Phone = get.Optional.Field "Phone" Decode.string
                Company = get.Optional.Field "Company" Decode.string
                Title = get.Optional.Field "Title" Decode.string
                Address = get.Optional.Field "Address" Decode.string
                Photo = get.Optional.Field "Photo" Decode.string 
            }
        )

let internal contactsDecoder : Decoder<ContactDto list> =
    Decode.field "Data" (Decode.list contactDecoder)

let private orDefault def = Option.defaultValue def

let internal toBusinessCard (dto: ContactDto) : BusinessCard =
    { 
        Name = dto.Name |> orDefault "N/A" |> Name
        Position = dto.Title |> orDefault "N/A" |> Position
        CompanyName = dto.Company |> orDefault "N/A" |> CompanyName
        Address = dto.Address |> orDefault "N/A" |> Address
        Phone = dto.Phone |> orDefault "N/A" |> Phone
        Email = dto.Email |> orDefault "N/A" |> Email
        Photo = dto.Photo |> orDefault (randomPlaceholderPhotoPath >> runIO <| ()) |> PhotoPath 
    }