using UnityEngine;

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

        private GameObject _book;
        private Vector3 _bookSize;
        private GameObject _books;
        
        public void Awake()
        {
            _hardbackBookRenderer = hardbackBook.GetComponentInChildren<Renderer>();
            _paperbackBookRenderer = paperbackBook.GetComponentInChildren<Renderer>();
            _thinStapledBookRenderer = thinStapledBook.GetComponentInChildren<Renderer>();
            _books = GameObject.Find("Books");
        }
        
        public void GenerateBookshelf(GameObject bookshelf, Vector3 bookshelfSize)
        {
            int bookType = Random.Range(0, 3);
            _book = bookType switch
            {
                0 => Instantiate(hardbackBook, bookshelf.transform),
                1 => Instantiate(paperbackBook, bookshelf.transform),
                2 => Instantiate(thinStapledBook, bookshelf.transform),
                _ => _book
            };
            
            _bookSize = bookType switch
            {
                0 => _hardbackBookRenderer.bounds.size,
                1 => _paperbackBookRenderer.bounds.size,
                2 => _thinStapledBookRenderer.bounds.size,
                _ => _bookSize
            };
            
            _book.transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
            // _book.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            
            _book.transform.position = new Vector3(
                bookshelf.transform.position.x+_bookSize.z,
                bookshelf.transform.position.y,
                bookshelf.transform.position.z + (bookshelfSize.y - (_bookSize.y*1.15f))
            );
            
            _book.transform.SetParent(_books.transform);
        }
    }
}