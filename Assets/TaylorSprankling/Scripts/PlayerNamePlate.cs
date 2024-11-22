using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNamePlate : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}