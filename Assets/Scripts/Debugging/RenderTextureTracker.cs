using UnityEngine;

namespace Debugging
{
    public class RenderTextureTracker : MonoBehaviour
    {
        [ContextMenu("Log All Render Textures")]
        private void LogAllRenderTextures()
        {
            var textures = Resources.FindObjectsOfTypeAll<RenderTexture>();
            foreach (var rt in textures)
            {
                Debug.Log($"RenderTexture: {rt.name}, {rt.width}x{rt.height}, Format: {rt.format}");
            }
        }
    }
}
