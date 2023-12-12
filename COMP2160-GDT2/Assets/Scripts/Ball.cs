using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private int layerToIgnore;


    void Update()
    {
        layerToIgnore = LayerMask.NameToLayer("Player Invisible Wall");

        if (layerToIgnore != -1)
        {
            Physics.IgnoreLayerCollision(gameObject.layer, layerToIgnore, true);
        }
    }
}
