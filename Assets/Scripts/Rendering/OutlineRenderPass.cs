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
                    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
                    private const string KOutlinePassName = "OutlineRenderPass";
            
                    private readonly OutlineSettings _defaultSettings;
                    private readonly Material _material;
            
                    private RenderTargetIdentifier _outlineMaskRT;
                    private readonly RenderTexture _outlineMaskTexture;
                    private RenderTextureDescriptor _outlineTextureDescriptor;
            
                    public OutlineRenderPass(Material material, OutlineSettings defaultSettings, RenderTargetIdentifier outlineMaskRT, RenderTexture outlineMaskTexture)
                    {
                        this._material = material;
                        this._defaultSettings = defaultSettings;
                        this._outlineMaskRT = outlineMaskRT;
                        this._outlineMaskTexture = outlineMaskTexture;
            
                        _outlineTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height,
                            RenderTextureFormat.BGRA32, 0);
                    }
            
                    private void UpdateOutlineSettings()
                    {
                        if (!_material) return;
            
                        var volumeComponent = VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();
                        var outline = volumeComponent.outline.overrideState ?
                            volumeComponent.outline.value : _defaultSettings.outline;
                        _material.SetFloat(OutlineId, outline);
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
            
                        var srcCamColor = resourceData.activeColorTexture;
            
                        UpdateOutlineSettings();
                        _material.SetTexture(MainTex, _outlineMaskTexture);
            
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