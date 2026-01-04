using UnityEngine;
using UnityEngine.Rendering;

public class NoFog : MonoBehaviour
{
    public Camera cameraWithoutFog;
    private bool originalFogState;

    void Start()
    {
        // Store the original fog state
        originalFogState = RenderSettings.fog;
        // Subscribe to render pipeline events
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks when the object is destroyed
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == cameraWithoutFog)
        {
            RenderSettings.fog = false;
        }
    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == cameraWithoutFog)
        {
            // Restore the state immediately after the camera is done rendering
            RenderSettings.fog = originalFogState;
        }
    }
}
