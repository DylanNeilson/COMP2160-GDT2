using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameObject panelToHide;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HidePanel()
    {
        if (panelToHide != null)
        {
            panelToHide.SetActive(false);
            Debug.Log("HidePanel called");
        }
    }
}
