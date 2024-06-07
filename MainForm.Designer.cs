namespace ml_project_BarbanAlberto;

partial class MainForm {
  /// <summary>
  ///  Required designer variable.
  /// </summary>
  private System.ComponentModel.IContainer components = null;

  /// <summary>
  ///  Clean up any resources being used.
  /// </summary>
  /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
  protected override void Dispose(bool disposing) {
    if (disposing && (components != null)) {
      components.Dispose();
    }
    base.Dispose(disposing);
  }

  #region Windows Form Designer generated code

  /// <summary>
  ///  Required method for Designer support - do not modify
  ///  the contents of this method with the code editor.
  /// </summary>
  private void InitializeComponent() {
    Label labelTitle;
    TableLayoutPanel layoutMain;
    TableLayoutPanel layoutTeam;
    Label labelTeam;
    Label labelName;
    Label labelMoves;
    TableLayoutPanel layoutOtherInputs;
    TableLayoutPanel layoutOpponent;
    Label labelOpponent;
    Label labelOpponentName;
    Label labelStats;
    Label labelPlayerStats;
    Label labelOpponentStats;
    Label labelPlayer;
    this.layoutPokémons = new TableLayoutPanel();
    this.layoutPokémon = new TableLayoutPanel();
    this.pictureBoxPokémon = new PictureBox();
    this.comboBoxName = new ComboBox();
    this.comboBoxOpponent = new ComboBox();
    this.pictureBoxOpponent = new PictureBox();
    this.layoutStats = new TableLayoutPanel();
    this.buttonPredict = new Button();
    labelTitle = new Label();
    layoutMain = new TableLayoutPanel();
    layoutTeam = new TableLayoutPanel();
    labelTeam = new Label();
    labelName = new Label();
    labelMoves = new Label();
    layoutOtherInputs = new TableLayoutPanel();
    layoutOpponent = new TableLayoutPanel();
    labelOpponent = new Label();
    labelOpponentName = new Label();
    labelStats = new Label();
    labelPlayerStats = new Label();
    labelOpponentStats = new Label();
    labelPlayer = new Label();
    layoutMain.SuspendLayout();
    layoutTeam.SuspendLayout();
    this.layoutPokémon.SuspendLayout();
    ((System.ComponentModel.ISupportInitialize) this.pictureBoxPokémon).BeginInit();
    layoutOtherInputs.SuspendLayout();
    layoutOpponent.SuspendLayout();
    ((System.ComponentModel.ISupportInitialize) this.pictureBoxOpponent).BeginInit();
    this.layoutStats.SuspendLayout();
    this.SuspendLayout();
    // 
    // labelTitle
    // 
    labelTitle.AutoSize = true;
    layoutMain.SetColumnSpan(labelTitle, 2);
    labelTitle.Dock = DockStyle.Fill;
    labelTitle.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
    labelTitle.Location = new Point(3, 0);
    labelTitle.Name = "labelTitle";
    labelTitle.Size = new Size(794, 37);
    labelTitle.TabIndex = 0;
    labelTitle.Text = "AI per battaglie Pokémon";
    labelTitle.TextAlign = ContentAlignment.MiddleCenter;
    // 
    // layoutMain
    // 
    layoutMain.ColumnCount = 2;
    layoutMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
    layoutMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
    layoutMain.Controls.Add(labelTitle, 0, 0);
    layoutMain.Controls.Add(layoutTeam, 0, 1);
    layoutMain.Controls.Add(layoutOtherInputs, 1, 1);
    layoutMain.Dock = DockStyle.Fill;
    layoutMain.Location = new Point(0, 0);
    layoutMain.Name = "layoutMain";
    layoutMain.RowCount = 2;
    layoutMain.RowStyles.Add(new RowStyle());
    layoutMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
    layoutMain.Size = new Size(800, 450);
    layoutMain.TabIndex = 1;
    // 
    // layoutTeam
    // 
    layoutTeam.ColumnCount = 1;
    layoutTeam.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
    layoutTeam.Controls.Add(labelTeam, 0, 0);
    layoutTeam.Controls.Add(this.layoutPokémons, 0, 1);
    layoutTeam.Controls.Add(this.layoutPokémon, 0, 2);
    layoutTeam.Dock = DockStyle.Fill;
    layoutTeam.Location = new Point(3, 40);
    layoutTeam.Name = "layoutTeam";
    layoutTeam.RowCount = 3;
    layoutTeam.RowStyles.Add(new RowStyle());
    layoutTeam.RowStyles.Add(new RowStyle());
    layoutTeam.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
    layoutTeam.Size = new Size(394, 407);
    layoutTeam.TabIndex = 1;
    // 
    // labelTeam
    // 
    labelTeam.AutoSize = true;
    labelTeam.Dock = DockStyle.Fill;
    labelTeam.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
    labelTeam.Location = new Point(3, 0);
    labelTeam.Name = "labelTeam";
    labelTeam.Size = new Size(388, 25);
    labelTeam.TabIndex = 0;
    labelTeam.Text = "Squadra";
    labelTeam.TextAlign = ContentAlignment.MiddleCenter;
    // 
    // layoutPokémons
    // 
    this.layoutPokémons.AutoSize = true;
    this.layoutPokémons.ColumnCount = 3;
    this.layoutPokémons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
    this.layoutPokémons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
    this.layoutPokémons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
    this.layoutPokémons.Dock = DockStyle.Fill;
    this.layoutPokémons.Location = new Point(3, 28);
    this.layoutPokémons.Name = "layoutPokémons";
    this.layoutPokémons.RowCount = 2;
    this.layoutPokémons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
    this.layoutPokémons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
    this.layoutPokémons.Size = new Size(388, 1);
    this.layoutPokémons.TabIndex = 1;
    // 
    // layoutPokémon
    // 
    this.layoutPokémon.ColumnCount = 2;
    this.layoutPokémon.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
    this.layoutPokémon.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
    this.layoutPokémon.Controls.Add(labelName, 0, 0);
    this.layoutPokémon.Controls.Add(this.pictureBoxPokémon, 0, 1);
    this.layoutPokémon.Controls.Add(this.comboBoxName, 1, 0);
    this.layoutPokémon.Controls.Add(labelMoves, 0, 2);
    this.layoutPokémon.Dock = DockStyle.Fill;
    this.layoutPokémon.Location = new Point(3, 34);
    this.layoutPokémon.Name = "layoutPokémon";
    this.layoutPokémon.RowCount = 5;
    this.layoutPokémon.RowStyles.Add(new RowStyle());
    this.layoutPokémon.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
    this.layoutPokémon.RowStyles.Add(new RowStyle());
    this.layoutPokémon.RowStyles.Add(new RowStyle());
    this.layoutPokémon.RowStyles.Add(new RowStyle());
    this.layoutPokémon.Size = new Size(388, 370);
    this.layoutPokémon.TabIndex = 2;
    // 
    // labelName
    // 
    labelName.AutoSize = true;
    labelName.Dock = DockStyle.Fill;
    labelName.Location = new Point(3, 0);
    labelName.Name = "labelName";
    labelName.Size = new Size(188, 29);
    labelName.TabIndex = 0;
    labelName.Text = "Nome:";
    labelName.TextAlign = ContentAlignment.MiddleLeft;
    // 
    // pictureBoxPokémon
    // 
    this.layoutPokémon.SetColumnSpan(this.pictureBoxPokémon, 2);
    this.pictureBoxPokémon.Dock = DockStyle.Fill;
    this.pictureBoxPokémon.Location = new Point(3, 32);
    this.pictureBoxPokémon.Name = "pictureBoxPokémon";
    this.pictureBoxPokémon.Size = new Size(382, 314);
    this.pictureBoxPokémon.SizeMode = PictureBoxSizeMode.Zoom;
    this.pictureBoxPokémon.TabIndex = 1;
    this.pictureBoxPokémon.TabStop = false;
    // 
    // comboBoxName
    // 
    this.comboBoxName.Dock = DockStyle.Fill;
    this.comboBoxName.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxName.FormattingEnabled = true;
    this.comboBoxName.Location = new Point(197, 3);
    this.comboBoxName.Name = "comboBoxName";
    this.comboBoxName.Size = new Size(188, 23);
    this.comboBoxName.TabIndex = 2;
    // 
    // labelMoves
    // 
    labelMoves.AutoSize = true;
    this.layoutPokémon.SetColumnSpan(labelMoves, 2);
    labelMoves.Dock = DockStyle.Fill;
    labelMoves.Location = new Point(3, 352);
    labelMoves.Margin = new Padding(3);
    labelMoves.Name = "labelMoves";
    labelMoves.Size = new Size(382, 15);
    labelMoves.TabIndex = 7;
    labelMoves.Text = "Mosse:";
    labelMoves.TextAlign = ContentAlignment.MiddleCenter;
    // 
    // layoutOtherInputs
    // 
    layoutOtherInputs.ColumnCount = 1;
    layoutOtherInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
    layoutOtherInputs.Controls.Add(layoutOpponent, 0, 0);
    layoutOtherInputs.Controls.Add(this.layoutStats, 0, 1);
    layoutOtherInputs.Controls.Add(this.buttonPredict, 0, 2);
    layoutOtherInputs.Dock = DockStyle.Fill;
    layoutOtherInputs.Location = new Point(403, 40);
    layoutOtherInputs.Name = "layoutOtherInputs";
    layoutOtherInputs.RowCount = 3;
    layoutOtherInputs.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
    layoutOtherInputs.RowStyles.Add(new RowStyle());
    layoutOtherInputs.RowStyles.Add(new RowStyle());
    layoutOtherInputs.Size = new Size(394, 407);
    layoutOtherInputs.TabIndex = 2;
    // 
    // layoutOpponent
    // 
    layoutOpponent.ColumnCount = 2;
    layoutOpponent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
    layoutOpponent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
    layoutOpponent.Controls.Add(labelOpponent, 0, 0);
    layoutOpponent.Controls.Add(labelOpponentName, 0, 1);
    layoutOpponent.Controls.Add(this.comboBoxOpponent, 1, 1);
    layoutOpponent.Controls.Add(this.pictureBoxOpponent, 0, 2);
    layoutOpponent.Dock = DockStyle.Fill;
    layoutOpponent.Location = new Point(3, 3);
    layoutOpponent.Name = "layoutOpponent";
    layoutOpponent.RowCount = 3;
    layoutOpponent.RowStyles.Add(new RowStyle());
    layoutOpponent.RowStyles.Add(new RowStyle());
    layoutOpponent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
    layoutOpponent.Size = new Size(388, 296);
    layoutOpponent.TabIndex = 6;
    // 
    // labelOpponent
    // 
    labelOpponent.AutoSize = true;
    layoutOpponent.SetColumnSpan(labelOpponent, 2);
    labelOpponent.Dock = DockStyle.Fill;
    labelOpponent.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
    labelOpponent.Location = new Point(3, 0);
    labelOpponent.Name = "labelOpponent";
    labelOpponent.Size = new Size(382, 25);
    labelOpponent.TabIndex = 0;
    labelOpponent.Text = "Avversario";
    labelOpponent.TextAlign = ContentAlignment.MiddleCenter;
    // 
    // labelOpponentName
    // 
    labelOpponentName.AutoSize = true;
    labelOpponentName.Dock = DockStyle.Fill;
    labelOpponentName.Location = new Point(3, 25);
    labelOpponentName.Name = "labelOpponentName";
    labelOpponentName.Size = new Size(188, 29);
    labelOpponentName.TabIndex = 1;
    labelOpponentName.Text = "Nome:";
    labelOpponentName.TextAlign = ContentAlignment.MiddleLeft;
    // 
    // comboBoxOpponent
    // 
    this.comboBoxOpponent.Dock = DockStyle.Fill;
    this.comboBoxOpponent.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxOpponent.FormattingEnabled = true;
    this.comboBoxOpponent.Location = new Point(197, 28);
    this.comboBoxOpponent.Name = "comboBoxOpponent";
    this.comboBoxOpponent.Size = new Size(188, 23);
    this.comboBoxOpponent.TabIndex = 2;
    // 
    // pictureBoxOpponent
    // 
    layoutOpponent.SetColumnSpan(this.pictureBoxOpponent, 2);
    this.pictureBoxOpponent.Dock = DockStyle.Fill;
    this.pictureBoxOpponent.Location = new Point(3, 57);
    this.pictureBoxOpponent.Name = "pictureBoxOpponent";
    this.pictureBoxOpponent.Size = new Size(382, 236);
    this.pictureBoxOpponent.SizeMode = PictureBoxSizeMode.Zoom;
    this.pictureBoxOpponent.TabIndex = 3;
    this.pictureBoxOpponent.TabStop = false;
    // 
    // layoutStats
    // 
    this.layoutStats.AutoSize = true;
    this.layoutStats.ColumnCount = 6;
    this.layoutStats.ColumnStyles.Add(new ColumnStyle());
    this.layoutStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
    this.layoutStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
    this.layoutStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
    this.layoutStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
    this.layoutStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
    this.layoutStats.Controls.Add(labelStats, 0, 0);
    this.layoutStats.Controls.Add(labelPlayerStats, 0, 2);
    this.layoutStats.Controls.Add(labelOpponentStats, 0, 3);
    this.layoutStats.Controls.Add(labelPlayer, 0, 1);
    this.layoutStats.Dock = DockStyle.Fill;
    this.layoutStats.Location = new Point(3, 305);
    this.layoutStats.Name = "layoutStats";
    this.layoutStats.RowCount = 4;
    this.layoutStats.RowStyles.Add(new RowStyle());
    this.layoutStats.RowStyles.Add(new RowStyle());
    this.layoutStats.RowStyles.Add(new RowStyle());
    this.layoutStats.RowStyles.Add(new RowStyle());
    this.layoutStats.Size = new Size(388, 70);
    this.layoutStats.TabIndex = 7;
    // 
    // labelStats
    // 
    labelStats.AutoSize = true;
    this.layoutStats.SetColumnSpan(labelStats, 6);
    labelStats.Dock = DockStyle.Fill;
    labelStats.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
    labelStats.Location = new Point(3, 0);
    labelStats.Name = "labelStats";
    labelStats.Size = new Size(382, 25);
    labelStats.TabIndex = 0;
    labelStats.Text = "Cambiamenti di stato";
    labelStats.TextAlign = ContentAlignment.MiddleCenter;
    // 
    // labelPlayerStats
    // 
    labelPlayerStats.AutoSize = true;
    labelPlayerStats.Dock = DockStyle.Fill;
    labelPlayerStats.Location = new Point(3, 40);
    labelPlayerStats.Name = "labelPlayerStats";
    labelPlayerStats.Size = new Size(62, 15);
    labelPlayerStats.TabIndex = 1;
    labelPlayerStats.Text = "Tu";
    labelPlayerStats.TextAlign = ContentAlignment.MiddleLeft;
    // 
    // labelOpponentStats
    // 
    labelOpponentStats.AutoSize = true;
    labelOpponentStats.Dock = DockStyle.Fill;
    labelOpponentStats.Location = new Point(3, 55);
    labelOpponentStats.Name = "labelOpponentStats";
    labelOpponentStats.Size = new Size(62, 15);
    labelOpponentStats.TabIndex = 2;
    labelOpponentStats.Text = "Avversario";
    labelOpponentStats.TextAlign = ContentAlignment.MiddleLeft;
    // 
    // labelPlayer
    // 
    labelPlayer.AutoSize = true;
    labelPlayer.Dock = DockStyle.Fill;
    labelPlayer.Location = new Point(3, 25);
    labelPlayer.Name = "labelPlayer";
    labelPlayer.Size = new Size(62, 15);
    labelPlayer.TabIndex = 3;
    labelPlayer.Text = "Giocatore";
    labelPlayer.TextAlign = ContentAlignment.MiddleLeft;
    // 
    // buttonPredict
    // 
    this.buttonPredict.Dock = DockStyle.Fill;
    this.buttonPredict.Location = new Point(3, 381);
    this.buttonPredict.Name = "buttonPredict";
    this.buttonPredict.Size = new Size(388, 23);
    this.buttonPredict.TabIndex = 8;
    this.buttonPredict.Text = "Ottieni mossa consigliata";
    this.buttonPredict.UseVisualStyleBackColor = true;
    this.buttonPredict.Click += this.onClickPredict;
    // 
    // MainForm
    // 
    this.AutoScaleDimensions = new SizeF(7F, 15F);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.ClientSize = new Size(800, 450);
    this.Controls.Add(layoutMain);
    this.Name = "MainForm";
    this.Text = "AI Pokémon";
    layoutMain.ResumeLayout(false);
    layoutMain.PerformLayout();
    layoutTeam.ResumeLayout(false);
    layoutTeam.PerformLayout();
    this.layoutPokémon.ResumeLayout(false);
    this.layoutPokémon.PerformLayout();
    ((System.ComponentModel.ISupportInitialize) this.pictureBoxPokémon).EndInit();
    layoutOtherInputs.ResumeLayout(false);
    layoutOtherInputs.PerformLayout();
    layoutOpponent.ResumeLayout(false);
    layoutOpponent.PerformLayout();
    ((System.ComponentModel.ISupportInitialize) this.pictureBoxOpponent).EndInit();
    this.layoutStats.ResumeLayout(false);
    this.layoutStats.PerformLayout();
    this.ResumeLayout(false);
  }

  #endregion
  private PictureBox pictureBoxPokémon;
  private ComboBox comboBoxName;
  private TableLayoutPanel layoutStats;
  private ComboBox comboBoxOpponent;
  private PictureBox pictureBoxOpponent;
  private TableLayoutPanel layoutPokémon;
  private TableLayoutPanel layoutPokémons;
  private Button buttonPredict;
}
