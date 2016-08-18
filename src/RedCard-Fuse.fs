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
      league: string
      country: string
      players: Player seq
    }

  type League =
    {
      name: string
      country: string
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

  let printCount name items =
    let count = Seq.length items
    printfn "%s: %d" name count
    items

  (* Data stuff *)
  let playersToTeams (players : Player seq) =
    players
    |> Seq.groupBy (fun p -> p.team)
    |> Seq.sortBy (fun (name, _) -> name)
    |> Seq.map (fun (teamName, teamPlayers) ->
         let firstPlayer = Seq.head teamPlayers
         {
           name = teamName
           league = firstPlayer.league
           country = firstPlayer.country
           players = teamPlayers
         }
       )
    |> printCount "teams"

  let teamsToLeagues (teams : Team seq) =
    teams
    |> Seq.groupBy (fun t -> t.league)
    |> Seq.sortBy (fun (name, _) -> name)
    |> Seq.map (fun (leagueName, leagueTeams) ->
         {
           name = leagueName
           country = (Seq.head leagueTeams).country
           teams = leagueTeams
         }
       )
    |> printCount "leagues"

  let leaguesToCountries (leagues : League seq) =
    leagues
    |> Seq.groupBy (fun l -> l.country)
    |> Seq.sortBy (fun (name, _) -> name)
    |> Seq.map (fun (countryName, countryLeagues) ->
         {
           name = countryName
           leagues = countryLeagues
         }
       )
    |> printCount "countries"

  let printAndReturn thing =
    printfn "%A" thing
    thing

  let fetchPlayers url callback =
    async {
      let! players = fetchAs<Player[]>(url, [])

      players
      |> playersToTeams
      |> teamsToLeagues
      |> leaguesToCountries
      |> Seq.iter callback
    }


  (* Set up observables and functions for binding and whatnot *)

  let countries = Observable.create()

  let loadData () =
    fetchPlayers "http://45.55.167.132/api/players" (fun data -> countries.add data)
    |> Async.Start

  let clearData () = countries.clear()

  loadData()
