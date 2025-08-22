using Player;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

public class PauseMenuManager : MonoBehaviour {
    public CameraController camCon;
    public Transform panel;
    public GameObject cursorUI;
    
    [Header("Buttons")]
    public GameObject resumeButton;
    public GameObject settingsButton;
    public GameObject mainMenuButton;
    public GameObject quitButton;

    private void Start() {
        // initialise
        panel.localScale = new Vector3(0f,0f,1f);
    }
    
    public void PauseGame() {
        panel.DOScaleX(1f, 0.25f).SetEase(Ease.OutBack);
        panel.DOScaleY(1f, 0.25f).SetEase(Ease.OutBack);
        // DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.05f, 0.25f).SetUpdate(true);
        cursorUI.SetActive(false);
    }

    public void ResumeGame() {
        panel.DOScaleX(0f, 0.25f).SetEase(Ease.InBack);
        panel.DOScaleY(0f, 0.25f).SetEase(Ease.OutBack);
        // DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 0.25f).SetUpdate(true);
        cursorUI.SetActive(true);
    }
    
    public void QuitGame() {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
}
