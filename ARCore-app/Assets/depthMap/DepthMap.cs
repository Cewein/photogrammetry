using UnityEngine;

public class DepthMap : MonoBehaviour
{
    public Shader shader;
    public Camera cam;

    //render texture that any one can sample, updated at each frame
    [HideInInspector]
    public RenderTexture depthTexture;

    private Material mat;

    // Start is called before the first frame update
    void OnPreRender()
    {
        cam.depthTextureMode = DepthTextureMode.Depth;
    }

    private void Awake()
    {
        depthTexture = new RenderTexture(1, 1, 1);
    }

    public int GetSize()
    {
        return depthTexture.width * depthTexture.height;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {

        if (mat == null) mat = new Material(shader);

        if (Input.GetKey(KeyCode.Space))
        {
            depthTexture.Release();
            depthTexture = new RenderTexture(src);
            Graphics.Blit(src, depthTexture, mat);
        }

        Graphics.Blit(src, dest);
    }
}

