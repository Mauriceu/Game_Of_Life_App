using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Game_Of_Life_App
{
    public class Cell
    {

        private const int OVERPOPULATION_MINIMUM = 4;
        private const int SOLITUDE_MAXIMUM = 1;
        private const int BIRTH = 3;
        
        public string Id { get; }
        private bool _cellLivesAfterGenerationChange;
        private bool _cellIsAlive;
        private readonly List<Cell> _neighbours;
        
        // reference to rendered object
        private Rectangle _rectangle;
        public Cell(string id)
        {
            Id = id;
            _neighbours = new List<Cell>();
            _cellIsAlive = false;
            _cellLivesAfterGenerationChange = _cellIsAlive;
            
        }

        // used for mouseDown-event
        private void StatusChange(object sender, MouseButtonEventArgs e)
        {
            _cellIsAlive = !_cellIsAlive;
            _cellLivesAfterGenerationChange = _cellIsAlive;
            
            _rectangle.Fill = _cellIsAlive ? Brushes.Black : Brushes.White;
        }
        // used for randomization of start-cells
        public void StatusChange(bool value)
        {
            _cellIsAlive = value;
            _cellLivesAfterGenerationChange = value;
            
            _rectangle.Fill = value ? Brushes.Black : Brushes.White;
        }

        public void AddNeighbour(Cell neighbour)
        {
            _neighbours.Add(neighbour);
        }
        public bool HasNeighbour(Cell cell)
        {
            return _neighbours.Contains(cell);
        }

        public bool IsAlive()
        {
            return _cellIsAlive;
        }
        public void EvolveCell()
        {
            _cellIsAlive = _cellLivesAfterGenerationChange;
            _rectangle.Fill = _cellIsAlive ? Brushes.Black : Brushes.White;
        }

        public void CheckIfCellLivesAfterGenerationChange()
        {
            int livingNeighbours = 0;

            foreach (var cell in _neighbours)
            {
                if (cell.IsAlive())
                {
                    livingNeighbours++;
                }

                // Zelle stirbt an Überpopulation
                if (IsAlive() && livingNeighbours >= OVERPOPULATION_MINIMUM)
                {
                    _cellLivesAfterGenerationChange = false;
                    return;
                }
            }

            // Zelle stirbt an Einsamkeit
            if (IsAlive() && livingNeighbours <= SOLITUDE_MAXIMUM)
            {
                _cellLivesAfterGenerationChange = false;
                return;
            }

            // Zelle wird geboren (falls Tod)
            if (!IsAlive() && livingNeighbours == BIRTH)
            {
                _cellLivesAfterGenerationChange = true;
            }
        }

        public void RenderRectangle(int row, int cell, int height, int width, Canvas spielfläche)
        {
            var rectangle = new Rectangle
            {
                Height = spielfläche.ActualHeight / height - 2,
                Width = spielfläche.ActualWidth / width - 2,
                Fill = Brushes.White,
                DataContext = this
            };
            rectangle.MouseDown += StatusChange;
            _rectangle = rectangle;

            spielfläche.Children.Add(rectangle);
            Canvas.SetLeft(rectangle, row * spielfläche.ActualHeight / height + 1 );
            Canvas.SetTop(rectangle, cell * spielfläche.ActualWidth / width + 1 );
        }
    }
}