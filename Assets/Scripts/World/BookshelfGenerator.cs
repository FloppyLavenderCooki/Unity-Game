using System.Collections.Generic;
using Books;
using Player;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace World
{
    public class BookshelfGenerator : MonoBehaviour
    {
        public GameObject[] bookPrefabs;
        private Renderer[] _bookPrefabRenderers;
        
        private EntityManager _entityManager;
        private SaveSystem _saveSystem;
        private readonly List<BookAttributes> _bookData = new();
        
        private Vector3 _bookSize;
        private GameObject _books;

        public float xOffset;
        public float yOffset = 0.725f;
        public float zOffset = 0.12f;

        private BookList _bookJson;
        
        public void Awake()
        {
            _bookPrefabRenderers = new Renderer[bookPrefabs.Length];
            for (int i = 0; i < bookPrefabs.Length; i++)
            {
                Debug.Log(bookPrefabs[i].GetComponentInChildren<Renderer>());
                _bookPrefabRenderers[i] = bookPrefabs[i].GetComponentInChildren<Renderer>();
            }
            _books = GameObject.Find("Books");
            _saveSystem = GameObject.Find("Save System").GetComponent<SaveSystem>();
            
            string jsonData = Resources.Load<TextAsset>("names").text;
            _bookJson = JsonUtility.FromJson<BookList>(jsonData);
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
            
            _saveSystem.SaveBooks(_bookData);
        }

        private void GenerateBook(GameObject bookshelf, Vector3 bookshelfSize, bool flip = false)
        {
            int bookType = Random.Range(0, bookPrefabs.Length);
            // GameObject book = Instantiate(bookPrefabs[bookType], _books.transform, true);
            BookAttributes book = new BookAttributes();
            book.position = _books.transform.position + bookPrefabs[bookType].transform.position;

            // Book bookName = _bookJson.books[Random.Range(0, _bookJson.books.Length)];
            // book.name = $"{bookName.name} - {bookName.author}";
            
            _bookSize = _bookPrefabRenderers[bookType].bounds.size;
            
            // Renderer[] bookRenderers = bookPrefabs[bookType].GetComponentsInChildren<Renderer>();
            Color bookColor = Random.ColorHSV();

            // foreach (var bookRenderer in bookRenderers)
            // {
            //     Material[] mats = bookRenderer.materials;
            //
            //     foreach (var mat in mats)
            //     {
            //         if (mat.name.StartsWith("Orange"))
            //         {
            //             mat.color = bookColor;
            //         }
            //     }
            // }

            if (Random.Range(0, 1) > 0.95) flip = !flip;
            if (flip)
            {
                book.rotation = Quaternion.Euler(
                    bookshelf.transform.eulerAngles.x - 90.0f,
                    bookshelf.transform.eulerAngles.y - 90.0f,
                    bookshelf.transform.eulerAngles.z
                );
            }
            else
            {
                book.rotation = Quaternion.Euler(
                    bookshelf.transform.eulerAngles.x - 90.0f,
                    bookshelf.transform.eulerAngles.y + 90.0f,
                    bookshelf.transform.eulerAngles.z
                );
            }

            book.position = new Vector3(
                bookshelf.transform.position.x,
                bookshelf.transform.position.y + (bookshelfSize.y - (_bookSize.y * yOffset)),
                bookshelf.transform.position.z
            );
            
            switch (bookType)
            {
                case 0:
                    book.position -= new Vector3(0, 0.0261f, 0);
                    break;
                case 2:
                    book.position -= new Vector3(0, 0.01305f, 0);
                    break;
            }

            book.position += bookshelf.transform.up * (bookshelfSize.x - xOffset);
            book.position += bookshelf.transform.right * zOffset;
            
            book.scale = new Vector3(0.8f, 0.8f, 0.8f);

            _bookData.Add(book);

            xOffset += _bookSize.x/2 + 0.05f;
        }
    }
}