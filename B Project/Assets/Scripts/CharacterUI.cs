using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    // Panel for all status effects
    public Transform StatusPanel;

    // Char name
    public Text CharacterName;
    // Char HP text
    public Text CharacterHP;
    // HP bar 
    public Image HPBar;
    // Threat bar
    public Image ThreatBar;
    // Threat Icon
    public Image ThreatIcon;
    // Enemy tell
    public Image EnemyTell;

    public Sprite[] HPBars;
    public Sprite[] ThreatIcons;


    // Each Icon inside status panel
    private Image[] _statusIcons;
    // Text components inside status icons
    private Text[] _iconsTexts;

    void Awake()
    {
        // Assign all refs for the panel status
        _statusIcons = new Image[StatusPanel.childCount];
        _iconsTexts = new Text[StatusPanel.childCount];

        for (int i = 0; i < StatusPanel.childCount; i++)
        {
            Transform icon = StatusPanel.GetChild(i);
            _statusIcons[i] = icon.GetComponent<Image>();
            icon.gameObject.SetActive(false);
            _iconsTexts[i] = icon.GetComponentInChildren<Text>();
            _iconsTexts[i].transform.parent.gameObject.SetActive(false);
        }

        
        // TEST status setup
        Sprite sprite = Resources.Load<Sprite>("Sprites/Icons/shieldbreak");

        SetStatusIcons(new Sprite[] {sprite,sprite, sprite  }, new string[] { "a","b","c" });
        
        SetFocus(false);
    }

    public void OnPointerEnter(int n)
    {
        //for (int i = 0; i < _statusIcons.Length; i++)
        {
            _iconsTexts[n].transform.parent.gameObject.SetActive(true);
        }

        Debug.Log("pointer enters on " + transform.root.name);
    }

    public void OnPointerExit(int n)
    {
        _iconsTexts[n].transform.parent.gameObject.SetActive(false);
        Debug.Log("pointer exits from on " + transform.root.name);

    }

    public void SetStatusIcons(Sprite[] sprites, string[] descriptions)
    {
        Debug.Log("adding " + sprites.Length + " sprite icons");
        for (int i = 0; i < _statusIcons.Length; i++)
        {
            if (i<sprites.Length)
            {
                _statusIcons[i].sprite = sprites[i];
                _iconsTexts[i].text = descriptions[i];
                _statusIcons[i].gameObject.SetActive(true);
            }
            else
            {
                _statusIcons[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void SetUnit(BaseUnit unit)
    {
        CharacterName.text = unit.UnitName;
        CharacterHP.text = unit.GetActualHP() + "/" + unit.MaxHP;
        float ratio= (float)unit.GetActualHP() / unit.MaxHP;
        HPBar.fillAmount = ratio;

        // Change HP sprite based on level
        if (ratio <0.33)
            HPBar.sprite = HPBars[0];
        else if (ratio< 0.66 )
            HPBar.sprite = HPBars[1];
        else
            HPBar.sprite = HPBars[2];

        SetThreatIcon(unit.Threat);

        // Update status icons
        List<BaseStatus> statuses = unit.Statuses;
        string[] descriptions = new string[statuses.Count];
        Sprite[] sprites = new Sprite[statuses.Count];
        for (int i = 0; i < statuses.Count; i++)
        {
            descriptions[i] = statuses[i].GetDescription();
            sprites[i] = statuses[i].Icon;
        }
        SetStatusIcons(sprites, descriptions);
    }

    public void SetFocus(bool focus)
    {
        CharacterName.enabled = focus;
        CharacterHP.enabled = focus;
        
        //EnemyTell.enabled = focus;
        
        /*
        if (focus)
        {
            HPBar.transform.parent.localScale = Vector3.one;
        }
        else
        {
            HPBar.transform.parent.localScale = Vector3.one * 0.5f;
        }
        */
    }

    public void SetEnemyTell(bool value)
    {
        // Temp threat bar
        ThreatBar.transform.parent.gameObject.SetActive(!value);
        ThreatBar.enabled = !value;

        EnemyTell.gameObject.SetActive(value);      
        ThreatIcon.enabled = !value;        
    }

    public void SetNextCard(Card card)
    {
        EnemyTell.GetComponentInChildren<Text>().text = card.title;
    }

    public void SetThreatIcon(float threat)
    {
        // Temp threat bar
        ThreatBar.fillAmount = threat;

        if (threat < 0.15)
            ThreatIcon.sprite = ThreatIcons[0];
        else if (threat < 0.3)
            ThreatIcon.sprite = ThreatIcons[1];
        else
            ThreatIcon.sprite = ThreatIcons[2];
    }
    

}
