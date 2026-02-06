module Connection

open eWayCRM.API
open Newtonsoft.Json.Linq

//*****************************************************************************

let [<Literal>] internal SERVICE_URL = "https://free.eway-crm.com/31994"
let [<Literal>] internal USERNAME = "api"
let [<Literal>] internal PASSWORD = "ApiTrial@eWay-CRM"
let [<Literal>] internal APP_ID = "ExampleApplication100"
 
let private establishConnection() = new Connection(SERVICE_URL, USERNAME, Connection.HashPassword(PASSWORD), APP_ID)

//*****************************************************************************

let internal withConnection (f: Connection -> 'a option) =  //new connection with each call
    try
        let conn = establishConnection()
        try
            f conn
        finally
            try 
                conn.LogOut() |> ignore<JObject>
            with
            | _ -> ()
    with
    | _ -> None

//****************************************************************************

//Trvalé připojení zatím neřešeno, neb bych musel přidávat kontrolky a zabývat se opět UX/UI/FE.
//Pokud potřebujete vidět, jak to řeším, kód naleznete např. v této aplikaci:
//https://github.com/MiroslavHustak/OdisTimetableDownloaderMAUI

let private connectionInstance = lazy (establishConnection())  //connection kept

let internal withConnection2 (f: Connection -> 'a option) =
    try
        f connectionInstance.Value
    with
    | _ -> None

let internal cleanup() =  //Not used yet

    match connectionInstance.IsValueCreated with
    | false 
        -> ()
    | true 
        ->
        try 
            connectionInstance.Value.LogOut() |> ignore<JObject>
        with 
        | _ -> ()