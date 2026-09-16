module IO_MonadSimulation
    
type [<Struct>] internal Impure<'a> = Impure of (unit -> 'a) // wrapping custom type simulating Haskell's monads (without the monad, of course)

let internal runImpure (Impure action) = action () 
let internal runImpureAsync (Impure action) : Async<'a> = async { return action () }