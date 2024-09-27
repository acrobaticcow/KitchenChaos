using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        LookAt,
        LookAtInvert,
        Forward,
        ForwardInverted
    }

    [SerializeField]
    private Mode mode;

    private void LateUpdate()
    {
        var dirFromCamera = transform.position - Camera.main.transform.position;
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInvert:
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case Mode.Forward:
                transform.forward = Camera.main.transform.position;
                break;
            case Mode.ForwardInverted:
                transform.forward = -Camera.main.transform.position;
                break;
        }
    }
}
