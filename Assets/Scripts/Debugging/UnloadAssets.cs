using UnityEngine;

namespace Debugging
{
    public class UnloadAssets : MonoBehaviour
    {
        [ContextMenu("Unload Unused Assets")]
        private void UnloadUnusedAssets()
        {
            Debug.Log("Unloading unused assets...");
            
            try
            {
                Resources.UnloadUnusedAssets();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error unloading unused assets: {ex.Message}\n{ex.StackTrace}");
            }
            
            Debug.Log("Unused assets unloaded successfully.");
        }
    }
}
