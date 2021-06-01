using System;

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace Game_Of_Life_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly int HEIGHT_MAXIMUM = 200;
        private static readonly int WIDTH_MAXIMUM = 200;
        private static readonly int TIMER_MAXIMUM = 60;

        private GameBoard _board;
        
        // if playing with a timer
        private DispatcherTimer _timerRef;

        private int _numberRows = 10;
        private int _numberColumns = 10;
        private int _timer = 5;
        private int _customMaxLivingStartCells = 10;
        private int MAX_LIVING_STARTCELLS = 100;


        //regex that matches disallowed text for text-inputs
        private static readonly Regex _regex = new Regex("[0-9]+$");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonRender_Click(object sender, RoutedEventArgs e)
        {
            _board = new GameBoard(_numberRows, _numberColumns);
            _board.FillBoard();
            InitializeGrid();

            MAX_LIVING_STARTCELLS = _numberRows * _numberColumns;
            if (_customMaxLivingStartCells > MAX_LIVING_STARTCELLS)
            {
                _customMaxLivingStartCells = _numberRows * _numberColumns;
                InputStartCells.Text = (_numberRows * _numberColumns).ToString();
            }
            
            // activate buttons
            ButtonStart.IsEnabled = true;
            InputStartCells.IsEnabled = true;
            ButtonRandomize.IsEnabled = true;
        }
        
        /**
         * Create Grid-Rows and -Columns
         */
        private void InitializeGrid()
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
            CreateRectangles();
        }
        
        private void CreateRectangles()
        {
            for (int posY = 0; posY < _numberRows; posY++)
            {
                for (int posX = 0; posX < _numberColumns; posX++)
                {
                    Rectangle rectangle = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        Fill = Brushes.White,
                    };
                    _board.Board[posY][posX].SetRectangle(rectangle);

                    Grid.SetColumn(rectangle, posX);
                    Grid.SetRow(rectangle, posY);
                    Spielfläche.Children.Add(rectangle);
                }
            }
        }
        
        
        private void ButtonRandomize_OnClick(object sender, RoutedEventArgs e)
        {
            _board.FillBoard();
            _board.RandomizeLivingCells(_customMaxLivingStartCells);
        }
        
        private void Start_Click(object sender, RoutedEventArgs e)  
        {

            if (_timer == 0)
            {
                DeactivateButtons();
                _board.NextGeneration();
                _timerRef = _board.PlayWithTimer(10);
            }
            else if (_timer > 0 && _timer < Int32.MaxValue)
            {
                DeactivateButtons();
                _board.NextGeneration();
                _timerRef = _board.PlayWithTimer(_timer * 1000);
            }
            else
            {
                _board.NextGeneration();
                ButtonCancel.IsEnabled = true;
            }
        }
        


        private void DeactivateButtons()
        {
            ButtonCancel.IsEnabled = true;
            foreach ( var cell in Spielfläche.Children)
            {
                ((Rectangle) cell).IsEnabled = false;
            }
            
            ButtonRandomize.IsEnabled = false;
            ButtonRender.IsEnabled = false;
            ButtonStart.IsEnabled = false;
            InputHeight.IsEnabled = false;
            InputWidth.IsEnabled = false;
            InputTimer.IsEnabled = false;
        }
        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            _timerRef.Stop();
            foreach ( var cell in Spielfläche.Children)
            {
                ((Rectangle) cell).IsEnabled = true;
            }
            ButtonCancel.IsEnabled = false;
            ButtonRandomize.IsEnabled = true;
            ButtonRender.IsEnabled = true;
            ButtonStart.IsEnabled = true;
            InputHeight.IsEnabled = true;
            InputWidth.IsEnabled = true;
            InputTimer.IsEnabled = true;
        }

        private void InputWidth_OnTextInput(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;
            if (!_regex.IsMatch(text))
            {
                if (text == String.Empty) return;
                
                InputWidth.BorderBrush = Brushes.Red;
                ShowError("Breite muss eine ganze Zahl sein");
                return;
            }

            int value = Int32.Parse(text);
            if (value <= WIDTH_MAXIMUM)
            {
                InputWidth.BorderBrush = Brushes.Black;
                _numberColumns = value;
            }
            else
            {
                InputWidth.BorderBrush = Brushes.Red;
                ShowError("Breite darf nicht größer als " + WIDTH_MAXIMUM + " sein");
            }
        }
        private void InputHeight_OnTextInput(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;
            if (!_regex.IsMatch(text))
            {
                if (text == String.Empty) return;
                
                InputHeight.BorderBrush = Brushes.Red;
                ShowError("Höhe muss eine ganze Zahl sein");
                return;
            }


            int value = Int32.Parse(text);
            if (value <= HEIGHT_MAXIMUM)
            {
                InputHeight.BorderBrush = Brushes.Black;
                _numberRows = value;
            }           
            else
            {
                InputHeight.BorderBrush = Brushes.Red;
                ShowError("Höhe darf nicht größer als " + HEIGHT_MAXIMUM + " sein");
            }
        }

        private void InputStartCells_OnTextInput(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox) sender).Text;
            if (!_regex.IsMatch(text))
            {
                if (text == String.Empty) return;
                
                InputStartCells.BorderBrush = Brushes.Red;
                ShowError("Anzahl muss eine ganze Zahl sein");
                return;
            }

            int value = Int32.Parse(text);
            if (value <= MAX_LIVING_STARTCELLS)
            {
                InputStartCells.BorderBrush = Brushes.Black;
                _customMaxLivingStartCells = value;
            }
            else
            {
                ShowError("Anzahl darf nicht größer als " + _customMaxLivingStartCells + " sein");
                InputStartCells.BorderBrush = Brushes.Red;
            }
        }

        private void InputTimer_OnTextInput(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;

            if (text == "")
            {
                _timer = Int32.MaxValue;
                return;
            }

            if (!_regex.IsMatch(text))
            {
                InputTimer.BorderBrush = Brushes.Red;
                ShowError("Zeit in Sekunden muss eine ganze Zahl sein. \nValide Werte sind außerdem 0 für die höchstmöglichste Frequenz (Hardwarebedingt), sowie keine Angabe für manuellen Generationswechsel.");
                return;
            }


            int value = Int32.Parse(text);
            if (value <= TIMER_MAXIMUM)
            {
                InputTimer.BorderBrush = Brushes.Black;
                _timer = value;
            }
            else
            {
                InputTimer.BorderBrush = Brushes.Red;
                ShowError("Zeit in Sekunden darf nicht höher als " + TIMER_MAXIMUM + " (1 Minute) sein. \nValide Werte sind außerdem 0 für die höchstmöglichste Frequenz (Hardwarebedingt), sowie keine Eingabe für manuellen Generationswechsel.");
            }
        }
        private void PreviewTextInput_Number(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_regex.IsMatch(e.Text);
        }

        private void ShowError(string errorText)
        {
            MessageBox.Show(errorText, "Ungültige Eingabe", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
    }
}