from commons import transform, check
from itertools import groupby


def transform_types(type1: str, type2: str):
  yield type1
  if type2 != "None" and type2 != "NA": yield type2


weights9 = iter((
  56, 48.2, 30, 85, 210, 113, 42.9, 6, 0.6, 21, 3, 62, 700, 45, 10.5, 4.9, 152.2, 223, 30.7, 63, 14.9,
  11.9, 220, 43.3, 90, 160, 10.9, 60.2, 37, 1.5, 12.2, 4, 17, 9.8, 240, 30, 2.6, 8, 45, 27.2, 320, 35,
  15, 11, 380.7, 111, 36, 303, 240, 35, 38.6, 120, 79, 303, 10.2, 17.5, 61, 16, 2.6, 31.2, 240, 16, 105,
  1, 120, 310, 78.8, 2.5, 6.5, 41, 61.9, 6.1, 21.5, 3.5, 1, 120, 380, 60, 15, 8, 0.7, 326.5, 92, 6.5,
  16.5, 4.1, 2.4, 0.4, 1.8, 4, 8, 699.7, 8.9, 112.8, 59.1, 33, 58, 35, 90, 3.6, 1.8, 74.2, 5.4
))

transform(
  "pokémon",
  ("name", "types", "levitate", "weight", "hp", "attack", "defense", "special_attack", "special_defense", "speed",
   "weakness_bug", "weakness_dark", "weakness_dragon", "weakness_electric", "weakness_fairy", "weakness_fighting",
   "weakness_fire", "weakness_flying", "weakness_ghost", "weakness_grass", "weakness_ground", "weakness_ice",
   "weakness_normal", "weakness_poison", "weakness_psychic", "weakness_rock", "weakness_steel", "weakness_water"),
  lambda rows: ((
    row[1].lstrip(), ";".join(transform_types(row[7].lower(), row[8].lower())),
    int("Levitate" in (ability[1:-1] for ability in row[4][1:-1].split(", "))),
    row[6], *row[10:34]
  ) for row in rows),
  lambda rows: ((
    row[0], ";".join(transform_types(row[1], row[2])), "0", next(weights9), *row[3:9],
    *(row[i] for i in (20, 24, 23, 12, 26, 15, 10, 18, 22, 13, 17, 14, 9, 16, 19, 21, 25, 11))
  ) for row in rows),
)

transform(
  "mosse",
  ("name", "category", "type", "power", "accuracy", "pp", "priority"),
  lambda rows: ((
    row[0],
    {"Physical": "physical", "Special": "special", "Status": "status"}[row[9]],
    check(row[6], lambda type: type.isalpha() and type[0].isupper() and type[1:].islower()).lower(),
    int(float(row[4] or "0")), int(float(row[2] or "0")),
    row[3], row[5]
  ) for row in rows if "  " not in row[0]),
  lambda rows: ((
    row[0],
    {"physical": "physical", "other": "status"}.get(row[2], "special"),
    check(row[1], lambda type: type.isalpha() and type.islower()),
    row[4], row[5], row[3], row[10]
  ) for row in (
    next(group[1]) for group in groupby(rows, lambda row: row[0])
  )),
  lambda _, second: second[4] == "101"
)
