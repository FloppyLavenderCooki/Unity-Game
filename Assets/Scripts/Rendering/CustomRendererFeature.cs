using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

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

        private void OnEnable()
        {
            CreateRenderTexture();
        }

        private void CreateRenderTexture()
        {
            if (_outlineRT)
            {
                _outlineRT.Release();
            }

            _outlineRT = new RenderTexture(Screen.width, Screen.height, 24);
            _outlineRT.Create();
            
            Debug.Log(Screen.width+", "+Screen.height);
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
                if ((enableInEditor || renderingData.cameraData.cameraType == CameraType.Game) && renderingData.cameraData.camera.name == "Outline Camera")
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
            }
            else
            {
                DestroyImmediate(_aberrationMaterial);
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
