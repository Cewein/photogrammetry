using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instancing : MonoBehaviour
{
    private int instanceCount;
    public Mesh instanceMesh;
    public Material instanceMaterial;
    public int subMeshIndex = 0;
    public float size = 0.01f;

    public DepthMap depthMap;

    private int cachedInstanceCount = -1;
    private int cachedSubMeshIndex = -1;
    private float cachedSize = -1.0f;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private int imageWidth, imageHeight;
    private Vector4 cameraPosition, cameraForward, cameraUp;
    private float vfov;

    void Start()
    {
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        instanceCount = depthMap.GetSize();
        UpdateBuffers();
    }

    void Update()
    {
        // Update starting position buffer
        instanceCount = depthMap.GetSize();

        if (cachedInstanceCount != instanceCount || cachedSubMeshIndex != subMeshIndex || cachedSize != size)
            UpdateBuffers();


        //need to call the mat and bind the renderTexture
        instanceMaterial.SetTexture("depthTexture", depthMap.depthTexture);
        instanceMaterial.SetInt("imageWidth", depthMap.depthTexture.width);
        instanceMaterial.SetInt("imageHeight", depthMap.depthTexture.height);

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
        if (argsBuffer != null)
            argsBuffer.Release();
        argsBuffer = null;
    }
}
