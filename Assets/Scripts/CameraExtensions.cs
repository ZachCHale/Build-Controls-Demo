using UnityEngine;

public static class CameraExtensions
{
    public static Vector3 GetScreenPointIntersectionWithPlane(this Camera cam, Vector2 screenPoint, Vector3 planeNormal, Vector3 planePoint)
    {
        Plane plane = new Plane(planeNormal, planePoint);
        Ray ray = cam.ScreenPointToRay(new Vector3(screenPoint.x, screenPoint.y, cam.nearClipPlane));
        float distance;
        plane.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
}
