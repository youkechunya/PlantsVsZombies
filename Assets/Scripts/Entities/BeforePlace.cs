using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

/// <summary>
/// 放置植物前的脚本
/// </summary>
public class BeforePlace : MonoBehaviour
{
    // 动画组件
    private Animator anim;
    /// <summary>
    /// 是否已经种植
    /// </summary>
    [SerializeField] private bool place;
    /// <summary>
    /// 鼠标位置
    /// </summary>
    private Vector3 mousePos;
    /// <summary>
    /// InputAction 组件
    /// </summary>
    private Game gamePlay;
    /// <summary>
    /// 种子脚本
    /// </summary>
    public Seed seed;
    /// <summary>
    /// 阴影
    /// </summary>
    [SerializeField] private GameObject shadow;
    /// <summary>
    /// 提示体
    /// </summary>
    [SerializeField] private GameObject tip;
    /// <summary>
    /// 格子坐标
    /// </summary>
    Vector3Int cellPosition;
    /// <summary>
    /// 子图像数组
    /// </summary>
    private SpriteRenderer[] sprites;
    // 植物的作用脚本
    /// <summary>
    /// 植物
    /// </summary>
    private BasePlant plant;
    /// <summary>
    /// 费用
    /// </summary>
    public int cost;

    /// <summary>
    /// 是否需要提示体（用于不需要鼠标的直接放置）
    /// </summary>
    private bool needTipObj;
    public bool NeedTipObj
    {
        get => needTipObj;
        set
        {
            needTipObj = value;
            // 每次修改该值时都初始化一次脚本
            Initialized();
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        // 获取常规植物的行为脚本
        plant = GetComponent<BasePlant>();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Initialized()
    {
        Debug.Log("初始化");
        // 初始化即点击种子后产生的植物预制体，此时必定没被种植在草坪上
        place = false;
        // 将植物的阴影先隐藏
        shadow.SetActive(false);
        // 将植物的碰撞体先关闭防止选择位置的时候被僵尸啃咬或其他触发事件
        GetComponent<Collider2D>().enabled = false;
        // 创建新的 InputAction
        gamePlay ??= new Game();
        // 启用 InputAction
        gamePlay.GamePlay.Enable();
        // 订阅事件
        SubscribeEvents();
        // 如果需要提示体
        if (NeedTipObj)
        {
            Debug.Log($"获取{name.Replace("(Clone)", "").Trim()}Tip");
            // 从对象池中获取对应提示体
            tip = ObjectPool.Instance.GetFromPool(name.Replace("(Clone)", "").Trim() + "Tip");
        }
        plant.enabled = false;

        // 获取所有子图像
        sprites = transform.GetComponentsInChildren<SpriteRenderer>();
        // 预种植时的图层在最前面
        foreach (var sprite in sprites)
        {
            sprite.sortingOrder += 99;
        }
    }

    void Update()
    {
        // 如果已经种植则直接跳出
        if (place)
            return;

        // 获取鼠标的世界坐标
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        // 更新当前位置
        transform.position = mousePos;
        // 获取当前位置的格子坐标
        cellPosition = LevelManager.Instance.grid.WorldToCell(transform.position);
        cellPosition.x = Mathf.Clamp(cellPosition.x, -6, 2);
        cellPosition.y = Mathf.Clamp(cellPosition.y, -3, 1);
        // 如果当前格子没有植物
        if (!LevelManager.Instance.CheckGrid(new Vector3Int(cellPosition.x + 6, cellPosition.y + 3)))
        {
            // 获取当前格子坐标的中心位置
            Vector3 targetPosition = LevelManager.Instance.grid.GetCellCenterWorld(cellPosition);
            if (NeedTipObj)
            {
                tip.SetActive(true);
                tip.transform.position = targetPosition;
            }
        }
        // 如果当前格子有植物
        else
        {
            if (NeedTipObj)
            {
                tip.SetActive(false);
            }
        }
    }

    public void Place(InputAction.CallbackContext ctx)
    {
        // 检查当前格子是否存在植物
        if (!LevelManager.Instance.CheckGrid(new Vector3Int(cellPosition.x + 6, cellPosition.y + 3)))
        {
            // 更新种植参数
            place = true;
            // 更新种子脚本中生成植物的参数
            seed.isSpawning = false;
            // 启用植物碰撞体
            GetComponent<Collider2D>().enabled = true;
            // 恢复子图像图册
            foreach (var sprite in sprites)
            {
                sprite.sortingOrder -= 99;
            }
            // 取消订阅事件
            UnSubscribeEvents();
            // 禁用 InputAction
            gamePlay.GamePlay.Disable();
            // 播放种植声音
            AudioManager.Instance.sfxPool.PlaySFX("plant1");
            // 更新当前格子信息
            LevelManager.Instance.UpdateGrid(new Vector3Int(cellPosition.x + 6, cellPosition.y + 3), 0);
            // 将当前格子赋给 Damageable 脚本
            GetComponent<BasePlant>().cellPosition = new Vector3Int(cellPosition.x + 6, cellPosition.y + 3);
            // 更新动画
            anim.SetTrigger("idle");
            // 使种子开始冷却
            seed.StartCooldown();
            // 更新当前位置
            transform.position = LevelManager.Instance.grid.GetCellCenterWorld(cellPosition);
            // 获取种植土特效
            GameObject dirt = ParticlePool.Instance.GetFromPool("Dirt");
            // 获取种植土特效的位置
            Vector3 ground = transform.position;
            ground.y -= 0.3f;
            dirt.transform.position = ground;
            // 如果有提示体则将提示体丢回对象池
            if (NeedTipObj)
                GameEvents.OnReturnToPool(tip.name, tip);
            // 启用阴影
            shadow.SetActive(true);
            // 更新阳光
            SunManager.Instance.Sun -= cost;
            // 如果是常规植物
            if (plant != null)
            {
                // 启用常规植物的脚本
                plant.enabled = true;
                plant.cellPosition = new Vector3Int(cellPosition.x + 6, cellPosition.y + 3);
                plant.onDeadEvent += DeadEvent;
            }
            // 将当前脚本禁用
            enabled = false;
        }
    }

    private void DeadEvent()
    {
        // 取消订阅事件
        plant.onDeadEvent -= DeadEvent;
        // 更新当前格子信息
        LevelManager.Instance.UpdateGrid(new Vector3Int(cellPosition.x + 6, cellPosition.y + 3), 1);
        // 播放被吃掉的音效
        AudioManager.Instance.sfxPool.PlaySFX("gulp");
        // 丢回对象池
        GameEvents.OnReturnToPool(gameObject.name, gameObject);
    }

    public void Cancel(InputAction.CallbackContext ctx)
    {
        // 恢复子图像图册
        foreach (var sprite in sprites)
        {
            sprite.sortingOrder -= 99;
        }
        // 更新种子脚本
        seed.CancelPlace();
        // 禁用 InputAction
        gamePlay.GamePlay.Disable();
        GameEvents.OnReturnToPool(tip.name, tip);
        GameEvents.OnReturnToPool(gameObject.name, gameObject);
    }

    public void PlacePosition(Vector3Int position)
    {
        // 不需要提示体
        NeedTipObj = false;
        // 更新种植参数
        place = true;
        // 启用碰撞器
        GetComponent<Collider2D>().enabled = true;
        // 取消订阅事件
        UnSubscribeEvents();
        // 禁用 InputAction
        gamePlay.GamePlay.Disable();
        // 播放种植声音
        AudioManager.Instance.sfxPool.PlaySFX("plant1");
        // 更新当前格子信息
        LevelManager.Instance.UpdateGrid(new Vector3Int(position.x + 6, position.y + 3), 0);
        // 将当前格子赋给 Damageable 脚本
        GetComponent<BasePlant>().cellPosition = new Vector3Int(position.x + 6, position.y + 3);
        // 更新动画
        anim.SetTrigger("idle");
        // 更新当前位置
        transform.position = LevelManager.Instance.grid.GetCellCenterWorld(position);
        // 获取种植土特效
        GameObject dirt = ParticlePool.Instance.GetFromPool("Dirt");
        // 获取种植土特效的位置
        Vector3 ground = transform.position;
        ground.y -= 0.3f;
        dirt.transform.position = ground;
        // 如果有提示体则将提示体丢回对象池
        if (NeedTipObj)
            GameEvents.OnReturnToPool(tip.name, tip);
        // 启用阴影
        shadow.SetActive(true);
        if (plant != null)
        {
            plant.enabled = true;
            plant.cellPosition = new Vector3Int(position.x + 6, position.y + 3);
        }
        // 将当前脚本禁用
        enabled = false;
    }

    private void SubscribeEvents()
    {
        gamePlay.GamePlay.Place.performed += Place;
        gamePlay.GamePlay.Cancel.performed += Cancel;
    }

    private void UnSubscribeEvents()
    {
        gamePlay.GamePlay.Place.performed -= Place;
        gamePlay.GamePlay.Cancel.performed -= Cancel;
    }
}
