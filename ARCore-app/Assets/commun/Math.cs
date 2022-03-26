using UnityEngine;


public class Math
{
    public static Matrix4x4 CameraIntrinsicMatrix(float width, float height, float fov)
    {
        float px = width / 2.0f;
        float py = height / 2.0f;

        float hfov = fov / 360.0f * 2.0f * Mathf.PI;

        float fx = width / (2.0f * Mathf.Tan(hfov / 2.0f));

        float vfov = 2.0f * Mathf.Atan(Mathf.Tan(hfov) * height / width);
        float fy = height / (2.0f * Mathf.Tan(vfov / 2.0f));

        Matrix4x4 mat = Matrix4x4.identity;

        mat.SetRow(0, new Vector4(fx, 0.0f, px, 0.0f));
        mat.SetRow(1, new Vector4(0.0f, fy, py, 0.0f));

        MonoBehaviour.print(mat);

        return mat;
    }
}