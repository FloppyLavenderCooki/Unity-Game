using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class LoadingUI : MonoBehaviour
    {
        private static VisualElement _root;
        private static VisualElement _loadingCover;

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
            _loadingCover = _root.Q<VisualElement>("loading-cover");

            var loading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            if (loading == null) return;

            MonoBehaviour currentInstance = FindAnyObjectByType<LoadingUI>();
            if (currentInstance)
            {
                currentInstance.StartCoroutine(UpdateLoadingText(loading, _loadingCover));
            }
        }

        private static IEnumerator UpdateLoadingText(AsyncOperation operation, VisualElement loadingCover)
        {
            while (!operation.isDone)
            {
                loadingCover.style.translate =
                    new StyleTranslate(new Translate(Length.Percent(90 + (operation.progress / 2 * 35)), 0));

                yield return null;
            }
        }
    }
}
