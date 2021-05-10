using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Game_Of_Life_App
{
    public class Camera
    {
        private readonly Grid _spielfläche;
        private readonly int _height;
        private readonly int _width;
        private readonly List<List<Cell>> _board;
        
        public Camera(int height, int width, List<List<Cell>> board, Grid spielfläche)
        {
            _spielfläche = spielfläche;
            _height = height;
            _width = width;
            _board = board;
        }
        
        // Re-Renders the grid according to a given offset
        public void UpdateGrid(int offsetY = 0, int offsetX = 0)
        {
            // camera moved ->
            //    vertical
            if (offsetY != 0 && offsetX == 0)
            {
                CameraMove_Vertical(offsetY);
            }

            // camera moved ->
            //    horizontal
            if (offsetY == 0 && offsetX != 0)
            {
                CameraMove_Horizontal(offsetX);
            }
            
            // camera moved ->
            //    top           right
            if (offsetY < 0 && offsetX > 0)
            {
                CameraMove_TopRight(offsetY, offsetX);
            }
            
            // camera moved ->
            //    top           left
            if (offsetY < 0 && offsetX < 0)
            {
                CameraMove_TopLeft(offsetY, offsetX);
            }
            
            // camera moved ->
            //   Bottom         right
            if (offsetY > 0 && offsetX > 0)
            {
                CameraMove_BottomRight(offsetY, offsetX);
            }

            // camera moved ->
            //   bottom          left
            if (offsetY > 0 && offsetX < 0)
            {
                CameraMove_BottomLeft(offsetY, offsetX);
            }

        }

        private void CameraMove_Vertical(int offsetY)
        {
            if (offsetY > 0)
            {
                int inViewY = 0;
                for (int i = _height - offsetY; i < _height; i++)
                {
                    List<Cell> outOfViewRow = _board[i];
                    List<Cell> inViewRow = _board[inViewY];

                    _board[i] = inViewRow;
                    _board[inViewY] = outOfViewRow;
                    
                    for (int ii = 0; ii < _width; ii++)
                    {
                        inViewRow[ii].RenderRectangle(i, ii, _height, _width, _spielfläche);
                        outOfViewRow[ii].RenderRectangle(inViewY, ii, _height, _width, _spielfläche);
                    }
                    inViewY++;
                }
            }
            else
            {
                int inViewY = Math.Abs(offsetY);
                for (int i = Math.Abs(offsetY); i < _height; i++)
                {
                    for (int ii = 0; ii < _width; ii++)
                    {
                        Cell outOfViewCell = _board[i][ii]; // S1
                        Cell inViewCell = _board[inViewY][ii]; // S2
    
                        _board[i][ii] = inViewCell;
                        _board[inViewY][ii] = outOfViewCell;
                        inViewCell.RenderRectangle(i, ii, _height, _width, _spielfläche);
                        outOfViewCell.RenderRectangle(inViewY, ii, _height, _width, _spielfläche);
                    }
                    inViewY++;
                }
            }

        }
        
        private void CameraMove_Horizontal(int offsetX)
        {
            if (offsetX > 0)
            {
                for (int i = 0; i < _height; i++)
                {
                    int inViewX = offsetX;
                    for (int ii = offsetX; ii < _width; ii++)
                    {
                        Cell outOfViewCell = _board[i][ii]; // S1
                        Cell inViewCell = _board[i][inViewX]; // S2
    
                        _board[i][ii] = inViewCell;
                        _board[i][inViewX] = outOfViewCell;
                        inViewCell.RenderRectangle(i, ii, _height, _width, _spielfläche);
                        outOfViewCell.RenderRectangle(i, inViewX, _height, _width, _spielfläche);
                        inViewX++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _height; i++)
                {
                    int inViewX = 0;
                    for (int ii = 0; ii < Math.Abs(offsetX); ii++)
                    {
                        Cell outOfViewCell = _board[i][ii]; // S1
                        Cell inViewCell = _board[i][inViewX]; // S2

                        _board[i][ii] = inViewCell;
                        _board[i][inViewX] = outOfViewCell;
                        inViewCell.RenderRectangle(i, ii, _height, _width, _spielfläche);
                        outOfViewCell.RenderRectangle(i, inViewX, _height, _width, _spielfläche);
                        inViewX++;
                    }
                }
            }
        }
        
        private void CameraMove_TopRight(int offsetY, int offsetX)
        {
            int inViewY = 0;
            for (int i = 0; i < Math.Abs(offsetY); i++)
            {
                int inViewX = offsetX;
                for (int ii = offsetX; ii < _width; ii++)
                {
                    Cell outOfViewCell = _board[i][ii]; // S1
                    Cell inViewCell = _board[inViewY][inViewX]; // S2

                    _board[i][ii] = inViewCell;
                    _board[inViewY][inViewX] = outOfViewCell;
                    inViewX++;
                    inViewCell.RenderRectangle(i, ii, _height, _width, _spielfläche);
                    outOfViewCell.RenderRectangle(inViewY, inViewX, _height, _width, _spielfläche);
                }
                inViewY++;
            }
        }
        
        private void CameraMove_TopLeft(int offsetY, int offsetX)
        {
            int inViewY = 0;
            for (int i = 0; i < _height - Math.Abs(offsetY); i++)
            {
                int inViewX = 0;
                for (int ii = 0; ii < _width - Math.Abs(offsetX); ii++)
                {
                    Cell outOfViewCell = _board[i][ii]; // S1
                    Cell inViewCell = _board[inViewY][inViewX]; // S2

                    _board[i][ii] = inViewCell;
                    _board[inViewY][inViewX] = outOfViewCell;
                    inViewX++;
                    inViewCell.RenderRectangle(i, ii, _height, _width, _spielfläche);
                    outOfViewCell.RenderRectangle(inViewY, inViewX, _height, _width, _spielfläche);
                }
                inViewY++;
            }
        }
        
        private void CameraMove_BottomRight(int offsetY, int offsetX)
        {
            int inViewY = offsetY;
            for (int i = offsetY; i < _height; i++)
            {
                int inViewX = offsetX;
                for (int ii = offsetX; ii < _width; ii++)
                {
                    Cell outOfViewCell = _board[i][ii]; // S1
                    Cell inViewCell = _board[inViewY][inViewX]; // S2

                    _board[i][ii] = inViewCell;
                    _board[inViewY][inViewX] = outOfViewCell;
                    inViewX++;
                    inViewCell.RenderRectangle(i, ii, _height, _width, _spielfläche);
                    outOfViewCell.RenderRectangle(inViewY, inViewX, _height, _width, _spielfläche);
                }
                inViewY++;
            }
        }
        
        private void CameraMove_BottomLeft(int offsetY, int offsetX)
        {
            int inViewY = offsetY;
            for (int i = 0; i < _height - offsetY; i++)
            {
                int inViewX = 0;
                for (int ii = Math.Abs(offsetX); ii < _width; ii++)
                {
                    Cell outOfViewCell = _board[i][ii]; // S1
                    Cell inViewCell = _board[inViewY][inViewX]; // S2

                    _board[i][ii] = inViewCell;
                    _board[inViewY][inViewX] = outOfViewCell;
                    inViewX++;
                    inViewCell.RenderRectangle(i, ii, _height, _width, _spielfläche);
                    outOfViewCell.RenderRectangle(inViewY, inViewX, _height, _width, _spielfläche);
                }
                inViewY++;
            }
            
        }
        
    }
}