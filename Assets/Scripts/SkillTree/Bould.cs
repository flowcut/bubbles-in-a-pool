using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bould : Skill
{
    int level = 0;
    public float[] swellingScale = new float[3] { 1.1f , 1.2f, 1.3f };
    public int[] lifeAddition = new int[3] { 15, 35, 55 };

    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        Destroy(gameObject.GetComponent<Bould>());
    }
    public static void Register(SideName side)
    {
        if (GameManager.Instance.sideSpawner[side].GetComponent<Bould>() == null)
        {
            GameManager.Instance.sideSpawner[side].AddComponent<Bould>();
        }
        else
        {
            GameManager.Instance.sideSpawner[side].GetComponent<Bould>().Upgrade();
        }        
    }

    public void Upgrade()
    {
        level += 1;
    }
    public void BouldSetBubble(GameObject bubble)
    {
        BubbleController bc = bubble.GetComponent<BubbleController>();

        bc.maxHealth += lifeAddition[level];
        bc.currentHealth += lifeAddition[level];

        bc.radius *= swellingScale[level];
        bc.hitForce *= swellingScale[level];

        bubble.transform.localScale *= swellingScale[level];
        bubble.GetComponent<Rigidbody2D>().mass *= swellingScale[level];
        // This will change collider size as well
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        level = 0;
        SpawnerController Spawner = gameObject.GetComponent<SpawnerController>();
        Spawner.setBubbleMethod += BouldSetBubble;
        // Spawner.numSpawningBubble += 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
