using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockController : MonoBehaviour
{
    Subscription<RestartEvent> restartEventSubscription;
    Subscription<RestartFinishEvent> restartFinishEventSubscription;
    Subscription<ClockStopEvent> clockStopEventSubscription;

    public int current_snap = -1;
    private bool going = true;
    void Start()
    {
        restartEventSubscription = EventBus.Subscribe<RestartEvent>(_OnRestart);
        restartFinishEventSubscription = EventBus.Subscribe<RestartFinishEvent>(_OnRestartFinish);
        clockStopEventSubscription = EventBus.Subscribe<ClockStopEvent>(_OnClockStop);
        // StartCoroutine(ClockSnap());
    }

    public void StartClock()
    {
        StartCoroutine(ClockSnap());
    }

    private void _OnClockStop(ClockStopEvent e)
    {
        if (e.isStopped)
        {
            StopAllCoroutines();
            going = false;
        }
        else
        {
            if (!going)
            {
                StartCoroutine(ClockSnap());
                going = true;
            }            
        }
    }

    private void _OnRestart(RestartEvent e)
    {
        GameManager.state = GameState.RESTARTING;
        StopAllCoroutines();
        going = false;
        //StartCoroutine(RestartClock());
    }

    private void _OnRestartFinish(RestartFinishEvent e)
    {
        GameManager.state = GameState.RUNNING;
        current_snap = -1;
        if (!going)
        {
            StartCoroutine(ClockSnap());
            going = true;
        }
    }

    public IEnumerator ClockSnap()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameManager.snapTime);
            current_snap++;
            current_snap %= GameManager.clockNumSnap;
            EventBus.Publish<SnapEvent>(new SnapEvent(current_snap));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    { 
        EventBus.Unsubscribe(clockStopEventSubscription);
        EventBus.Unsubscribe(restartEventSubscription);
        EventBus.Unsubscribe(restartFinishEventSubscription);
        
    }
}

public class SnapEvent
{
    public int current_snap;

    public SnapEvent(int _current_snap) { current_snap = _current_snap; }

    public override string ToString()
    {
        return "#Snap : " + current_snap;
    }
}

