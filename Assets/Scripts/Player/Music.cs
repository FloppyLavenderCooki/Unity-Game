using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class Music : MonoBehaviour {
    [SerializeField] private EventReference musicEvent; // assign this in the Inspector

    private EventInstance musicInstance;

    private void Start() {
        musicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        
        if (musicEvent.IsNull) {
            Debug.LogWarning("No FMOD music event assigned.");
            return;
        }

        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
    }

    private void OnDestroy() {
        musicInstance.stop(STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}