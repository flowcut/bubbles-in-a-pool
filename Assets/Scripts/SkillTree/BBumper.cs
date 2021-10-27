using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBumper : Skill
{
    public int level = 0;
    public int[] healthDeduction = new int[3] { -15, -15, -15 };
    public float forceMagnitude = 50f;
    public int HealthDeduction { get { return healthDeduction[level]; } }
    public SideName side;


    // Start is called before the first frame update
    public GameObject[] bumpers = new GameObject[5];
    public static void Register(SideName side)
    {
        if (!GameManager.Instance.Arena.GetComponent<BBumper>().enabled)
        {
            GameManager.Instance.Arena.GetComponent<BBumper>().side = side;
            GameManager.Instance.Arena.GetComponent<BBumper>().enabled = true;

        }
        else
        {
            GameManager.Instance.Arena.GetComponent<BBumper>().Upgrade();
        }
    }

    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        foreach (var bumper in bumpers)
        {
            bumper.SetActive(false);
        }
        level = 0;
        GameManager.Instance.Arena.GetComponent<BBumper>().enabled = false;
    }

    public void Upgrade()
    {
        level += 1;
        switch (level)
        {
            case 1:
                bumpers[1].SetActive(true);
                bumpers[2].SetActive(true);

                break;
            case 2:
                bumpers[3].SetActive(true);
                bumpers[4].SetActive(true);

                break;
        }

    }

    public void OnEnable()
    {
        foreach (var bumper in bumpers)
        {
            bumper.GetComponent<HasSide>().side = side;
            bumper.GetComponent<Painter>().TakeEffect();
        }
        bumpers[0].SetActive(true);
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