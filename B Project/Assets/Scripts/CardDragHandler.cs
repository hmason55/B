using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	[SerializeField] Card card;
	[SerializeField] Image outline;
	[SerializeField] Transform hand;
	LineRenderer targetLine;
	Vector3 mouseOffset;

	BaseUnit[] targets;

	void Awake() {
		hand = GameObject.FindGameObjectWithTag("Hand").transform;
		GameObject lineGO = GameObject.FindGameObjectWithTag("TargetLine");
		if(lineGO != null) {
			targetLine = lineGO.GetComponent<LineRenderer>();
		}
	}

    public void OnBeginDrag(PointerEventData eventData)
    {
        mouseOffset = transform.position - Input.mousePosition;
        Debug.Log("Drag " + gameObject.name);

        if (hand == null) { return; }
        foreach (Transform cardTransform in hand)
        {
            cardTransform.GetComponent<Card>().DisableRaycast();
        }

        hand.GetComponent<Hand>().draggedCard = card;

        // Notify battleground of target shape (and in future side too)
        // Check if the spell requires a different target
        TargetEntity entity = TargetEntity.Enemy;
        if (card.targetType == Card.TargetType.Ally)
            entity = TargetEntity.Player;
        // TEMP only work for miasma
        if (card.targetType == Card.TargetType.Tile)
        {
            if (card.cardData.Description=="Miasma")
                Battleground.Instance.SetTargetShape(TargetShape.Cross, entity);
            else
                Battleground.Instance.SetTargetShape(TargetShape.Veritcal, entity);
        }
        else
            Battleground.Instance.SetTargetShape(TargetShape.Single, entity);
    }

    public void OnDrag(PointerEventData eventData) {
        Vector3 mousePosition = Input.mousePosition;
        if (card.requireTarget) {

            if (Input.mousePosition.y > hand.GetComponent<Hand>().verticalThreshold) {
                transform.position = new Vector3(Screen.width / 2f, 210f, 0f);
                EnableLineTarget();
                                
                /* Commented to keep this calculation on battleground
                int layerMask = 1 << 9; // Entity layer
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);
                if (hit) {
                    EnemyUnit enemyUnit = hit.transform.parent.GetComponent<EnemyUnit>();
                    BaseUnit allyUnit = hit.transform.parent.GetComponent<BaseUnit>();

                    if (enemyUnit) {
                        targets = new BaseUnit[1];
                        targets[0] = enemyUnit;
                    } else if (allyUnit) {
                        targets = new BaseUnit[1];
                        targets[0] = allyUnit;
                    } else {
                        targets = null;
                    }
                } else {
                    targets = null;
                }*/
            } else {
                transform.position = mousePosition + mouseOffset;
                DisableLineTarget();
            }
        } else {
            transform.position = mousePosition + mouseOffset;
            DisableLineTarget();
        }

        GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
        transform.SetAsLastSibling();
        GetComponent<CardMouseOverHandler>().ShowOutline();
    }

	public void OnEndDrag(PointerEventData eventData) {
               
        targets = Battleground.Instance.GetTargetUnits().ToArray();
        Debug.Log("Drop " + gameObject.name+ " on "+targets.Length +" targets");
		if(hand == null) {return;}
        
        
		if(targets != null) {
			if(EvaluateTargets()) {
				Play();
			} else {
				Debug.Log("Invalid target");
                CancelDrag();
			}
		} else {
			CancelDrag();
		}

        // Return battleground target to standard
        Battleground.Instance.SetTargetShape(TargetShape.Single, TargetEntity.Unit);
    }

    bool EvaluateTargets()
    {
        Debug.Log("Evaluating targets");

        if (card.targetType == Card.TargetType.Tile)
        {
            Debug.Log("shooting on tile " + Battleground.Instance.GetCurrentTile());
            // Tile target
            if (Battleground.Instance.GetCurrentTile() < 9)
                return false;
        }
        else
        {
            // Units target
            if (targets == null)
            {
                return false;
            }

            foreach (BaseUnit target in targets)
            {
                if (!target.IsTargetable())
                {
                    Debug.Log("Not targetable");
                    return false;
                }
            }
        }
        return true;
    }

	void Play() {
		DisableLineTarget();

		foreach(Transform cardTransform in hand) {
			Card card = cardTransform.GetComponent<Card>();
			card.ResumeRaycastState();
		}

		hand.GetComponent<Hand>().draggedCard = null;

        // Call different methods based on target type
        if (card.targetType == Card.TargetType.Tile)
            card.Play(Battleground.Instance.GetCurrentTile());
        else
            card.Play(targets);
	}

	public void CancelDrag() {
		card.SnapToHand();
		DisableLineTarget();

		foreach(Transform cardTransform in hand) {
			Card card = cardTransform.GetComponent<Card>();
			card.ResumeRaycastState();
		}

		GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		GetComponent<CardMouseOverHandler>().HideOutline();
		transform.SetSiblingIndex(card.zIndex);

		hand.GetComponent<Hand>().draggedCard = null;
	}

	void EnableLineTarget() {
		if(targetLine != null) {
			targetLine.enabled = true;
			Vector3 cardWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(transform.position.x, transform.position.y, transform.position.z));
			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			cardWorldPosition.z = 0;
			mouseWorldPosition.z = 0;
			Vector3 unitVector = mouseWorldPosition-cardWorldPosition;

			targetLine.SetPosition(0, new Vector3(cardWorldPosition.x, cardWorldPosition.y, 1f));
			targetLine.SetPosition(1, new Vector3(cardWorldPosition.x, cardWorldPosition.y, 1f) + unitVector*0.795f);
			targetLine.SetPosition(2, new Vector3(cardWorldPosition.x, cardWorldPosition.y, 1f) + unitVector*0.80f);
			targetLine.SetPosition(3, new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 1f));
		}
	}

	void DisableLineTarget() {
		if(targetLine != null) {
			targetLine.SetPosition(0, Vector3.zero);
			targetLine.SetPosition(1, Vector3.zero);
			targetLine.SetPosition(2, Vector3.zero);
			targetLine.SetPosition(3, Vector3.zero);
		}
	}
}
