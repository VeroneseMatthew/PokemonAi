import csv
from itertools import islice as slice
import numpy as np
from typing import TypeAlias, Literal

import os
os.environ["TF_CPP_MIN_LOG_LEVEL"] = "1"
import tensorflow as tf
from tensorflow import keras

from pypokedex.pokemon import Pokemon as Pokemon
from pypokedex import get as getPokemon
from pypokedex.exceptions import PyPokedexHTTPError as PokemonNotFoundError

class NotFullTeamException(Exception): pass

Player = Literal["p1a", "p2a"]
Stat = Literal["atk", "def", "spa", "spd", "spe"] # Stats che possono cambiare: atk, def, spa, spd, spe, [accuracy, evasion - da ignorare]
Teams = dict[Player, list[Pokemon]]
TeamsStatuses = dict[Player, dict[str, int]]
TeamsStats = dict[Player, dict[Stat, int]]
STATS_SHORT = "atk", "def", "spa", "spd", "spe"

Pokemon.__hash__ = lambda self: self.dex
standardPok = getPokemon(name="magikarp")
NULL_POKEMON = "magikarp"
NULL_MOVE = "Splash"

#Placeholder necessario per non individuare un tipo mancante come normale
POK_TYPES = ("placeholder", "normal", "fire", "water", "grass", "flying", "fighting", "poison", "electric", "ground", "rock", "psychic", "ice", "bug", "ghost", "steel", "dragon", "dark", "fairy")
NUM_POK_ATTR = 8
POK_ATTR_MAX_VALUES = (1, 1000, 255, 255, 255, 255, 255, 255)
NUM_POK_STAT_CHANGES = 5

#Un neurone per tipo possibile per il proprio pokemon e quello avversario; un neurone per ogni caratteristica del proprio pokemon e quello avversario; un neurone per ogni cambio di stat per squadra
INPUT_SIZE = len(POK_TYPES)*2 + NUM_POK_ATTR*2 + NUM_POK_STAT_CHANGES*2
TRAIN_LINES = 10000
TEST_LINES = 1170

SWITCH_OUT_MOVES: tuple[str] = ("Baton Pass", "Chilly Reception", "Flip Turn", "Parting Shot", "Shed Tail", "U-turn", "Volt Switch")

def formatPokemon(pokemon: str):
    return (pokemon.replace(", M", "")
               .replace(", F", "")
               .replace(", shiny", "")
               .replace("|\n", "")
               .lower()
               .replace("p1a: ", "")
               .replace("p2a: ", "")
               .replace(" ", "-")
               .replace("-*", ""))


# Turno, inteso come l'uso di una singola mossa, non come l'azione di entrambi i Pokémon in campo
class Turn: 
  active_player: Player # Da ignorare durante il salvataggio in CSV, serve per controllare quale squadra aveva il Pokémon che ha fatto una certa cosa
  active_pokemon: str
  opponent_pokemon: str
  used_move: str # Switch per indicare un cambio di Pokémon --> Ci sono anche mosse che cambiano Pokémon
  entering_pokemon: str | None = None # Lasciare None se non si è cambiato, altrimenti mettere il nome del Pokémon
  global_stats_changes: TeamsStats
  team: list[str] # Lista della squadra Pokémon (compreso il Pokémon stesso)


class Move:
  def __init__(self, name: str, category: str, type: str, power: str, accuracy: str, pp: str, priority: str): # Priority: -3 -> 3
    self.name = name
    self.category = category
    self.type = type
    self.power = power
    self.accuracy = accuracy
    self.pp = pp
    self.priority = priority

with open("database/pokémons.csv") as f:
  reader = csv.reader(f)
  pokemons = {row[0]: row[1:] for row in slice(reader, 1, None)}

with open("database/mosse.csv") as f:
    reader = csv.reader(f)
    pokemonMoves = {row[0].lower(): Move(*row[:3], *map(int, row[3:])) for row in slice(reader, 1, None)}

with open("database/mosse.csv") as f:
    reader = csv.reader(f)
    movesDisplayNames = [row[0] for row in slice(reader, 1, None)]
    pokemonMovesNames = [move.lower() for move in movesDisplayNames]

OUTPUT_SIZE = len(pokemonMoves)

def getPokemonWithControl(inputStr: str) -> Pokemon | None:

    poke: str = input(inputStr).lower()

    if(poke == "none"):
            raise NotFullTeamException("Veccio il team non è completo")

    poke = formatPokemon(poke)
    
    try:
        pokemon: Pokemon = getPokemon(name=poke)
        return pokemon

    except PokemonNotFoundError:
        print("Pokemon non trovato, inserire nuovamente")
        return None


def getGlobalChanges() -> dict[Player, dict[Stat, int]] | None:
    changesHappened: bool = input("Sono avvenuti cambiamenti di stato? [Si] [No]").lower().strip() == "si"

    if(not changesHappened):
        return None
    
    statsChanges = {player: {stat: 0 for stat in ("atk", "spa", "def", "spd", "spe")} for player in ("p1a", "p2a")}

    for playerMsg, player in (("Proprio pokemon: ", "p1a"), ("Pokemon avversario: ", "p2a")):
        for stat in ("atk", "spa", "def", "spd", "spe"):
            while(True):
                try:
                    statChange = int(input(f"{playerMsg}\n{stat}: "))
                    break
                except ValueError:
                    print("Valore non valido")
            
            statsChanges[player][stat] = statChange
    
    return statsChanges


