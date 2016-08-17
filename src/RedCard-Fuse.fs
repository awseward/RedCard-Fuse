namespace App

open Fable.Core
open Fable.Core.JsInterop
open Fuse
open Fable.Import.Fetch

module RedCardFuse =
  type Player =
    {
      id: int
      name: string
      position: string
      yellowCards: int
      redCards: int
      team: string
      league: string
      country: string
    }

  let parsePosition abbrev =
    match abbrev with
    | "F" -> "Forward"
    | "M" -> "Midfielder"
    | "D" -> "Defender"
    | "G" -> "Goaltender"
    | _   -> "Bench"

  let fixPosition player =
    { player with
        position = (parsePosition player.position)
    }

  let playersJson = """[{"id":1,"name":"Bro Duderson","position":"F","yellowCards":10,"redCards":2,"team":"UCSB","league":"NCAA","country":"USA"}]"""

  let parsedPlayers =
    ofJson<Player[]>(playersJson)
    |> Seq.map fixPosition

  printfn "%A" parsedPlayers

  let DefaultPlayer =
    {
      id = 1
      name = "Person McPersonface"
      position = parsePosition "M"
      yellowCards = 10
      redCards = 2
      team = "UCSB"
      league = "NCAA"
      country = "USA"
    }

  let players = Observable.create()

  players.add DefaultPlayer
  players.add
    { DefaultPlayer with
        id = 2
        name = "Jane Doe"
        position = parsePosition "F"
    }
  players.add
    { DefaultPlayer with
        id = 2
        name = "Donny Nullerson"
        position = parsePosition "bad_data"
    }
