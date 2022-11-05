using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Bounds GetBounds(GameObject gameObject)
    {
        return gameObject.GetComponent<Renderer>()?.bounds ?? new Bounds();
    }

    public static Vector3 GetRenderOffset(GameObject gameObject)
    {
        Bounds bounds = gameObject.GetComponent<Renderer>()?.bounds ?? new Bounds();
        return gameObject.transform.position - bounds.center;
    }
}
