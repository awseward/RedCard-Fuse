namespace App

open Fable.Core
open Fable.Core.JsInterop
open Fuse
open Fable.Import.Fetch
open Fable.Helpers.Fetch

module RedCardFuse =
  (* Type stuff *)

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

  (* Data stuff *)
  let playersJson = """[{"id":1,"name":"Person McPersonFace","position":"F","yellowCards":10,"redCards":2,"team":"UCSB","league":"NCAA","country":"USA"}]"""
  let parsedPlayers = ofJson<Player[]>(playersJson)

  let fetchedPlayers = [] // TODO

  (* Set up Observables the App actually will look at *)
  let players = Observable.create()

  parsedPlayers
  |> Seq.map fixPosition
  |> Seq.iter (fun p -> players.add p)

  fetchedPlayers
  |> Seq.map fixPosition
  |> Seq.iter (fun p -> players.add p)
