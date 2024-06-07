using System.Diagnostics;

using ml_project_BarbanAlberto.models;

namespace ml_project_BarbanAlberto;

public partial class MainForm : Form {
  private const string ScriptPath = @"model\main.py";
  private readonly Pok�mon[] team = Pok�mon.CreateTeam();
  private int currPok�monIndex;
  private readonly Pok�mon opponent = new();
  private readonly Button[] buttonsPok�mons;
  private readonly ComboBox[] comboBoxesMoves;
  private readonly NumericUpDown[] numericUpDownsStats;
  private readonly Action updateImage;

  public MainForm() {
    this.InitializeComponent();

    this.buttonsPok�mons = generate(this.layoutPok�mons, Pok�mon.TeamSize, i => {
      Button b = new() {
        Dock = DockStyle.Fill,
        Text = $"Pok�mon {i + 1}",
      };
      b.Click += (_, _) => this.showPok�mon(i);
      return b;
    });

    this.updateImage = bindImage(
      this.comboBoxName,
      this.pictureBoxPok�mon,
      () => this.currPok�mon
    );

    this.comboBoxesMoves = generate(this.layoutPok�mon, Pok�mon.MovesNumber, i => {
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
      Text = Pok�mon.Stats[i],
      TextAlign = ContentAlignment.MiddleCenter
    });

    this.numericUpDownsStats = generate(this.layoutStats, 10, i => new NumericUpDown() {
      Dock = DockStyle.Fill,
      Maximum = 250,
      Minimum = -250
    });

    this.showPok�mon(0);
  }

  private Pok�mon currPok�mon => this.team[this.currPok�monIndex];

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

  private static Action bindImage(ComboBox name, PictureBox pb, Func<Pok�mon> toUpdate) {
    void updateImage() => pb.Load(Pok�mon.GetImageURL(name.SelectedIndex));

    bind(name, Pok�mon.Pok�monNames, i => {
      toUpdate().Index = i;
      updateImage();
    });

    updateImage();
    return updateImage;
  }

  private void bindMoves(ComboBox move, int i) =>
    bind(move, Pok�mon.AllMoves, selected => this.currPok�mon.Moves[i] = selected);

  private static void bind(ComboBox cb, string[] items, Action<int> onChange) {
    _ = cb.Items.Add("");
    cb.Items.AddRange(items);
    cb.SelectedIndex = 0;
    cb.SelectedIndexChanged += (_, _) => onChange(cb.SelectedIndex - 1);
  }

  private void showPok�mon(int i) {
    this.currPok�monIndex = i;
    forEachEnumerate(
      this.buttonsPok�mons,
      (b, i) => b.Enabled = i != this.currPok�monIndex
    );

    this.comboBoxName.SelectedIndex = this.currPok�mon.Index + 1;
    this.updateImage();
    forEachEnumerate(
      this.comboBoxesMoves,
      (move, i) => move.SelectedIndex = this.currPok�mon.Moves[i] + 1
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
    if (this.currPok�mon.Index == -1) {
      showError(false, "Pok�mon attivo non selezionato");
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
    foreach (Pok�mon pok in this.team) {
      if (pok.Index == -1) continue;

      teamSize++;
      write(pok.Name);

      int moves = 0;
      foreach (int i in pok.Moves) {
        if (i >= 0) {
          moves++;
          write(Pok�mon.AllMoves[i]);
        }
      }

      if (moves < Pok�mon.MovesNumber) write("None");
    }

    if (teamSize < 6) write("None");

    write(this.currPok�mon.Name);
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
