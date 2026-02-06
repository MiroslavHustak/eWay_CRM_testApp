(*
MIT License for software design in this source file
    
Copyright (c) 2021 Bent Tranberg
    
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
    
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*)    
namespace eWay_CRM_testApp.Models

open System
open System.Windows  

open Elmish
open Elmish.WPF

open CEBuilders
open eWay_CRM_testApp.Models

//*******************************************
    
//Non-optional variant
module MainWindowNonOpt =
    
    let private header1 = " Hlavní stránka "    
    let private newGuid () = Guid.NewGuid()
            
    type Toolbutton =
        {
            Id: Guid
            Text: string
            IsMarkable: bool
        }
    
    type Tab =
        {
            Id: Guid
            Header: string
            Toolbuttons: Toolbutton list
        }
    
    type Msg =
        | ButtonClick of id: Guid
        | ShowE_Way
        | E_WayMsg of E_Way.Msg
        | SetSelectedTabHeader of tabHeader:string
    
    type internal Model =
        {
            Tabs: Tab list
            MarkedButton: Guid
            E_WayTestPage: E_Way.Model 
            SelectedTabHeader: string
        }
               
    let tbNone = newGuid ()
    let tbE_Way = newGuid ()
    //let tbLicences = newGuid ()  
    
    let tabs =
        let tab header toolButtons =
            { Id = newGuid (); Header = header; Toolbuttons = toolButtons }           
        [ tab header1 []]
        
    let internal e_WayPage, (e_WayPageCmd: Cmd<E_Way.Msg>) = E_Way.init ()
        
    let internal startModel =
        {
            Tabs = tabs
            MarkedButton = tbE_Way           
            E_WayTestPage = e_WayPage               
            SelectedTabHeader = (tabs |> List.item 0).Header
        }
    
    let internal init () : Model * Cmd<Msg> = startModel, Cmd.map E_WayMsg e_WayPageCmd
    
    let internal findButton (id: Guid) (m: Model) =
        m.Tabs |> List.tryPick (fun tab -> tab.Toolbuttons |> List.tryFind (fun tb -> tb.Id = id))
    
    let internal update (msg: Msg) (m: Model) = 
        match msg with
        | ButtonClick id 
            ->
            match findButton id m with
            | None             
                -> m, Cmd.none
            | Some clickedButton 
                ->                                   
                let m = 
                    match clickedButton.IsMarkable with
                    | true  -> { m with MarkedButton = id }
                    | false -> m                            
    
                pyramidOfDamnation    
                    {         
                        // Keep the CE as it is, to allow more tbE_Way-like buttons to be added without changing the ROP logic
                        let! _ = not (clickedButton.Id = tbE_Way), (m, Cmd.ofMsg ShowE_Way) 
                        return m, Cmd.none 
                    }       
    
        | ShowE_Way 
            -> { m with E_WayTestPage = fst (E_Way.init()) }, Cmd.none   
        
        | E_WayMsg msg
            ->
            let m', cmd' = E_Way.update msg m.E_WayTestPage
            { m with E_WayTestPage = m' }, Cmd.map E_WayMsg cmd'    
       
        | SetSelectedTabHeader header 
            ->           
            match header with
            | value 
                when header1 = header
                -> { m with MarkedButton = tbE_Way; SelectedTabHeader = value }, Cmd.ofMsg ShowE_Way 
            | _ -> { m with SelectedTabHeader = header }, Cmd.none

    let internal bindings () : Binding<Model, Msg> list =
        [            
            "Tabs"
            |> Binding.subModelSeq
                (
                    (fun m -> m.Tabs),
                    (fun t -> t.Id),
                    fun () -> ["Header" |> Binding.oneWay (fun (_, t) -> t.Header)]
                )    

            "E_WayPage"
            |> Binding.SubModel.required E_Way.bindings
            |> Binding.mapModel (fun m -> m.E_WayTestPage)
            |> Binding.mapMsg E_WayMsg       

            "E_WayPageVisible"
            |> Binding.oneWay 
                (fun m -> match m.MarkedButton = tbE_Way with true -> Visibility.Visible | false -> Visibility.Collapsed)               
    
            "SelectedTabHeader"
            |> Binding.twoWay
                (
                    (fun m -> m.SelectedTabHeader),
                    SetSelectedTabHeader
                )
        ]
        
    let designVm = ViewModel.designInstance startModel (bindings())