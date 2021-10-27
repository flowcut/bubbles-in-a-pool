using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillToggle : MonoBehaviour
{
    public GameObject background;
    public GameObject checkmark;
    public GameObject other;
    public GameObject submissionButton;
    public int selectionIndex = 0;
    public GameObject upgradeSign;
    // Start is called before the first frame update

    public void SetSprite(Sprite sprite)
    {
        background.GetComponent<Image>().sprite = sprite;
        checkmark.GetComponent<Image>().sprite = sprite;
    }

    public void OnChange()
    {
        submissionButton.GetComponent<SelectionSubmit>().selectionIndex = selectionIndex;
    }

    public void OnHover()
    {

    }

    public void Upgrade()
    {
        upgradeSign.SetActive(true);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Destroy(upgradeSign);
    }
}
