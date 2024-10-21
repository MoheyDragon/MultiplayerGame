using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Transform playerCamera;
    [SerializeField] float updateTime;
    float loopCounter;
    private void Awake()
    {
        SetCamera(Camera.main.transform);
    }

    public void SetCamera(Transform camera)
    {
        playerCamera = camera;
    }

    void Update()
    {
        if(loopCounter>=updateTime)
        {
            loopCounter++;
        }
        else
        {
            loopCounter = 0;
            _LookAtCamera();
        }
    }
    private void _LookAtCamera()
    {
        Vector3 direction = playerCamera.position - transform.position;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }
}
