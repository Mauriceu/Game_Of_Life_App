using System;
using Game_Of_Life_App;
using NUnit.Framework;

namespace Game_Of_life_Test
{
    [TestFixture]
    public class BlackBox
    {
        private GameBoard _board;

        private int _height = 4;
        private int _width = 4;

        [SetUp]
        public void Setup()
        {
            _board = new GameBoard(_height, _width);
            _board.FillBoard();
        }

        [Test]
        public void GenerationChange_Test()
        {
            _board.Board[0][0].StatusChange(true); // Should die of solitude
            _board.Board[0][2].StatusChange(true);
            _board.Board[1][2].StatusChange(true); // Should die of overpopulation
            _board.Board[1][3].StatusChange(true);
            _board.Board[2][1].StatusChange(true);
            _board.Board[2][2].StatusChange(true);
            _board.NextGeneration();

            // First Row
            Assert.IsFalse(_board.Board[0][0].IsAlive());
            Assert.IsTrue(_board.Board[0][1].IsAlive());
            Assert.IsTrue(_board.Board[0][2].IsAlive());
            Assert.IsTrue(_board.Board[0][3].IsAlive());
            // Second Row
            Assert.IsFalse(_board.Board[1][0].IsAlive());
            Assert.IsFalse(_board.Board[1][1].IsAlive());
            Assert.IsFalse(_board.Board[1][2].IsAlive());
            Assert.IsTrue(_board.Board[1][3].IsAlive());
            // Third Row
            Assert.IsFalse(_board.Board[2][0].IsAlive());
            Assert.IsTrue(_board.Board[2][1].IsAlive());
            Assert.IsTrue(_board.Board[2][2].IsAlive());
            Assert.IsTrue(_board.Board[2][3].IsAlive());
        }

        [Test]
        public void Neighbours_Test()
        {
            int currentRow = 1;
            foreach (var row in _board.Board)
            {
                int currentCell = 1;
                foreach (var cell in row)
                {
                    var amountNeighbours = cell.GetNeighbours().Count;
                    
                    // First and Last Row
                    if (currentRow == 1 || currentRow == _height)
                    {
                        // First and Last Cell of row
                        if (currentCell == 1 || currentCell == _width)
                        {
                            Assert.AreEqual(3, amountNeighbours);
                        }
                        // Middle-Cells
                        if (currentCell > 1 && currentCell < _width)
                        {
                            Assert.AreEqual(5, amountNeighbours);
                        }
                    }
                    // Middle-Rows
                    if (currentRow > 1 && currentRow < _height)
                    {
                        // First and Last Cell of row
                        if (currentCell == 1 || currentCell == _width)
                        {
                            Assert.AreEqual(5, amountNeighbours);
                        }
                        // Middle-Cells
                        if (currentCell > 1 && currentCell < _width)
                        {
                            Assert.AreEqual(8, amountNeighbours);
                        }
                    }
                    currentCell++;
                }
                currentRow++;
            }
        }
    }
}