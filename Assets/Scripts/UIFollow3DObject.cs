using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow3DObject : MonoBehaviour
{
    public RectTransform myRectTransform;
    public Transform followTransform;
    public Vector3 screenSpaceOffset;
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(followTransform.position);
        //screenPos = screenPos / Screen.width * 1024.0f;     // @HARDCODE
        myRectTransform.SetPositionAndRotation(screenPos + screenSpaceOffset, Quaternion.identity);
    }
}
