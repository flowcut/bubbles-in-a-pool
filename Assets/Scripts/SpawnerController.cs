using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    Subscription<RestartEvent> restartEventSubscription;
    Subscription<SnapEvent> snapEventSubscription;
    public GameObject defaultBubble;
    public float spawnRandomInterrupt = 0.2f;
    public int numSpawningBubble = 0;

    public delegate void SetBubbleDelegate(GameObject bubble);
    public SetBubbleDelegate setBubbleMethod;
    public int spawningBubbleIndex = 0;

    public delegate int NumEffectedBubble(int numBubble);

    public Dictionary<SetBubbleDelegate, NumEffectedBubble> singleNodeEffectRange
        = new Dictionary<SetBubbleDelegate, NumEffectedBubble>();

    private SideName side;
    void Start()
    {
        restartEventSubscription = EventBus.Subscribe<RestartEvent>(_OnRestart);
        snapEventSubscription = EventBus.Subscribe<SnapEvent>(_OnSnap);
        side = GetComponent<HasSide>().side;
        setBubbleMethod = DefaultSetBubble;
    }

    void _OnRestart(RestartEvent e)
    {
        StopAllCoroutines();
        setBubbleMethod = DefaultSetBubble;
        singleNodeEffectRange.Clear();
    }

    void _OnSnap(SnapEvent e)
    {
        if (e.current_snap != 1)
        {
            return;
        }
        StartCoroutine(SpawnAll());
        // TODO: spawn compensate
    }

    IEnumerator SpawnAll()
    {
        SetBubbleDelegate[] singleEffectSetBubbles = new SetBubbleDelegate[numSpawningBubble];
        spawningBubbleIndex = 0;
        foreach (var item in singleNodeEffectRange)
        {
            for(int i=0; i<item.Value(numSpawningBubble); ++i)
            {
                singleEffectSetBubbles[spawningBubbleIndex] += item.Key;
                spawningBubbleIndex++;
                spawningBubbleIndex %= numSpawningBubble;
            }
        }
        // TODO: Change this

        yield return new WaitForSeconds(Random.Range(0f, spawnRandomInterrupt));
        spawningBubbleIndex = 0;
        GameObject previousBubble = SpawnOne();
        setBubbleMethod(previousBubble);
        if (singleEffectSetBubbles[spawningBubbleIndex] != null)
            singleEffectSetBubbles[spawningBubbleIndex](previousBubble);
        for(int i = 1; i < numSpawningBubble; ++i)
        {
            spawningBubbleIndex = i;
            while (true)
            {
                Vector3 distance = previousBubble.gameObject.transform.position -
                    gameObject.transform.position;
                if (distance.magnitude > 2 * previousBubble.GetComponent<BubbleController>().radius)
                {
                    break;
                    // TODO: radius change
                }
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(0f, spawnRandomInterrupt));
            previousBubble = SpawnOne();
            setBubbleMethod(previousBubble);
            if (singleEffectSetBubbles[spawningBubbleIndex] != null)
                singleEffectSetBubbles[spawningBubbleIndex](previousBubble);
        }
    }

    private void DefaultSetBubble(GameObject bubble)
    {
        BubbleController bc = bubble.GetComponent<BubbleController>();

        bc.maxHealth = GameManager.bubbleStandardMaxHealth;
        bc.currentHealth = bc.maxHealth;

        bc.radius = GameManager.bubbleStandardRadius;
        bc.damage = GameManager.bubbleStandardHitDamage;
        bc.hitForce = GameManager.bubbleStandardHitForce;

        bubble.GetComponent<Rigidbody2D>().mass = GameManager.bubbleStandardMass;
    }

    GameObject SpawnOne()
    {
        GameObject aBubble = Instantiate(
            defaultBubble,
            gameObject.transform.position,
            Quaternion.identity);

        HasSide BSide = aBubble.AddComponent<HasSide>();
        BSide.side = side;

        CircleCollider2D Collider = aBubble.AddComponent<CircleCollider2D>();
        Collider.radius = GameManager.bubbleStandardColliderRadius;

        aBubble.AddComponent<BubbleEnvironmentalForce>();
      
        return aBubble;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(snapEventSubscription);
    }
}
