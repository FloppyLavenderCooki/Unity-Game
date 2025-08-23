using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class LoadingUI : MonoBehaviour
    {
        private static VisualElement root;
        private static VisualElement loadingCover;

        private void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument)
            {
                root = uiDocument.rootVisualElement;
            }
        }

        public static void LoadScene(string sceneName)
        {
            if (root == null) return;

            root.Q<VisualElement>("container").style.visibility = Visibility.Visible;
            loadingCover = root.Q<VisualElement>("loading-cover");

            var loading = SceneManager.LoadSceneAsync(sceneName);
            if (loading == null) return;

            MonoBehaviour currentInstance = FindAnyObjectByType<LoadingUI>();
            if (currentInstance)
            {
                currentInstance.StartCoroutine(UpdateLoadingText(loading));
            }
        }

        private static IEnumerator UpdateLoadingText(AsyncOperation operation)
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
