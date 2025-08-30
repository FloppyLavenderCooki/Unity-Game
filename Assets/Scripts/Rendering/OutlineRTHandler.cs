using UnityEngine;
using UnityEngine.UIElements;

namespace Rendering
{
    public class OutlineRTHandler : MonoBehaviour
    {
        private UIDocument _outlineDoc;
        private RenderTexture _outlineRT;
        private Camera _outlineCamera;
        private VisualElement _uiElement;

        private void Start()
        {
            if (GameObject.Find("Outline Camera")) _outlineCamera = GameObject.Find("Outline Camera").GetComponent<Camera>();
            if (!_outlineDoc && GameObject.Find("Outline UI")) _outlineDoc = GameObject.Find("Outline UI").GetComponent<UIDocument>();
            if (_outlineDoc) _uiElement = _outlineDoc?.rootVisualElement?.Q<VisualElement>("main");
            CreateRenderTexture();
        }
        
        private void CreateRenderTexture()
        {
            if (Screen.width + Screen.height == 0) return;
            _outlineRT = new RenderTexture(Screen.width, Screen.height, 24)
            {
                name = "OutlineRT",
                filterMode = FilterMode.Point
            };
            _outlineRT.Create();

            if (!_outlineCamera) _outlineCamera = GameObject.Find("Outline Camera").GetComponent<Camera>();
            _outlineCamera.targetTexture = _outlineRT;
            if (_uiElement != null) _uiElement.style.backgroundImage = Background.FromRenderTexture(_outlineRT);
        }

        private void OnDestroy()
        {
            if (_outlineRT) _outlineRT.Release();
        }
    }
}
