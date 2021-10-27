using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BKicker : Skill
{
    public int level = 0;
    public int[] healthRegen = new int[3] { 30, 40, 50 };
    public float forceMagnitude = 200f;
    public int HealthRegen { get { return healthRegen[level];  } }
    public SideName side;


    // Start is called before the first frame update
    public GameObject[] kickers = new GameObject[5];
    public static void Register(SideName side)
    {
        if (!GameManager.Instance.Arena.GetComponent<BKicker>().enabled)
        {
            GameManager.Instance.Arena.GetComponent<BKicker>().side = side;
            GameManager.Instance.Arena.GetComponent<BKicker>().enabled = true;
            
        }
        else
        {
            GameManager.Instance.Arena.GetComponent<BKicker>().Upgrade();
        }
    }

    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        foreach (var kicker in kickers)
        {
            kicker.SetActive(false);
        }
        level = 0;
        GameManager.Instance.Arena.GetComponent<BKicker>().enabled = false;
    }

    public void Upgrade()
    {
        level += 1;
        switch (level)
        {
            case 1:
                kickers[2].SetActive(true);
                kickers[3].SetActive(true);
                break;
            case 2:
                kickers[0].SetActive(false);
                kickers[1].SetActive(false);
                kickers[2].SetActive(false);
                kickers[3].SetActive(false);
                kickers[4].SetActive(true);
                break;
        }
            
    }

    public void OnEnable()
    {
        foreach (var kicker in kickers)
        {
            kicker.GetComponent<HasSide>().side = side;
            kicker.GetComponent<Painter>().TakeEffect();
        }
        kickers[0].SetActive(true);
        kickers[1].SetActive(true);
    }

    public override void Start()
    {
        base.Start();
        level = 0;        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
