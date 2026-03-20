using System.Collections;
using UnityEngine;

public class CharredPlant : BasePlant
{
    private void OnDisable()
    {
        GetComponent<BeforePlace>().enabled = true;
    }

    /// <summary>
    ///  Ä¬ÈÏÎªÓ£ÌÒÕ¨µ¯
    /// </summary>
    public virtual void Boom()
    {
        GameObject boomParticle = ParticlePool.Instance.GetFromPool("Boom");
        boomParticle.transform.position = transform.position;
        GameObject boom = ObjectPool.Instance.GetFromPool("BoomTrigger");
        boom.transform.position = transform.position;
        boom.GetComponent<Boom>().isCharred = true;
        boom.GetComponent<Boom>().damage = 1800;
        boom.GetComponent<Boom>().BoomRadius = 1.4f;
        AudioManager.Instance.sfxPool.PlaySFX("cherrybomb");
        LevelManager.Instance.UpdateGrid(cellPosition, 1);
        GameEvents.OnReturnToPool(gameObject.name, gameObject);
    }

    protected override void UpdateIdle() { }

    protected override void UpdateAction() { }
}
