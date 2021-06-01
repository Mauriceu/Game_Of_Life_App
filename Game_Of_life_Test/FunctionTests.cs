using Game_Of_Life_App;
using NUnit.Framework;

namespace Game_Of_life_Test
{
    public class GameBoardTest
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
        public void FillBoard_Test()
        {
            Assert.IsTrue(_board.Board.Count == 4);
            Assert.IsTrue(_board.Board[0].Count == 4);
            Assert.IsTrue(_board.Board[1].Count == 4);
            Assert.IsTrue(_board.Board[2].Count == 4);
            Assert.IsTrue(_board.Board[3].Count == 4);
            Assert.True(_board.Board[0][0].GetType() == typeof(Cell));
        }

        [Test]
        public void CheckNeighbours()
        {
            for (int posY = 0; posY < _height; posY++)
            {
                for (int posX = 0; posX < _width; posX++)
                {
                    // first row
                    if (posY == 0 && posX == 0)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 3);
                    if (posY == 0 && posX > 0 && posX < 3)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 5);
                    if (posY == 0 && posX == 3)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 3);
                    
                    // second row
                    if (posY == 1 && posX == 0)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 5);
                    if (posY == 1 && posX > 0 && posX < 3)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 8);
                    if (posY == 1 && posX == 3)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 5);
                    
                    // third row
                    if (posY == 2 && posX == 0)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 5);
                    if (posY == 2 && posX > 0 && posX < 3)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 8);
                    if (posY == 2 && posX == 3)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 5);
                    
                    // fourth row
                    if (posY == 3 && posX == 0)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 3);
                    if (posY == 3 && posX > 0 && posX < 3)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 5);
                    if (posY == 3 && posX == 3)
                        Assert.IsTrue(_board.Board[posY][posX].GetNeighbours().Count == 3);

                }
            }
        }

        [Test]
        public void RandomizeLivingCells_Test()
        {
            GameBoard board = new GameBoard(_height, _width);
            board.FillBoard();
            board.RandomizeLivingCells(4);

            int livingCells = 0;
            foreach (var row in board.Board)
            {
                foreach (Cell cell in row)
                {
                    if (cell.IsAlive())
                        livingCells++;
                }
            }
            Assert.IsTrue(livingCells == 4);
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

    }

    public class CellTest
    {
        [Test]
        public void StatusChange_Test()
        {
            Cell cell = new Cell("00");
            Assert.IsFalse(cell.IsAlive());
            cell.StatusChange(true);
            Assert.IsTrue(cell.IsAlive());
        }

        [Test]
        public void Neighbour_Test()
        {
            Cell cell = new Cell("00");
            Cell neighbour = new Cell("01");
            
            cell.AddNeighbour(neighbour);
            Assert.IsTrue(cell.HasNeighbour(neighbour));
            Assert.IsTrue(cell.GetNeighbours().Count == 1);
        }

        [Test]
        public void GenerationChange_Test()
        {
            Cell cell = new Cell("00");
            Cell neighbour1 = new Cell("01");
            neighbour1.StatusChange(true);
            Cell neighbour2 = new Cell("10");
            neighbour2.StatusChange(true);
            Cell neighbour3 = new Cell("11");
            neighbour3.StatusChange(true);
            
            cell.AddNeighbour(neighbour1);
            cell.AddNeighbour(neighbour2);
            cell.AddNeighbour(neighbour3);
            
            cell.CheckIfCellLivesAfterGenerationChange();
            
            Assert.IsFalse(cell.WillBeDead());
            cell.EvolveCell();
            Assert.IsTrue(cell.IsAlive());
        }
    }
}