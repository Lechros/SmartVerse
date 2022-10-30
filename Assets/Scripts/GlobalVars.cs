using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVars : MonoBehaviour
{
    GameObject freeCam;

    // Start is called before the first frame update
    void Start()
    {
        freeCam = GameObject.Find("Third Person Camera");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
