using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(Vector3.forward, 90 * Random.Range(0,3));
        transform.Rotate(Vector3.up, 180 * Random.Range(0, 2));
    }
}
