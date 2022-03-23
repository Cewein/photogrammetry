using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloud : MonoBehaviour
{

    public Material mat;
    public Mesh mesh;
    public Camera cam;

    public DepthMap depthMap;

    private ComputeBuffer argsBuffer;
    private uint numInstances;
   

    // Start is called before the first frame update
    void Start()
    {
        numInstances = (uint)(depthMap.GetSize());

        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(new uint[] { mesh.GetIndexCount(0), numInstances, 0, 0, 0 });

        print(numInstances);
    }

    // Update is called once per frame
    void Update()
    {
        uint numInstancesTmp = (uint)(depthMap.GetSize());

        if (numInstancesTmp > 1 && numInstances != numInstancesTmp)
        {
            numInstances = numInstancesTmp;
            argsBuffer.Release();
            argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
            argsBuffer.SetData(new uint[] { mesh.GetIndexCount(0), numInstances, 0, 0, 0 });

            print(numInstances);
        }

        mat.SetTexture("DepthTexture", depthMap.depthTexture);
        mat.SetInt("width", depthMap.depthTexture.width);
        mat.SetInt("height", depthMap.depthTexture.height);

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
