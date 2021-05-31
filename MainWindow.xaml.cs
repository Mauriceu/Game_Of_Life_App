using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Game_Of_Life_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly int HEIGHT_MAXIMUM = 500;
        private static readonly int WIDTH_MAXIMUM = 500;
        private static readonly int TIMER_MAXIMUM = 500;

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
            Spielfläche.Children.Clear();
            _board = new GameBoard(_numberRows, _numberColumns);
            _board.FillBoard(Spielfläche);
            
            MAX_LIVING_STARTCELLS = _numberRows * _numberColumns;
            if (_customMaxLivingStartCells > MAX_LIVING_STARTCELLS)
            {
                _customMaxLivingStartCells = _numberRows * _numberColumns;
                InputStartCells.Text = (_numberRows * _numberColumns).ToString();
            }
            ButtonStart.IsEnabled = true;
            InputStartCells.IsEnabled = true;
            ButtonRandomize.IsEnabled = true;
        }
        
        private void ButtonRandomize_OnClick(object sender, RoutedEventArgs e)
        {
            _board.FillBoard(Spielfläche);
            _board.RandomizeLivingCells(_customMaxLivingStartCells);
        }
        
        private void Start_Click(object sender, RoutedEventArgs e)  
        {
            if (_timer > 0)
            {
                DeactivateButtons();
                _board.NextGeneration();
                _timerRef = _board.PlayWithTimer(_timer);
            }
            else
            {
                ButtonCancel.IsEnabled = true;
                _board.NextGeneration();
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
            var box = (TextBox)sender;
            if (!_regex.IsMatch(box.Text))
            {
                InputWidth.BorderBrush = Brushes.Red;
                return;
            }

            int value = Int32.Parse(box.Text);
            if (value <= WIDTH_MAXIMUM)
            {
                InputWidth.BorderBrush = Brushes.Black;
                _numberColumns = value;
            }
            else
            {
                InputWidth.BorderBrush = Brushes.Red;
            }
        }
        private void InputHeight_OnTextInput(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            if (!_regex.IsMatch(box.Text))
            {
                InputHeight.BorderBrush = Brushes.Red;
                return;
            }


            int value = Int32.Parse(box.Text);
            if (value <= HEIGHT_MAXIMUM)
            {
                InputHeight.BorderBrush = Brushes.Black;
                _numberRows = value;
            }
            else
            {
                InputHeight.BorderBrush = Brushes.Red;
            }
        }

        private void InputStartCells_OnTextInput(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox) sender).Text;
            if (!_regex.IsMatch(text))
            {
                InputStartCells.BorderBrush = Brushes.Red;
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
                InputStartCells.BorderBrush = Brushes.Red;
            }
        }

        private void InputTimer_OnTextInput(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;
            
            if (text == "")
                _timer = 0;
            

            if (!_regex.IsMatch(text))
            {
                InputTimer.BorderBrush = Brushes.Red;
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
            }
        }
        private void PreviewTextInput_Number(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_regex.IsMatch(e.Text);
        }
        
    }
}