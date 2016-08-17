namespace App

open Fable.Core
open Fuse
open Fable.Import.Fetch
open Fable.Helpers.Fetch

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

  let players = Observable.create()

  promise {
    let! req = GlobalFetch.fetchAs<Player[]> (Url "http://45.55.167.132/api/players")
    let! json = req.json ()
    do (players.value <- json) } |> ignore

  printfn "%A" players.value
