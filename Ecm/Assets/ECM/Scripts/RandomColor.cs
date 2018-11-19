using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour {

    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        rend = GetComponent<Renderer>();

        rend.GetPropertyBlock(propBlock);

        propBlock.SetColor("_Color", Color.HSVToRGB(Random.value, .5f + Random.value * .5f, 1));
        rend.SetPropertyBlock(propBlock);
    }

}
