using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        private GameObject _kioskUI;
        private VisualElement _kioskRoot;
        private Button _playButton;
        private Button _optionsButton;
        private Button _aboutButton;
        private Button _quitButton;
        private VisualElement _root;
        private ListView _buttonList;
        private VisualElement _image;
        private Label _info;

        private void Start()
        {
            _kioskUI = GameObject.Find("Kiosk UI");
            _kioskRoot = _kioskUI.GetComponent<UIDocument>().rootVisualElement;
            
            _playButton = _kioskRoot.Q<Button>("start");
            _optionsButton = _kioskRoot.Q<Button>("options");
            _aboutButton = _kioskRoot.Q<Button>("about");
            _quitButton = _kioskRoot.Q<Button>("quit");
            
            _playButton.clicked += () =>
            {
                SelectKioskButton(_playButton);
                PlayList();
                ResetImage();
                _info.text = "Scene Select";
            };
            _optionsButton.clicked += () =>
            {
                SelectKioskButton(_optionsButton);
                OptionsList();
                ResetImage();
                _info.text = "Game Settings";
            };
            _aboutButton.clicked += () =>
            {
                SelectKioskButton(_aboutButton);
                AboutList();
                ResetImage();
                _info.text = "Credits (github.com/)";
            };
            _quitButton.clicked += () =>
            {
                SelectKioskButton(_quitButton);
                QuitList();
                SetImage("Images/thanks");
                _info.text = "Quit Game?";
            };
            
            var uiDocument = GetComponent<UIDocument>();
            if (!uiDocument) return;
            
            _root = uiDocument.rootVisualElement;
            _image = _root.Q<VisualElement>("image");
            _info = _root.Q<Label>("info");

            _root.Q<VisualElement>("background").pickingMode = PickingMode.Ignore;
            _root.Q<VisualElement>("kiosk").pickingMode = PickingMode.Ignore;
            
            _buttonList = _root.Q<ListView>("button-list");
            
            Resources.UnloadUnusedAssets();
        }

        private void Update()
        {
            if (!float.IsNaN(_image.resolvedStyle.width) && _image.resolvedStyle.height == 0)
                _image.style.height = _image.resolvedStyle.width;
        }
        
        private void PlayList()
        {
            ClearList();
            
            var buttonItems = new List<string> { "Normal Library", "Smart Library" };
            
            _buttonList.makeItem = () => new VisualElement();
            _buttonList.bindItem = (element, i) => {
                element.AddToClassList("list-item");
                
                var button = new Button();
                element.Add(button);
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
            ClearList();
            
            var buttonItems = new List<string> { "Mouse Sensitivity", "Target Frame Rate", "Resolution" };
            
            _buttonList.makeItem = () => new VisualElement();
            _buttonList.bindItem = (element, i) => {
                element.AddToClassList("list-item");
                
                var button = new Button();
                element.Add(button);
                button.text = buttonItems[i];
                button.clicked += () => {
                    SetImage("Images/" + button.text);
                };
            };
            _buttonList.itemsSource = buttonItems;
        }
        
        private void AboutList()
        {
            ClearList();
            
            var buttonItems = new List<string> { "SunnyFloppyDiskStudios", "Cooki-Studios", "salping" };
            
            _buttonList.makeItem = () => new VisualElement();
            _buttonList.bindItem = (element, i) => {
                element.AddToClassList("list-item");
                
                var button = new Button();
                element.Add(button);
                button.text = buttonItems[i];
                button.clicked += () => {
                    SetImage("Images/" + button.text);
                    SetImageRadius(50);
                };
            };
            _buttonList.itemsSource = buttonItems;
        }

        private void QuitList()
        {
            ClearList();
            
            var buttonItems = new List<string> { "Yes", "No" };
            
            _buttonList.makeItem = () => new VisualElement();
            _buttonList.bindItem = (element, i) => {
                element.AddToClassList("list-item");
                
                var button = new Button();
                element.Add(button);
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
                        ClearList();
                    }
                };
            };
            _buttonList.itemsSource = buttonItems;
        }

        private void ClearList()
        {
            _info.text = "Select\u0020a button from the left";
            ResetImage();
            _buttonList.ClearSelection();
            _buttonList.Clear();
            _buttonList.itemsSource = null;
        }
        
        private void SetImageRadius(float radius)
        {
            _image.style.borderTopLeftRadius = new StyleLength(Length.Percent(radius));
            _image.style.borderTopRightRadius = new StyleLength(Length.Percent(radius));
            _image.style.borderBottomLeftRadius = new StyleLength(Length.Percent(radius));
            _image.style.borderBottomRightRadius = new StyleLength(Length.Percent(radius));
        }

        private void ResetImage()
        {
            SetImageRadius(0);
            _image.style.backgroundImage = new StyleBackground();
            Resources.UnloadUnusedAssets();
        }

        private void SetImage(string imagePath)
        {
            _image.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(imagePath));
        }
        
        // private void SetImage(Sprite image)
        // {
        //     _image.style.backgroundImage = new StyleBackground(image);
        // }

        private void SelectKioskButton(Button button)
        {
            _playButton.RemoveFromClassList("selected");
            _optionsButton.RemoveFromClassList("selected");
            _aboutButton.RemoveFromClassList("selected");
            _quitButton.RemoveFromClassList("selected");
            
            button.AddToClassList("selected");
        }
    }
}
