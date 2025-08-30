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
        private Camera _outlineCamera;
        private VisualElement _uiElement;

        private void CreateRenderTexture()
        {
            _outlineRT = new RenderTexture(Screen.width, Screen.height, 24)
            {
                name = "OutlineRT",
                filterMode = FilterMode.Point
            };
            _outlineRT.Create();
            
            _outlineCamera.targetTexture = _outlineRT;
            _uiElement.style.backgroundImage = Background.FromRenderTexture(_outlineRT);
        }

        private void InitialiseObjects()
        {
            if (GameObject.Find("Outline Camera")) _outlineCamera = GameObject.Find("Outline Camera").GetComponent<Camera>();
            _uiElement = GameObject.Find("Outline UI").GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("main");
            CreateRenderTexture();
            CreateMaterials();
        }
        
        public void OnEnable() { InitialiseObjects(); }

        public override void Create() {}

        private void CreateMaterials()
        {
            if (!_outlineCamera) _outlineCamera = GameObject.Find("Outline Camera").GetComponent<Camera>();
            
            if (!_aberrationMaterial && aberrationShader)
            {
                _aberrationMaterial = new Material(aberrationShader);
                _aberrationRenderPass = new AberrationRenderPass(_aberrationMaterial, aberrationSettings)
                {
                    renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
                };
            }

            if (!_outlineMaterial && outlineShader)
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
            InitialiseObjects();
            
            var volumeComponent = VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();
            var enableInEditor = volumeComponent.enableInEditor.overrideState && volumeComponent.enableInEditor.value;

            if (_outlineRenderPass != null)
            {
                if ((Screen.width + Screen.height != 0) &&
                    (enableInEditor || renderingData.cameraData.cameraType == CameraType.Game) &&
                    renderingData.cameraData.camera.name == "Outline Camera")
                {
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
            
            var textures = Resources.FindObjectsOfTypeAll<RenderTexture>();
            foreach (var rt in textures)
            {
                if (!_outlineCamera || rt == _outlineCamera.targetTexture || rt.name != "OutlineRT" || rt == _outlineRT) continue;
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
        public bool includeObject = true;
    }
}
