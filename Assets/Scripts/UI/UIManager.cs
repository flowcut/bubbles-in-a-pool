using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    Subscription<RestartEvent> restartEventSubscription;

    public static float restartTime = 1.0f;

    public GameObject blurBackground;
    public GameObject inGameUI;
    public GameObject activePowerUI;
    public GameObject blackCurtain;
    public GameObject highlightCurtatin;
    public Canvas canvas;
    public GameObject standardSkillToggle;
    public GameObject skillSelectionSubmissionButton;
    public ToggleGroup skillSelectionToggleGroup;
    public GameObject descriptionTrunk;
    public GameObject pausePanel;
    public GameObject Tutorial;
    public GameObject Logo;
    public GameObject LogoBlackCurtain;

    public GameObject blackDisplay;
    public GameObject redDisplay;

    public Dictionary<SideName, GameObject> sideDisplay = new Dictionary<SideName, GameObject>();
    public Dictionary<SideName, List<GameObject>> sideSkillDisplayList = new Dictionary<SideName, List<GameObject>>();

    public Dictionary<SideName, Vector3> sideSkillInitialPos = new Dictionary<SideName, Vector3>();

    public GameObject skillDisplay;
    public GameObject levelDisplay;

    private GameObject[] icons;

    Dictionary<int, List<Vector3>> IconPosition = new Dictionary<int, List<Vector3>>()
    {
        { 1, new List<Vector3> {
            new Vector3(0, 100, 0) } },
        { 2, new List<Vector3> {
            new Vector3(-200, 100, 0),
            new Vector3(200, 100, 0) } },
        { 3, new List<Vector3> {
            new Vector3(-380, 100, 0),
            new Vector3(0, 100, 0),
            new Vector3(380, 100, 0) } },
    };

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        restartEventSubscription = EventBus.Subscribe<RestartEvent>(_OnRestart);

        sideDisplay[SideName.BLACK] = blackDisplay;
        sideDisplay[SideName.RED] = redDisplay;
        sideSkillDisplayList[SideName.BLACK] = new List<GameObject>();
        sideSkillDisplayList[SideName.RED] = new List<GameObject>();
        sideSkillInitialPos[SideName.BLACK] = new Vector3(9, -190, 0);
        sideSkillInitialPos[SideName.RED] = new Vector3(-9, -190, 0);
    }

    void _OnRestart(RestartEvent e)
    {
        StartCoroutine(CleanDisplay());
    }

    IEnumerator CleanDisplay()
    {
        foreach (var display in sideSkillDisplayList[SideName.BLACK])
        {
            StartCoroutine(Fade(display, duration: restartTime, fadeIn: false));
        }

        foreach (var display in sideSkillDisplayList[SideName.RED])
        {
            StartCoroutine(Fade(display, duration: restartTime, fadeIn: false));
        }

        yield return new WaitForSeconds(restartTime + 1f);

        foreach (var display in sideSkillDisplayList[SideName.BLACK])
        {
            Destroy(display);
        }

        foreach (var display in sideSkillDisplayList[SideName.RED])
        {
            Destroy(display);
        }

        sideSkillDisplayList[SideName.BLACK].Clear();
        sideSkillDisplayList[SideName.RED].Clear();
    }

    public IEnumerator SkillSelectionPanel(List<string> pushingNodesName, System.Action<string> callback)
    {
        GameManager.state = GameState.PAUSED;
        inGameUI.GetComponent<CanvasGroup>().interactable = false;
        activePowerUI.GetComponent<CanvasGroup>().interactable = false;
        pausePanel.GetComponent<CanvasGroup>().interactable = false;
        blurBackground.SetActive(true);
        blackCurtain.SetActive(true);
        Vector4 color = blackCurtain.GetComponent<Image>().color;
        color[3] = 55f / 255;
        blackCurtain.GetComponent<Image>().color = color;

        if (GameManager.Instance.ActivePowerController.GetComponent<ForceLineController>().enabled)
        {
            GameManager.Instance.ActivePowerController.GetComponent<ForceLineController>().canUse = false;
        }

        if (GameManager.Instance.ActivePowerController.GetComponent<EmperorController>().enabled)
        {
            GameManager.Instance.ActivePowerController.GetComponent<EmperorController>().canUse = false;
        }

        int numSelection = pushingNodesName.Count;
        icons = new GameObject[numSelection];
        GameObject[] descriptions = new GameObject[numSelection];
        for (int i = 0; i < numSelection; ++i)
        {
            icons[i] = Instantiate(standardSkillToggle,
                IconPosition[numSelection][i], Quaternion.identity);
            icons[i].transform.parent = canvas.transform;
            icons[i].GetComponent<RectTransform>().localPosition = IconPosition[numSelection][i];
            SkillTreeNode iconNode = SkillTree.Instance.FindNodeByName(pushingNodesName[i]);
            icons[i].GetComponent<SkillToggle>().SetSprite(Resources.Load<Sprite>(iconNode.iconPath));
            icons[i].GetComponent<SkillToggle>().submissionButton = skillSelectionSubmissionButton;
            icons[i].GetComponent<Toggle>().group = skillSelectionToggleGroup;

            icons[i].GetComponent<RectTransform>().sizeDelta = iconNode.iconSize;

            Vector3 newScale = new Vector3(iconNode.iconScale, iconNode.iconScale, 1);
            icons[i].GetComponent<RectTransform>().localScale = newScale;

            icons[i].GetComponent<SkillToggle>().selectionIndex = i;

            int level =
                SkillTree.Instance.activeNodesLevel[SideName.BLACK].ContainsKey(pushingNodesName[i]) ?
                SkillTree.Instance.activeNodesLevel[SideName.BLACK][pushingNodesName[i]] + 1 :
                0;

            if (level != 0)
            {
                icons[i].GetComponent<SkillToggle>().Upgrade();
            }

            descriptions[i] = Instantiate(descriptionTrunk);
            descriptions[i].transform.SetParent(canvas.transform);
            descriptions[i].GetComponent<RectTransform>().localPosition =
                IconPosition[numSelection][i];
            descriptions[i].GetComponent<DescriptionText>().nameText.text =
                iconNode.name;
            if (iconNode.maxLevel != 1)
            {
                descriptions[i].GetComponent<DescriptionText>().nameText.text +=
                    " Lv." + (level + 1).ToString();
            }
            descriptions[i].GetComponent<DescriptionText>().descriptionText.text =
                iconNode.description[level];
        }

        icons[0].GetComponent<Toggle>().isOn = true;

        skillSelectionSubmissionButton.SetActive(true);

        while (!skillSelectionSubmissionButton.GetComponent<SelectionSubmit>().selected)
        {
            yield return null;
        }

        int selectedIndex = skillSelectionSubmissionButton.GetComponent<SelectionSubmit>().selectionIndex;

        for (int i = 0; i < numSelection; ++i)
        {
            Destroy(icons[i]);
            Destroy(descriptions[i]);
        }

        if (GameManager.Instance.ActivePowerController.GetComponent<ForceLineController>().enabled)
        {
            GameManager.Instance.ActivePowerController.GetComponent<ForceLineController>().canUse = true;
        }

        if (GameManager.Instance.ActivePowerController.GetComponent<EmperorController>().enabled)
        {
            GameManager.Instance.ActivePowerController.GetComponent<EmperorController>().canUse = true;
        }

        skillSelectionSubmissionButton.SetActive(false);
        color[3] = 1.0f;
        blackCurtain.GetComponent<Image>().color = color;
        blackCurtain.SetActive(false);
        blurBackground.SetActive(false);
        inGameUI.GetComponent<CanvasGroup>().interactable = true;
        activePowerUI.GetComponent<CanvasGroup>().interactable = true;
        pausePanel.GetComponent<CanvasGroup>().interactable = true;
        GameManager.state = GameState.RUNNING;
        yield return null;
        callback(pushingNodesName[selectedIndex]);
    }

    public void DisplayNewSkill(SideName side, string newNodeName)
    {
        GameObject newDisplay = Instantiate(skillDisplay);
        newDisplay.transform.parent = sideDisplay[side].transform;

        SkillTreeNode node = SkillTree.Instance.FindNodeByName(newNodeName);

        newDisplay.GetComponent<RectTransform>().localPosition =
            sideSkillInitialPos[side] + new Vector3(0, -80 * sideSkillDisplayList[side].Count, 0);
        newDisplay.GetComponent<Image>().sprite = 
            Resources.Load<Sprite>(node.iconPath);
        newDisplay.GetComponent<SkillDisplayInfo>().skillName = node.name;
        newDisplay.GetComponent<SkillDisplayInfo>().skillDescription = node.description[0];
        if (node.maxLevel > 1)
        {
            newDisplay.GetComponent<SkillDisplayInfo>().levelDisplay.GetComponent<SkillLevelDisplay>().LevelUp();
        }        
        // newDisplay.GetComponent<SkillDisplay>().
        Vector4 color = GameManager.sideColorOffset[side] / 255f;
        newDisplay.GetComponent<Image>().color = color;
        newDisplay.GetComponent<SkillDisplayInfo>().levelDisplay.GetComponent<SkillLevelDisplay>().SetColor(color);
        sideSkillDisplayList[side].Add(newDisplay);
    }

    public void DisplaySkillUpgrade(SideName side, string nodeName)
    {
        foreach (var display in sideSkillDisplayList[side])
        {
            if (display.GetComponent<SkillDisplayInfo>().skillName ==
                nodeName)
            {
                display.GetComponent<SkillDisplayInfo>().levelDisplay.GetComponent<SkillLevelDisplay>().LevelUp();
                int level = display.GetComponent<SkillDisplayInfo>().levelDisplay.GetComponent<SkillLevelDisplay>().level - 1;
                display.GetComponent<SkillDisplayInfo>().skillDescription =
                    SkillTree.Instance.FindNodeByName(nodeName).description[level];
                break;
            }
        }
    }

    public static IEnumerator Fade(GameObject target, float duration, bool fadeIn = true)
    {
        Color color = target.GetComponent<Image>().color;
        float timer = 0.0f;
        Color newColor;
        while (timer < duration)
        {
            newColor = color;
            float factor = fadeIn ? timer / duration : 1 - timer / duration;
            newColor[3] *= factor;
            target.GetComponent<Image>().color = newColor;
            timer += Time.deltaTime;
            yield return null;
        }
        newColor = color;
        newColor[3] = fadeIn ? color[3] : 0;
        target.GetComponent<Image>().color = newColor;
    }

    public IEnumerator ShowLogo()
    {
        LogoBlackCurtain.SetActive(true);
        Logo.SetActive(true);
        yield return Fade(Logo, 1, true);
        yield return new WaitForSeconds(1);
        yield return Fade(Logo, 1, false);
        Logo.SetActive(false);
        yield return Fade(LogoBlackCurtain, 1, false);
        LogoBlackCurtain.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
