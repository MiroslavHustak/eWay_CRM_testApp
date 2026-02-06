module Elmish.Program

open Elmish.WPF
open eWay_CRM_testApp.Models

let main window = 
  
    WpfProgram.mkProgram MainWindowNonOpt.init MainWindowNonOpt.update MainWindowNonOpt.bindings
    |> WpfProgram.startElmishLoop window