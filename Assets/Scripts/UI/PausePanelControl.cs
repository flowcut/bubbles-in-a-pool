using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanelControl : MonoBehaviour
{
    // Start is called before the first frame update
    public bool controlEnable = true;
    public bool off = true;
    public float smoothTime = 0.3f;

    Vector3 offPos;
    Vector3 OnPos = new Vector3 (720, 534, 0);
    private Vector3 velocity = Vector3.zero;
    int previous_timeScale;
    public GameState previousState;
    public GameObject PanelElements;
    void Start()
    {
        controlEnable = true;
        off = true;
        PanelElements.GetComponent<CanvasGroup>().interactable = false;
        offPos = gameObject.GetComponent<RectTransform>().position;
        Debug.Log("Panel Pos:" + offPos.ToString());
    }

    public IEnumerator Toggle(bool restart = false)
    {
        if (!controlEnable)
        {
            yield break;
        }

        if (!off)
        {
            PanelElements.GetComponent<CanvasGroup>().interactable = false;
        }
        else
        {
            previousState = GameManager.state;
            GameManager.state = GameState.PAUSED;
            previous_timeScale = (int)Time.timeScale;
            EventBus.Publish<TimeScaleEvent>(new TimeScaleEvent(0));
            EventBus.Publish<ClockStopEvent>(new ClockStopEvent(_isStopped: true));
            UIManager.Instance.inGameUI.GetComponent<CanvasGroup>().interactable = false;
        }

        controlEnable = false;
        Vector3 targetPosition = off ? OnPos : offPos;
        float timer = 0.0f;
        while (timer < smoothTime + 1f)
        {
            gameObject.GetComponent<RectTransform>().position =
            Vector3.SmoothDamp(
                gameObject.GetComponent<RectTransform>().position,
                targetPosition, ref velocity,
                smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        controlEnable = true;
        off = !off;
        if (!off)
        {
            PanelElements.GetComponent<CanvasGroup>().interactable = true;
        }
        else
        {
            EventBus.Publish<TimeScaleEvent>(new TimeScaleEvent(previous_timeScale));
            if (previousState != GameState.RESTARTING && !restart)                
            {
                EventBus.Publish<ClockStopEvent>(new ClockStopEvent(_isStopped: false));
            }
            if (restart)
            {
                GameManager.state = GameState.RESTARTING;
            }
            else
            {
                GameManager.state = previousState;
            }
            
            UIManager.Instance.inGameUI.GetComponent<CanvasGroup>().interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
