using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace World
{
    public class MazeGenerator : MonoBehaviour
    {
        public GameObject bookshelfModel;
        
        [SerializeField] private Vector2Int size = new(8, 8);
        [SerializeField] private Vector3 offset = new(0, 0, 0);
        
        private Vector3 _bookshelfSize;
        private int _width;
        private int _height;
        private List<List<Dictionary<string, bool>>> _bookshelves = new();
        // private List<List<Dictionary<string, bool>>> _hilbertCurve = new();

        private float _widthOffset;
        // private float _heightOffset;
        
        public int level = 3;
        public float stepSize = 1f;
        public Vector2 startPosition = Vector2.zero;
        private HilbertCurve _hilbert;

        private void Start()
        {
            _width = size.x;
            _height = size.y;
            
            // GameObject bookshelf = Resources.Load<GameObject>("OldBookshelf");
            Renderer bookshelfRenderer = bookshelfModel.GetComponent<Renderer>();
            _bookshelfSize = bookshelfRenderer.bounds.size;
            
            _widthOffset = _bookshelfSize.x + _bookshelfSize.z;
            // _heightOffset = _bookshelfSize.z;
            
            GenerateEmptyBookshelves();
            GenerateHilbertMaze();
            PlaceBookshelves();
        }

        private void Update()
        {
            if (!Keyboard.current.rKey.wasPressedThisFrame) return;
            
            Debug.Log("Regenerating maze...");
            foreach (Transform child in transform) { Destroy(child.gameObject); }
            GenerateEmptyBookshelves();
            GenerateHilbertMaze();
            PlaceBookshelves();
        }

        private void GenerateEmptyBookshelves()
        {
            _bookshelves = new List<List<Dictionary<string, bool>>>();
            for (int y = 0; y < _height; y++)
            {
                _bookshelves.Add(new List<Dictionary<string, bool>>());
                for (int x = 0; x < _width; x++)
                {
                    _bookshelves[y].Add(new Dictionary<string, bool>{
                        { "up", true },
                        { "down", true },
                        { "left", true },
                        { "right", true }
                    });
                }
            }
        }
        
        private void PlaceBookshelf(GameObject bookshelf, float x, float y, int angle = 0, GameObject bookshelfGroup = null)
        {
            if (!bookshelf) return;

            Vector3 position = new Vector3(x * 1.95f, 0, y * 1.95f);
            Vector3 rotation = bookshelf.transform.rotation.eulerAngles;
            rotation.y += angle;

            GameObject bookshelfInstance = Instantiate(bookshelf, position, Quaternion.Euler(rotation), transform);

            if (bookshelfGroup)
            {
                bookshelfInstance.transform.SetParent(bookshelfGroup.transform);
            }
            else
            {
                bookshelfInstance.transform.SetParent(transform);
            }
        }

        private void GenerateHilbertMaze()
        {
            _hilbert = new HilbertCurve(startPosition, stepSize);
            _hilbert.GenerateHilbert(level, HilbertCurve.Direction.Up);

            Dictionary<Vector2Int, int> pointIndices = new Dictionary<Vector2Int, int>();
            for (int i = 0; i < _hilbert.Points.Count; i++)
            {
                Vector2Int point = Vector2Int.RoundToInt(_hilbert.Points[i]);
                point.x = Mathf.Clamp(point.x, 0, _width - 1);
                point.y = Mathf.Clamp(point.y, 0, _height - 1);

                pointIndices.TryAdd(point, i);
            }

            List<Vector2Int> directions = new List<Vector2Int>
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            for (int i = 0; i < _hilbert.Points.Count; i++)
            {
                Vector2Int p1 = Vector2Int.RoundToInt(_hilbert.Points[i]);
                p1.x = Mathf.Clamp(p1.x, 0, _width - 1);
                p1.y = Mathf.Clamp(p1.y, 0, _height - 1);

                List<Vector2Int> validNeighbors = new List<Vector2Int>();

                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighbor = p1 + dir;

                    if (IsOutOfBounds(neighbor))
                        continue;

                    if (pointIndices.ContainsKey(neighbor) && pointIndices[neighbor] > i)
                    {
                        validNeighbors.Add(neighbor);
                    }
                }

                if (validNeighbors.Count <= 0) continue;
                {
                    int randomIndex = Random.Range(0, validNeighbors.Count);
                    Vector2Int p2 = validNeighbors[randomIndex];

                    Vector2Int dir = p2 - p1;

                    if (dir == Vector2Int.up)
                    {
                        _bookshelves[p1.y][p1.x]["up"] = false;
                        _bookshelves[p2.y][p2.x]["down"] = false;
                    }
                    else if (dir == Vector2Int.down)
                    {
                        _bookshelves[p1.y][p1.x]["down"] = false;
                        _bookshelves[p2.y][p2.x]["up"] = false;
                    }
                    else if (dir == Vector2Int.left)
                    {
                        _bookshelves[p1.y][p1.x]["left"] = false;
                        _bookshelves[p2.y][p2.x]["right"] = false;
                    }
                    else if (dir == Vector2Int.right)
                    {
                        _bookshelves[p1.y][p1.x]["right"] = false;
                        _bookshelves[p2.y][p2.x]["left"] = false;
                    }
                }
            }
        }

        private bool IsOutOfBounds(Vector2Int p)
        {
            return p.x < 0 || p.x >= _width || p.y < 0 || p.y >= _height;
        }

        private void PlaceBookshelves()
        {
            _bookshelves[0][(_width-1)/2]["down"] = false;
            _bookshelves[0][((_width-1)/2)+1]["down"] = false;
            
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    GameObject bookshelfGroup = new GameObject("BookshelfGroup_" + x + "_" + y);
                    
                    if (_bookshelves[y][x]["up"])
                    {
                        PlaceBookshelf(bookshelfModel, 0, 0 + _bookshelfSize.x, 90, bookshelfGroup);
                    }

                    if (_bookshelves[y][x]["down"])
                    {
                        PlaceBookshelf(bookshelfModel, 0, 0 - _bookshelfSize.x, -90, bookshelfGroup);
                    }

                    if (_bookshelves[y][x]["left"])
                    {
                        PlaceBookshelf(bookshelfModel, 0 - _bookshelfSize.x, 0, 0, bookshelfGroup);
                    }

                    if (_bookshelves[y][x]["right"])
                    {
                        PlaceBookshelf(bookshelfModel, 0 + _bookshelfSize.x, 0, 180, bookshelfGroup);
                    }
                    
                    if (bookshelfGroup.transform.childCount > 0)
                    {
                        bookshelfGroup.transform.SetParent(transform);
                        bookshelfGroup.transform.localPosition = new Vector3(x * _widthOffset, 0, y * _widthOffset);
                    }
                    else
                    {
                        Destroy(bookshelfGroup);
                    }
                }
            }
            
            transform.position = offset + new Vector3((_width - 0.75f) * -_widthOffset / 2, 0, _height * -_widthOffset / 2);
        }
    }
}
