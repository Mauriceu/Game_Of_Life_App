using System;
using System.Collections.Generic;
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

        // 2D-Liste, enthält alle existierenden Zellen
        public List<List<Cell>> Board;
        
        private readonly int _numberRows;
        private readonly int _numberColumns;

        private bool _generationChangeFinished = true;
        public GameBoard(int numberRows, int numberColumns)
        {
            _numberRows = numberRows;
            _numberColumns = numberColumns;
        }
        
        /**
         * Erstellt die 2D-Liste
         */
        public void FillBoard(Grid Spielfläche)
        {
            InitializeGrid(Spielfläche);

            Board = new List<List<Cell>>();

            for (int posY = 0; posY < _numberRows; posY++)
            {
                Board.Add(new List<Cell>());

                for (int posX = 0; posX < _numberColumns; posX++)
                {
                    CreateCell(posY, posX, Spielfläche);
                }
                SetNeighbourCells(posY);
            }
        }

        /**
         * Create Grid-Rows and -Columns
         */
        private void InitializeGrid(Grid Spielfläche)
        {
            Spielfläche.Children.Clear();
            Spielfläche.RowDefinitions.Clear();
            Spielfläche.ColumnDefinitions.Clear();

            for (int posY = 0; posY < _numberRows; posY++)
            {
                Spielfläche.RowDefinitions.Add(new RowDefinition());
            }

            for (int posX = 0; posX < _numberColumns; posX++)
            {
                Spielfläche.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
        
        // Erstellt die Zelle und rendered die Zell-Fläche auf dem Canvas
        private void CreateCell(int posY, int posX, Grid Spielfläche)
        {
            string id = posY.ToString() + posX;
            Cell gameCell = new Cell(id);
            
            Rectangle rectangle = new Rectangle
            {
                Stroke = Brushes.Black,
                Fill = Brushes.White,
            };
            gameCell.SetRectangle(rectangle);
            Board[posY].Add(gameCell);

            Grid.SetColumn(rectangle, posX);
            Grid.SetRow(rectangle, posY);
            
            Spielfläche.Children.Add(rectangle);
        }

        /**
         * Iteriert die erstellte Board-Reihe und setzt die entsprechenden Zellen der vorherigen (falls vorhanden)
         * und aktuellen Reihe als Nachbarzellen
         */
        private void SetNeighbourCells(int posY)
        {
            for (int posX = 0; posX < _numberColumns; posX++)
            {
                IterateRowHelper(posY, posX,-1); // iterate previous row
                IterateRowHelper(posY, posX,0); // iterate current row
            }
        }
        
        /**
         * Iteriert durch die unmittelbaren Nachbarzellen links und rechts der aktuellen Zelle und setzt diese als Nachbarzellen,
         * ignoriert OutOfBounds-Error:
         * Diese bedeuten nämlich einfach, dass an diesem Platz keine Nachbarzelle existiert (außerhalb des Spielfeldes)
         */
        private void IterateRowHelper(int posY, int posX,  int offsetY)
        {
            // if row is out of bounds or maxValue
            if ((posY + offsetY) == -1 || (posY + offsetY) == _numberRows)
            {
                return;
            }
            
            // get currentCell
            Cell currentCell = Board[posY][posX];
            
            // currentCell is "centered", iterate from next left item to next right item
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
                    neighbour.GetId() != Board[posY][posX].GetId() &&
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
                int row = r.Next(0, _numberRows);
                int cell = r.Next(0, _numberColumns);

                if (!Board[row][cell].IsAlive())
                {
                    Board[row][cell].StatusChange(true);
                    currentLivingStartCells++;
                }
            } while (currentLivingStartCells < maxLivingStartCells);
        }
        
        /**
         * Board wird von oben links nach unten rechts iteriert. Jede Zelle durchläuft die "lebt"/"stirbt"-Logik
         */
        public void NextGeneration()
        {
            _generationChangeFinished = false;
            foreach (var row in Board)
            {
                foreach (Cell cell in row)
                {
                    cell.CheckIfCellLivesAfterGenerationChange();
                }
            }
            FinishGenerationChange();
            _generationChangeFinished = true;
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
            dispatcherTimer.Interval = new TimeSpan(0,0,0, 0,interval);
            dispatcherTimer.Start();

            return dispatcherTimer;
        }
        /**
         * Übergabeparameter werden von einer Timer-Event-Funktion erwartet, sind jedoch nicht notwendig
         */
        private void NextGenerationOnTimer(object sender, EventArgs e)
        {
            if (_generationChangeFinished)
            {
                NextGeneration();
            }
            
        }
    }
}