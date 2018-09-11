using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiasmaFieldEffect : BaseFieldEffect
{
    // Particle ref
    private GameObject _particle;


    public MiasmaFieldEffect(int strength, int duration, BaseUnit owner, int tile) : base(strength, duration, owner, tile)
    {
        Icon = Resources.Load<Sprite>("Sprites/Icons/poison");
        GameObject particlePrefab = Resources.Load<GameObject>("Prefabs/Particles/Miasma Particle");
        Vector2 position = Battleground.Instance.GetPositionFromTile(tile);
        _particle = GameObject.Instantiate(particlePrefab);
        _particle.transform.position = position;
    }

    public override void EndStatusExecute()
    {
        // Remove ground effect etc
        GameObject.Destroy(_particle.gameObject);
    }

    public override string GetDescription()
    {
        string msg = "Deals " + Strength + " damage for "+Duration+" turn";
        if (Duration > 1)
            msg += "s";
        return msg;
    }

    public override void StartTurnExecute()
    {
        if (Duration < 1)
        {
            // The status was expired
            UnityEngine.Debug.Log("Status " + this + " expired but stil active");
            return;
        }

        // Search if there is a unit on the tile and apply damage in case
        BaseUnit target= Battleground.Instance.GetUnitOnTile(Position);
        if (target)
            target.DealDamage(Strength);

        // reduce duration
        Duration--;
    }

    public override void Update(BaseStatus newStatus)
    {
        // Update duration
        Duration = newStatus.Duration;
    }
}
