using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demoInstancing : MonoBehaviour
{
    public int instanceCount = 100000;
    public Mesh instanceMesh;
    public Material instanceMaterial;
    public int subMeshIndex = 0;
    public float size = 0.01f;

    private int cachedInstanceCount = -1;
    private int cachedSubMeshIndex = -1;
    private float cachedSize = -1.0f;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    void Start()
    {
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        instanceCount = DemoDepth.depthTexture.width * DemoDepth.depthTexture.height;
        UpdateBuffers();
    }

    void Update()
    {
        // Update starting position buffer
        instanceCount = DemoDepth.depthTexture.width * DemoDepth.depthTexture.height;

        if (cachedInstanceCount != instanceCount || cachedSubMeshIndex != subMeshIndex || cachedSize != size)
            UpdateBuffers();

        //need to call the mat and bind the renderTexture
        instanceMaterial.SetTexture("depthTexture", DemoDepth.depthTexture);
        instanceMaterial.SetInt("width", DemoDepth.depthTexture.width);

        // Render
        Graphics.DrawMeshInstancedIndirect(instanceMesh, subMeshIndex, instanceMaterial, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(265, 25, 200, 30), "Instance Count: " + instanceCount.ToString());
    }

    void UpdateBuffers()
    {
        // Ensure submesh index is in range
        if (instanceMesh != null)
            subMeshIndex = Mathf.Clamp(subMeshIndex, 0, instanceMesh.subMeshCount - 1);

        // Positions
        if (positionBuffer != null)
            positionBuffer.Release();
        positionBuffer = new ComputeBuffer(instanceCount, 16);
        Vector4[] positions = new Vector4[instanceCount];

        int width = DemoDepth.depthTexture.width;
        int height = DemoDepth.depthTexture.height;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                positions[i + j * Mathf.Max(width,height)] = new Vector4((i * 1.0f) / width, (j * 1.0f) / height, 0, size);
            }
        }
        positionBuffer.SetData(positions);
        instanceMaterial.SetBuffer("positionBuffer", positionBuffer);

        // Indirect args
        if (instanceMesh != null)
        {
            args[0] = (uint)instanceMesh.GetIndexCount(subMeshIndex);
            args[1] = (uint)instanceCount;
            args[2] = (uint)instanceMesh.GetIndexStart(subMeshIndex);
            args[3] = (uint)instanceMesh.GetBaseVertex(subMeshIndex);
        }
        else
        {
            args[0] = args[1] = args[2] = args[3] = 0;
        }
        argsBuffer.SetData(args);

        cachedInstanceCount = instanceCount;
        cachedSubMeshIndex = subMeshIndex;
        cachedSize = size;
    }

    void OnDisable()
    {
        if (positionBuffer != null)
            positionBuffer.Release();
        positionBuffer = null;

        if (argsBuffer != null)
            argsBuffer.Release();
        argsBuffer = null;
    }
}
