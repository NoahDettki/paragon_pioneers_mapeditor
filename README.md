# Paragon Pioneers Mapeditor
Dieses Projekt ist im Rahmen des Lernfelds 12a der Ausbildung "Fachinformatiker für Anwendungsentwicklung" am OSZIMT entstanden. Entwickelt wurde es Ende 2024 und Anfang 2025 von Marvin Pöhls und Noah Dettki.

## Programm ausführen (Windows)
Um das Projekt testen zu können, wird **Microsoft Visual Studio 2022** (2019 funktioniert wahrscheinlich auch), **.NET Developer Pack 4.8** sowie **Git** benötigt.
1. Öffnen Sie den Ordner, in den das Repository heruntergeladen werden soll, in der cmd.
2. Geben Sie den Befehl ```git clone https://github.com/NoahDettki/paragon_pioneers_mapeditor.git``` ein.
3. Öffnen Sie die Projektmappe in Microsoft Visual Studio 2022. Wählen Sie dazu ```Datei > Öffnen > Projekt/Projektmappe``` und wählen Sie die Datei ```ParagonPioneers.sln``` im Projektordner aus.
4. Falls ein Warnhinweis für eine falsche .NET Framework Version erscheint, können Sie unter ```https://dotnet.microsoft.com/en-us/download/visual-studio-sdks?cid=getdotnetsdk``` .NET Framework 4.8 Developer Pack herunterladen und installieren. Wiederholen Sie danach Schritt 3.
5. Klicken Sie auf den Knopf "Starten" mit dem grünen Pfeil um das Programm zu starten.

## Testkarten
Tragen Sie eine der folgenden Beispielkarten in das Import-Fenster ein, falls Sie noch keine ParagonPioneersMap Datei zum Laden haben und drücken Sie "Generieren".  
Leere Karte 9x9:
```
000000000
000000000
000000000
000000000
000000000
000000000
000000000
000000000
000000000
```
Beispiel Karte 9x9:
```
KKKWWKKKK
K0KKWK02K
K31KWKK1K
K00KKWK0K
KGGGKWK0K
KG2GKWKKK
KGGGKKWWW
K0101KWWW
KKKKKKWWW
```

## Steuerung im Editor
- Rechte Maustaste: Karte ziehen
- Mausrad: Zoomen
- Linke Maustaste: Zuerst Bodentyp an der rechten Seite des Editors auswählen, dann gedrückt halten um den Bodentyp auf den Kacheln anzuwenden
