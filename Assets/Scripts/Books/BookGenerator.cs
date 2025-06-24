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
    
    public void GenerateBooks(Transform basePosition, bool isNew, bool isReverse) {
        // CALL THIS FUNCTION WHEN A SHELF IS INSTANTIATED!
        // can call this on Start() for the smart library. Will have to do something about that LAG though!
        
        setPos = basePosition.position;
        int randomAmount;

        if (isNew) {
            randomAmount = booksPerRowNew + 1;
        } else {
            randomAmount = booksPerRowOld + 1;
        }
        
        foreach (int i in Enumerable.Range(0, randomAmount)) {
            int isSpecial = Random.Range(0, 2);

            specialsUsed = specialBookPrefabs.Count;

            if (isSpecial == 1 && specialBookPrefabs.Count != 0) {
                int specialChosen = Random.Range(0, specialBookPrefabs.Count);
                finalBook = Instantiate(specialBookPrefabs[specialChosen], transform);
                specialBookPrefabs.RemoveAt(specialChosen);
                
                FinalInstantiate(isReverse, i);
            } else {
                CreateBook(isReverse, i);
            }
        }
    }

    private void FinalInstantiate(bool isReverse, int i) {
        finalBook.transform.parent = bookParent;

        if (!isReverse) {
            finalBook.transform.rotation = new Quaternion(-90, 90, 180,0);

            if (i > 0) {
                setPos.y += finalBook.transform.localScale.y / 2 + 0.075f;
            } else {
                setPos.z += finalBook.transform.localScale.z / 2 + 0.075f;
            }
        } else {
            finalBook.transform.rotation = new Quaternion(-90, 90, 0,0);
            if (i > 0) {
                setPos.y -= (finalBook.transform.localScale.y / 2 + 0.075f) / 100;
            } else {
                setPos.z += (finalBook.transform.localScale.z / 2 + 0.075f) / 100;
            }
        }
        
        finalBook.transform.position = setPos;
    }

    private void CreateBook(bool isReverse, int i) {
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
        
        finalBook.GetComponent<Renderer>().material = GenBookColour();
        finalBook.transform.localScale =  GenBookVariantSizing();
        
        TextMeshProUGUI[] tmpguis = finalBook.GetComponentsInChildren<TextMeshProUGUI>();
        string bookName = RandomiseName();
        
        foreach (TextMeshProUGUI tmp in tmpguis) {
            tmp.text = bookName;
        }
        
        // just make sure the object has a box-collider and a rigidbody! would lead to issues if i set this stuff here...
        
        FinalInstantiate(isReverse, i);
    }

    private Material GenBookColour() {
        int chosenColour = Random.Range(0, bookMaterials.Length);
        return bookMaterials[chosenColour];
    }

    private Vector3 GenBookVariantSizing() {
        float changeValue = 0.015f;
        
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
