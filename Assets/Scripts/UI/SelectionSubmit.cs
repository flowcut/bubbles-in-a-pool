using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSubmit : MonoBehaviour
{
    public bool selected = false;
    public int selectionIndex = 0;

    // Start is called before the first frame update

    private void OnEnable()
    {
        selected = false;
        selectionIndex = 0;
    }

    public void onClick()
    {
        selected = true;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
