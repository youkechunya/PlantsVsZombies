using UnityEngine;

public class ObjectReturnPool : MonoBehaviour
{
    private void OnDisable()
    {
        ParticlePool.Instance.ReturnPool(gameObject.name, gameObject);
    }
}
