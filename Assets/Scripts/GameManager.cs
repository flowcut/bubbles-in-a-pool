using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SideName { BLACK, RED, CYAN }
public enum GameState { RUNNING, PAUSED, RESTARTING }

public class GameManager : Singleton<GameManager>
{
    Subscription<SnapEvent> snapEventSubscription;
    Subscription<TimeScaleEvent> timeScaleEventSubscription;
    Subscription<RestartEvent> restartEventSubscription;

    // ======== Side Parameters ========   
    public static Dictionary<SideName, SideName> otherSide =
        new Dictionary<SideName, SideName>
        {
            { SideName.BLACK, SideName.RED },
            { SideName.RED, SideName.BLACK },
        };

    public static Dictionary<SideName, Vector4> sideColorOffset =
        new Dictionary<SideName, Vector4> 
        {
            { SideName.BLACK, new Vector4(43f, 43f, 43f, 255f) },
            { SideName.RED, new Vector4(191f, 37f, 37f, 255f) },
            { SideName.CYAN, new Vector4(0f, 180f, 180f, 255f)}
        };

    public static Dictionary<SideName, int> sideNumLivingBubble =
        new Dictionary<SideName, int> 
        {
            { SideName.BLACK, 0 },
            { SideName.RED, 0 },
        };

    public static Dictionary<SideName, int> sideNumSpawningBubble =
        new Dictionary<SideName, int>
        {
            { SideName.BLACK, 0 },
            { SideName.RED, 0 },
        };

    public static Dictionary<SideName, int> sideMaximumHealth =
        new Dictionary<SideName, int>
        {
            { SideName.BLACK, 30 },
            { SideName.RED, 30 },
        };

    public static Dictionary<SideName, int> sideCurrentHealth =
        new Dictionary<SideName, int>
        {
            { SideName.BLACK, 30 },
            { SideName.RED, 30 },
        };

    public static Dictionary<SideName, Vector3> sideHealthPos =
        new Dictionary<SideName, Vector3>
        {
            // { SideName.BLACK, new Vector3 (-155f, 110f, 0f) },
            // { SideName.RED, new Vector3 (155f, 110f, 0f) },
        };

    public static SideName defaultSide = SideName.BLACK;

    // ======== Arena Parameters ========
    public static float arenaInnerRadius = 77f;
    public static float arenaOuterRadius = 137f;
    public static float arenaMaxForce = 160.0f;

    // ======== Bubble Parameters ========    
    public static float bubbleStandardRadius = 4.0f;
    public static float bubbleStandardMass = 1.0f;

    public static int bubbleStandardMaxHealth = 100;
    public static int bubbleStandardHitDamage = -20;
    public static float bubbleStandardHitForce = 100f;

    public static float bubbleStandardColliderRadius = 4.6f;

    // ======== Clock Parameters ========   
    public static float snapTime = 2.0f;
    public static int clockNumSnap = 24;

    public float fixedDeltaTime; // IMPORTANT!!!
    public GameObject clock;

    // ======== Skill Selection Parameters ========   
    public static Dictionary<SideName, int> sideNumSelection =
        new Dictionary<SideName, int>
        {
            { SideName.BLACK, 3 },
            { SideName.RED, 3 },
        };

    // ======== Spawner Info ========
    public GameObject BlackSpawner;
    public GameObject RedSpawner;

    public Dictionary<SideName, GameObject> sideSpawner = new Dictionary<SideName, GameObject>();

    // ======== Active Power ========
    public GameObject ActivePowerController;

    // ======== Global Game Info ========
    public static int round = 0;
    public GameObject Arena;
    public static GameState state = GameState.RUNNING;


    // ======== Effect Info ========
    public GameObject bubbleBurstParticle;

    public void GameStart()
    {
        state = GameState.RUNNING;
        clock.GetComponent<ClockController>().StartClock();
    }

