module ErrorTypes

type [<Struct>] Errors =
    | FileNotExisting
    | ImageFileNotExisting //not used yet
    | SerializationError 
    | DeserializationError 
    | CRMError 
    | ConnectionError  
    | UserInputError1 
    | UserInputError2