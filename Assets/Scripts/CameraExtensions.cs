using UnityEngine;

public static class CameraExtensions
{
    public static bool GetScreenPointIntersectionWithPlane(this Camera camera, Vector2 screenPoint, Vector3 planeNormal, Vector3 planePoint, out Vector3 intersectionPoint)
    {
        Plane targetPlane = new Plane(planeNormal, planePoint);
        Ray screenPointRay = camera.ScreenPointToRay(new Vector3(screenPoint.x, screenPoint.y, camera.nearClipPlane));
        bool valid = targetPlane.Raycast(screenPointRay, out float distance);
        bool parallelEdgeCase = !valid && distance == 0;
        bool oppositeDirectionEdgeCase = !valid && !parallelEdgeCase;
        intersectionPoint = parallelEdgeCase ? Vector3.zero : screenPointRay.GetPoint(distance);
        return valid;
    }
}
