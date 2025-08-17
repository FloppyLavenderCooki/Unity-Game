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

        public int level = 3;
        public int stepSize = 1;
        public Vector2Int startPosition = Vector2Int.zero;
        private HilbertCurve _hilbert;
        private BookshelfGenerator _bookshelfGenerator;
        private GameObject _books;

        private void Start()
        {
            _books = GameObject.Find("Books");
            
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
                if (p.y < 0) p.y = -p.y;
                _hilbert.Points[i] = p;
            }

            GenerateEmptyArray();
            GenerateMaze(startPosition);
            
            Destroy(GameObject.Find("Loading UI"));
        }

        private void Update()
        {
            if (!Keyboard.current.rKey.wasPressedThisFrame) return;
            
            foreach (Transform child in _books.transform) { Destroy(child.gameObject); }
            foreach (Transform child in transform) { Destroy(child.gameObject); }
        }

        private void GenerateEmptyArray()
        {
            int maxX = 0;
            int maxY = 0;
            foreach (var p in _hilbert.Points)
            {
                if (p.x > maxX) maxX = p.x;
                if (p.y > maxY) maxY = p.y;
            }
            
            _rows = (maxY + 1) * 2;
            _cols = (maxX + 1) * 2;
            
            startPosition = new Vector2Int(maxY, maxX);
            Debug.Log(startPosition);
            
            _array = new Dictionary<string, bool>[_rows, _cols];
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _cols; x++)
                {
                    _array[y, x] = new Dictionary<string, bool>
                    {
                        { "pillar", false }
                    };
                }
            }
        }
        
        private void GenerateMaze(Vector2Int position)
        {
            if (position == Vector2Int.zero)
            {
                CreateMazeObjects();
                return;
            }
            
            _array[(position.x) * 2, (position.y) * 2]["pillar"] = _hilbert.Points.Contains(position);

            Vector2Int[] directions = { Vector2Int.up,Vector2Int.down, Vector2Int.left, Vector2Int.right };
            Vector2Int newPosition = position + directions[Random.Range(0, directions.Length)];
            
            int positionHilbert = _hilbert.Points.FindIndex(p => p == position);
            int newPositionHilbert = _hilbert.Points.FindIndex(p => p == newPosition);
            
            if (_hilbert.Points.Contains(newPosition) && positionHilbert > newPositionHilbert)
            {
                GenerateMaze(newPosition);
            }
            else
            {
                GenerateMaze(position);
            }
        }

        private void CreateMazeObjects()
        {
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _cols; x++)
                {
                    if (_array[y, x]["pillar"])
                    {
                        GameObject pillarInstance = Instantiate(pillarModel, transform);
                        pillarInstance.transform.position += new Vector3(x, 0, -y);
                    }
                }
            }
        }
    }
}
