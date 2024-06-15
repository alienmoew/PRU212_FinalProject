using UnityEngine;

public static class MouseUtils
{
    public static Vector3 GetMouseWorldPosition2D(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 mousePosition = worldCamera.ScreenToWorldPoint(screenPosition);
        mousePosition.z = 0f;
        return mousePosition;
    }
    public static Vector3 GetMouseWorldPosition2D(Camera worldCamera)
    {
        return GetMouseWorldPosition2D(Input.mousePosition, worldCamera);
    }
    public static Vector3 GetMouseWorldPosition2D()
    {
        return GetMouseWorldPosition2D(Input.mousePosition, Camera.main);
    }

}
