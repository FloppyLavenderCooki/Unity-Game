using UnityEngine;
using UnityEngine.UIElements;

namespace World
{
    public class Objectives : MonoBehaviour
    {
        private UIDocument _document;
        private VisualElement _objectives;
        private int _objectiveNum;
        private string _bookName = "THE BOOK";

        private void Start()
        {
            _document = GetComponent<UIDocument>();
            _objectives = _document.rootVisualElement.Q<VisualElement>("objectives");

            AddObjective($"Get the book: <gradient=\"Book Name Gradient\">{_bookName}</gradient>");
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
