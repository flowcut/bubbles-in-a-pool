using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    public int numRestartGo = 360;
    public GameObject PausePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnRestartClick()
    {
        EventBus.Publish<RestartEvent>(new RestartEvent());
        //GameManager.state = GameState.RESTARTING;
        StartCoroutine(PausePanel.GetComponent<PausePanelControl>().Toggle(true));
        StartCoroutine(RestartButtonReset());
    }

    IEnumerator RestartButtonReset()
    {
        GetComponent<Button>().interactable = false;
        
        RectTransform tf = GetComponent<RectTransform>();
        for (int i = 0; i < numRestartGo; ++i)
        {
            yield return new WaitForSecondsRealtime(UIManager.restartTime / numRestartGo);
            tf.Rotate(0, 0, -360f / numRestartGo);
        }
        GetComponent<Button>().interactable = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}

public class RestartEvent
{
    public RestartEvent() { }

    public override string ToString()
    {
        return "Restart.";
    }
}
