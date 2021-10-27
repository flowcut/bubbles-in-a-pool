using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emperor : Skill
{

    public int level = 0;
    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        // forcelineButton.SetActive(false);
        GameManager.Instance.ActivePowerController.GetComponent<EmperorController>().enabled = false;
        GameManager.Instance.ActivePowerController.GetComponent<Emperor>().enabled = false;
        //Destroy(gameObject.GetComponent<Bould>());
    }

    public static void Register(SideName side)
    {
        if (!GameManager.Instance.ActivePowerController.GetComponent<Emperor>().enabled)
        {
            // Debug.Log("here");
            GameManager.Instance.ActivePowerController.GetComponent<Emperor>().enabled = true;
        }
        else
        {
            GameManager.Instance.ActivePowerController.GetComponent<Emperor>().Upgrade();
        }
    }

    public void Upgrade()
    {
        level += 1;
    }

    private void OnEnable()
    {
        GameManager.Instance.ActivePowerController.GetComponent<EmperorController>().enabled = true;
        // forcelineButton.SetActive(true);
        // StartCoroutine(Highlight());
        // StartCoroutine(Blink());
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
