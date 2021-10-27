using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockDisplay : MonoBehaviour
{
    public GameObject snapHand;
    public int numSnapGo = 10;
    public float timeSnapGo = 0.1f;
    public float current_degree = 0;

    public int numRestoreGo = 180;
    public int currentSnap = 0;

    Subscription<SnapEvent> snapEventSubscription;
    Subscription<RestartEvent> restartEventSubscription;
    void Start()
    {
        snapEventSubscription = EventBus.Subscribe<SnapEvent>(_OnSnap);
        restartEventSubscription = EventBus.Subscribe<RestartEvent>(_OnRestart);
    }

    void _OnRestart(RestartEvent e)
    {
        StopAllCoroutines();
        StartCoroutine(HandRestore());
    }

    IEnumerator HandRestore()
    {
        if (currentSnap != GameManager.clockNumSnap - 1)
        {
            for (int i = 0; i < numRestoreGo; ++i)
            {
                yield return new WaitForSeconds(UIManager.restartTime / numRestoreGo / 2);
                Vector3 hand = snapHand.transform.localPosition;
                hand = Quaternion.Euler(0, 0, -current_degree / numRestoreGo) * hand;
                snapHand.transform.Rotate(0, 0, -current_degree / numRestoreGo);
                snapHand.transform.localPosition = hand;
            }            
        }
        current_degree = 0;
        EventBus.Publish<RestartFinishEvent>(new RestartFinishEvent());
    }

    void _OnSnap(SnapEvent e)
    {
        currentSnap = e.current_snap;
        StartCoroutine(HandGo());
    }

    IEnumerator HandGo()
    {
        for (int i = 0; i < numSnapGo; ++i)
        {
            yield return new WaitForSeconds(timeSnapGo / numSnapGo);
            Vector3 hand = snapHand.transform.localPosition;
            hand = Quaternion.Euler(0, 0, -360f / GameManager.clockNumSnap / numSnapGo) * hand;
            snapHand.transform.Rotate(0, 0, -360f / GameManager.clockNumSnap / numSnapGo);
            current_degree += -360f / GameManager.clockNumSnap / numSnapGo;
            if (current_degree < -360)
            {
                current_degree += 360;
            }
            snapHand.transform.localPosition = hand;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(snapEventSubscription);
        EventBus.Unsubscribe(restartEventSubscription);
    }
}

public class RestartFinishEvent
{
    public RestartFinishEvent() { }

    public override string ToString()
    {
        return "Restart finished.";
    }
}