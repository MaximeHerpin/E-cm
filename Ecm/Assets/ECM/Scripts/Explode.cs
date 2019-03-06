using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour {

    public GameObject ruins;
    public GameObject explosionFX;

    public void Start()
    {
        ruins.SetActive(false);
    }

    public void Boom()
    {
        ruins.SetActive(true);
        if (explosionFX != null)
            Instantiate(explosionFX, transform.position, Quaternion.identity);
    }
}
