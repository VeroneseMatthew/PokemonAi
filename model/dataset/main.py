import requests as req
import csv
from typing import Literal
from itertools import islice as slice
from utils import format_pokemon, multi_split, get_replay_ids, cast


Player = Literal["p1a", "p2a"]
Stat = Literal["atk", "def", "spa", "spd", "spe"] # Stats che possono cambiare: atk, def, spa, spd, spe, [accuracy, evasion - da ignorare]
Pokemon = tuple[str, str] # (nome Pokémon con forma, nome Pokémon senza forma), es. (arcanine-hisui, arcanine)
Teams = dict[Player, list[Pokemon]]
TeamsStatuses = dict[Player, dict[str, int]]
TeamsStats = dict[Player, dict[Stat, int]]

TEAM_SIZE = 6
STATS_SHORT = "atk", "def", "spa", "spd", "spe"
STATS_LONG = "attack", "defense", "special_attack", "special_defense", "speed"
SWITCH_OUT_MOVES = "Baton Pass", "Chilly Reception", "Flip Turn", "Parting Shot", "Shed Tail", "U-turn", "Volt Switch"
IGNORE_MOVES = "Circle Throw", "Destiny Bond", "Dragon Tail", "Future Sight", "Rest", "Roar", "Sleep Talk", "Substitute", "Tera Blast", "Trick", "Whirlwind"
IGNORE_ATTRIBUTES = "-activate", "-end", "-enditem", "-fieldend", "-fieldstart", "-item", "-sideend", "-terastallize", "-weather"
NULL_POKEMON = "magikarp"


class UnhandlableException(Exception): pass


# Turno, inteso come l'uso di una singola mossa, non come l'azione di entrambi i Pokémon in campo
class Turn: 
  active_player: Player # Da ignorare durante il salvataggio in CSV, serve per controllare quale squadra aveva il Pokémon che ha fatto una certa cosa
  active_pokemon: str
  opponent_pokemon: str
  used_move: str # Switch per indicare un cambio di Pokémon --> Ci sono anche mosse che cambiano Pokémon
  entering_pokemon: str | None = None # Lasciare None se non si è cambiato, altrimenti mettere il nome del Pokémon
  global_stats_changes: TeamsStats


class Move:
  def __init__(self, name: str, category: str, type: str, power: str, accuracy: str, pp: str, priority: str): # Priority: -3 -> 3
    self.name = name
    self.category = category
    self.type = type
    self.power = power
    self.accuracy = accuracy
    self.pp = pp
    self.priority = priority


with open("../database/mosse.csv") as f:
  reader = csv.reader(f)
  moves = [row[0] for row in slice(reader, 1, None)]


active_stats_changes: TeamsStats = {}


def with_roles(first_active: bool):
  return ("p1a", "p2a") if first_active else ("p2a", "p1a")


def get_pokemon_team(replay: str, player: int) -> list[Pokemon]:
  team = replay.split(f"|poke|p{player}|")
  team.pop(0) # Rimuovo le informazioni dei giocatori
  team[-1] = team[-1].split("|poke|" if player == 1 else "|teampreview")[0] # Tengo solo il Pokémon della squadra del blocco di testo finale

  # Sistemo la formattazione dei Pokémon per permettere l'uso dell'API per le informazioni
  return [
    (pok, pok.split("-")[0]) for pok in (format_pokemon(team[i]) if i < len(team) else NULL_POKEMON for i in range(TEAM_SIZE))
  ]


def create_teams_stats() -> TeamsStats:
  return {player: {stat: 0 for stat in STATS_SHORT} for player in ("p1a", "p2a")}


def update_turn_stats(move_parts: list[str]):
  boost_state: tuple[bool, Player, Stat] | tuple[bool, Player] | bool | None = None

  for part in move_parts:
    if part == "-boost" or part == "-unboost":
      if boost_state != None: raise Exception("Stavo già parsando un boost")
      boost_state = part == "-boost"
      continue

    match boost_state:
      case bool():
        boost_state = boost_state, cast(Player, part[:3])
      case boosting, player:
        boost_state = (boosting, player, cast(Stat, part)) if part not in ("accuracy", "evasion") else None
      case boosting, player, stat:
        delta = (1 if boosting else -1) * int(part)
        active_stats_changes[player][stat] += delta
        boost_state = None

  if boost_state != None: raise Exception("Boost incompleto")


