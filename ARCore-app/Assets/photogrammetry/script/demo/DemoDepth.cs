using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoDepth : MonoBehaviour
{
    public ComputeShader compute;
    public Camera cam;

    [HideInInspector]
    static public RenderTexture depthTexture;


    // Start is called before the first frame update
    void OnPreRender()
    {
        cam.depthTextureMode = DepthTextureMode.Depth;  
    }

    private void Awake()
    {
        depthTexture = new RenderTexture(1, 1, 1);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (Input.GetKey(KeyCode.Space))
            depthTexture.Release();
        RenderTexture depthBuffer = new RenderTexture(src.width, src.height, 1);
        depthBuffer.enableRandomWrite = true;
        if (Input.GetKey(KeyCode.Space))
            depthTexture = new RenderTexture(depthBuffer);
        if (depthBuffer.Create())
        {
            compute.SetTexture(0, "_ColorBuffer", src);
            compute.SetTexture(0, "_DetphBufferRW", depthBuffer);

            int tileX = (src.width + 7) / 8;
            int tileY = (src.height + 7) / 8;

            compute.Dispatch(0, tileX, tileY, 1);

            if(Input.GetKey(KeyCode.Space))
                Graphics.Blit(depthBuffer, depthTexture);

            Graphics.Blit(depthBuffer, dest);
        }

        depthBuffer.Release();
    }
}
