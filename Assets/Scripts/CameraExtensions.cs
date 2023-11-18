using UnityEngine;

public static class CameraExtensions
{
    /// <summary>
    /// <para>For a given point in pixel coordinates, finds the intersection point of a ray cast from the camera
    /// and some plane given its normal and some position representative of any point on the plane.</para>
    /// </summary>
    /// <param name="camera">The camera source to cast the screen position into a ray from.</param>
    /// <param name="screenPoint">Position in pixel coordinates to be raycast.</param>
    /// <param name="planeNormal">Normal of the plane to intersect</param>
    /// <param name="planePoint">Any position the lies on the plane to intersect</param>
    /// <param name="intersectionPoint">When this method returns, contains the intersection point on the plane if the ray
    /// cast for the given screen point is not parallel to the plane; otherwise, defaults to <c>Vector3.zero()</c>. Will still
    /// contain the correct intersection point the intersection point is in the opposite direction of the ray. </param>
    /// <returns><c>true</c> if the ray cast through the given pixel coordinates is not parallel to the given plane and
    /// the intersection point is not in the opposite direction of the ray.</returns>
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
