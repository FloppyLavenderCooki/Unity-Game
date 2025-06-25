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

        private void Start()
        {
            _kioskUI = GameObject.Find("Kiosk Screen (Canvas)/Panel");
            
            _playButton = _kioskUI.transform.Find("Start Button").GetComponent<UIButton>();
            _optionsButton = _kioskUI.transform.Find("Options Button").GetComponent<UIButton>();
            _aboutButton = _kioskUI.transform.Find("About Button").GetComponent<UIButton>();
            _quitButton = _kioskUI.transform.Find("Quit Button").GetComponent<UIButton>();
            
            _playButton.onClick.AddListener(() => LoadingUI.LoadScene("Scenes/NormalLibrary"));
            
            var uiDocument = GetComponent<UIDocument>();
            if (!uiDocument) return;
            
            _root = uiDocument.rootVisualElement;

            _root.Q<VisualElement>("background").pickingMode = PickingMode.Ignore;
            _root.Q<VisualElement>("kiosk").pickingMode = PickingMode.Ignore;
        }
    }
}
