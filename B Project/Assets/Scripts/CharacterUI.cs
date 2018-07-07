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
    // Enemy tell
    public Image EnemyTell;

    public Sprite[] HPBars;
    public Sprite[] ThreatIcons;


    // Each Icon inside status panel
    private Image[] _statusIcons;
    // Text components inside status icons
    private Text[] _iconsTexts;

    void Start()
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
        Sprite sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");

        SetStatusIcons(new Sprite[] { sprite, sprite, sprite }, new string[] { "test1", "test2", "test3" });

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
        HPBar.fillAmount =(float) unit.GetActualHP() / unit.MaxHP;
        ThreatBar.fillAmount = unit.Threat;
    }

    public void SetFocus(bool focus)
    {
        CharacterName.enabled = focus;
        CharacterHP.enabled = focus;
        //ThreatBar.transform.parent.gameObject.SetActive( focus);
        //EnemyTell.enabled = focus;
        ThreatBar.gameObject.SetActive(focus);
        if (focus)
        {
            HPBar.transform.parent.localScale = Vector3.one;
        }
        else
        {
            HPBar.transform.parent.localScale = Vector3.one * 0.5f;
        }
    }

    public void SetEnemyTell(bool value)
    {
        EnemyTell.gameObject.SetActive(value);
        //ThreatBar.transform.parent.gameObject.SetActive(!value);
        ThreatBar.enabled = !value;
    }

    public void SetNextCard(Card card)
    {

    }

}
