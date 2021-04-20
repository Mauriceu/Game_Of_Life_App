using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Game_Of_Life_App
{
    
    public class GameBoard
    {

        // Reference to rendered Grid
        private Grid Spielfläche;

        // 2D-Liste, enthält alle existierenden Zellen
        public ObservableCollection<List<Cell>> Board;

        private int _width;
        private int _height;

        public GameBoard(Grid spielfläche, int height, int width)
        {
            Spielfläche = spielfläche;
            _width = width;
            _height = height;
        }
        
        // Renders the grid with given postion-values
        public void RenderGrid()
        {
            Spielfläche.ColumnDefinitions.Clear();
            Spielfläche.RowDefinitions.Clear();

            for (int i = 0; i < _width; i++)
            {
                Spielfläche.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < _height; i++)
            {
                Spielfläche.RowDefinitions.Add(new RowDefinition());
            }
            
            for (int row = 0; row < _height; row++)
            {
                for (int cell = 0; cell < _width; cell++)
                {
                    Cell gameCell = Board[row][cell];
                    gameCell.RenderRectangle(row, cell, _height, _width, Spielfläche);
                    
                }
            }
        }

        public void UpdateGrid(int offsetX = 0, int offsetY = 0)
        {
            if (offsetY != 0)
            {
                for (int i = 0; i < _height; i++)
                {
                    int actualViewY = _height - offsetX;
                    for (int ii = 0; ii < _height - offsetY; ii++)
                    { 
                        Cell gameCell = Board[actualViewY][ii];
                        gameCell.RenderRectangle(actualViewY, ii, _height, _width, Spielfläche);
                    }
                }
            }
            
            if (offsetX != 0)
            {
                for (int i = offsetX; i >= 0; i--)
                {
                    int actualViewY = _height - offsetX;
                    int actualViewX = _width;

                    Cell gameCell = Board[actualViewY][actualViewX];
                    gameCell.RenderRectangle(actualViewY, actualViewX, _height, _width, Spielfläche);
                }
            }

            if (offsetY != 0 && offsetX != 0)
            {
                for (int i = offsetX + offsetY; i >= 0; i--)
                {
                    int actualViewY = _height - i;
                    int actualViewX = _width + offsetX;

                    Cell gameCell = Board[actualViewY][actualViewX];
                    gameCell.RenderRectangle(actualViewY, actualViewX, _height, _width, Spielfläche);
                }
            }
        }
        
        /**
         * Erstellt die 2D-Liste,
         * Anhand der Übergabeparameter wird die Größe (Höhe, Breite) festgelegt
         */
        public void FillBoard()
        {
            Board = new ObservableCollection<List<Cell>>();

            for (int posY = 0; posY < _height; posY++)
            {
                Board.Add(new List<Cell>());
                for (int posX = 0; posX < _width; posX++)
                {
                    CreateCell(posY, posX);
                }
                SetNeighbourCells(posY);
            }
        }
        
        // Erstellt die Zelle und rendered die Zell-Fläche auf dem Grid
        private void CreateCell(int row, int cell)
        {
            string id = row.ToString() + cell;
            Cell gameCell = new Cell(id);
            
            gameCell.RenderRectangle(row, cell, _height, _width, Spielfläche);

            Board[row].Add(gameCell);
        }

        /**
         * Iteriert die aktuelle Board-Reihe ein zweites mal und setzt die Zelle der vorherigen Reihe (falls vorhanden)
         * und die Zellen der aktuellen Reihe als Nachbarzellen
         */
        private void SetNeighbourCells(int posY)
        {
            for (int posX = 0; posX < _width; posX++)
            {
                IterateRowHelper(posY, posX,-1);
                IterateRowHelper(posY, posX,0);
            }
        }
        
        /**
         * Iteriert die aktuelle Board-Reihe durch und setzt gefundene Zellen als Nachbarzellen,
         * ignoriert OutOfBounds-Error:
         * Diese bedeuten nämlich einfach, dass an diesem Platz keine Nachbarzelle existiert (außerhalb des Spielfeldes)
         */
        private void IterateRowHelper(int posY, int posX,  int offsetY)
        {
            if ((posY + offsetY) == -1 || (posY + offsetY) == _height)
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


        public void RandomizeLivingCells(int maxLivingStartCells)
        {
            int currentLivingStartCells = 0;
            var r = new Random();
            
            do
            {
                int row = r.Next(1, _height);
                int cell = r.Next(1, _width);

                if (!Board[row][cell].IsAlive())
                {
                    Board[row][cell].StatusChange(true);
                    currentLivingStartCells++;
                }
            } while (currentLivingStartCells < maxLivingStartCells);
        }
        
        /**
         * Board wird von oben links nach unten rechts iteriert. Jede Zelle durchläuft die "lebt"/"stirbt"-Logik
         * Übergabeparameter werden von der Timer-Funktion erwartet, sind jedoch nicht notwendig
         */
        public void NextGeneration()
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


        // Interval in s
        public DispatcherTimer PlayWithTimer(int interval)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += NextGenerationOnTimer;
            dispatcherTimer.Interval = new TimeSpan(0,0,interval);
            dispatcherTimer.Start();

            return dispatcherTimer;
        }
        private void NextGenerationOnTimer(object sender, EventArgs e)
        {
            NextGeneration();
        }
    }
}