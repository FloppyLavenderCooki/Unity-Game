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
            zOffset = 0.12f;
            yOffset = 0.725f;
            for (int i = 0; i < 4; i++)
            {
                xOffset = 0;
                while (xOffset < bookshelfSize.z*0.675f)
                {
                    GenerateBook(bookshelf, bookshelfSize);
                }
                yOffset += bookshelfSize.y/2;
            }

            if (oneSided) return;
            zOffset = -0.12f;
            yOffset = 0.725f;
            for (int i = 0; i < 4; i++)
            {
                xOffset = 0;
                while (xOffset < bookshelfSize.z * 0.675f)
                {
                    GenerateBook(bookshelf, bookshelfSize, true);
                }

                yOffset += bookshelfSize.y / 2;
            }
        }

        private void GenerateBook(GameObject bookshelf, Vector3 bookshelfSize, bool flip = false)
        {
            int bookType = Random.Range(0, 3);
            GameObject book = bookType switch
            {
                0 => Instantiate(hardbackBook, bookshelf.transform),
                1 => Instantiate(paperbackBook, bookshelf.transform),
                2 => Instantiate(thinStapledBook, bookshelf.transform),
                _ => throw new ArgumentOutOfRangeException()
            };
            Renderer bookRenderer = book.GetComponentInChildren<Renderer>();
            
            _bookSize = bookType switch
            {
                0 => _hardbackBookRenderer.bounds.size,
                1 => _paperbackBookRenderer.bounds.size,
                2 => _thinStapledBookRenderer.bounds.size,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            bookRenderer.material = new Material(bookRenderer.sharedMaterial)
                { color = Random.ColorHSV() };

            if (flip)
            {
                book.transform.localRotation = Quaternion.Euler(
                    bookshelf.transform.rotation.x,
                    bookshelf.transform.rotation.y + 90.0f,
                    bookshelf.transform.rotation.z - 90.0f
                );
            }
            else
            {
                book.transform.localRotation = Quaternion.Euler(
                    bookshelf.transform.rotation.x,
                    bookshelf.transform.rotation.y - 90.0f,
                    bookshelf.transform.rotation.z - 90.0f
                );
            }

            book.transform.position = new Vector3(
                bookshelf.transform.position.x,
                bookshelf.transform.position.y + (bookshelfSize.y - (_bookSize.y * yOffset)),
                bookshelf.transform.position.z
            );
            
            if (bookType == 0)
            {
                book.transform.position -= new Vector3(0, 0.01305f, 0);
            }

            book.transform.position += bookshelf.transform.up * (bookshelfSize.x - xOffset);
            book.transform.position += bookshelf.transform.right * zOffset;
            
            book.transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
            
            book.transform.SetParent(_books.transform);

            xOffset += _bookSize.x/2 + 0.05f;
        }
    }
}