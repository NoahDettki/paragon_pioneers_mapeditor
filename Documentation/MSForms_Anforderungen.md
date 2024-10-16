# Anforderungen an Windows Forms f�r das Projekt �Paragon Pioneers � Karteneditor�

## Karten-Dateien k�nnen per Drag & Drop in den Karteneditor importiert werden.
Mit Microsoft Forms kann man sehr einfach eine Drag & Drop Funktion in eine Form integrieren. Nachdem der Dateipfad bekannt ist, kann man die Datei �ber die �blichen Wege auslesen.
```
public Form1() {
    InitializeComponent();
    this.AllowDrop = true;
    this.DragEnter += new DragEventHandler(Form1_DragEnter);
    this.DragDrop += new DragEventHandler(Form1_DragDrop);
}

void Form1_DragEnter(object sender, DragEventArgs e) {
    if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
}

void Form1_DragDrop(object sender, DragEventArgs e) {
    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
    foreach (string file in files) System.Diagnostics.Debug.WriteLine(file);
}
```


## Die Karte ist im Karteneditor frei navigierbar. Der Nutzer kann mit gedr�ckter Maustaste die Karte im Ansichtspanel verschieben. �ber das Mausrad kann die Ansicht verkleinert und vergr��ert werden.
Es gibt keine Komponente die von sich aus diese Funktionen erlaubt. An dieser Stelle m�ssen wir selbst die Funktionen implementieren, indem wir auf die Eingaben der Maus reagieren. Dazu k�nnen wir EventListener benutzen.

## Im Export-Men� �ffnet sich der Windows File Explorer, wenn der Nutzer den Dateipfad �ndern will.
Mit dem OpenFileDialog von Microsoft Forms kann diese Funktion umgesetzt werden.
```
private Button selectButton;
private OpenFileDialog openFileDialog1;

public Form1() {
    //InitializeComponent();
    openFileDialog1 = new OpenFileDialog();
    selectButton = new Button {
        Size = new Size(100, 20),
        Location = new Point(15, 15),
        Text = "Select file"
    };
    selectButton.Click += new EventHandler(SelectButton_Click);
    ClientSize = new Size(330, 360);
    Controls.Add(selectButton);
}

private void SelectButton_Click(object sender, EventArgs e) {
    if (openFileDialog1.ShowDialog() == DialogResult.OK) {
        try {
            // openFileDialog1.FileName ist der Name und Pfad des ausgew�hlten Files
            Console.WriteLine(openFileDialog1.FileName);
        } catch (SecurityException ex) {
            MessageBox.Show($"Security error.\n\nError message: {ex.Message}");
        }
    }
}
```


## Die Sprites der Kartenfelder werden korrekt gerendert und k�nnen in der Gr��e skalieren.
Die PictureBox Komponente von Microsoft Forms erlaubt es Bilder anzuzeigen und zu skalieren!