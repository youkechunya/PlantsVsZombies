using System.Linq;
using UnityEngine;

public class Armor : MonoBehaviour
{
    [Header("防具配置")]
    [SerializeField] private ArmorData primaryArmor;
    [SerializeField] private ArmorData secondaryArmor;

    /// <summary>
    /// 防具列表与防具优先级
    /// </summary>
    [SerializeField] private ArmorData[] armors;

    /// <summary>
    /// 本体
    /// </summary>
    private BaseZombie character;

    private void Awake()
    {
        character = GetComponent<BaseZombie>();
        // 默认二类防具优先被攻击
        if (secondaryArmor.armor == null && primaryArmor.armor == null)
            Debug.LogError("挂了脚本但没设置任何防具！");
        else if (secondaryArmor.armor == null && primaryArmor.armor != null)
            armors = new[] { primaryArmor };
        else if (secondaryArmor.armor != null && primaryArmor.armor == null)
            armors = new[] { secondaryArmor };
        else
            armors = new[] { secondaryArmor, primaryArmor };
    }

    private void OnEnable()
    {
        foreach (var armor in armors)
        {
            if (armor == null) continue;
            armor.Initialize();
        }
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="hitPoint"></param>
    public void TakeDamage(int damage, Vector2 hitPoint)
    {
        // 获取当前防具
        var activeArmor = armors.FirstOrDefault(a => a.currentHP > 0);
        if (activeArmor == null || !activeArmor.IsInDefenseRange(GetHitAngle(hitPoint)))
        {
            character.TakeDamage(damage, hitPoint, true);
            return;
        }

        // 计算防具受到的实际伤害
        int actualDamage = Mathf.Min(damage, activeArmor.currentHP);
        // 计算溢出伤害
        int overflow = damage - actualDamage;
        activeArmor.currentHP -= actualDamage;
        // 播放受击音效
        PlayHitSound(activeArmor);
        // 更新防具外观
        UpdateAppearance(activeArmor);
        // 更新波长
        UpdateWaveHP(actualDamage);

        if (overflow > 0)
            character.TakeDamage(overflow, hitPoint, true);

        if (activeArmor.currentHP <= 0)
            BreakArmor(activeArmor);
    }

    private void UpdateWaveHP(int actualDamage)
    {
        //计算对当前波的血量影响
        if (character.spawner == null)
        {
            Debug.LogError("警告！本体的生成器没啦！（一般不会出现这个 bug 吧）");
            return;
        }

        // 判断当前敌人是否在当前波
        if (character.belongingWave != character.spawner.wave)
            return;

        character.spawner.currentWaveHP -= actualDamage;
    }

    /// <summary>
    /// 更新外观
    /// </summary>
    /// <param name="activeArmor"></param>
    private void UpdateAppearance(ArmorData activeArmor)
    {
        activeArmor.armor.GetComponent<SpriteRenderer>().sprite = activeArmor.GetCurrentAppearance();
    }

    /// <summary>
    /// 播放受击音效
    /// </summary>
    /// <param name="activeArmor"></param>
    private void PlayHitSound(ArmorData activeArmor)
    {
        int randomSound = Random.Range(0, activeArmor.hitSounds.Length);
        AudioManager.Instance.sfxPool.PlaySFX(activeArmor.hitSounds[randomSound].name);
    }

    /// <summary>
    /// 获取受击点到防具的角度
    /// </summary>
    /// <param name="hitPoint"></param>
    /// <returns></returns>
    private float GetHitAngle(Vector2 hitPoint)
    {
        Vector2 direction = (hitPoint - (Vector2)transform.position).normalized;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 防具损坏
    /// </summary>
    /// <param name="armor"></param>
    private void BreakArmor(ArmorData armor)
    {
        armor.armor.SetActive(false);

        armor.drop.Play();

        // 遍历防具列表
        if (armors.All(a => a.currentHP <= 0))
            character.armor = null;
    }

    public void BeCharred()
    {
        foreach (var armor in armors)
        {
            character.spawner.currentWaveHP -= armor.currentHP;
        }
    }
}
