using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Rendering
{
    public class OutlineRenderPass : ScriptableRenderPass
    {
        private static readonly int OutlineId = Shader.PropertyToID("_Outline");
        private const string KOutlinePassName = "OutlineRenderPass";
        private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int IncludeObjectId = Shader.PropertyToID("_IncludeObject");

        private readonly OutlineSettings _defaultSettings;
        private readonly Material _material;

        private RenderTextureDescriptor _outlineTextureDescriptor;

        public OutlineRenderPass(Material material, OutlineSettings defaultSettings)
        {
            this._material = material;
            this._defaultSettings = defaultSettings;

            _outlineTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height,
                RenderTextureFormat.BGRA32, 0);
        }

        private void UpdateOutlineSettings()
        {
            var volumeComponent = VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();
            var outline = volumeComponent.outline.overrideState ?
                volumeComponent.outline.value : _defaultSettings.outline;
            _material.SetFloat(OutlineId, outline);
            
            var outlineColor = volumeComponent.outlineColor.overrideState ?
                volumeComponent.outlineColor.value : _defaultSettings.outlineColor;
            _material.SetColor(OutlineColorId, outlineColor);
            
            var includeObject = volumeComponent.includeObject.overrideState ?
                volumeComponent.includeObject.value : _defaultSettings.includeObject;
            _material.SetInt(IncludeObjectId, includeObject ? 1 : 0);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var volumeComponent = VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();
            var enableOutline = volumeComponent.enableOutline.overrideState ?
                volumeComponent.enableOutline.value : _defaultSettings.enableOutline;
            
            if (!enableOutline)
                return;
            
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            if (resourceData.isActiveTargetBackBuffer)
                return;

            _outlineTextureDescriptor.width = cameraData.cameraTargetDescriptor.width;
            _outlineTextureDescriptor.height = cameraData.cameraTargetDescriptor.height;
            _outlineTextureDescriptor.depthBufferBits = 0;
            _outlineTextureDescriptor.colorFormat = RenderTextureFormat.ARGB32;

            var srcCamColor = resourceData.activeColorTexture;
            
            if (!_material) return;
            UpdateOutlineSettings();
            
            if (!srcCamColor.IsValid())
                return;
            
            var workTexture = renderGraph.CreateTexture(new TextureDesc(_outlineTextureDescriptor) { name = "WorkTexture" });
            
            renderGraph.AddBlitPass(
                new RenderGraphUtils.BlitMaterialParameters(srcCamColor, workTexture, _material, 0),
                "Pre-Blit");
            
            renderGraph.AddBlitPass(
                new RenderGraphUtils.BlitMaterialParameters(workTexture, srcCamColor, _material, 0),
                KOutlinePassName);
        }
    }
}
