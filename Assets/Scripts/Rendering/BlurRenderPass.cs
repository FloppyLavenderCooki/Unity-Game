using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace Rendering
{
    public class BlurRenderPass : ScriptableRenderPass
    {
        private static readonly int BlurId = Shader.PropertyToID("_Blur");
        private const string KBlurPassName = "BlurRenderPass";

        private readonly BlurSettings _defaultSettings;
        private readonly Material _material;

        private RenderTextureDescriptor _blurTextureDescriptor;

        public BlurRenderPass(Material material, BlurSettings defaultSettings)
        {
            this._material = material;
            this._defaultSettings = defaultSettings;

            _blurTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height,
                RenderTextureFormat.BGRA32, 0);
        }

        private void UpdateBlurSettings()
        {
            var volumeComponent = VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();
            var blur = volumeComponent.blur.overrideState ?
                volumeComponent.blur.value : _defaultSettings.blur;
            _material.SetFloat(BlurId, blur);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var volumeComponent = VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();
            var enableBlur = volumeComponent.enableBlur.overrideState ?
                volumeComponent.enableBlur.value : _defaultSettings.enableBlur;
            var enableOutline = volumeComponent.enableOutline.value;

            if (GameObject.Find("Outline UI") && GameObject.Find("Outline Camera"))
            {
                UIDocument outlineUI = GameObject.Find("Outline UI").GetComponent<UIDocument>();
                Camera outlineCamera = GameObject.Find("Outline Camera").GetComponent<Camera>();

                if (enableOutline)
                {
                    outlineUI.enabled = true;
                    outlineCamera.enabled = true;
                }
                else
                {
                    outlineUI.enabled = false;
                    outlineCamera.enabled = false;
                }
            }

            if (!enableBlur)
            {
                return;
            }

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            if (resourceData.isActiveTargetBackBuffer)
                return;

            _blurTextureDescriptor.width = cameraData.cameraTargetDescriptor.width;
            _blurTextureDescriptor.height = cameraData.cameraTargetDescriptor.height;
            _blurTextureDescriptor.depthBufferBits = 0;
            _blurTextureDescriptor.colorFormat = RenderTextureFormat.ARGB32;

            var srcCamColor = resourceData.activeColorTexture;
            
            if (!_material) return;
            UpdateBlurSettings();

            if (!srcCamColor.IsValid())
                return;
            
            var workTexture = renderGraph.CreateTexture(new TextureDesc(_blurTextureDescriptor)
                { name = "WorkTexture", colorFormat = GraphicsFormat.R8G8B8A8_SRGB });
            
            renderGraph.AddBlitPass(
                new RenderGraphUtils.BlitMaterialParameters(srcCamColor, workTexture, _material, 0),
                "Pre-Blit");
            
            renderGraph.AddBlitPass(
                new RenderGraphUtils.BlitMaterialParameters(workTexture, srcCamColor, _material, 0),
                KBlurPassName);
        }
    }
}
