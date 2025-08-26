using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace World
{
    public class Objectives : MonoBehaviour
    {
        private UIDocument _document;
        private VisualElement _objectives;
        private int _objectiveNum;
        private Transform _books;
        public TextColorGradient gradient;

        private void Start()
        {
            _document = GetComponent<UIDocument>();
            _objectives = _document.rootVisualElement.Q<VisualElement>("objectives");
            _books = GameObject.Find("Books").transform;
        }

        private void Update()
        {
            if (!_books || _objectives == null || _books.childCount <= 0 || _objectives.childCount != 0
                || Keyboard.current.rKey.wasPressedThisFrame) return;
            
            GameObject book = _books.GetChild(Random.Range(0, _books.childCount)).gameObject;
            Color bookColor = book.GetComponentInChildren<Renderer>().material.color;
            if (book.GetComponentInChildren<Renderer>().material.color.grayscale < 0.2) bookColor *= 2.5f;
            if (book.GetComponentInChildren<Renderer>().material.color.grayscale < 0.01) bookColor = Color.white;
            gradient.topLeft = gradient.topRight = gradient.bottomLeft = gradient.bottomRight = bookColor;
            
            AddObjective($"Get the book: <gradient=\"{gradient.name}\">{book.name}</gradient>");
            AddObjective("Drop it on the kiosk");
            AddObjective("Put it in the box");
        }
        
        private void AddObjective(string objectiveText)
        {
            _objectiveNum++;
            Label objective = new Label
            {
                name = $"objective-{_objectiveNum}", text = objectiveText,
                enableRichText = true
            };
            objective.AddToClassList("objective");
            
            _objectives.Add(objective);
        }
    }
}
