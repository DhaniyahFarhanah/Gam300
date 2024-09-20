using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysLookAtCamera : MonoBehaviour
{
    private Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
    }
    private void FixedUpdate()
    {
        if (cam == null)
            return;

        transform.LookAt(cam);
    }
}
