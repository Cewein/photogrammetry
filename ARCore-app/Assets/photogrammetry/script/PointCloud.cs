using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloud : MonoBehaviour
{

    public Material mat;
    public Mesh mesh;

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
        Camera cam = GetComponent<Camera>();
        mat.SetTexture("DepthTexture", DemoDepth.depthTexture);
        Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, mesh.bounds, argsBuffer);
    }

    private void OnDestroy()
    {
        argsBuffer.Release();
    }
}
