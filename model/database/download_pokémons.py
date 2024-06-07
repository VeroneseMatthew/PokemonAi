from pypokedex import get as get_pokemon
from typing import cast
import csv

with open("pokémons.csv", "w") as f:
  writer = csv.writer(f)
  writer.writerow(("name", "types", "levitate", "weight", "hp", "attack", "defense", "special_attack", "special_defense", "speed"))
  for i in range(1, 1026):
    pokemon = get_pokemon(dex=i)
    print(f"{i}: {pokemon.name}")
    writer.writerow((
      pokemon.name,
      ";".join(pokemon.types),
      int("levitate" in pokemon.abilities),
      pokemon.weight / 10,
      *cast(tuple[int, int, int, int, int, int], pokemon.base_stats)
    ))
