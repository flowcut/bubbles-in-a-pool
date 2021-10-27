using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    Subscription<RestartEvent> restartEventSubscription;

    public virtual void _OnRestart(RestartEvent e)
    {

    }
    // Start is called before the first frame update
    public virtual void Start()
    {
        restartEventSubscription = EventBus.Subscribe<RestartEvent>(_OnRestart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnDestroy()
    {
        EventBus.Unsubscribe(restartEventSubscription);
    }
}
