namespace eWay_CRM_testApp.Models

open System

open Elmish
open Elmish.WPF

open FsToolkit.ErrorHandling

open Types
open Helpers
open CEBuilders
open BusinessLogic
open Serialization
open CoreDataModelling

//***************************************************************

module E_Way =

    type Model =
        { 
              MessageDisplayText: BusinessCard
              EmailInputString: string
              EmailAddresses: string list
              SelectedEmail: string option
              ErrorMessage: string option
        }  

    type Msg =
        | EmailInputStringChanged of string
        | EmailSelected of string option
        | EmailsLoaded of Result<string list, string>
        | EmailsSaved of Result<unit, string>
        | ShowData    
      
    let initialModel listOfEmails =
        {
            MessageDisplayText = businessCardDefault
            EmailInputString = String.Empty
            EmailAddresses = listOfEmails
            SelectedEmail = None
            ErrorMessage = None
        }

    let loadEmailsCmd () =
        Cmd.OfAsync.perform
            (fun () -> deserializeWithThothAsync "emails.json")
            ()
            EmailsLoaded  

    let saveEmailsCmd newEmails =
        Cmd.OfAsync.perform
            (fun () -> serializeWithThothAsync newEmails "emails.json")
            ()
            EmailsSaved
    
    let init () : Model * Cmd<Msg> =

        let initialEmailList =  //pro testovani aplikace
            [
                "mroyster@royster.com"
                "ealbares@gmail.com"
                "oliver@hotmail.com"
                //"michael.ostrosky@ostrosky.com"
                //"kati.rulapaugh@hotmail.com"            
            ]

        let initialEmailList = List.Empty  

        let m = initialModel initialEmailList  // Start with default/empty list
        m, loadEmailsCmd ()
    
    let update msg m =
        match msg with
        | EmailsLoaded (Ok emails) 
            ->
            // Replace the initial list with loaded emails
            { m with EmailAddresses = emails }, Cmd.none
        
        | EmailsLoaded (Error err) 
            ->
            // Keep using initialEmailList (already in model)
            { m with ErrorMessage = Some "Kontakty nebyly načteny." }, Cmd.none

        | EmailsSaved (Ok _) 
            ->
            m, Cmd.none

        | EmailsSaved (Error err) 
            ->
            { m with ErrorMessage = Some "Kontakty nebyly uloženy." }, Cmd.none

        | EmailInputStringChanged email
            ->
            let typedEmail = email.Trim()
        
            // Only update email list if valid
            let newEmails =
                pyramidOfDoom
                    {
                        let! validEmail = isValidEmail typedEmail, m.EmailAddresses
                        let cond = (validEmail <> String.Empty && not (m.EmailAddresses |> List.contains validEmail))
                        let! _ = cond |> Option.ofBool, m.EmailAddresses
                        return typedEmail :: m.EmailAddresses 
                    }
           
            let errorMsg =
                isValidEmail typedEmail
                |> Option.map (fun _ -> None)  // Valid -> None (no error)
                |> Option.defaultValue (Some "Chybný formát emailu.")  // Invalid -> Some error
        
            {
                m with
                    EmailInputString = typedEmail
                    EmailAddresses   = newEmails
                    MessageDisplayText =
                        getUniqueData typedEmail
                        |> Result.defaultValue { businessCardDefault with Email = Email "Chybný formát emailu." }                  
                    ErrorMessage = errorMsg
            }, saveEmailsCmd newEmails
    
        | EmailSelected emailOpt 
            ->
            match emailOpt with
            | Some email
                ->
                let valid = isValidEmail email
                let updatedModel =
                    { 
                        m with
                            SelectedEmail = Some email
                            EmailInputString = email
                            ErrorMessage = 
                                valid 
                                |> Option.map (fun _ -> None)
                                |> Option.defaultValue (Some "Chybný formát emailu.")
                    }
    
                match valid with
                | Some email  ->
                    // Update business card only if valid
                    { 
                        updatedModel with
                            MessageDisplayText =
                                getUniqueData email 
                                |> Result.defaultWith (fun err -> { businessCardDefault with Email = Email err } )  
                    }, Cmd.none
                | None ->
                    updatedModel, Cmd.none
    
            | None -> { m with SelectedEmail = None }, Cmd.none
    
        | ShowData 
            ->
            let typedEmail = m.EmailInputString.Trim()

            pyramidOfDamnation
                {
                    let! _ = 
                        (typedEmail <> String.Empty, 
                         ({ m with ErrorMessage = Some "Prosím zadejte platný email." }, Cmd.none))
                
                    let! _ = 
                        (isValidEmail typedEmail |> Option.toBool,
                         ({ m with ErrorMessage = Some "Chybný formát emailu." }, Cmd.none))
                
                    let updatedEmails =
                        match not (m.EmailAddresses |> List.contains typedEmail) with
                        | true  -> typedEmail :: m.EmailAddresses
                        | false -> m.EmailAddresses
                
                    return 
                        {
                            m with
                                MessageDisplayText = 
                                    getUniqueData typedEmail
                                    |> Result.defaultWith (fun err -> { businessCardDefault with Email = Email err } )  
                                EmailAddresses = updatedEmails
                                ErrorMessage = None
                        }, Cmd.none
                }

    let bindings () : Binding<Model, Msg> list =
        [
            "MessageDisplayText"
            |> Binding.oneWay (fun m -> m.MessageDisplayText)
    
            "PhotoPath"
            |> Binding.oneWay 
                (fun m 
                    ->
                    let (PhotoPath path) = m.MessageDisplayText.Photo
                    path
                )
    
            "EmailInputButton"
            |> Binding.cmd ShowData
    
            "EmailInputString"
            |> Binding.twoWay
                (
                    (fun m -> m.EmailInputString),
                    EmailInputStringChanged
                )
    
            "EmailAddresses"
            |> Binding.oneWay (fun m -> m.EmailAddresses)
    
            "SelectedEmail"
            |> Binding.twoWayOpt
                (
                    (fun m -> m.SelectedEmail),
                    EmailSelected
                )

            "ErrorMessage"
            |> Binding.oneWay (fun m -> m.ErrorMessage |> Option.defaultValue String.Empty)
        ]