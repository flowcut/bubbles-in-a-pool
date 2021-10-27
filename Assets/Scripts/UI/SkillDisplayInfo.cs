using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDisplayInfo : MonoBehaviour
{
    public string skillName;
    public string skillDescription;
    public GameObject levelDisplay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Destroy(levelDisplay);
    }
}
