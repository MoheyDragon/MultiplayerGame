using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDebugger : MonoBehaviour
{
    [SerializeField] KeyTime []timeKeys;
    private void Update()
    {
        for (int i = 0; i < timeKeys.Length; i++)
        {
            if (Input.GetKeyDown(timeKeys[i].keyCode))
                Time.timeScale = timeKeys[i].timeSpeed;
        }
    }
}
[System.Serializable]   
public class KeyTime
{
    [Range(0,100)]
    public float timeSpeed;

    public KeyCode keyCode;
}
