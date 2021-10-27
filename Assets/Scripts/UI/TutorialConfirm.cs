using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialConfirm : MonoBehaviour
{
    public GameObject tutorial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        tutorial.SetActive(false);
        GameManager.Instance.GameStart();
    }
}
