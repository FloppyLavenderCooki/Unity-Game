using System.Collections.Generic;
using UnityEngine;

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

        private float _widthOffset;
        private float _heightOffset;

        private void Start()
        {
            _width = size.x;
            _height = size.y;
            
            // GameObject bookshelf = Resources.Load<GameObject>("OldBookshelf");
            Renderer bookshelfRenderer = bookshelfModel.GetComponent<Renderer>();
            _bookshelfSize = bookshelfRenderer.bounds.size;
            
            _widthOffset = _bookshelfSize.x + _bookshelfSize.z;
            _heightOffset = _bookshelfSize.z;
            
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
    }
}
