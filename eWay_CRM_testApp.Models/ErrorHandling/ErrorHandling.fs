module ErrorHandling

open ErrorTypes

let internal errFn =  
   
    function
        | FileNotExisting -> "Soubor s uloženým emailem neexistuje."
        | ImageFileNotExisting -> "Soubor s uloženou fotografií neexistuje."  //not used yet
        | SerializationError -> "Kontakty nebyly uloženy."
        | DeserializationError -> "Kontakty nebyly načteny."
        | CRMError -> "Nebylo možno získat data z CRM na základě příslušného emailu."
        | ConnectionError -> "Nebylo možno získat data. Ověř formát emailu a připojení."
        | UserInputError1 -> "Chybný formát emailu."
        | UserInputError2 -> "Prosím zadejte platný email."