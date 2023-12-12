using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool goal;

    void Start()
    {
        goal = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Throwable"))
        {
            goal = true;
            Debug.Log("Player Scored");
        }
    }
}
