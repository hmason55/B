using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldEffectUI : MonoBehaviour
{
    public GameObject Panel;
    public GameObject DescriptionObject;
    public Text Description;
    public Image Icon;
    
    public float DelayBeforeShow;

    private LineRenderer _line;
    private float _timer;
    private Vector3 _oldMousePos = Vector3.zero;
    
    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.startColor = Color.white*0.5f;
        _line.endColor = Color.white;
        _line.startWidth = 0.1f;
        _line.endWidth = 0.4f;
        _line.enabled = false;
    }
    
    void Update()
    {
        // Check for mouse movement
        if (Input.mousePosition.Equals(_oldMousePos))
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _timer = 0;
        }
        _oldMousePos = Input.mousePosition;
        
        int tile = Battleground.Instance.GetCurrentTile();
        if (tile < 0 || _timer<DelayBeforeShow)
        {
            // No valid tile turn UI off
            Panel.SetActive(false);
            _line.enabled = false;
            return;
        }
        BaseFieldEffect fieldEffect = Battleground.Instance.GetFieldEffect(tile);
        if (fieldEffect== null)
        {
            Panel.SetActive(false);
            _line.enabled = false;
            return;
        }
        Icon.sprite = fieldEffect.Icon;
        Description.text = fieldEffect.GetDescription();
        Panel.SetActive(true);
        Vector3 start = (Vector3)Battleground.Instance.GetPositionFromTile(tile)+ new Vector3(0f,-1.5f,11f);
        Vector3 end=start + new Vector3(-6f, -4f, 11f);
        Panel.transform.position = end + new Vector3(0f, -1f, 0f);
        _line.enabled = true;
        
        _line.SetPosition(0,start );
        _line.SetPosition(1, end);
    }
}
