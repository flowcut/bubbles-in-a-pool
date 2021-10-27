using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierType { SINGLE, GLOBAL, ENVIRONMENT, ESTABLISHMENT, ACTIVE};
public class SkillTreeNode
{
    public string name;
    public ModifierType type;

    public string iconPath;
    public float iconScale;
    public Vector2 iconSize; // Width, Height

    public List<string> description = new List<string>();

    public int maxLevel;

    public List<string> nextNodesName = new List<string>();

    public List<string> requiredNodesName = new List<string>();
    public List<string> excluedeNodesName = new List<string>();

    public delegate bool NodeRequirementDelegate(SideName side);
    public NodeRequirementDelegate CheckMethod;

    public delegate void NodeEffect(SideName side);
    public NodeEffect Register;

    private bool DefaultCheck(SideName side)
    {
        return true;
    }

    public SkillTreeNode(string _name, ModifierType _type) {
        name = _name;
        type = _type;
        CheckMethod = DefaultCheck;
    }

}
public class SkillTree : Singleton<SkillTree>
{
    Subscription<SnapEvent> snapEventSubscription;
    Subscription<RestartEvent> restartEventSubscription;

    public Dictionary<SideName, Dictionary<string, int>> activeNodesLevel =
        new Dictionary<SideName, Dictionary<string, int>>
        {
            { SideName.BLACK, new Dictionary<string, int>() },
            { SideName.RED, new Dictionary<string, int>() }
        };

    public Dictionary<SideName, List<string>> activeNodesName =
        new Dictionary<SideName, List<string>>
        {
            { SideName.BLACK, new List<string>() },
            { SideName.RED, new List<string>() }
        };

    public Dictionary<SideName, List<string>> waitingNodesName =
        new Dictionary<SideName, List<string>>
        {
            { SideName.BLACK, new List<string>() },
            { SideName.RED, new List<string>() }
        };
    // need to be checked before being activated
    public Dictionary<SideName, List<string>> excludedNodesName =
        new Dictionary<SideName, List<string>>
        {
            { SideName.BLACK, new List<string>() },
            { SideName.RED, new List<string>() }
        };

    private Dictionary<string, SkillTreeNode> nameToNode = new Dictionary<string, SkillTreeNode>();
    public List<SkillTreeNode> nodeCollection = new List<SkillTreeNode>();

    public SkillTreeNode FindNodeByName(string name)
    {
        if (nameToNode[name] != null)
        {
            return nameToNode[name];
        }
        else
        {
            Debug.Log("Node not exists");
            return null;
        }
    }

    private IEnumerator SelectSkill(SideName side)
    {

        List<string> qualifiedNodesName = new List<string>();
        foreach (string nodeName in waitingNodesName[side])
        {
            if (nameToNode[nodeName].CheckMethod(side))
            {
                qualifiedNodesName.Add(nodeName);
            }
        }
            

        if (qualifiedNodesName.Count == 0)
        {
            yield break;
        }
        List<string> pushingNodesName = new List<string>();

        pushingNodesName = qualifiedNodesName;
        while (pushingNodesName.Count > GameManager.sideNumSelection[side])
        {
            // Now No rarity setting
            pushingNodesName.Remove(pushingNodesName[Random.Range(0, pushingNodesName.Count)]);
        }

        string chosenNodeName = "";
        
        if (side == SideName.BLACK)
        {
            yield return UIManager.Instance.SkillSelectionPanel(pushingNodesName, 
                (returnNodeName) => { chosenNodeName = returnNodeName; });
        }
        else
        {
            // AI random selection
            chosenNodeName = pushingNodesName[Random.Range(0, pushingNodesName.Count)];
        }

        
        nameToNode[chosenNodeName].Register(side);
        
        // Debug.Log(side.ToString() + " " + nodeName);
        if (activeNodesName[side].Contains(chosenNodeName))
        {
            activeNodesLevel[side][chosenNodeName]++;
            UIManager.Instance.DisplaySkillUpgrade(side, chosenNodeName);
        }
        else
        {
            activeNodesName[side].Add(chosenNodeName);
            activeNodesLevel[side][chosenNodeName] = 0;
            UIManager.Instance.DisplayNewSkill(side, chosenNodeName);
            // waitingNodesName[side].Remove(chosenNodeName);
            if (nameToNode[chosenNodeName].excluedeNodesName.Count > 0)
            {
                foreach (var newNodeName in nameToNode[chosenNodeName].excluedeNodesName)
                {
                    excludedNodesName[side].Add(newNodeName);
                    waitingNodesName[side].Remove(newNodeName);
                }
            }
            if (nameToNode[chosenNodeName].nextNodesName.Count > 0)
            {
                foreach (var newNodeName in nameToNode[chosenNodeName].nextNodesName)
                {
                    if (!excludedNodesName[side].Contains(newNodeName))
                    {
                        waitingNodesName[side].Add(newNodeName);
                    }
                }
            }

            
        }
        
        if (activeNodesLevel[side][chosenNodeName] + 1 >= nameToNode[chosenNodeName].maxLevel)
        {
            waitingNodesName[side].Remove(chosenNodeName);
        }
    }

