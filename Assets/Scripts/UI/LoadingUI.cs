using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class LoadingUI : MonoBehaviour
    {
        private static VisualElement _root;
        
        private void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument)
            {
                _root = uiDocument.rootVisualElement;
            }
        }
        
        public static void LoadScene(string sceneName)
        {
            if (_root == null) return;
            
            _root.Q<VisualElement>("container").style.visibility = Visibility.Visible;
            
            var loading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
