using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace World
{
    public class MazeGenerator : MonoBehaviour
    {
        public GameObject bookshelfModel;
        
        private Vector3 _bookshelfSize;
        private List<List<Dictionary<string, bool>>> _bookshelves = new();

        public int level = 3;
        public float stepSize = 1f;
        public Vector2 startPosition = Vector2.zero;
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
            
            string path = Application.persistentDataPath + "/points.txt";

            List<string> lines = new List<string>();
            foreach (Vector2 v in _hilbert.Points)
            {
                lines.Add(v.x + "," + v.y);
            }
            File.WriteAllLines(path, lines);

            Debug.Log("List<Vector2> saved to: " + path);
            
            Destroy(GameObject.Find("Loading UI"));
        }

        private void Update()
        {
            if (!Keyboard.current.rKey.wasPressedThisFrame) return;
            
            foreach (Transform child in _books.transform) { Destroy(child.gameObject); }
            foreach (Transform child in transform) { Destroy(child.gameObject); }
        }
    }
}
