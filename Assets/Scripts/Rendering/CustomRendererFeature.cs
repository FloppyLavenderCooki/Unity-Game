using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace Rendering
{
    public class CustomRendererFeature : ScriptableRendererFeature
    {
        [SerializeField] private AberrationSettings aberrationSettings;
        [SerializeField] private Shader aberrationShader;
        [SerializeField] private OutlineSettings outlineSettings;
        [SerializeField] private Shader outlineShader;
        private Material _aberrationMaterial;
        private AberrationRenderPass _aberrationRenderPass;
        private Material _outlineMaterial;
        private OutlineRenderPass _outlineRenderPass;
        private CustomVolumeComponent _volumeComponent;
        private RenderTexture _outlineRT;

        private void CreateRenderTexture()
        {
            _outlineRT = new RenderTexture(Screen.width, Screen.height, 24)
            {
                name = "OutlineRT",
                filterMode = FilterMode.Point
            };
            _outlineRT.Create();
            
            var textures = Resources.FindObjectsOfTypeAll<RenderTexture>();
            foreach (var rt in textures)
            {
                if (rt == GameObject.Find("Outline Camera").GetComponent<Camera>().targetTexture || rt.name != "OutlineRT" || rt == _outlineRT) continue;
                rt.Release();
                if (Application.isPlaying)
                {
                    Destroy(rt);
                }
                else
                {
                    DestroyImmediate(rt);
                }
            }
            
            GameObject.Find("Outline Camera").GetComponent<Camera>().targetTexture = _outlineRT;
            GameObject.Find("Outline UI").GetComponent<UIDocument>().rootVisualElement
                .Q<VisualElement>("main").style.backgroundImage = Background.FromRenderTexture(_outlineRT);
        }

        public override void Create()
        {
            if (aberrationShader)
            {
                _aberrationMaterial = new Material(aberrationShader);
                _aberrationRenderPass = new AberrationRenderPass(_aberrationMaterial, aberrationSettings)
                {
                    renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
                };
            }

            if (outlineShader)
            {
                _outlineMaterial = new Material(outlineShader);
                _outlineRenderPass = new OutlineRenderPass(_outlineMaterial, outlineSettings)
                {
                    renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
                };
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var volumeComponent = VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();
            var enableInEditor = volumeComponent.enableInEditor.overrideState && volumeComponent.enableInEditor.value;

            if (_outlineRenderPass != null)
            {
                if ((Screen.width + Screen.height != 0) && (enableInEditor || renderingData.cameraData.cameraType == CameraType.Game) && renderingData.cameraData.camera.name == "Outline Camera")
                {
                    CreateRenderTexture();
                    renderer.EnqueuePass(_outlineRenderPass);
                }
            }
            
            if (_aberrationRenderPass != null)
            {
                if (enableInEditor || renderingData.cameraData.cameraType == CameraType.Game)
                {
                    renderer.EnqueuePass(_aberrationRenderPass);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Application.isPlaying)
            {
                Destroy(_aberrationMaterial);
                Destroy(_outlineMaterial);
            }
            else
            {
                DestroyImmediate(_aberrationMaterial);
                DestroyImmediate(_outlineMaterial);
            }
        }
    }

    [Serializable]
    public class AberrationSettings
    {
        public bool enableAberration;
        [Range(0, 0.01f)] public float aberration;
    }
    
    [Serializable]
    public class OutlineSettings
    {
        public bool enableOutline;
        [Range(0, 0.01f)] public float outline;
        public Color outlineColor = Color.white;
    }
}
