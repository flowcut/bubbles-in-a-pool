using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkenOnBubble : MonoBehaviour
{
    Subscription<SnapEvent> snapEventSubscription;

    public int coolDown;
    public int timer;
    public bool canBlock;

    public GameObject linken;

    public GameObject linkenObj;

    // Start is called before the first frame update
    void Start()
    {
        canBlock = true;
        timer = 0;
        snapEventSubscription = EventBus.Subscribe<SnapEvent>(_OnSnap);
        linken = Resources.Load<GameObject>("Prefabs/LinkenProtection");

        linkenObj = Instantiate(linken,
            gameObject.transform.position,
            Quaternion.identity,
            gameObject.transform);
        Vector3 scale = linkenObj.transform.localScale;
        scale *= gameObject.GetComponent<BubbleController>().radius
            / GameManager.bubbleStandardRadius;
        linkenObj.transform.localScale = scale;
    }

    public bool breakLinken()
    {
        if (!canBlock)
        {
            return false;
        }
        canBlock = false;
        timer = coolDown;
        linkenObj.SetActive(false);
        return true;
    }

    void _OnSnap(SnapEvent e)
    {
        if (timer != 0)
        {
            timer -= 1;
        }
        else
        {
            if (!canBlock)
            {
                canBlock = true;
                linkenObj.SetActive(true);
            }
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
