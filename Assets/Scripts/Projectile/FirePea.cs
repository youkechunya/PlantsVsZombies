using UnityEngine;

public class FirePea : BaseProjectile
{
    float rotation = 0;

    protected override void OnEnable()
    {
        rotation = 0;
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        base.OnEnable();
    }

    protected override void Update()
    {
        base.Update();
        rotation += Time.deltaTime * 200;
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    //protected override void HitEvent(BaseCharacter beHitObject)
    //{
    //    Collider2D[] zombies = Physics2D.OverlapCircleAll(transform.position, 1.4f, zombieMask);
    //    GameObject boom = ObjectPool.Instance.GetFromPool("BoomTrigger");
    //    boom.transform.position = transform.position;
    //    int zombieCount = Mathf.Max(1, zombies.Length);
    //    boom.GetComponent<Boom>().damage = Mathf.Clamp(40 / zombieCount, 1, 13);
    //    boom.GetComponent<Boom>().isCharred = false;
    //    boom.GetComponent<Boom>().BoomRadius = 1f;
    //    base.HitEvent(beHitObject);
    //}
}
