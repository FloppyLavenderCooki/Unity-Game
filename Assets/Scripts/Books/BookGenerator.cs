using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class BookGenerator : MonoBehaviour {
    // singleton class might be best case here
    public static BookGenerator instance;
    
    private void Awake() {
        if (instance != null) {
            Debug.LogWarning("additional book gens found!");
        }
        
        instance = this;
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
    
    public void GenerateBooks(Transform basePosition) {
        // CALL THIS FUNCTION WHEN A SHELF IS INSTANTIATED!
        // can call this on Start() for the smart library. Will have to do something about that LAG though!
        
        foreach (int i in Enumerable.Range(0, 11)) {
            int isSpecial = Random.Range(0, 2);

            specialsUsed = specialBookPrefabs.Count;

            if (isSpecial == 1 && specialBookPrefabs.Count != 0) {
                int specialChosen = Random.Range(0, specialBookPrefabs.Count);
                finalBook = Instantiate(specialBookPrefabs[specialChosen], transform);
                specialBookPrefabs.RemoveAt(specialChosen);
                finalBook.transform.parent = bookParent;
                finalBook.transform.position = basePosition.position;
            } else {
                CreateBook(basePosition);
            }
        }
    }

    private void CreateBook(Transform basePosition) {
        int randInt = Random.Range(0, 3);
        
        if (randInt == 0) {
            finalBook = Instantiate(hardBookPrefab);
        } else if (randInt == 1) {
            finalBook = Instantiate(softBookPrefab);
        } else if (randInt == 2) {
            finalBook = Instantiate(stapleBookPrefab);
        } else {
            // finalBook = ... wait a minute
        }
        
        finalBook.GetComponent<Renderer>().material.color = GenBookColour();
        finalBook.transform.localScale =  GenBookVariantSizing();
        
        TextMeshProUGUI[] tmpguis = finalBook.GetComponentsInChildren<TextMeshProUGUI>();
        string bookName = RandomiseName();
        
        foreach (TextMeshProUGUI tmp in tmpguis) {
            tmp.text = bookName;
        }
        
        finalBook.AddComponent<Rigidbody>();
        finalBook.AddComponent<BoxCollider>();
        
        finalBook.transform.parent = bookParent;
        finalBook.transform.position = basePosition.position;
    }

    private Color GenBookColour() {
        int chosenColour = Random.Range(0, bookMaterials.Length);
        return bookMaterials[chosenColour].color;
    }

    private Vector3 GenBookVariantSizing() {
        float changeValue = 0.05f;
        
        float thickness = Random.Range(finalBook.transform.localScale.x - changeValue, finalBook.transform.localScale.x + changeValue);
        float sizeY = Random.Range(finalBook.transform.localScale.y - changeValue, finalBook.transform.localScale.y + changeValue);
        float sizeZ = Random.Range(finalBook.transform.localScale.z - changeValue, finalBook.transform.localScale.z + changeValue);
        
        return new Vector3(thickness, sizeY, sizeZ);
    }

    private string RandomiseName() {
        return "amazing book!";
    }
    
    // jarvis can you add your book gen stuff here ⬇! then feed it into the RandomiseName() function!
}