    void Start()
    {
        state = GameState.PAUSED;
        fixedDeltaTime = Time.fixedDeltaTime;
        DontDestroyOnLoad(gameObject);
        snapEventSubscription = EventBus.Subscribe<SnapEvent>(_OnSnap);
        timeScaleEventSubscription = EventBus.Subscribe<TimeScaleEvent>(_OnTimeScaleChange);
        restartEventSubscription = EventBus.Subscribe<RestartEvent>(_OnRestart);

        sideCurrentHealth[SideName.BLACK] = sideMaximumHealth[SideName.BLACK];
        sideCurrentHealth[SideName.RED] = sideMaximumHealth[SideName.RED];

        sideSpawner[SideName.BLACK] = BlackSpawner;
        sideSpawner[SideName.RED] = RedSpawner;

        var upperLeftScreen = new Vector3(0, Screen.height, 0);
        var upperRightScreen = new Vector3(Screen.width, Screen.height, 0);


        sideHealthPos[SideName.BLACK] = 
            Camera.main.ScreenToWorldPoint(upperLeftScreen)
            + new Vector3(45, -45, 10);

        sideHealthPos[SideName.RED] =
            Camera.main.ScreenToWorldPoint(upperRightScreen)
            + new Vector3(-45, -45, 10);

        StartCoroutine(UIManager.Instance.ShowLogo());

        // Debug.Log(sideHealthPos[SideName.BLACK].ToString());

        // EventBus.Publish<SideHealthUpdateEvent>(new SideHealthUpdateEvent(SideName.BLACK));
        // EventBus.Publish<SideHealthUpdateEvent>(new SideHealthUpdateEvent(SideName.RED));
    }

    void _OnRestart(RestartEvent e)
    {       
        round = 0;
        // sideCurrentHealth[SideName.BLACK] = sideMaximumHealth[SideName.BLACK];
        // sideCurrentHealth[SideName.RED] = sideMaximumHealth[SideName.RED];
        StartCoroutine(GlobalHealthRegen());
       
        // EventBus.Publish<SideHealthUpdateEvent>(new SideHealthUpdateEvent(SideName.BLACK));
        // EventBus.Publish<SideHealthUpdateEvent>(new SideHealthUpdateEvent(SideName.RED));
    }

