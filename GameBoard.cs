using System;
using System.Collections.Generic;
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
        public void FillBoard()
        {
            Board = new List<List<Cell>>();

            for (int posY = 0; posY < _numberRows; posY++)
            {
                Board.Add(new List<Cell>());

                for (int posX = 0; posX < _numberColumns; posX++)
                {
                    CreateCell(posY, posX);
                }
                SetNeighbourCells(posY);
            }
        }

        // Erstellt die Zelle
        private void CreateCell(int posY, int posX)
        {
            string id = posY.ToString() + posX;
            Cell gameCell = new Cell(id);
            Board[posY].Add(gameCell);
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
            if (!_generationChangeFinished) return;

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
            NextGeneration();
        }
    }
}