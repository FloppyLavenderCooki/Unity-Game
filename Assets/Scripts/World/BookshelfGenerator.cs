using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace World
{
    public class BookshelfGenerator : MonoBehaviour
    {
        public GameObject hardbackBook;
        public GameObject paperbackBook;
        public GameObject thinStapledBook;
        public Material bookMaterial;
        
        private Renderer _hardbackBookRenderer;
        private Renderer _paperbackBookRenderer;
        private Renderer _thinStapledBookRenderer;
        
        private Vector3 _bookSize;
        private GameObject _books;

        public float xOffset;
        public float yOffset = 0.725f;
        public float zOffset = 0.12f;
        
        public void Awake()
        {
            _hardbackBookRenderer = hardbackBook.GetComponentInChildren<Renderer>();
            _paperbackBookRenderer = paperbackBook.GetComponentInChildren<Renderer>();
            _thinStapledBookRenderer = thinStapledBook.GetComponentInChildren<Renderer>();
            _books = GameObject.Find("Books");
        }
        
        public void GenerateBookshelf(GameObject bookshelf, Vector3 bookshelfSize, bool oneSided)
        {
            xOffset = 0;
            while (xOffset < bookshelfSize.z*0.65f)
            {
                GenerateBook(bookshelf, bookshelfSize, oneSided);
            }
        }

        private void GenerateBook(GameObject bookshelf, Vector3 bookshelfSize, bool oneSided)
        {
            int bookType = Random.Range(0, 3);
            GameObject book = bookType switch
            {
                0 => Instantiate(hardbackBook, bookshelf.transform),
                1 => Instantiate(paperbackBook, bookshelf.transform),
                2 => Instantiate(thinStapledBook, bookshelf.transform),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _bookSize = bookType switch
            {
                0 => _hardbackBookRenderer.bounds.size,
                1 => _paperbackBookRenderer.bounds.size,
                2 => _thinStapledBookRenderer.bounds.size,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            switch(bookType)
            {
                case 0:
                    _hardbackBookRenderer.material = new Material(_hardbackBookRenderer.sharedMaterial)
                        { color = Random.ColorHSV() };
                    break;
                case 1:
                    _paperbackBookRenderer.material = new Material(_paperbackBookRenderer.sharedMaterial)
                        { color = Random.ColorHSV() };
                    break;
                case 2:
                    _thinStapledBookRenderer.material = new Material(_thinStapledBookRenderer.sharedMaterial)
                        { color = Random.ColorHSV() };
                    break;
            };

            book.transform.localRotation = Quaternion.Euler(
                bookshelf.transform.rotation.x,
                bookshelf.transform.rotation.y - 90.0f,
                bookshelf.transform.rotation.z - 90.0f
            );

            if (bookType == 0)
            {
                book.transform.position = new Vector3(
                    bookshelf.transform.position.x,
                    bookshelf.transform.position.y + (bookshelfSize.y - ((_bookSize.y+0.018f) * yOffset)),
                    bookshelf.transform.position.z
                );
            }
            else
            {
                book.transform.position = new Vector3(
                    bookshelf.transform.position.x,
                    bookshelf.transform.position.y + (bookshelfSize.y - (_bookSize.y * yOffset)),
                    bookshelf.transform.position.z
                );
            }

            book.transform.position += bookshelf.transform.up * (bookshelfSize.x - xOffset);
            book.transform.position += bookshelf.transform.right * zOffset;
            
            book.transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
            
            book.transform.SetParent(_books.transform);

            xOffset += _bookSize.x/2 + 0.05f;
        }
    }
}