using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceLineButton : MonoBehaviour
{
    Subscription<SnapEvent> snapEventSubscription;
    public GameObject ActivePowerController;

    // Start is called before the first frame update
    void Start()
    {
        snapEventSubscription = EventBus.Subscribe<SnapEvent>(_OnSnap);
    }

    void _OnSnap(SnapEvent e)
    {
        if (e.current_snap == 0)
        {
            GetComponent<Button>().interactable = true;
        }
    }
    public void OnClick()
    {
        // Debug.Log("Clicked");
        ActivePowerController.GetComponent<ForceLineController>().enabled = true;
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
