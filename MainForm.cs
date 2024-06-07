using System.Diagnostics;

using ml_project_BarbanAlberto.models;

namespace ml_project_BarbanAlberto;

public partial class MainForm : Form {
  private const string ScriptPath = @"model\main.py";
  private readonly Pokémon[] team = Pokémon.CreateTeam();
  private int currPokémonIndex;
  private readonly Pokémon opponent = new();
  private readonly Button[] buttonsPokémons;
  private readonly ComboBox[] comboBoxesMoves;
  private readonly NumericUpDown[] numericUpDownsStats;
  private readonly Action updateImage;

  public MainForm() {
    this.InitializeComponent();

    this.buttonsPokémons = generate(this.layoutPokémons, Pokémon.TeamSize, i => {
      Button b = new() {
        Dock = DockStyle.Fill,
        Text = $"Pokémon {i + 1}",
      };
      b.Click += (_, _) => this.showPokémon(i);
      return b;
    });

    this.updateImage = bindImage(
      this.comboBoxName,
      this.pictureBoxPokémon,
      () => this.currPokémon
    );

    this.comboBoxesMoves = generate(this.layoutPokémon, Pokémon.MovesNumber, i => {
      ComboBox cb = new() {
        Dock = DockStyle.Fill,
        DropDownStyle = ComboBoxStyle.DropDownList
      };
      this.bindMoves(cb, i);
      return cb;
    });

    _ = bindImage(this.comboBoxOpponent, this.pictureBoxOpponent, () => this.opponent);

    _ = generate(this.layoutStats, 5, i => new Label() {
      AutoSize = true,
      Dock = DockStyle.Fill,
      Text = Pokémon.Stats[i],
      TextAlign = ContentAlignment.MiddleCenter
    });

    this.numericUpDownsStats = generate(this.layoutStats, 10, i => new NumericUpDown() {
      Dock = DockStyle.Fill,
      Maximum = 250,
      Minimum = -250
    });

    this.showPokémon(0);
  }

  private Pokémon currPokémon => this.team[this.currPokémonIndex];

  private static T[] generate<T>(
    TableLayoutPanel table,
    int count,
    Func<int, T> generator
  ) where T : Control =>
    Enumerable.Range(0, count).Select(i => {
      T control = generator(i);
      table.Controls.Add(control);
      return control;
    }).ToArray();

  private static Action bindImage(ComboBox name, PictureBox pb, Func<Pokémon> toUpdate) {
    void updateImage() => pb.Load(Pokémon.GetImageURL(name.SelectedIndex));

    bind(name, Pokémon.PokémonNames, i => {
      toUpdate().Index = i;
      updateImage();
    });

    updateImage();
    return updateImage;
  }

  private void bindMoves(ComboBox move, int i) =>
    bind(move, Pokémon.AllMoves, selected => this.currPokémon.Moves[i] = selected);

  private static void bind(ComboBox cb, string[] items, Action<int> onChange) {
    _ = cb.Items.Add("");
    cb.Items.AddRange(items);
    cb.SelectedIndex = 0;
    cb.SelectedIndexChanged += (_, _) => onChange(cb.SelectedIndex - 1);
  }

  private void showPokémon(int i) {
    this.currPokémonIndex = i;
    forEachEnumerate(
      this.buttonsPokémons,
      (b, i) => b.Enabled = i != this.currPokémonIndex
    );

    this.comboBoxName.SelectedIndex = this.currPokémon.Index + 1;
    this.updateImage();
    forEachEnumerate(
      this.comboBoxesMoves,
      (move, i) => move.SelectedIndex = this.currPokémon.Moves[i] + 1
    );
  }

  private static void forEachEnumerate<T>(T[] source, Action<T, int> action) {
    for (int i = 0; i < source.Length; i++) action(source[i], i);
  }

  private async void onClickPredict(object sender, EventArgs e) {
    using CancellationTokenSource source = new();
    this.animatePredict(source.Token);
    await this.predict();
    source.Cancel();
  }

  private async void animatePredict(CancellationToken token) {
    this.buttonPredict.Enabled = false;
    string prevText = this.buttonPredict.Text;

    try {
      for (int n = 0; ; n = (n + 1) % 4) {
        this.buttonPredict.Text = $"Valutazione in corso{new('.', n)}";
        await Task.Delay(250, token);
      }
    } catch (OperationCanceledException) { }

    this.buttonPredict.Text = prevText;
    this.buttonPredict.Enabled = true;
  }

  private async Task predict() {
    if (this.currPokémon.Index == -1) {
      showError(false, "Pokémon attivo non selezionato");
      return;
    }

    if (this.opponent.Index == -1) {
      showError(false, "Avversario non specificato");
      return;
    }

    string? python = getPythonPath();
    if (python == null) {
      showError(true, "Python non trovato");
      return;
    }

    using Process? p = launchPythonScript(python, ScriptPath);
    if (p == null) {
      showError(true, "Impossibile avviare il modello: errore sconosciuto");
      return;
    }

    void write(string str) => p.StandardInput.WriteLine(str);

    int teamSize = 0;
    foreach (Pokémon pok in this.team) {
      if (pok.Index == -1) continue;

      teamSize++;
      write(pok.Name);

      int moves = 0;
      foreach (int i in pok.Moves) {
        if (i >= 0) {
          moves++;
          write(Pokémon.AllMoves[i]);
        }
      }

      if (moves < Pokémon.MovesNumber) write("None");
    }

    if (teamSize < 6) write("None");

    write(this.currPokémon.Name);
    write(this.opponent.Name);
    write("Si");
    foreach (NumericUpDown nup in this.numericUpDownsStats) write(nup.Value.ToString());

    string err = await p.StandardError.ReadToEndAsync();
    if (err.Length > 0) {
      showError(true, $"Errore durante l'esecuzione del modello:\n{err}");
      return;
    }

    string output = await p.StandardOutput.ReadToEndAsync();
    _ = MessageBox.Show(output[output.IndexOf("Mosse")..^2]);
  }

  private static void showError(bool severe, string message) =>
    MessageBox.Show(
      message,
      severe ? "Errore del modello" : "Input errato",
      MessageBoxButtons.OK,
      severe ? MessageBoxIcon.Error : MessageBoxIcon.Warning
    );

  private static Process? launchPythonScript(string pythonPath, string scriptPath) =>
    Process.Start(new ProcessStartInfo() {
      FileName = pythonPath,
      Arguments = Path.GetFileName(scriptPath),
      WorkingDirectory = Path.GetDirectoryName(scriptPath),
      RedirectStandardInput = true,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      CreateNoWindow = true
    });

  private static string? getPythonPath() =>
    Environment.GetEnvironmentVariable("PATH")
      ?.Split(";")
      .Select(p => Path.Combine(p, "python.exe"))
      .FirstOrDefault(File.Exists);
}
