using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPanel : MonoBehaviour, IPanel
{
    [SerializeField]
    Transform ground;

    [SerializeField]

    public float worldSize;

    void Awake()
    {
        ground.localScale = new Vector3(worldSize / 10f, 1f, worldSize / 10f);
    }

    void Update()
    {
        ground.localScale = new Vector3(worldSize / 10f, 1f, worldSize / 10f);
    }

    public void OnSetActive(bool value)
    {

    }
}
