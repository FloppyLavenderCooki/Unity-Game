using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Rendering
{
    public class CustomRendererFeature : ScriptableRendererFeature
    {
        [SerializeField] private AberrationSettings settings;
        [SerializeField] private Shader aberrationShader;
        private Material _material;
        private AberrationRenderPass _aberrationRenderPass;
        private CustomVolumeComponent _volumeComponent;

        public override void Create()
        {
            if (!aberrationShader)
                return;

            _material = new Material(aberrationShader);
            _aberrationRenderPass = new AberrationRenderPass(_material, settings)
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_aberrationRenderPass == null)
                return;

            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(_aberrationRenderPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Application.isPlaying)
            {
                Destroy(_material);
            }
            else
            {
                DestroyImmediate(_material);
            }
        }
    }

    [Serializable]
    public class AberrationSettings
    {
        public bool enableAberration;
        [Range(0, 0.01f)] public float aberration;
    }
}