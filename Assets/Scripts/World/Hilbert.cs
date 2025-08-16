using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public class HilbertCurve
    {
        public enum Direction
        {
            Up,
            Left,
            Down,
            Right
        }

        public List<Vector2> Points { get; }
        private Vector2 _currentPosition;
        private readonly float _stepSize;

        public HilbertCurve(Vector2 startPosition, float stepSize)
        {
            _currentPosition = startPosition;
            _stepSize = stepSize;
            Points = new List<Vector2> { _currentPosition };
        }

        public void GenerateHilbert(int level, Direction direction)
        {
            if (level == 1)
            {
                switch (direction)
                {
                    case Direction.Left:
                        Move(Direction.Right);
                        Move(Direction.Down);
                        Move(Direction.Left);
                        break;
                    case Direction.Right:
                        Move(Direction.Left);
                        Move(Direction.Up);
                        Move(Direction.Right);
                        break;
                    case Direction.Up:
                        Move(Direction.Down);
                        Move(Direction.Right);
                        Move(Direction.Up);
                        break;
                    case Direction.Down:
                        Move(Direction.Up);
                        Move(Direction.Left);
                        Move(Direction.Down);
                        break;
                }
            }
            else
            {
                switch (direction)
                {
                    case Direction.Left:
                        GenerateHilbert(level - 1, Direction.Up);
                        Move(Direction.Right);
                        GenerateHilbert(level - 1, Direction.Left);
                        Move(Direction.Down);
                        GenerateHilbert(level - 1, Direction.Left);
                        Move(Direction.Left);
                        GenerateHilbert(level - 1, Direction.Down);
                        break;
                    case Direction.Right:
                        GenerateHilbert(level - 1, Direction.Down);
                        Move(Direction.Left);
                        GenerateHilbert(level - 1, Direction.Right);
                        Move(Direction.Up);
                        GenerateHilbert(level - 1, Direction.Right);
                        Move(Direction.Right);
                        GenerateHilbert(level - 1, Direction.Up);
                        break;
                    case Direction.Up:
                        GenerateHilbert(level - 1, Direction.Left);
                        Move(Direction.Down);
                        GenerateHilbert(level - 1, Direction.Up);
                        Move(Direction.Right);
                        GenerateHilbert(level - 1, Direction.Up);
                        Move(Direction.Up);
                        GenerateHilbert(level - 1, Direction.Right);
                        break;
                    case Direction.Down:
                        GenerateHilbert(level - 1, Direction.Right);
                        Move(Direction.Up);
                        GenerateHilbert(level - 1, Direction.Down);
                        Move(Direction.Left);
                        GenerateHilbert(level - 1, Direction.Down);
                        Move(Direction.Down);
                        GenerateHilbert(level - 1, Direction.Left);
                        break;
                }
            }
        }

        private void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    _currentPosition += Vector2.up * _stepSize;
                    if (_currentPosition.y < 0) _currentPosition.y = 0;
                    break;
                case Direction.Down:
                    _currentPosition += Vector2.down * _stepSize;
                    if (_currentPosition.y < 0) _currentPosition.y = 0;
                    break;
                case Direction.Left:
                    _currentPosition += Vector2.left * _stepSize;
                    break;
                case Direction.Right:
                    _currentPosition += Vector2.right * _stepSize;
                    break;
            }
            Points.Add(_currentPosition);
        }
    }
}
