using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    public float waitingTime = 3f;

    void OnEnable()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        StartCoroutine(DisappearIEnumerator());
    }

    IEnumerator DisappearIEnumerator()
    {
        yield return new WaitForSeconds(waitingTime);
        GetComponent<SpriteRenderer>().DOColor(new Color(1, 1, 1, 0), 2).OnComplete(() => GameEvents.OnReturnToPool(gameObject.name,gameObject));
    }
}
