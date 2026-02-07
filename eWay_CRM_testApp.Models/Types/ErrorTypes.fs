module ErrorTypes

type [<Struct>] Errors =
    | FileNotExisting
    | SerializationError 
    | DeserializationError 
    | CRMError 
    | ConnectionError  
    | UserInputError1 
    | UserInputError2
