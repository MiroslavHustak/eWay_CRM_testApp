module IO_MonadSimulation
    
type [<Struct>] internal IO<'a> = IO of (unit -> 'a) // wrapping custom type simulating Haskell's IO Monad (without the monad, of course)

let internal runIO (IO action) = action () 
let internal runIOAsync (IO action) : Async<'a> = async { return action () }