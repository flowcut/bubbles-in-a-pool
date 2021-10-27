using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBubble : Skill
{

    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        Destroy(gameObject.GetComponent<FirstBubble>());
    }
    public static void Register(SideName side)
    {
        // Debug.Log("there");
        if (GameManager.Instance.sideSpawner[side].GetComponent<FirstBubble>() == null)
        {
            GameManager.Instance.sideSpawner[side].AddComponent<FirstBubble>();
        }
        
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        // Debug.Log("here");
        gameObject.GetComponent<SpawnerController>().numSpawningBubble = 7;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
