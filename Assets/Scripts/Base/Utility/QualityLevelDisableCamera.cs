using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityLevelDisableCamera : MonoBehaviour
{
    public int disableIfLower;

    private void Start()
    {
        if (QualitySettings.GetQualityLevel() <= disableIfLower)
        {
            GetComponent<Camera>().enabled = false;
        }
    }
}
