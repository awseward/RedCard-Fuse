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

  type Team =
    {
      name: string
      players: Player seq
    }

  type League =
    {
      name: string
      teams: Team seq
    }


  type Country =
    {
      name: string
      leagues: League seq
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

  let printAndReturn thing =
    printfn "%A" thing
    thing

  let fetchPlayers url callback =
    async {
      let! players = fetchAs<Player[]>(url, [])

      players
      |> List.ofSeq
      |> List.sortBy (fun p -> p.redCards)
      |> List.rev
      // NOTE: Currently taking only 100 because otherwise the UI chugs as it
      // builds ~5k UI elements
      |> Seq.take 100
      |> Seq.map fixPosition
      |> Seq.map printAndReturn
      |> Seq.iter callback
    }


  (* Set up observables and functions for binding and whatnot *)

  let players = Observable.create()

  let clearPlayers () = players.clear()
  let loadPlayers () =
    fetchPlayers "http://45.55.167.132/api/players" (fun data -> players.add data)
    |> Async.Start

  loadPlayers()
