using System.Globalization;

using Microsoft.VisualBasic.FileIO;

namespace ml_project_BarbanAlberto.models;

public class Pokémon {
  public const int TeamSize = 6;
  public const int MovesNumber = 4;
  public static readonly string[] PokémonNames;
  private static readonly int[] pokémonIndices;
  public static readonly string[] AllMoves;
  public static readonly string[] Stats = { "Attacco", "Difesa", "Attacco speciale", "Difesa speciale", "Velocità" };
  private const string ImageURLFormat = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{0}.png";
  public int Index = -1;
  public int[] Moves = new int[MovesNumber] { -1, -1, -1, -1 };

  public string Name => PokémonNames[this.Index];

  public static string GetImageURL(int index) => string.Format(
    ImageURLFormat,
    index == 0 ? 0 : pokémonIndices[index - 1]
  );

  public static Pokémon[] CreateTeam() {
    Pokémon[] team = new Pokémon[TeamSize];
    for (int i = 0; i < team.Length; i++) team[i] = new();
    return team;
  }

  static Pokémon() {
    TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
    (PokémonNames, pokémonIndices) = unzip(
      getFirstColumn(
        "model/database/pokémons.csv",
        f => textInfo.ToTitleCase(f.Replace('-', ' '))
      )
      .Select((name, i) => (name, i + 1))
      .OrderBy(p => p.name)
    );

    AllMoves = getFirstColumn("model/database/mosse.csv", f => f).ToArray();
  }

  private static List<string> getFirstColumn(string path, Func<string, string> transformer) {
    using TextFieldParser parser = new(path);
    parser.Delimiters = new[] { "," };
    _ = parser.ReadLine();

    List<string> column = new();
    while (!parser.EndOfData) column.Add(transformer(parser.ReadFields()![0]));
    return column;
  }

  private static (T1[], T2[]) unzip<T1, T2>(IEnumerable<(T1, T2)> source) {
    List<T1> l1 = new();
    List<T2> l2 = new();

    foreach ((T1 t1, T2 t2) in source) {
      l1.Add(t1);
      l2.Add(t2);
    }

    return (l1.ToArray(), l2.ToArray());
  }
}
