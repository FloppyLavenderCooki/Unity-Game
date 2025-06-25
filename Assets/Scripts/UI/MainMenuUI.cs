using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UIButton = UnityEngine.UI.Button;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        private GameObject _kioskUI;
        private UIButton _playButton;
        private UIButton _optionsButton;
        private UIButton _aboutButton;
        private UIButton _quitButton;
        private VisualElement _root;
        private ListView _buttonList;
        private VisualElement _image;

        private void Start()
        {
            _kioskUI = GameObject.Find("Kiosk Screen (Canvas)/Panel");
            
            _playButton = _kioskUI.transform.Find("Start Button").GetComponent<UIButton>();
            _optionsButton = _kioskUI.transform.Find("Options Button").GetComponent<UIButton>();
            _aboutButton = _kioskUI.transform.Find("About Button").GetComponent<UIButton>();
            _quitButton = _kioskUI.transform.Find("Quit Button").GetComponent<UIButton>();
            
            _playButton.onClick.AddListener(() =>
            {
                PlayList();
                
                _image.style.backgroundImage = new StyleBackground();

                _buttonList.selectionChanged += selectedItems =>
                {
                    var enumerable = selectedItems.ToList();
                    if (enumerable.Count == 0) return;

                    var selectedButton = (Button)enumerable[0];
                    LoadingUI.LoadScene(selectedButton.text == "Normal Library"
                        ? "Scenes/NormalLibrary"
                        : "Scenes/SmartLibrary");
                };
            });
            _quitButton.onClick.AddListener(() =>
            {
                QuitList();

                _image.style.backgroundImage = new StyleBackground(
                    Resources.Load<Sprite>("Images/areyousure"));

                _buttonList.selectionChanged += selectedItems =>
                {
                    var enumerable = selectedItems.ToList();
                    if (enumerable.Count == 0) return;

                    var selectedButton = (Button)enumerable[0];
                    if (selectedButton.text == "Yes")
                    {
#if UNITY_EDITOR
                        EditorApplication.ExitPlaymode();
#else
                            Application.Quit();
#endif
                    }
                    else
                    {
                        _buttonList.ClearSelection();
                    }
                };
            });
            
            var uiDocument = GetComponent<UIDocument>();
            if (!uiDocument) return;
            
            _root = uiDocument.rootVisualElement;
            _image = _root.Q<VisualElement>("image");

            _root.Q<VisualElement>("background").pickingMode = PickingMode.Ignore;
            _root.Q<VisualElement>("kiosk").pickingMode = PickingMode.Ignore;
            
            _buttonList = _root.Q<ListView>("button-list");
        }
        
        private void PlayList()
        {
            _buttonList.Clear();
            
            var buttonItems = new List<string> { "Normal Library", "Smart Library" };
            
            _buttonList.makeItem = () => new Button();
            _buttonList.bindItem = (element, i) => ((Button)element).text = buttonItems[i];
            _buttonList.itemsSource = buttonItems;
        }

        private void QuitList()
        {
            _buttonList.Clear();
            
            var buttonItems = new List<string> { "Yes", "No" };
            
            _buttonList.makeItem = () => new Button();
            _buttonList.bindItem = (element, i) => ((Button)element).text = buttonItems[i];
            _buttonList.itemsSource = buttonItems;
        }
    }
}