# teams_statuses è la percentuale di vita dei Pokémon, aggiornata ad ogni turno
# Ritorno None se la mossa non è valida o non è una vera mossa
def get_turn(move: str, teams: Teams, teams_statuses: TeamsStatuses, prev_turn: Turn):
  move_parts = [part.replace("\n", "") for part in move.split("|") if part not in ("", "\n", "t:", "upkeep\n")]

  if len(move_parts) <= 2: return None

  active_player, opponent_player = with_roles(move_parts[1].startswith("p1a"))
  remaining_life = (
    next((int(part[:-4]) for part in reversed(move_parts) if part.endswith("/100")), 0)
    if "faint" not in move_parts
    else 0
  )

  update_turn_stats(move_parts)

  if len(move_parts) == 3: return None
  if "[still]" in move_parts: raise UnhandlableException("Veccio è una mossa multi-turn")

  # Formatto i Pokémon propri e dell'avversario
  def format_pokemons(player: Player, i: int):
    move_parts[i] = format_pokemon(move_parts[i])
    # pok: (pokForma, pokNoForma)
    for pok in teams[player]: # Sostituisco il Pokémon generico con la sua forma utilizzata
      if move_parts[i] == pok[1]: move_parts[i] = pok[0]

  format_pokemons(active_player, 1)
  if move_parts[0] == "move": format_pokemons(opponent_player, 3)

  if "-heal" in move_parts: # Ho recuperato vita, aggiorno la vita ma non il dataset
    teams_statuses[active_player][format_pokemon(move_parts[3])] = remaining_life # Ottengo il nome formattato del Pokémon e aggiorno la sua vita
    return None

  if "-resisted" in move_parts or "typechange" in move_parts: # Poco danno o terastalizzazione, aggiorno la vita ma non il dataset
    teams_statuses[opponent_player][format_pokemon(move_parts[3])] = remaining_life # Ottengo il nome formattato del Pokémon e aggiorno la sua vita
    return None

  if "faint" in move_parts and not "-supereffective" in move_parts: # Per evitare dati biased se un Pokémon ha poca vita
    teams_statuses[opponent_player][format_pokemon(move_parts[3])] = 0 # Ottengo il nome formattato del Pokémon e aggiorno la sua vita
    return None

  # Evitiamo di aggiungere dati per le mosse da zero danno ed evitiamo le mosse che mettono condizioni a terra, troppo difficili da implementare
  if "-fail" in move_parts or "-immune" in move_parts or "-miss" in move_parts or "-sidestart" in move_parts: 
    return None

  move_parts = [
    part for part in move_parts
    if part not in ("-damage", "-crit", "-supereffective", "[silent]")
    and "ability" not in part
    and "[from]" not in part
    and "[of]" not in part
  ]

  if len(move_parts) < 4: return None

  if move_parts[-1].isdigit() and int(move_parts[-1]) > 100: move_parts.pop() # Rimuovo info legate al tempo del match

  move_parts[min((move_parts.index(part) for part in IGNORE_ATTRIBUTES if part in move_parts), default=len(move_parts)):] = ()
  turn = Turn()
  turn.active_player = active_player

  match move_parts[0]:
    case "switch":
      # Se il Pokémon ha un soprannome ignoro il replay
      if move_parts[1] != format_pokemon(move_parts[2]): raise UnhandlableException("Veccio ha un soprannome")

      turn.active_pokemon, turn.opponent_pokemon = (
        (prev_turn.active_pokemon, prev_turn.opponent_pokemon)
        if prev_turn.active_player == active_player
        else (prev_turn.opponent_pokemon, prev_turn.active_pokemon)
      )

      turn.used_move = "Switch"
      turn.entering_pokemon = move_parts[1]

    case "move":
      _, turn.active_pokemon, turn.used_move, turn.opponent_pokemon, *_ = move_parts
      if turn.used_move not in moves: return None

      teams_statuses[opponent_player][move_parts[3]] = remaining_life

    case _: return None

  turn.global_stats_changes = {player: stats.copy() for player, stats in active_stats_changes.items()}
  return turn


