module Settings

let [<Literal>] internal SERVICE_URL = "https://free.eway-crm.com/31994"
let [<Literal>] internal USERNAME = "api"
let [<Literal>] internal PASSWORD = "ApiTrial@eWay-CRM"
let [<Literal>] internal PASSWORD_HASH = "470AE7216203E23E1983EF1851E72947"
let [<Literal>] internal APP_ID = "ExampleApplication100"

let [<Literal>] internal pathToJson = "emails.json"
let [<Literal>] internal pathToCRM_Photos = "eWayCRM_Photos"
let [<Literal>] internal pathTooLongLimit = 200 // to avoid "PathTooLong" issues (max. 260 characters) 
let [<Literal>] internal maxListBoxItems = 10