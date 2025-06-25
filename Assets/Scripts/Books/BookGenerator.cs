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
            int isSpecial = Random.Range(0, 5);

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
            finalBook.transform.rotation = Quaternion.Euler(0, 90, 0);

            if (i > 0) {
                setPos.z += finalBook.transform.GetComponentInChildren<Renderer>().bounds.size.z / 2 + 0.045f;
            } else {
                setPos.y += finalBook.transform.GetComponentInChildren<Renderer>().bounds.size.y / 2 + 0.045f;
            }
        } else {
            finalBook.transform.rotation = Quaternion.Euler(0, -90, 0);
            
            if (i > 0) {
                setPos.z -= finalBook.transform.GetComponentInChildren<Renderer>().bounds.size.z / 2 + 0.045f;
            } else {
                setPos.y += finalBook.transform.GetComponentInChildren<Renderer>().bounds.size.y / 2 + 0.045f;
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
        
        finalBook.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = GenBookColour();
        finalBook.transform.localScale =  GenBookVariantSizing(randInt);
        
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

    private Vector3 GenBookVariantSizing(int v) {
        float changeValue = 0f;
        if (v < 2) {
            changeValue = 0.025f;
        } else {
            changeValue = 0.01f;
        }
        
        
        float thickness = Random.Range(finalBook.transform.localScale.x - changeValue, finalBook.transform.localScale.x + changeValue);
        float sizeY = Random.Range(finalBook.transform.localScale.y - changeValue, finalBook.transform.localScale.y + changeValue);
        float sizeZ = Random.Range(finalBook.transform.localScale.z - changeValue, finalBook.transform.localScale.z + changeValue);
        
        return new Vector3(thickness, sizeY, sizeZ);
    }

    private string RandomiseName() {
        int pattern = Random.Range(0, 4);
        switch (pattern) {
            case 0: return $"The {RandomFrom(adjectives)} {RandomFrom(nouns)}";
            case 1: return $"{RandomFrom(verbs)} of the {RandomFrom(nouns)}";
            case 2: return $"{RandomFrom(names)}'s {RandomFrom(nouns)}";
            case 3: return $"The {RandomFrom(nouns)} of {RandomFrom(places)}";
            default: return "Untitled Book";
        }
    }
    
    string RandomFrom(string[] list) => list[Random.Range(0, list.Length)];
    
    
    // thanks chatgpt for generating most of these goofy ahh words!
    string[] adjectives = {
        "Dark", "Hidden", "Mysterious", "Lost", "Ancient",
        "Silent", "Burning", "Twisted", "Forbidden", "Eternal",
        "Shattered", "Crimson", "Fallen", "Golden", "Wicked",
        "Enchanted", "Broken", "Lonely", "Frozen", "Sacred",
        "Bloody", "Timeless", "Shadowed", "Ghostly", "Cursed",
        "Brilliant", "Forsaken", "Forgotten", "Radiant", "Hollow"
    };
    
    string[] nouns = {
        "Forest", "Empire", "Book", "Secret", "Shadow",
        "Flame", "Crown", "Blade", "Curse", "Dream",
        "Throne", "Path", "Fury", "Whisper", "Star",
        "Gate", "Mask", "Truth", "Key", "Storm",
        "Song", "Hollow", "Stone", "Rift", "Scroll",
        "Memory", "Moon", "Heart", "Light", "Night"
    };

    string[] verbs = {
        "Rise", "Fall", "Return", "Curse", "Whispers",
        "Burn", "Shatter", "Wander", "Escape", "Search",
        "Break", "Unveil", "Summon", "Haunt", "Awaken",
        "Forge", "Reveal", "Guard", "Chase", "Claim",
        "Follow", "Embrace", "Silence", "Cross", "Tear",
        "Remember", "Call", "Bind", "Hide", "Rule"
    };

    string[] names = {
        "Arav", "Kieran", "Jarvis", "Paul Crawford", "Kael",
        "Nora", "Thorne", "Lyra", "Dorian", "Vera",
        "Rowan", "Elias", "Zara", "Corwin", "Iris",
        "Soren", "Mira", "Jude", "Alaric", "Lira",
        "Orin", "Maeve", "Cassian", "Nyra", "Lucien",
        "Bryn", "Ezra", "Vanya", "Calen", "Riven"
    };

    string[] places = {
        "New Zealand", "Elaria", "Midreach", "The North", "Valemire",
        "Duskwatch", "Thornfell", "Ashmere", "Drakethorn", "Frostmoor",
        "Silvershore", "Nightspire", "Redreach", "Ebonvale", "Stormhold",
        "Blackridge", "Hollowmere", "Brightfen", "Dreadhollow", "Wyrmwood",
        "Suncrest", "Ironkeep", "Glimmerdeep", "Mistpeak", "Shadowfen",
        "Ravenmark", "Crystalrun", "Moonspire", "Greywatch", "Fallowbrook"
    };
}
