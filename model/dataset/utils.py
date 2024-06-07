import requests as __requests
from typing import cast
import re as __re
from re import split as multi_split


def get_replay_ids(pages: int):
  for page in range(1, pages + 1):
    try:
      resp = __requests.get("https://replay.pokemonshowdown.com/search.json", {
        "format": "gen9ou",
        "page": page
      })
      resp.raise_for_status()
      yield from [cast(str, replay["id"]) for replay in resp.json()][:50]
    except Exception as e:
      print(e)


__format_pattern1 = __re.compile(r", (?:[MF]|shiny)|\|\n")
__format_pattern2 = __re.compile(r"p[12]a: ")
def format_pokemon(pokemon: str):
  return (
    __format_pattern2.sub("", __format_pattern1.sub("", pokemon).lower())
      .replace(" ", "-")
      .replace("-*", "")
  )
