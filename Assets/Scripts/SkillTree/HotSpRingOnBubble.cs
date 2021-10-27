using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpRingOnBubble : MonoBehaviour
{
    Subscription<SnapEvent> snapEventSubscription;
    public float innerRadius = 66.0f;
    public float outerRadius = 89.0f;
    public float standardBubbleRadius = 4.0f;
    public int healthRegen = 0;
    public GameObject healingEffect;
    // Start is called before the first frame update
    void Start()
    {
        snapEventSubscription = EventBus.Subscribe<SnapEvent>(_OnSnap);
        innerRadius = GameManager.arenaInnerRadius;
        outerRadius = GameManager.arenaOuterRadius;
        standardBubbleRadius = GameManager.bubbleStandardRadius;
        healingEffect = Resources.Load<GameObject>("Prefabs/healingEffect");
    }

    void _OnSnap(SnapEvent e)
    {
        Vector3 distance = transform.position;
        float radius = transform.localScale[0] * standardBubbleRadius;
        if (distance.magnitude - radius > outerRadius)
        {
            return;
        }
        if (distance.magnitude + radius > innerRadius)
        {
            gameObject.GetComponent<BubbleController>().HealthChange(healthRegen);
            GameObject healingEffectObj = Instantiate(healingEffect,
                gameObject.transform.position + 
                new Vector3(radius * 0.7f, radius * 0.7f, 0),
                Quaternion.identity);
            healingEffectObj.AddComponent<HasSide>().side =
                gameObject.GetComponent<HasSide>().side;
            Vector3 scale = healingEffectObj.transform.localScale;
            scale *= gameObject.GetComponent<BubbleController>().radius
                / GameManager.bubbleStandardRadius;
            healingEffectObj.transform.localScale = scale;
        }                
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
