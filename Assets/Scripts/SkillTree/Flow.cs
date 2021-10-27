using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flow : Skill
{
    // public GameObject forcelineButton;
    public int level = 0;
    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        // forcelineButton.SetActive(false);
        GameManager.Instance.ActivePowerController.GetComponent<ForceLineController>().enabled = false;
        GameManager.Instance.ActivePowerController.GetComponent<Flow>().enabled = false;
        //Destroy(gameObject.GetComponent<Bould>());
    }

    public static void Register(SideName side)
    {
        if (!GameManager.Instance.ActivePowerController.GetComponent<Flow>().enabled)
        {
            // Debug.Log("here");
            GameManager.Instance.ActivePowerController.GetComponent<Flow>().enabled = true;
        }
        else
        {
            GameManager.Instance.ActivePowerController.GetComponent<Flow>().Upgrade();
        }
    }

    public void Upgrade()
    {
        level += 1;
    }

    private void OnEnable()
    {
        GameManager.Instance.ActivePowerController.GetComponent<ForceLineController>().enabled = true;
        // forcelineButton.SetActive(true);
        // StartCoroutine(Highlight());
        // StartCoroutine(Blink());
    }

    private IEnumerator Highlight()
    {
        UIManager.Instance.highlightCurtatin.SetActive(true);
        Color color = UIManager.Instance.highlightCurtatin.GetComponent<Image>().color;
        color[3] = 55f / 255;
        UIManager.Instance.highlightCurtatin.GetComponent<Image>().color = color;
        //Debug.Log("Timescale:" + Time.timeScale.ToString());
        yield return new WaitForSeconds(3f);
        
        yield return StartCoroutine(UIManager.Fade(UIManager.Instance.highlightCurtatin, 1.0f, false));
        UIManager.Instance.highlightCurtatin.GetComponent<Image>().color = color;
        UIManager.Instance.highlightCurtatin.SetActive(false);
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        // forcelineButton = FindObjectOfType<ForceLineButton>().gameObject;
        // forcelineButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
