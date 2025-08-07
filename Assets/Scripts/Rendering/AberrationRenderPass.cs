using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace Rendering
{
    public class AberrationRenderPass : ScriptableRenderPass
    {
        private static readonly int AberrationId = Shader.PropertyToID("_Aberration");
        private const string KAberrationPassName = "AberrationRenderPass";

        private readonly AberrationSettings _defaultSettings;
        private readonly Material _material;

        private RenderTextureDescriptor _aberrationTextureDescriptor;
        
        private readonly UIDocument _outlineUI = GameObject.Find("Outline UI").GetComponent<UIDocument>();
        private readonly Camera _outlineCamera = GameObject.Find("Outline Camera").GetComponent<Camera>();

        public AberrationRenderPass(Material material, AberrationSettings defaultSettings)
        {
            this._material = material;
            this._defaultSettings = defaultSettings;

            _aberrationTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height,
                RenderTextureFormat.BGRA32, 0);
        }

        private void UpdateAberrationSettings()
        {
            if (!_material) return;

            var volumeComponent = VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();
            var aberration = volumeComponent.aberration.overrideState ?
                volumeComponent.aberration.value : _defaultSettings.aberration;
            _material.SetFloat(AberrationId, aberration);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var volumeComponent = VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();
            var enableAberration = volumeComponent.enableAberration.overrideState ?
                volumeComponent.enableAberration.value : _defaultSettings.enableAberration;
            var enableOutline = volumeComponent.enableOutline.value;

            if (enableOutline)
            {
                _outlineUI.enabled = true;
                _outlineCamera.enabled = true;
            }
            else
            {
                _outlineUI.enabled = false;
                _outlineCamera.enabled = false;
            }
            
            if (!enableAberration)
            {
                return;
            }

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            if (resourceData.isActiveTargetBackBuffer)
                return;

            _aberrationTextureDescriptor.width = cameraData.cameraTargetDescriptor.width;
            _aberrationTextureDescriptor.height = cameraData.cameraTargetDescriptor.height;
            _aberrationTextureDescriptor.depthBufferBits = 0;
            _aberrationTextureDescriptor.colorFormat = RenderTextureFormat.ARGB32;

            var srcCamColor = resourceData.activeColorTexture;

            UpdateAberrationSettings();

            if (!srcCamColor.IsValid())
                return;
            
            var workTexture = renderGraph.CreateTexture(new TextureDesc(_aberrationTextureDescriptor) { name = "WorkTexture" });

            renderGraph.AddBlitPass(
                new RenderGraphUtils.BlitMaterialParameters(srcCamColor, workTexture, _material, 0),
                "Pre-Blit");
            
            renderGraph.AddBlitPass(
                new RenderGraphUtils.BlitMaterialParameters(workTexture, srcCamColor, _material, 0),
                KAberrationPassName);
        }
    }
}
