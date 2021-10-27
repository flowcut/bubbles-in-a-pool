using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject PausePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClick()
    {
        if (PausePanel.GetComponent<PausePanelControl>().controlEnable)
        {
            StartCoroutine(PausePanel.GetComponent<PausePanelControl>().Toggle());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
