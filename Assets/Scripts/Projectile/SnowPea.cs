using UnityEngine;

public class SnowPea : BaseProjectile
{
    [Header("ºıÀŸ ±≥§")]
    [SerializeField] private float decelerationDuration = 10f;

    protected override void HitEvent(BaseCharacter beHitObject)
    {
        beHitObject.GetComponent<BaseCharacter>().Slowed(decelerationDuration);
        base.HitEvent(beHitObject);
    }
}
