using UnityEngine;

public static class ScreenBounds2D
{
    public static void GetWorldBounds(Camera cam, out float left, out float right, out float bottom, out float top)
    {
        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        left = bl.x;
        bottom = bl.y;
        right = tr.x;
        top = tr.y;
    }
}
