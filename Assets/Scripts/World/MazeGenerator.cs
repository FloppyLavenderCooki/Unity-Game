using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace World
{
    public class MazeGenerator : MonoBehaviour
    {
        public GameObject bookshelfModel;
        public GameObject pillarModel;
        
        private Vector3 _bookshelfSize;
        private Dictionary<string,bool>[,] _array;
        private int _rows;
        private int _cols;
        private int _maxX;
        private int _maxY;

        public int level = 3;
        public int stepSize = 1;
        public Vector2Int startPosition = Vector2Int.zero;
        private HilbertCurve _hilbert;
        private BookshelfGenerator _bookshelfGenerator;
        // private GameObject _books;
        
        private readonly Dictionary<Vector2Int, string> _bookshelfMap = new()
        {
            { Vector2Int.up, "bookshelf_up" },
            { Vector2Int.down, "bookshelf_down" },
            { Vector2Int.left, "bookshelf_left" },
            { Vector2Int.right, "bookshelf_right" }
        };

        private void Start()
        {
            // _books = GameObject.Find("Books");
            
            _bookshelfGenerator = gameObject.GetComponent<BookshelfGenerator>();
            
            Renderer bookshelfRenderer = bookshelfModel.GetComponent<Renderer>();
            _bookshelfSize = bookshelfRenderer.bounds.size;
            
            _hilbert = new HilbertCurve(startPosition, stepSize);
            _hilbert.GenerateHilbert(level, HilbertCurve.Direction.Down);
            
            // Make all points positive (HilbertCurve.Direction.Up doesn't work)
            for (int i = 0; i < _hilbert.Points.Count; i++)
            {
                var p = _hilbert.Points[i];
                if (p.x < 0) p.x = -p.x;
                // if (p.y < 0) p.y = -p.y;
                p.y = (level * 2 + 1) - p.y;
                _hilbert.Points[i] = p;
            }
            _hilbert.Points.Reverse();

            GenerateEmptyArray();
            GenerateHilbertShelves();
            GenerateMaze();
            
            Destroy(GameObject.Find("Loading UI"));
        }

        // private void Update()
        // {
        //     if (!Keyboard.current.rKey.wasPressedThisFrame) return;
        //     
        //     foreach (Transform child in _books.transform) { Destroy(child.gameObject); }
        //     foreach (Transform child in transform) { Destroy(child.gameObject); }
        //     
        //     GenerateEmptyArray();
        //     GenerateHilbertShelves();
        //     GenerateMaze();
        // }

        private void GenerateEmptyArray()
        {
            _maxX = 0;
            _maxY = 0;
            foreach (var p in _hilbert.Points)
            {
                if (p.x > _maxX) _maxX = p.x;
                if (p.y > _maxY) _maxY = p.y;
            }
            
            _rows = (_maxY + 2) * 2;
            _cols = (_maxX + 2) * 2;
            
            // startPosition = new Vector2Int(_maxY, _maxX);
            
            _array = new Dictionary<string, bool>[_rows+1, _cols+1];
            for (int y = 0; y < _rows+1; y++)
            {
                for (int x = 0; x < _cols+1; x++)
                {
                    _array[y, x] = new Dictionary<string, bool>
                    {
                        { "pillar", false },
                        { "bookshelf_up", false },
                        { "bookshelf_down", false },
                        { "bookshelf_left", false },
                        { "bookshelf_right", false }
                    };
                }
            }
        }

        private void GenerateHilbertShelves()
        {
            for (int y = 0; y < _rows; y += 2)
            {
                for (int x = 0; x < _cols; x += 2)
                {
                    if (!_array[y, x].ContainsValue(true))
                    {
                        _array[y, x]["pillar"] = true;
                    }
                }
            }

            // Generate grid
            foreach (var point in _hilbert.Points)
            {
                int x = point.x * 2+1;
                int y = point.y * 2+1;

                if (_array[y + 1, x] == null || _array[y - 1, x] == null || _array[y, x - 1] == null ||
                    _array[y, x + 1] == null) continue;
                
                _array[y - 1, x]["bookshelf_up"] = true;
                _array[y, x - 1]["bookshelf_left"] = true;
                
                if (x == _maxX * 2+1) _array[y, x + 1]["bookshelf_right"] = true;
                if (y == _maxY * 2+1) _array[y + 1, x]["bookshelf_down"] = true;
            }
        }
        
        private void GenerateMaze()
        {
            foreach (var point in _hilbert.Points)
            {
                int x = point.x * 2 + 1;
                int y = point.y * 2 + 1;

                int i = _hilbert.Points.FindIndex(v => v == point);
                int i2 = -1;
                Vector2Int direction = Vector2Int.zero;

                while (i2 == -1)
                {
                    List<Vector2Int> directions =
                        new List<Vector2Int>() { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                    direction = directions[Random.Range(0, directions.Count)];

                    i2 = _hilbert.Points.FindIndex(v => v == point + direction);
                    if (i2 != _hilbert.Points.Count-4 && i2 <= i) i2 = -1;
                }

                if (_bookshelfMap.TryGetValue(direction, out string bookshelfKey))
                {
                    switch (bookshelfKey)
                    {
                        case "bookshelf_up":
                            _array[y + 1, x]["bookshelf_up"] = false;
                            break;
                        case "bookshelf_down":
                            _array[y - 1, x]["bookshelf_up"] = false;
                            break;
                        case "bookshelf_left":
                            _array[y, x - 1]["bookshelf_left"] = false;
                            break;
                        case "bookshelf_right":
                            _array[y, x + 1]["bookshelf_left"] = false;
                            break;
                    }
                }
            }

            CreateMazeObjects();
        }

        private void CreateMazeObjects()
        {
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _cols; x++)
                {
                    if (_array[y, x]["pillar"])
                    {
                        if ((y == _rows - 2 && x == _cols / 2 - 1)) continue;
                        
                        GameObject pillarInstance = Instantiate(pillarModel, transform);
                        pillarInstance.name = $"P ({x}, {y})";
                        pillarInstance.transform.position += new Vector3(x, 0, -y);
                    }
                    
                    foreach (var dir in _bookshelfMap)
                    {
                        if (!_array[y, x][dir.Value] || 
                            (y == _rows - 2 && x == _cols / 2 - 2) || 
                            (y == _rows - 2 && x == _cols / 2)
                        ) continue;
                        
                        GameObject bookshelfInstance = Instantiate(bookshelfModel, transform);
                        bookshelfInstance.name = $"B{dir.Value.Replace("bookshelf","")} ({x}, {y})";
                        bookshelfInstance.transform.position += new Vector3(x, 0, -y);

                        if (dir.Key == Vector2Int.up)
                            bookshelfInstance.transform.rotation = Quaternion.Euler(270, 90, 0);
                        else if (dir.Key == Vector2Int.down)
                            bookshelfInstance.transform.rotation = Quaternion.Euler(270, 270, 0);
                        else if (dir.Key == Vector2Int.left)
                            bookshelfInstance.transform.rotation = Quaternion.Euler(270, 0, 0);
                        else if (dir.Key == Vector2Int.right)
                            bookshelfInstance.transform.rotation = Quaternion.Euler(270, 180, 0);

                        bool outerShelves = (y == 0);
                        _bookshelfGenerator.GenerateBookshelf(bookshelfInstance, _bookshelfSize, outerShelves);
                    }
                }
            }
        }
    }
}
