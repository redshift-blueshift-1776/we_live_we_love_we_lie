// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal;

// public class RetroBlitFeature : ScriptableRendererFeature
// {
//     class RetroBlitPass : ScriptableRenderPass
//     {
//         private Material material;
//         private RTHandle tempRT;

//         public RetroBlitPass(Material mat)
//         {
//             material = mat;
//             renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
//         }

//         public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
//         {
//             var desc = renderingData.cameraData.cameraTargetDescriptor;
//             desc.depthBufferBits = 0;

//             tempRT = RTHandles.Alloc(
//                 desc.width,
//                 desc.height,
//                 colorFormat: desc.colorFormat,
//                 name: "_RetroTempRT"
//             );
//         }

//         public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//         {
//             if (material == null)
//                 return;

//             CommandBuffer cmd = CommandBufferPool.Get("Retro Effect");

//             RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

//             // Source → Temp
//             Blitter.BlitCameraTexture(cmd, source, tempRT, material, 0);

//             // Temp → Source
//             Blitter.BlitCameraTexture(cmd, tempRT, source);

//             context.ExecuteCommandBuffer(cmd);
//             CommandBufferPool.Release(cmd);
//         }

//         public override void OnCameraCleanup(CommandBuffer cmd)
//         {
//             if (tempRT != null)
//                 RTHandles.Release(tempRT);
//         }
//     }

//     [SerializeField] private Shader retroShader;
//     private Material retroMaterial;
//     private RetroBlitPass pass;

//     public override void Create()
//     {
//         if (retroShader == null)
//             return;

//         retroMaterial = CoreUtils.CreateEngineMaterial(retroShader);
//         pass = new RetroBlitPass(retroMaterial);
//     }

//     public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
//     {
//         renderer.EnqueuePass(pass);
//     }
// }