def get_single_moves(turn_text: str, teams: Teams, teams_statuses: TeamsStatuses, prev_turn: Turn):
  turns: list[Turn] = []
  switchingOut = False

  skip = False
  for move in slice(multi_split("(?<=|)(?=move|switch)", turn_text), 1, None):
    if skip:
      skip = False
      continue

    turn = get_turn(move, teams, teams_statuses, prev_turn)
    if turn == None: continue

    # Se la mossa usata è difficile da gestire, la ignoro
    if turn.used_move in IGNORE_MOVES: return turns, prev_turn

    # Se la mossa usata presenta uno switch, evito di salvare subito il turno
    if turn.used_move in SWITCH_OUT_MOVES:
      switchingOut = True
      prev_turn = turn
    # Se la mossa precedente prevedeva uno switch e questa mossa è uno switch, lo aggiungo al precedente turno e lo salvo
    elif switchingOut and turn.used_move == "Switch":
      switchingOut = False
      prev_turn.entering_pokemon = turn.entering_pokemon
      turns.append(prev_turn)
    # Altrimenti aggiungo semplicemente il turno
    else:
      switchingOut = False
      prev_turn = turn
      turns.append(turn)

  return turns, prev_turn


def get_game_actions(replay: str, teams: Teams) -> list[Turn]:
  teams_statuses: TeamsStatuses = {player: {pokemon[0]: 100 for pokemon in team} for player, team in teams.items()}
  try:
    starting_conditions = replay.split("|start")[1].split("|turn")[0].split("switch")[1:]
  except IndexError as e:
    raise UnhandlableException("Veccio ha fatto subito forfeit") from e

  prev_turn = Turn() # Imposto i pokemon iniziali
  prev_turn.active_player = "p1a"

  def check_starting_conditions(conditions: str):
    parts = conditions.split("|")
    formatted = format_pokemon(parts[1])
    if format_pokemon(parts[2]) != formatted: raise UnhandlableException("Veccio ha il soprannome")
    return formatted

  prev_turn.active_pokemon = check_starting_conditions(starting_conditions[0])
  prev_turn.opponent_pokemon = check_starting_conditions(starting_conditions[1])

  global active_stats_changes; active_stats_changes = create_teams_stats()

  actions: list[Turn] = []
  for turn in replay.split("|turn|")[1:-1]:
    new_actions, prev_turn = get_single_moves(turn, teams, teams_statuses, prev_turn)
    actions += new_actions
  return actions


with open("../database/pokémons.csv") as f:
  reader = csv.reader(f)
  pokemons = {row[0]: row[1:] for row in slice(reader, 1, None)}

# 9 colonne
def serialize_pokemon(query: str):
  try:
    return pokemons[query]
  except KeyError:
    words = query.split("-")
    if len(words) > 1:
      try:
        return pokemons[words[0]]
      except KeyError:
        pass

  return pokemons[next(name for name in pokemons if name.startswith(query))]


# 9 + 9 + 9 + 5 * 2 + 1 = 38 colonne
def serialize_turn(turn: Turn):
  return (
    *serialize_pokemon(turn.active_pokemon),
    *serialize_pokemon(turn.opponent_pokemon),
    *serialize_pokemon(turn.entering_pokemon or NULL_POKEMON),
    *(turn.global_stats_changes[player][stat] for player in with_roles(turn.active_player == "p1a") for stat in STATS_SHORT),
    turn.used_move
  )


def pokemon_header(prefix: str):
  return (f"{prefix}_{prop}" for prop in ("types", "levitate", "weight", "hp", *STATS_LONG))


with open("replays_log.csv", "w") as log:
  writer = csv.writer(log)
  writer.writerow((
    *pokemon_header("active_pokemon"),
    *pokemon_header("opponent_pokemon"),
    *pokemon_header("entering_pokemon"),
    *(f"{player}_{stat}" for player in ("active", "opponent") for stat in STATS_LONG),
    "move"
  ))

  for i, id in enumerate(get_replay_ids(int(input("Inserisci il numero di pagine da prendere: ")))):
    replay = req.get(f"https://replay.pokemonshowdown.com/{id}.log").text
    teams: Teams = {
      "p1a": get_pokemon_team(replay, 1),
      "p2a": get_pokemon_team(replay, 2)
    }

    print(f"[{i + 1}] {id}:" + "".join(f"\n  {player}: {', '.join(pok[0] for pok in team)}" for player, team in teams.items()))

    try:
      writer.writerows(map(serialize_turn, get_game_actions(replay, teams)))
    except UnhandlableException as e:
      pass
