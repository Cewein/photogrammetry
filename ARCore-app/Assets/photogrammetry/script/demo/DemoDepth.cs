using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoDepth : MonoBehaviour
{
    public ComputeShader compute;

    [HideInInspector]
    static public RenderTexture depthTexture;


    // Start is called before the first frame update
    void OnPreRender()
    {
        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {

        RenderTexture depthBuffer = new RenderTexture(src.width, src.height, 1);
        depthBuffer.enableRandomWrite = true;
        if (depthBuffer.Create())
        {
            compute.SetTexture(0, "_ColorBuffer", src);
            compute.SetTexture(0, "_DetphBufferRW", depthBuffer);

            int tileX = (src.width + 7) / 8;
            int tileY = (src.height + 7) / 8;

            compute.Dispatch(0, tileX, tileY, 1);

            Graphics.Blit(depthBuffer, depthTexture);

            Graphics.Blit(depthBuffer, dest);
        }

        depthBuffer.Release();
    }
}
