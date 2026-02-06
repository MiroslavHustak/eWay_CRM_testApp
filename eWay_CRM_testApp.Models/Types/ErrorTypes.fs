module ErrorTypes

type [<Struct>] Errors =
    | FileNotExisting
    | SerializationError //"Failed getting path" "Path is invalid: %s" "Kontakty nebyly uloženy."
    | DeserializationError //"File does not exist"  "Failed to decode: %s") "Kontakty nebyly načteny."
    | CRMError //"Nebylo možno získat data z CRM na základě příslušného emailu."
    | ConnectionError  //"Nebylo možno získat data. Ověř připojení k CRM."
    | UserInputError1 //"Chybný formát emailu."
    | UserInputError2 //"Prosím zadejte platný email."