def setPokemonMoves() -> list[Move]:
    moves: list[Move] = []

    while(len(moves) < 4):
        movName = input(f"Inserisci mossa [{len(moves)}] (Se slot vuoto --> 'None'): ").lower()
        if(movName == "none"):
            break
        try:
            moves.append(pokemonMoves[movName])
        except KeyError:
            pass

    if len(moves) == 0: moves.append(pokemonMoves["splash"])

    return moves


def createTeam() -> dict[Pokemon, list[Move]]:
    team: dict[Pokemon, list[Move]] = {} #Lista di dizionari --> pokemon: mosse[]
    while (len(team.keys()) < 6):
        try:
            pokemon: Pokemon = getPokemonWithControl("Inserire nome proprio pokemon (se slot vuoto --> 'None'): ")
        except NotFullTeamException:
            break
        
        if(type(pokemon) == Pokemon):
            team[pokemon] = setPokemonMoves() 
    
    return team

def with_roles(first_active: bool):
  return ("p1a", "p2a") if first_active else ("p2a", "p1a")

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

def createCase(dataRow):
    data: np.ndarray = np.zeros(INPUT_SIZE)
    for j in range(2):
        pok_types: str = dataRow[j + j * NUM_POK_ATTR]
        for pok_type in pok_types.split(";"): #Imposto i tipi del pokemon
            data[ j * 1 + POK_TYPES.index(pok_type) + j * len(POK_TYPES)] = 1

        for attr in range(NUM_POK_ATTR): #Imposto le info del pokemon
            data[len(POK_TYPES) * (j + 1) + (j * NUM_POK_ATTR) + attr] = np.float64( float(dataRow[(j + 1) + (j * NUM_POK_ATTR) + attr]) / POK_ATTR_MAX_VALUES[attr])

        for stat in range(NUM_POK_STAT_CHANGES): #Imposto i cambi di stat dei pokemon
            data[len(POK_TYPES) * 2 + NUM_POK_ATTR * 2 + NUM_POK_STAT_CHANGES * j + stat] = np.float64( (float(dataRow[3 + NUM_POK_ATTR * 3 + (j * NUM_POK_STAT_CHANGES)]) + 6) / 12 )

    return data

def createData(lines, startSlice):
    with open("dataset/replays_log.csv") as f:
        reader = csv.reader(f)

        data: np.ndarray = np.zeros((lines, INPUT_SIZE)) #18 tipi - 8 caratteristiche - 18 tipi - 8 caratteristiche
        labels: np.ndarray = np.zeros((lines, OUTPUT_SIZE)) #826 mosse

        rows = list(slice(reader, startSlice, (startSlice if startSlice != 1 else 0) + lines+1))
        for i in range(lines):
            data[i] = createCase(rows[i])
            labels[i][ pokemonMovesNames.index(rows[i][-1].lower()) ] = 1 #Imposto l'indice corrispondente alla mossa a 1

    return data, labels

def createModel():
    model = keras.Sequential([
        keras.Input((INPUT_SIZE,)),
        keras.layers.Dense(512, activation='relu'),  # hidden layer
        keras.layers.Dense(256, activation='relu'),  # hidden layer
        keras.layers.Dense(OUTPUT_SIZE, activation='softmax') #uno per ogni mossa nel file mosse.csv
    ])

    model.compile(optimizer='SGD',
                  loss='categorical_crossentropy',
                  metrics=['accuracy'])

    train_data, train_labels = createData(TRAIN_LINES, 1)
    test_data, test_labels = createData(TEST_LINES, TRAIN_LINES)

    model.fit(train_data, train_labels, epochs=20) 

    test_loss, test_acc = model.evaluate(test_data,  test_labels, verbose=1)

    print('Test accuracy:', test_acc)

    model.save("model.keras")

    return model

#model = createModel()

model = keras.models.load_model("model.keras")

team = createTeam()

for pok in team:
    team[pok].append(Move("Switch","status","normal","0","101","40","3"))

globalStatsChanges: dict[Player, dict[Stat, int]] = {player: {stat: 0 for stat in ("atk", "spa", "def", "spd", "spe")} for player in ("p1a", "p2a")}


turn: Turn = Turn()

activePok: Pokemon | None = getPokemonWithControl("Inserire nome proprio pokemon attivo: ")
if(type(activePok) != Pokemon or not activePok in team.keys()):
    quit()

turn.active_player = "p1a"
turn.active_pokemon = activePok.name

availableMovesIndices = [ pokemonMovesNames.index(move.name.lower()) for move in team[activePok] ]

opponentPok: Pokemon | None = getPokemonWithControl("Inserire nome pokemon attivo avversario: ")
if(type(opponentPok) != Pokemon):
    quit()

turn.opponent_pokemon = opponentPok.name

globalStatsChangesTemp: dict[Player, dict[Stat, int]] | None = getGlobalChanges()
if(globalStatsChangesTemp != None): #Dati non cambiano dal caso precedente
    globalStatsChanges = globalStatsChangesTemp

turn.global_stats_changes = globalStatsChanges
turn.used_move = pokemonMovesNames[availableMovesIndices[0]]

data = createCase(serialize_turn(turn))
datas = np.zeros((1, INPUT_SIZE))
datas[0] = data
    
predictions: np.ndarray = model.predict(datas, verbose=2)[0]

sorted_indices = np.argsort(predictions)
#Ordino le predizioni dalla migliore alla peggiore tenendomi l'indice dell'output del modello per sapere a che azione corrispondono         
sorted_predictions = np.column_stack((sorted_indices, predictions[sorted_indices]))[::-1]

bestMoves: list[str] = []

for predict in sorted_predictions:
    if(predict[0] in availableMovesIndices):
        bestMoves.append(movesDisplayNames[int(predict[0])])

print(f"Mosse dalla migliore alla peggiore:\n{"\n".join(bestMoves)}")
