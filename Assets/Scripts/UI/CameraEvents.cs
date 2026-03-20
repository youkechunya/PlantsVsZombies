using DG.Tweening;
using System;
using UnityEngine;

public class CameraEvents : MonoBehaviour
{
    public static CameraEvents Instance { get; private set; }

    public static Action OnSelectSeed;

    public static Action OnReturnToOriginPosition;

    public static Action OnResetCameraPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        OnSelectSeed += SelectedSeedMoving;
        OnReturnToOriginPosition += ReturnToOriginPosition;
        OnResetCameraPosition += ResetCameraPosition;
    }

    private void OnDisable()
    {
        OnSelectSeed -= SelectedSeedMoving;
        OnReturnToOriginPosition -= ReturnToOriginPosition;
        OnResetCameraPosition -= ResetCameraPosition;
    }

    private void SelectedSeedMoving()
    {
        transform.DOMoveX(4, 1.5f).SetEase(Ease.InQuad);
    }

    private void ReturnToOriginPosition()
    {
        transform.DOMoveX(0, 1f).SetEase(Ease.InQuad);
    }

    private void ResetCameraPosition()
    {
        transform.position = new Vector3(0, 0, -10);
    }
}
