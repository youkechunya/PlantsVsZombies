using System;
using UnityEngine;

public abstract class BasePlant : BaseCharacter
{
    /// <summary>
    /// 死亡事件
    /// </summary>
    public Action onDeadEvent;
    /// <summary>
    /// 所占格子
    /// </summary>
    public Vector3Int cellPosition;
    /// <summary>
    /// 当前状态
    /// </summary>
    protected PlantState currentState;
    /// <summary>
    /// 放置前的预览脚本
    /// </summary>
    protected BeforePlace beforePlaceScript;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        beforePlaceScript = GetComponent<BeforePlace>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case PlantState.Idle:
                UpdateIdle();
                break;
            case PlantState.Action:
                UpdateAction();
                break;
        }
    }

    /// <summary>
    /// 闲置行为（一般用于检测僵尸是否存在）
    /// </summary>
    protected abstract void UpdateIdle();

    /// <summary>
    /// 基本行为（例如向日葵的产阳光，豌豆的攻击等）
    /// </summary>
    protected abstract void UpdateAction();

    private void OnDisable()
    {
        if (beforePlaceScript == null)
        {
            Debug.Log("警告！预放置脚本丢失！！！");
            return;
        }

        // 重新启用预放置脚本
        beforePlaceScript.enabled = true;
    }

    /// <summary>
    /// 修改状态
    /// </summary>
    /// <param name="state"></param>
    public void ChangeState(PlantState state)
    {
        currentState = state;
    }

    public override void TakeDamage(int damage, Vector2 hitPoint, bool ignoreArmor = false)
    {
        base.TakeDamage(damage, hitPoint, ignoreArmor);
        Health -= damage;
        if (Health <= 0)
        {
            Dead();
            return;
        }
    }

    protected override void Dead()
    {
        // 停止当前所有协程
        StopAllCoroutines();
        onDeadEvent?.Invoke();
    }
}
