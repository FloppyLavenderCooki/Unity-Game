using UnityEngine;

public class CreateBook : MonoBehaviour {

    private void Start() {
        BookGenerator.instance.GenerateBooks();
    }

}