    IEnumerator GlobalHealthRegen()
    {        
        while (sideCurrentHealth[SideName.BLACK] != sideMaximumHealth[SideName.BLACK]
            || sideCurrentHealth[SideName.RED] != sideMaximumHealth[SideName.RED])
        {
            if (sideCurrentHealth[SideName.BLACK] != sideMaximumHealth[SideName.BLACK])
            {
                sideCurrentHealth[SideName.BLACK] += 1;
            }
            if (sideCurrentHealth[SideName.RED] != sideMaximumHealth[SideName.RED])
            {
                sideCurrentHealth[SideName.RED] += 1;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    void _OnSnap(SnapEvent e)
    {
        if (e.current_snap == clockNumSnap - 1)
        {
            StartCoroutine(ArenaGarbageCollection());
            round++;
        }
    }

    void _OnTimeScaleChange(TimeScaleEvent e)
    {
        Time.timeScale = e.current_timescale;
        Time.fixedDeltaTime = fixedDeltaTime * e.current_timescale;
    }

    private IEnumerator ArenaGarbageCollection()
    {
        EventBus.Publish<ClockStopEvent>(new ClockStopEvent(_isStopped: true));
        UIManager.Instance.inGameUI.GetComponent<CanvasGroup>().interactable = false;
        UIManager.Instance.pausePanel.GetComponent<CanvasGroup>().interactable = false;
        Dictionary<SideName, List<GameObject>> remainingBubble =
            new Dictionary<SideName, List<GameObject>>
            {
                { SideName.BLACK, new List<GameObject>() },
                { SideName.RED, new List<GameObject>() },
            };
        //var bubbleList = ;
        foreach (var bubble in FindObjectsOfType<BubbleController>())
        {
            remainingBubble[bubble.GetComponent<HasSide>().side].Add(bubble.gameObject);
            bubble.GetComponent<CircleCollider2D>().enabled = false;
            bubble.GetComponent<Rigidbody2D>().gravityScale = 0f;
            bubble.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            bubble.GetComponent<Rigidbody2D>().drag = 0f;
            bubble.GetComponent<BubbleEnvironmentalForce>().enabled = false;
            // Destroy(bubble.gameObject);
            // yield return null;
        }

        Vector3 center = new Vector3(0, 0, 0);
        float flyTime = 1f;
        while (remainingBubble[SideName.BLACK].Count != 0 &&
            remainingBubble[SideName.RED].Count != 0)
        {
            GameObject blackTieBubble = remainingBubble[SideName.BLACK][0];
            GameObject redTieBubble = remainingBubble[SideName.RED][0];

            remainingBubble[SideName.BLACK].Remove(blackTieBubble);
            remainingBubble[SideName.RED].Remove(redTieBubble);


            StartCoroutine(shootBubble(blackTieBubble, center, flyTime, 0));
            StartCoroutine(shootBubble(redTieBubble, center, flyTime, 0));
            yield return new WaitForSeconds(0.5f);
        }
        
        int diff = remainingBubble[SideName.BLACK].Count - remainingBubble[SideName.RED].Count;

        if (diff == 0)
        {
            EventBus.Publish<ClockStopEvent>(new ClockStopEvent(_isStopped: false));
            UIManager.Instance.inGameUI.GetComponent<CanvasGroup>().interactable = true;
            UIManager.Instance.pausePanel.GetComponent<CanvasGroup>().interactable = true;
            yield break;
        }

        yield return new WaitForSeconds(1f);
        

        SideName remainingSide = diff < 0 ? SideName.RED : SideName.BLACK;

        foreach (var bubble in remainingBubble[remainingSide])
        {
            StartCoroutine(shootBubble(bubble, 
                sideHealthPos[otherSide[remainingSide]], 
                flyTime,
                bubble.GetComponent<BubbleController>().globalDamage));
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(flyTime);
        UIManager.Instance.inGameUI.GetComponent<CanvasGroup>().interactable = true;
        UIManager.Instance.pausePanel.GetComponent<CanvasGroup>().interactable = true;
        EventBus.Publish<ClockStopEvent>(new ClockStopEvent(_isStopped: false));
    }

    IEnumerator shootBubble(GameObject bubble, Vector3 dest, float duration, int healthDeduction)
    {
        SideName side = bubble.GetComponent<HasSide>().side;
        Vector3 distance = dest - bubble.transform.position;
        Vector3 direction = distance.normalized;
        Vector3 acceleration = 2 * distance / duration / duration;
        Acceleration acc =  bubble.AddComponent<Acceleration>();
        acc.acceleration = acceleration;
        yield return new WaitForSeconds(duration);
        if (healthDeduction != 0)
        {
            SideName lossSide = otherSide[side];
            sideCurrentHealth[lossSide] -= healthDeduction;
        }
        Destroy(bubble);
        GameObject particleObj = Instantiate(bubbleBurstParticle, dest, Quaternion.identity);
        particleObj.transform.up = -direction;
        particleObj.GetComponent<HasSide>().side = side;
        particleObj.GetComponent<ParticleSetSide>().SetSide();

        if (healthDeduction != 0)
        {
            particleObj.GetComponent<ParticleSystem>().startLifetime *= 2;
        }
        

        yield return new WaitForSeconds(particleObj.GetComponent<ParticleSystem>().main.startLifetimeMultiplier + 1f);
        Destroy(particleObj);
    }

    // Update is called once per frame
    void Update()
    {
        if (sideCurrentHealth[SideName.BLACK] <= 0 ||
            sideCurrentHealth[SideName.RED] <= 0)
        {
            EventBus.Publish<RestartEvent>(new RestartEvent());
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(snapEventSubscription);
        EventBus.Unsubscribe(timeScaleEventSubscription);
    }
}

public class SideHealthUpdateEvent
{
    public SideName side;

    public SideHealthUpdateEvent(SideName _side) { side = _side; }

    public override string ToString()
    {
        return "Side " + side.ToString() + " Health Updated";
    }
}

public class ClockStopEvent 
{
    public bool isStopped;
    public ClockStopEvent(bool _isStopped) { isStopped = _isStopped; }
}


