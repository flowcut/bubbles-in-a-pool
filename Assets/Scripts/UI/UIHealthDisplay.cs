using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthDisplay : MonoBehaviour
{
    Subscription<SideHealthUpdateEvent> sideHealthUpdateEventSubscription;

    public Text RedHealthDisplay;
    public Text BlackHealthDisplay;

    private Dictionary<SideName, Text> sideHealthDisplay = new Dictionary<SideName, Text>();
    // Start is called before the first frame update
    void Start()
    {
        sideHealthDisplay[SideName.BLACK] = BlackHealthDisplay;
        sideHealthDisplay[SideName.RED] = RedHealthDisplay;
        sideHealthUpdateEventSubscription = EventBus.Subscribe<SideHealthUpdateEvent>(_OnHealthUpdate);
    }

    void _OnHealthUpdate(SideHealthUpdateEvent e)
    {
        sideHealthDisplay[e.side].text = GameManager.sideCurrentHealth[e.side].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(sideHealthUpdateEventSubscription);
    }

}