    public void _OnSnap(SnapEvent e)
    {
        if (e.current_snap == 0)
        {            
            StartCoroutine(SelectSkillPhase());                
        }
    }

    public void _OnRestart(RestartEvent e)
    {
        ResetSideSkillTree(SideName.BLACK);
        ResetSideSkillTree(SideName.RED);
    }

    private IEnumerator SelectSkillPhase()
    {
        int previous_timeScale = (int)Time.timeScale;
        EventBus.Publish<TimeScaleEvent>(new TimeScaleEvent(0));
        yield return SelectSkill(SideName.BLACK);
        yield return SelectSkill(SideName.RED);        
        EventBus.Publish<TimeScaleEvent>(new TimeScaleEvent(previous_timeScale));
    }

    public void ResetSideSkillTree(SideName side)
    {
        activeNodesName[side].Clear();
        activeNodesLevel[side].Clear();
        waitingNodesName[side].Clear();
        excludedNodesName[side].Clear();

        waitingNodesName[side].Add("First of All");
    }

    // Start is called before the first frame update
    void Start()
    {
        snapEventSubscription = EventBus.Subscribe<SnapEvent>(_OnSnap);
        restartEventSubscription = EventBus.Subscribe<RestartEvent>(_OnRestart);

        // ======== First of All ======== 
        SkillTreeNode firstBubble = new SkillTreeNode("First of All", ModifierType.GLOBAL);

        firstBubble.iconPath = "icon/1x/Initial Bubbles";
        firstBubble.iconScale = 7f;
        firstBubble.iconSize = new Vector2(30, 30);

        firstBubble.Register = FirstBubble.Register;
        firstBubble.description.Add("Adds 7 Bubbles into the pool. Bubbles attack enemies when the clock dial moves.");
        firstBubble.maxLevel = 1;

        firstBubble.nextNodesName.Add("Flow");
        firstBubble.nextNodesName.Add("Emperor's New Bubble");
        firstBubble.nextNodesName.Add("Fool's Boon");
        firstBubble.nextNodesName.Add("PinBubble Kicker");
        firstBubble.nextNodesName.Add("PinBubble Bumper");
        firstBubble.nextNodesName.Add("Linken's Bubble");
        firstBubble.nextNodesName.Add("Vicious");
        firstBubble.nextNodesName.Add("Saturn");

        nodeCollection.Add(firstBubble);
        nameToNode[firstBubble.name] = nodeCollection[nodeCollection.Count - 1];

        waitingNodesName[SideName.BLACK].Add(firstBubble.name);
        waitingNodesName[SideName.RED].Add(firstBubble.name);

        // ======== Pack Tactics ======== 
        SkillTreeNode packTactics = new SkillTreeNode("Pack Tactics", ModifierType.GLOBAL);
        packTactics.iconPath = "icon/1x/Pack Tactics";
        packTactics.iconScale = 8f;
        packTactics.iconSize = new Vector2(30, 30);

        packTactics.Register = PackTactics.Register;
        packTactics.description.Add("Makes bubbles weaker and smaller, but add 3 more bubbles.");
        packTactics.description.Add("Makes bubbles even weaker, but add 6 more bubbles.");
        packTactics.description.Add("Never seen bubbles this weak. Adds 9 more bubbles.");

        packTactics.maxLevel = 3;

        packTactics.excluedeNodesName.Add("Boulder");

        nodeCollection.Add(packTactics);
        nameToNode[packTactics.name] = nodeCollection[nodeCollection.Count - 1];

        // ======== Boulder ======== 
        SkillTreeNode bould = new SkillTreeNode("Boulder", ModifierType.GLOBAL);
        bould.iconPath = "icon/1x/Bould";
        bould.iconScale = 7f;
        bould.iconSize = new Vector2(30, 30);

        bould.Register = Bould.Register;
        bould.description.Add("Makes bubbles stronger and larger.");
        bould.description.Add("Makes bubbles even stronger.");
        bould.description.Add("David could have beaten Goliath quicker with these bubbles.");
        bould.maxLevel = 3;

        bould.excluedeNodesName.Add("Pack Tactics");

        nodeCollection.Add(bould);
        nameToNode[bould.name] = nodeCollection[nodeCollection.Count - 1];

        // ======== Hot Sp Ring ======== 
        SkillTreeNode hotSpRing = new SkillTreeNode("Hot Sp Ring", ModifierType.GLOBAL);
        hotSpRing.iconPath = "icon/1x/Hot Sp Ring";
        hotSpRing.iconScale = 7f;
        hotSpRing.iconSize = new Vector2(30, 30);

        hotSpRing.Register = HotSpRing.Register;
        hotSpRing.description.Add("Bubbles regenerate when bathed in the ring.");
        hotSpRing.description.Add("Bubbles regenerate faster when bathed in the ring.");
        hotSpRing.description.Add("Bathed in the ring feels like bathed in a pool of nectar.");
        hotSpRing.maxLevel = 3;        

        nodeCollection.Add(hotSpRing);
        nameToNode[hotSpRing.name] = nodeCollection[nodeCollection.Count - 1];

        // ======== Bistol ======== 
        SkillTreeNode bistol = new SkillTreeNode("Bistol", ModifierType.SINGLE);
        bistol.iconPath = "icon/1x/Bistol";
        bistol.iconScale = 7f;
        bistol.iconSize = new Vector2(30, 30);

        bistol.Register = Bistol.Register;
        bistol.description.Add("Some of bubbles can make a ranged attack.");
        bistol.description.Add("Most of bubbles can make a ranged attack.");
        bistol.description.Add("The Bubble says: Guns, lots of guns.");
        bistol.maxLevel = 3;

        bistol.CheckMethod += (side) => { return activeNodesName[side].Count >= 3; };

        nodeCollection.Add(bistol);
        nameToNode[bistol.name] = nodeCollection[nodeCollection.Count - 1];

        // ======== Flow ========  
        SkillTreeNode flow = new SkillTreeNode("Flow", ModifierType.ACTIVE);
        flow.iconPath = "icon/1x/Flow";
        flow.iconScale = 7f;
        flow.iconSize = new Vector2(30, 30);

        flow.Register = Flow.Register;
        flow.description.Add("Allows you to push bubbles.\n Drag your mouse and release.");
        flow.maxLevel = 1;
        flow.CheckMethod += (side) => { return side != SideName.RED; };
        // flow.CheckMethod += (side) => { return activeNodesName[side].Count == 1; };

        flow.nextNodesName.Add("Pack Tactics");
        flow.nextNodesName.Add("Boulder");
        flow.nextNodesName.Add("Hot Sp Ring");
        flow.nextNodesName.Add("Bistol");

        flow.excluedeNodesName.Add("Fool's Boon");
        flow.excluedeNodesName.Add("Emperor's New Bubble");

        nodeCollection.Add(flow);
        nameToNode[flow.name] = nodeCollection[nodeCollection.Count - 1];

        // ======== Fool's Boon ======== 
        SkillTreeNode foolsBoon = new SkillTreeNode("Fool's Boon", ModifierType.GLOBAL);
        foolsBoon.iconPath = "icon/1x/Fool's Boon";
        foolsBoon.iconScale = 7f;
        foolsBoon.iconSize = new Vector2(30, 30);

        foolsBoon.Register = FoolsBoon.Register;
        foolsBoon.description.Add("Adds 1 more bubble.");
        foolsBoon.maxLevel = 1;
        // foolsBoon.CheckMethod += (side) => { return side != SideName.BLACK; };

        foolsBoon.nextNodesName.Add("Pack Tactics");
        foolsBoon.nextNodesName.Add("Boulder");
        foolsBoon.nextNodesName.Add("Hot Sp Ring");
        foolsBoon.nextNodesName.Add("Bistol");

        foolsBoon.excluedeNodesName.Add("Flow");
        foolsBoon.excluedeNodesName.Add("Emperor's New Bubble");

        nodeCollection.Add(foolsBoon);
        nameToNode[foolsBoon.name] = nodeCollection[nodeCollection.Count - 1];

        // ======== Emperor's New Bubble ======== 
        SkillTreeNode emperor = new SkillTreeNode("Emperor's New Bubble", ModifierType.ACTIVE);
        emperor.iconPath = "icon/1x/Emperor's New Bubble";
        emperor.iconScale = 7f;
        emperor.iconSize = new Vector2(30, 30);

        emperor.Register = Emperor.Register;
        emperor.description.Add("Creates an invisible bubble where you press and hold your mouse.");
        emperor.maxLevel = 1;
        emperor.CheckMethod += (side) => { return side != SideName.RED; };
        // emperor.CheckMethod += (side) => { return activeNodesName[side].Count == 1; };

        emperor.nextNodesName.Add("Pack Tactics");
        emperor.nextNodesName.Add("Boulder");
        emperor.nextNodesName.Add("Hot Sp Ring");
        emperor.nextNodesName.Add("Bistol");

        emperor.excluedeNodesName.Add("Fool's Boon");
        emperor.excluedeNodesName.Add("Flow");

        nodeCollection.Add(emperor);
        nameToNode[emperor.name] = nodeCollection[nodeCollection.Count - 1];

        // ======== PinBubble Kicker ======== 
        SkillTreeNode bKicker = new SkillTreeNode("PinBubble Kicker", ModifierType.ESTABLISHMENT);
        bKicker.iconPath = "icon/1x/PinBubble Kicker";
        bKicker.iconScale = 7f;
        bKicker.iconSize = new Vector2(30, 30);

        bKicker.Register = BKicker.Register;
        bKicker.description.Add("Creates kickers outside the pool to bounce friendly bubbles back and heal them.");
        bKicker.description.Add("Creates more kickers and strength the healing effect.");
        bKicker.description.Add("It's cheating, isn't it?");
        bKicker.maxLevel = 3;
        bKicker.CheckMethod += (side) => 
        {
            bool f1 = GameManager.round > 4;
            bool f2 = activeNodesName[GameManager.otherSide[side]].Contains(bKicker.name);
            return f1 && (!f2);
        };

        // bKicker.nextNodesName.Add("Linken's Bubble");

        bKicker.excluedeNodesName.Add("PinBubble Bumper");
        nodeCollection.Add(bKicker);
        nameToNode[bKicker.name] = nodeCollection[nodeCollection.Count - 1];

        // ======== PinBubble Bumper ======== 
        SkillTreeNode bBumper = new SkillTreeNode("PinBubble Bumper", ModifierType.ESTABLISHMENT);
        bBumper.iconPath = "icon/1x/PinBubble Bumper";
        bBumper.iconScale = 7f;
        bBumper.iconSize = new Vector2(30, 30);

        bBumper.Register = BBumper.Register;
        bBumper.description.Add("Creates bumpers inside the pool to bounce hostile bubbles back and damage them.");
        bBumper.description.Add("Creates more bumpers and increase the damage.");
        bBumper.description.Add("There's no safe place for your enemy.");
        bBumper.maxLevel = 3;
        bBumper.CheckMethod += (side) =>
        {
            bool f1 = GameManager.round > 4;
            bool f2 = activeNodesName[GameManager.otherSide[side]].Contains(bBumper.name);
            return f1 && (!f2);
        };

        // bBumper.nextNodesName.Add("Linken's Bubble");

        bBumper.excluedeNodesName.Add("PinBubble Kicker");
        nodeCollection.Add(bBumper);
        nameToNode[bBumper.name] = nodeCollection[nodeCollection.Count - 1];

        // ======== Linken's Bubble ======== 

        SkillTreeNode linken = new SkillTreeNode("Linken's Bubble", ModifierType.GLOBAL);
        linken.iconPath = "icon/1x/Linken";
        linken.iconScale = 7f;
        linken.iconSize = new Vector2(30, 30);

        linken.Register = Linken.Register;
        linken.description.Add("Your bubbles block an attack once every 6 clock dial moves.");
        linken.description.Add("Your bubbles block an attack once every 4 clock dial moves.");
        linken.description.Add("Your bubbles block an attack once every 2 clock dial moves.");
        linken.maxLevel = 3;
        linken.CheckMethod += (side) =>
        {
            bool f1 = GameManager.round > 6;            
            return f1;
        };

        linken.excluedeNodesName.Add("Saturn");
        linken.excluedeNodesName.Add("Vicious");
        nodeCollection.Add(linken);
        nameToNode[linken.name] = nodeCollection[nodeCollection.Count - 1];

        // ======== Saturn ========

        SkillTreeNode saturn = new SkillTreeNode("Saturn", ModifierType.SINGLE);
        saturn.iconPath = "icon/1x/Saturn";
        saturn.iconScale = 7f;
        saturn.iconSize = new Vector2(30, 30);

        saturn.Register = Saturn.Register;
        saturn.description.Add("Adds Saturn into the pool. Saturn doesn't attack, but has 3 satellites that damage enemies WHENEVER they collide.");
        saturn.description.Add("Saturn has 5 satellites that damage enemies WHENEVER they collide with each other.");
        saturn.description.Add("Saturn has 7 satellites that damage enemies WHENEVER they collide with each other.");
        saturn.maxLevel = 3;
        saturn.CheckMethod += (side) =>
        {
            bool f1 = GameManager.round > 6;
            bool f2 = side != SideName.RED;
            return f1 && f2;
        };

        saturn.excluedeNodesName.Add("Linken's Bubble");
        saturn.excluedeNodesName.Add("Vicious");
        nodeCollection.Add(saturn);
        nameToNode[saturn.name] = nodeCollection[nodeCollection.Count - 1];

        // ======== Vicious ========

        SkillTreeNode vicious = new SkillTreeNode("Vicious", ModifierType.GLOBAL);
        vicious.iconPath = "icon/1x/Vicious";
        vicious.iconScale = 7f;
        vicious.iconSize = new Vector2(30, 30);

        vicious.Register = Vicious.Register;
        vicious.description.Add("Your bubbles always attack the bubble with the lowest health.");
        vicious.description.Add("Your bubbles always attack the 2 bubbles with the lowest health, if possible.");
        vicious.description.Add("Your bubbles always attack the 3 bubbles with the lowest health, if possible.");
        vicious.maxLevel = 3;
        vicious.CheckMethod += (side) =>
        {
            bool f1 = GameManager.round > 6;            
            return f1;
        };

        vicious.excluedeNodesName.Add("Saturn");
        vicious.excluedeNodesName.Add("Linken's Bubble");
        nodeCollection.Add(vicious);
        nameToNode[vicious.name] = nodeCollection[nodeCollection.Count - 1];

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

