using System.Collections.Generic;
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
            });
            _optionsButton.onClick.AddListener(() =>
            {
                OptionsList();
                _image.style.backgroundImage = new StyleBackground();
            });
            _aboutButton.onClick.AddListener(() =>
            {
                AboutList();
                _image.style.backgroundImage = new StyleBackground();
            });
            _quitButton.onClick.AddListener(() =>
            {
                QuitList();
                _image.style.backgroundImage = new StyleBackground(
                    Resources.Load<Sprite>("Images/areyousure"));
            });
            
            var uiDocument = GetComponent<UIDocument>();
            if (!uiDocument) return;
            
            _root = uiDocument.rootVisualElement;
            _image = _root.Q<VisualElement>("image");

            _root.Q<VisualElement>("background").pickingMode = PickingMode.Ignore;
            _root.Q<VisualElement>("kiosk").pickingMode = PickingMode.Ignore;
            
            _buttonList = _root.Q<ListView>("button-list");
            
            Resources.UnloadUnusedAssets();
        }
        
        private void PlayList()
        {
            _buttonList.Clear();
            _buttonList.itemsSource = null;
            SetImageRadius(0);
            
            var buttonItems = new List<string> { "Normal Library", "Smart Library" };
            
            _buttonList.makeItem = () => new Button();
            _buttonList.bindItem = (element, i) => {
                var button = (Button)element;
                button.text = buttonItems[i];
                button.clicked += () =>
                {
                    LoadingUI.LoadScene(button.text == "Normal Library"
                        ? "Scenes/NormalLibrary"
                        : "Scenes/SmartLibrary");
                };
            };
            _buttonList.itemsSource = buttonItems;
        }
        
        private void OptionsList()
        {
            _buttonList.Clear();
            _buttonList.itemsSource = null;
            
            var buttonItems = new List<string> { "Mouse Sensitivity", "Target Frame Rate", "Resolution" };
            
            _buttonList.makeItem = () => new Button();
            _buttonList.bindItem = (element, i) => {
                var button = (Button)element;
                button.text = buttonItems[i];
                button.clicked += () => {
                    _image.style.backgroundImage = new StyleBackground(
                        Resources.Load<Sprite>("Images/" + button.text.Replace("@", "")));
                    SetImageRadius(50);
                };
            };
            _buttonList.itemsSource = buttonItems;
        }
        
        private void AboutList()
        {
            _buttonList.Clear();
            _buttonList.itemsSource = null;
            
            var buttonItems = new List<string> { "@SunnyFloppyDiskStudios", "@Cooki-Studios", "@salping" };
            
            _buttonList.makeItem = () => new Button();
            _buttonList.bindItem = (element, i) => {
                var button = (Button)element;
                button.text = buttonItems[i];
                button.clicked += () => {
                    _image.style.backgroundImage = new StyleBackground(
                        Resources.Load<Sprite>("Images/" + button.text.Replace("@", "")));
                    SetImageRadius(50);
                };
            };
            _buttonList.itemsSource = buttonItems;
        }

        private void QuitList()
        {
            _buttonList.Clear();
            _buttonList.itemsSource = null;
            SetImageRadius(0);
            
            var buttonItems = new List<string> { "Yes", "No" };
            
            _buttonList.makeItem = () => new Button();
            _buttonList.bindItem = (element, i) => {
                var button = (Button)element;
                button.text = buttonItems[i];
                button.clicked += () => {
                    if (button.text == "Yes")
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
            };
            _buttonList.itemsSource = buttonItems;
        }
        
        private void SetImageRadius(float radius)
        {
            _image.style.borderTopLeftRadius = new StyleLength(Length.Percent(radius));
            _image.style.borderTopRightRadius = new StyleLength(Length.Percent(radius));
            _image.style.borderBottomLeftRadius = new StyleLength(Length.Percent(radius));
            _image.style.borderBottomRightRadius = new StyleLength(Length.Percent(radius));
        }
    }
}
