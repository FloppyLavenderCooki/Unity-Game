using UnityEngine;

public class CreateBook : MonoBehaviour {

    public bool isNew;
    public bool isReverse;

    private void Start() {
        BookGenerator.instance.GenerateBooks(transform, isNew, isReverse);
    }
}
