using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloud : MonoBehaviour
{

    public Material mat;
    public Mesh mesh;
    public Camera cam;

    ComputeBuffer argsBuffer;
   

    // Start is called before the first frame update
    void Start()
    {
        uint numInstances = (uint)(DemoDepth.depthTexture.width * DemoDepth.depthTexture.height);

        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(new uint[] { mesh.GetIndexCount(0), numInstances, 0, 0, 0 });

        mat.SetTexture("DepthTexture", DemoDepth.depthTexture);
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetTexture("DepthTexture", DemoDepth.depthTexture);
        mat.SetInt("width", DemoDepth.depthTexture.width);
        mat.SetInt("height", DemoDepth.depthTexture.height);

        mat.SetVector("cameraPosition", cam.transform.position);
        mat.SetVector("cameraForward", cam.transform.forward);
        mat.SetVector("cameraUp", cam.transform.up);
        mat.SetVector("cameraRight", cam.transform.right);
        mat.SetFloat("cameraFOV", cam.fieldOfView);


        Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, mesh.bounds, argsBuffer);
    }

    private void OnDestroy()
    {
        argsBuffer.Release();
    }
}
