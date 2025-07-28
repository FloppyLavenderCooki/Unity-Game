using UnityEngine;

namespace World
{
    public class BookshelfGenerator : MonoBehaviour
    {
        public GameObject hardbackBook;
        public GameObject paperbackBook;
        public GameObject thinStapledBook;
        public GameObject bookshelfPillar;
        
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

            GameObject pillar1 = Instantiate(bookshelfPillar, bookshelf.transform);
            pillar1.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            pillar1.transform.rotation = Quaternion.Euler(
                bookshelf.transform.rotation.x,
                bookshelf.transform.rotation.y + 90.0f,
                bookshelf.transform.rotation.z
            );
            pillar1.transform.position = bookshelf.transform.position + bookshelf.transform.up * (bookshelfSize.y * 0.5f);
            
            GameObject pillar2 = Instantiate(bookshelfPillar, bookshelf.transform);
            pillar2.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            pillar2.transform.rotation = Quaternion.Euler(
                bookshelf.transform.rotation.x,
                bookshelf.transform.rotation.y + 90.0f,
                bookshelf.transform.rotation.z
            );
            pillar2.transform.position = bookshelf.transform.position - bookshelf.transform.up * (bookshelfSize.y * 0.5f);
            
            _bookSize = bookType switch
            {
                0 => _hardbackBookRenderer.bounds.size,
                1 => _paperbackBookRenderer.bounds.size,
                2 => _thinStapledBookRenderer.bounds.size,
                _ => _bookSize
            };

            _book.transform.localRotation = Quaternion.Euler(
                bookshelf.transform.rotation.x,
                bookshelf.transform.rotation.y - 90.0f,
                bookshelf.transform.rotation.z - 90.0f
            );
            
            _book.transform.position = new Vector3(
                bookshelf.transform.position.x,
                bookshelf.transform.position.y + (bookshelfSize.y - (_bookSize.y * 0.8f)),
                bookshelf.transform.position.z
            );
            _book.transform.position += bookshelf.transform.right * _bookSize.x;
            
            _book.transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
            
            _book.GetComponent<Rigidbody>().freezeRotation = true;
            _book.GetComponent<Rigidbody>().useGravity = false;
            
            _book.transform.SetParent(_books.transform);
        }
    }
}