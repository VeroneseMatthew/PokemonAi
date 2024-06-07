import csv as __csv
from typing import Callable as __Callable, Iterable as __Iterable, TypeVar as __TypeVar
from itertools import islice as __islice, chain as __chain, groupby as __groupby


def transform(
  name: str,
  header: tuple[str, ...],
  process8: __Callable[[__Iterable[list[str]]], __Iterable[__Iterable]],
  process9: __Callable[[__Iterable[list[str]]], __Iterable[__Iterable]],
  resolve_conflict: __Callable[[list[str], list[str]], bool] | None = None
):
  intermediate1 = f"8/{name}"
  intermediate2 = f"9/{name}"
  __transform(f"8/{name}_raw", intermediate1, header, process8)
  __transform(f"9/{name}_raw", intermediate2, header, process9)
  __merge(intermediate1, intermediate2, name, resolve_conflict)


def __transform(
  input: str,
  output: str,
  header: tuple[str, ...],
  process: __Callable[[__Iterable[list[str]]], __Iterable[__Iterable]]
):
  with (
    open(f"{input}.csv", encoding="latin-1", newline="") as input_file,
    open(f"{output}.csv", "w", newline="") as output_file
  ):
    reader = __csv.reader(input_file)
    writer = __csv.writer(output_file)

    writer.writerow(header)
    writer.writerows(process(__islice(reader, 1, None)))


def __merge(
  input1: str,
  input2: str,
  output: str,
  resolve_conflict: __Callable[[list[str], list[str]], bool] | None = None
):
  with (
    open(f"{input1}.csv", encoding="latin-1", newline="") as input1_file,
    open(f"{input2}.csv", encoding="latin-1", newline="") as input2_file,
    open(f"{output}.csv", "w", newline="") as output_file
  ):
    reader1 = __csv.reader(input1_file)
    reader2 = __csv.reader(input2_file)
    writer = __csv.writer(output_file)

    err_ctx = f"files {input1}.csv and {input2}.csv"

    header = next(reader1)
    assert header == next(reader2), f"Incompatible {err_ctx}"
    writer.writerow(header)

    for _, conflicts in __groupby(sorted(__chain(reader1, reader2)), lambda row: row[0]):
      first = next(conflicts)
      second = next(conflicts, None)
      if next(conflicts, None) != None:
        raise Exception(f"Cannot handle a conflict between more than 2 rows when merging {err_ctx}")

      if second != None:
        if first == second:
          writer.writerow(first)
          continue

        if resolve_conflict == None: raise Exception(f"Unhandled conflict when merging {err_ctx}")

        writer.writerow(second if resolve_conflict(first, second) else first)
      else: writer.writerow(first)


__T = __TypeVar("__T")
def check(x: __T, p: __Callable[[__T], bool]):
  assert p(x), f"Unexpected value: {x}"
  return x
