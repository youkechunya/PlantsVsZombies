using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trophy : MonoBehaviour
{
    [SerializeField] private GameObject awardRays;
    [SerializeField] private GameObject awardPickUpGrow;

    bool trigger;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (trigger)
            return;
        if (collision.CompareTag("Cursor"))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                trigger = true;
                transform.DOMove(new Vector3(0, 0, 0), 4f);
                awardPickUpGrow.transform.DOScale(new Vector3(30, 30, 30), 6f);
                awardRays.transform.DOScale(new Vector3(25, 25, 25), 6f);
                awardRays.transform.DORotate(new Vector3(0, 0, 1800), 6f, RotateMode.LocalAxisAdd);
                AudioManager.Instance.sfxPool.PlaySFX("winmusic");
                StartCoroutine(NextLevel());
            }
        }
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
