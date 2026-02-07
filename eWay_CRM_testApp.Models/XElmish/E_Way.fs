namespace eWay_CRM_testApp.Models

open System

open Elmish
open Elmish.WPF

open FsToolkit.ErrorHandling

open Types
open Helpers
open CEBuilders
open ErrorTypes
open BusinessLogic
open ErrorHandling
open CoreDataModelling
open IO_MonadSimulation
open EmailHistoryModelling

//***************************************************************

module E_Way =

    type internal Model =
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
        | EmailsLoaded of Result<string list, Errors>
        | EmailsSaved of Result<unit, Errors>
        | ShowData    
      
    let internal initialModel listOfEmails =
        {
            MessageDisplayText = businessCardDefault
            EmailInputString = String.Empty
            EmailAddresses = listOfEmails
            SelectedEmail = None
            ErrorMessage = None
        }

    let internal loadEmailsCmd () =
        Cmd.OfAsync.perform
            (fun () -> fromDto >> runIO <| ())
            ()
            EmailsLoaded  

    let internal saveEmailsCmd newEmails =
        Cmd.OfAsync.perform
            (fun () -> toDto >> runIO <| newEmails)
            ()
            EmailsSaved
    
    let internal init () : Model * Cmd<Msg> =

        let initialEmailList =  //for testing only
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
    
    let internal update msg m =
        match msg with
        | EmailsLoaded (Ok emails) 
            ->
            { m with EmailAddresses = emails }, Cmd.none
        
        | EmailsLoaded (Error err) 
            ->
            let errMsg = errFn err
            { m with ErrorMessage = Some errMsg }, Cmd.none

        | EmailsSaved (Ok _) 
            ->
            m, Cmd.none

        | EmailsSaved (Error err) 
            ->
            let errMsg = errFn err
            { m with ErrorMessage = Some errMsg }, Cmd.none

        | EmailInputStringChanged email
            ->
            let typedEmail = email.Trim()
        
            let newEmails =
                pyramidOfDoom
                    {
                        let! validEmail = isValidEmail >> runIO <| typedEmail, m.EmailAddresses
                        let cond = (validEmail <> String.Empty && not (m.EmailAddresses |> List.contains validEmail))
                        let! _ = cond |> Option.ofBool, m.EmailAddresses
                        return typedEmail :: m.EmailAddresses 
                    }
           
            let errorMsg =
                isValidEmail >> runIO <| typedEmail
                |> Option.map (fun _ -> None)  // Valid -> None (no error)
                |> Option.defaultValue (Some <| errFn UserInputError1)  // Invalid -> Some error
        
            {
                m with
                    EmailInputString = typedEmail
                    EmailAddresses = newEmails
                    MessageDisplayText =
                        getUniqueData >> runIO <| typedEmail
                        |> Result.defaultWith
                            (fun err 
                                -> 
                                let errMsg2 = errFn err
                                { businessCardDefault with Email = Email errMsg2 }   
                            )
                    ErrorMessage = errorMsg
            }, saveEmailsCmd newEmails
    
        | EmailSelected emailOpt 
            ->
            match emailOpt with
            | Some email
                ->
                let valid = isValidEmail >> runIO <| email
                let updatedModel =
                    { 
                        m with
                            SelectedEmail = Some email
                            EmailInputString = email
                            ErrorMessage = 
                                valid 
                                |> Option.map (fun _ -> None)
                                |> Option.defaultValue (Some <| errFn UserInputError1)
                    }
    
                match valid with
                | Some email  
                    ->
                    { 
                        updatedModel with
                            MessageDisplayText =
                                getUniqueData >> runIO <| email 
                                |> Result.defaultWith
                                    (fun err 
                                        -> 
                                        let errMsg = errFn err
                                        { businessCardDefault with Email = Email errMsg }
                                    )  
                    }, Cmd.none
                | None
                    ->
                    updatedModel, Cmd.none
    
            | None -> { m with SelectedEmail = None }, Cmd.none
    
        | ShowData 
            ->
            let typedEmail = m.EmailInputString.Trim()

            pyramidOfDamnation
                {
                    let! _ = 
                        (typedEmail <> String.Empty, 
                            ({ m with ErrorMessage = Some <| errFn UserInputError2 }, Cmd.none)) 
                
                    let! _ = 
                        ((isValidEmail >> runIO <| typedEmail) |> Option.toBool,
                            ({ m with ErrorMessage = Some <| errFn UserInputError1 }, Cmd.none))
                
                    let updatedEmails =
                        match not (m.EmailAddresses |> List.contains typedEmail) with
                        | true  -> typedEmail :: m.EmailAddresses
                        | false -> m.EmailAddresses
                
                    return 
                        {
                            m with
                                MessageDisplayText = 
                                    getUniqueData >> runIO <| typedEmail
                                    |> Result.defaultWith 
                                        (fun err
                                            -> 
                                            let errMsg = errFn err
                                            { businessCardDefault with Email = Email errMsg } 
                                        )  
                                EmailAddresses = updatedEmails
                                ErrorMessage = None
                        }, Cmd.none
                }

    let internal bindings () : Binding<Model, Msg> list =
        [
            "MessageDisplayText"
            |> Binding.oneWay (fun m -> formatBusinessCard m.MessageDisplayText)
    
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