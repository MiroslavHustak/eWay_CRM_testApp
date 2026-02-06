module Connection

open eWayCRM.API
open Newtonsoft.Json.Linq

open Settings
open IO_MonadSimulation

//*****************************************************************************
 
let private establishConnection() = new Connection(SERVICE_URL, USERNAME, Connection.HashPassword(PASSWORD), APP_ID)

//*****************************************************************************

let internal withConnection (f: Connection -> 'a option) =  

    IO (fun () ->
        let rec tryWithRetry attemptsLeft =
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
            | _ when attemptsLeft > 0 
                -> 
                tryWithRetry (attemptsLeft - 1)
            | _ -> 
                None
        
        tryWithRetry 1  // 1 retry = 2 total attempts
    )
//****************************************************************************

//Trvalé připojení zatím neřešeno, neb bych musel přidávat connectivity listener, kontrolky a zabývat se opět UX/UI/FE.
//Pokud potřebujete vidět, jak jsem to kdysi řešil, kód naleznete např. tady:
//https://github.com/MiroslavHustak/OdisTimetableDownloaderMAUI/blob/master/Connectivity/Connectivity.fs

let private connectionInstance = IO (fun () -> lazy (establishConnection()))  //Not used yet

let internal withConnection2 (f: Connection -> 'a option) =
    IO (fun () ->
        try
            f (runIO connectionInstance).Value
        with
        | _ -> None
    )

let internal cleanup() =  //Not used yet
    IO (fun () ->
        match (runIO connectionInstance).IsValueCreated with
        | false 
            -> ()
        | true 
            ->
            try 
                (runIO connectionInstance).Value.LogOut() |> ignore<JObject>
            with 
            | _ -> ()
    )