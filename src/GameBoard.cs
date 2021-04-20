using System;
using System.Collections.Generic;
using System.Timers;

namespace Game_Of_Life_App
{
    
    public class GameBoard
    {

        /**
         * 2D-Liste, enthält alle existierenden Zellen
         */
        public List<List<Cell>> Board;
        private int _width;
        public int Width => _width;

        private int _height;
        public int Height => _height;


        public GameBoard(int x, int y)
        {
            _width = x;
            _height = y;
        }
        public GameBoard()
        {
            _width = 5;
            _height = 5;
        }
        /**
         * Erstellt die 2D-Liste,
         * Anhand der Übergabeparameter wird die Größe (Höhe, Breite) festgelegt
         */
        public void FillBoard()
        {
            Board = new List<List<Cell>>();

            for (int posY = 0; posY < Height; posY++)
            {
                Board.Add(new List<Cell>());
                for (int posX = 0; posX < Width; posX++)
                {
                    Board[posY].Add(new Cell(posY.ToString() + posX));
                }
                SetNeighbourCells(posY);
            }
        }

        /**
         * Iteriert die aktuelle Board-Reihe ein zweites mal und setzt die Zelle der vorherigen Reihe (falls vorhanden)
         * und die Zellen der aktuellen Reihe als Nachbarzellen
         */
        private void SetNeighbourCells(int posY)
        {
            for (int posX = 0; posX < Width; posX++)
            {
                IterateRowHelper(posY, posX,-1);
                IterateRowHelper(posY, posX,0);
            }
        }
        
        /**
         * Iteriert die aktuelle Board-Reihe durch und setzt gefundene Zellen als Nachbarzellen,
         * ignoriert OutOfBounds-Error:
         * Diese bedeuten nämlich einfach, dass an diesem Platz keine Nachbarzelle existiert, da außerhalb des Spielfeldes
         */
        private void IterateRowHelper(int posY, int posX,  int offsetY)
        {
            if ((posY + offsetY) == -1 || (posY + offsetY) == Height)
            {
                return;
            }
            
            Cell currentCell = Board[posY][posX];
            for (var offsetX = -1; offsetX <= 1; offsetX++)
            {
                Cell neighbour = null;
                try
                {
                    neighbour = Board[posY + offsetY][posX + offsetX];
                }
                catch (Exception e)
                {
                    // ignored
                }

                if (neighbour != null &&
                    neighbour.Id != Board[posY][posX].Id &&
                    !currentCell.HasNeighbour(neighbour))
                {
                    currentCell.AddNeighbour(neighbour);
                    neighbour.AddNeighbour(currentCell);
                }
            }
        }

        /**
         * Board wird von oben links nach unten rechts iteriert. Jede Zelle durchläuft die "lebt"/"stirbt"-Logik
         * Übergabeparameter werden von der Timer-Funktion erwartet, sind jedoch nicht notwendig
         */
        public void NextGeneration(Object source = null, ElapsedEventArgs e = null)
        {
            foreach (var row in Board)
            {
                foreach (Cell cell in row)
                {
                    cell.CheckIfCellLivesAfterGenerationChange();
                }
            }
            FinishGenerationChange();
        }

        /**
         * Das gespeicherte Ergebnis des Generationswechsel wird nun für jede Zelle übernommen
         */
        private void FinishGenerationChange()
        {
            foreach (var row in Board)
            {
                foreach (Cell cell in row)
                {
                    cell.EvolveCell();
                }
            }
        }
    }
}