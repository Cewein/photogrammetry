using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloud : MonoBehaviour
{
    public Mesh instanceMesh;
    public Material instanceMaterial;
    public float MeshSize = 0.01f;

    public DepthMap depthMap;
    public Camera cam;

    private int instanceCount;
    private int cachedInstanceCount = -1;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    void Start()
    {
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        instanceCount = depthMap.GetSize();
        UpdateBuffers();
    }

    void Update()
    {
        // Update starting position buffer
        instanceCount = depthMap.GetSize();

        if (cachedInstanceCount != instanceCount)
        {
            instanceMaterial.SetMatrix("camMat", cam.cameraToWorldMatrix);
            instanceMaterial.SetMatrix("InvCamMatProj", Matrix4x4.Inverse(cam.projectionMatrix));
            instanceMaterial.SetVector("camPos", cam.transform.position);
            UpdateBuffers();
        }

        //need to call the mat and bind the renderTexture
        instanceMaterial.SetTexture("depthTexture", depthMap.depthTexture);
        instanceMaterial.SetInt("imageWidth", depthMap.depthTexture.width);
        instanceMaterial.SetInt("imageHeight", depthMap.depthTexture.height);
        instanceMaterial.SetMatrix("invKmat", Matrix4x4.Inverse(Math.CameraIntrinsicMatrix(depthMap.depthTexture.width, depthMap.depthTexture.height, cam.fieldOfView)));
        instanceMaterial.SetFloat("meshSize", MeshSize);

        // Render
        Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMaterial, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(265, 25, 200, 30), "Instance Count: " + instanceCount.ToString());
    }

    void UpdateBuffers()
    {
        // Indirect args
        if (instanceMesh != null)
        {
            args[0] = (uint)instanceMesh.GetIndexCount(0);
            args[1] = (uint)instanceCount;
            args[2] = (uint)instanceMesh.GetIndexStart(0);
            args[3] = (uint)instanceMesh.GetBaseVertex(0);
        }
        else
        {
            args[0] = args[1] = args[2] = args[3] = 0;
        }

        argsBuffer.SetData(args);

        cachedInstanceCount = instanceCount;
    }

    void OnDisable()
    {
        if (argsBuffer != null)
            argsBuffer.Release();
        argsBuffer = null;
    }
}
