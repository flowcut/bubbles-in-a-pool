using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaleChange : MonoBehaviour
{
    Subscription<RestartEvent> restartEventSubscription;

    public int timeScale;

    private bool on;
    // Start is called before the first frame update
    void Start()
    {
        restartEventSubscription = EventBus.Subscribe<RestartEvent>(_OnRestart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void _OnRestart(RestartEvent e)
    {
        /*if (timeScale == 1)
        {
            EventBus.Publish<TimeScaleEvent>(new TimeScaleEvent(timeScale));
            GetComponent<Toggle>().isOn = true;
        }*/
    }

    public void onChange()
    {
        if (GetComponent<Toggle>())
        {
            on = GetComponent<Toggle>().isOn;
            EventBus.Publish<TimeScaleEvent>(new TimeScaleEvent(timeScale));
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(restartEventSubscription);
    }
}

public class TimeScaleEvent
{
    public int current_timescale;

    public TimeScaleEvent(int _current_timescale) { current_timescale = _current_timescale; }

    public override string ToString()
    {
        return "Timescale : " + current_timescale;
    }
}