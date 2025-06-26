using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Book
{
    public string author;
    public string name;
}

[System.Serializable]
public class BookList
{
    public Book[] books;
}

public class BookGenerator : MonoBehaviour
{
    // singleton class might be best case here
    public static BookGenerator instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("additional book gens found!");
        }

        instance = this;

        string jsonData = Resources.Load<TextAsset>("names").text;
        Debug.Log("json data should be: " + jsonData);
        list_of_names = JsonUtility.FromJson<BookList>(jsonData);
    }

    [Header("Prefabs")]
    [SerializeField] private GameObject hardBookPrefab;
    [SerializeField] private GameObject softBookPrefab;
    [SerializeField] private GameObject stapleBookPrefab;
    private GameObject finalBook;

    [SerializeField] private List<GameObject> specialBookPrefabs;
    private int specialsUsed;

    [SerializeField] private Material[] bookMaterials;

    [Header("Settings")]
    [SerializeField] private Transform bookParent;

    [SerializeField] private int booksPerRowOld; // old bookshelf style
    [SerializeField] private int booksPerRowNew; // new bookshelf style, 

    // other
    private Vector3 setPos;

    private BookList list_of_names;

    public void Start()
    {
        Debug.Log("BookList object should be: " + list_of_names);
        Debug.Log("List of books should be:  " + list_of_names.books);
        Debug.Log("First Book object should be: " + list_of_names.books[0]);
        Debug.Log("First Book author should be: " + list_of_names.books[0].author);
    }

    public void GenerateBooks(Transform basePosition, bool isNew, bool isReverse)
    {
        // CALL THIS FUNCTION WHEN A SHELF IS INSTANTIATED!
        // can call this on Start() for the smart library. Will have to do something about that LAG though!

        setPos = basePosition.position;
        int randomAmount;

        if (isNew)
        {
            randomAmount = booksPerRowNew + 1;
        }
        else
        {
            randomAmount = booksPerRowOld + 1;
        }

        List<int> specialIndexes = new();
        int totalBooks = randomAmount;
        int numSpecials = Mathf.Min(specialBookPrefabs.Count, totalBooks);
        int minSpacing = 2;

        while (specialIndexes.Count < numSpecials)
        {
            int index = Random.Range(0, totalBooks);

            bool tooClose = specialIndexes.Any(i => Mathf.Abs(i - index) < minSpacing);
            if (!tooClose)
            {
                specialIndexes.Add(index);
            }
        }

        for (int i = 0; i < totalBooks; i++)
        {
            bool isSpecial = specialIndexes.Contains(i);

            if (isSpecial && specialBookPrefabs.Count > 0)
            {
                int specialChosen = Random.Range(0, specialBookPrefabs.Count);
                finalBook = Instantiate(specialBookPrefabs[specialChosen], transform);
                specialBookPrefabs.RemoveAt(specialChosen);

                FinalInstantiate(isReverse, i);
            }
            else
            {
                CreateBook(isReverse, i);
            }
        }
    }

    private void FinalInstantiate(bool isReverse, int i)
    {
        finalBook.transform.parent = bookParent;
        
        // Set rotation based on direction
        finalBook.transform.rotation = Quaternion.Euler(0, isReverse ? -90 : 90, 0);

        Vector3 forward = finalBook.transform.right;

        Renderer renderer = finalBook.GetComponentInChildren<Renderer>();
        float spacing = renderer.bounds.size.z / 2 + 0.045f;

        if (!isReverse)
        {
            finalBook.transform.rotation = Quaternion.Euler(0, 90, 0);
            forward = -finalBook.transform.right;

            if (i > 0)
            {
                setPos += forward * spacing;
            }
            else
            {
                setPos.y += renderer.bounds.size.y / 2 + 0.045f;
            }
        }
        else
        {
            finalBook.transform.rotation = Quaternion.Euler(0, -90, 0);

            if (i > 0)
            {
                setPos -= forward * spacing;
            }
            else
            {
                setPos.y += renderer.bounds.size.y / 2 + 0.045f;
            }
        }

        finalBook.transform.position = setPos;
    }

    private void CreateBook(bool isReverse, int i)
    {
        int randInt = Random.Range(0, 3);

        if (randInt == 0)
        {
            finalBook = Instantiate(hardBookPrefab);
        }
        else if (randInt == 1)
        {
            finalBook = Instantiate(softBookPrefab);
        }
        else if (randInt == 2)
        {
            finalBook = Instantiate(stapleBookPrefab);
        }
        else
        {
            // finalBook = ... wait a minute
        }

        finalBook.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = GenBookColour();
        finalBook.transform.localScale = GenBookVariantSizing(randInt);

        TextMeshProUGUI[] tmpguis = finalBook.GetComponentsInChildren<TextMeshProUGUI>();
        string bookName = RandomiseName();

        foreach (TextMeshProUGUI tmp in tmpguis)
        {
            tmp.text = bookName;
        }

        // just make sure the object has a box-collider and a rigidbody! would lead to issues if i set this stuff here...

        FinalInstantiate(isReverse, i);
    }

    private Material GenBookColour()
    {
        int chosenColour = Random.Range(0, bookMaterials.Length);
        return bookMaterials[chosenColour];
    }

    private Vector3 GenBookVariantSizing(int v)
    {
        float changeValue = 0f;
        if (v < 2)
        {
            changeValue = 0.025f;
        }
        else
        {
            changeValue = 0.01f;
        }


        float thickness = Random.Range(finalBook.transform.localScale.x - changeValue, finalBook.transform.localScale.x + changeValue);
        float sizeY = Random.Range(finalBook.transform.localScale.y - changeValue, finalBook.transform.localScale.y + changeValue);
        float sizeZ = Random.Range(finalBook.transform.localScale.z - changeValue, finalBook.transform.localScale.z + changeValue);

        return new Vector3(thickness, sizeY, sizeZ);
    }

    private string RandomiseName()
    {
        Book selected_book = list_of_names.books[Random.Range(0, list_of_names.books.Length)];
        string book_formatted = $"{selected_book.name} - {selected_book.author}";

        return book_formatted;
    }
}
