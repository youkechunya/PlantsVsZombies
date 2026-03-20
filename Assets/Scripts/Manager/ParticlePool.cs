using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [System.Serializable]
    public class Particle
    {
        public GameObject particle;
        public int initialSize;
    }

    [Header("粒子池配置")]
    [SerializeField] private List<Particle> particlesDataList = new();

    private readonly Dictionary<string, Queue<GameObject>> poolDict = new();

    private void Start()
    {
        foreach(var item in particlesDataList)
        {
            Queue<GameObject> pool = new();
            for (int i = 0; i < item.initialSize; i++)
            {
                GameObject particle = Instantiate(item.particle, transform);
                particle.SetActive(false);
                pool.Enqueue(particle);
            }
            poolDict[item.particle.name] = pool;
        }
    }

    /// <summary>
    /// 获取粒子
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetFromPool(string name)
    {
        if (poolDict.TryGetValue(name,out Queue<GameObject> pool))
        {
            if (pool.Count > 0)
            {
                GameObject particle = pool.Dequeue();
                particle.SetActive(true);
                return particle;
            }
            else
            {
                Debug.Log("当前对应粒子池为空，直接创建新的粒子");
                GameObject particle = Instantiate(GetPrefabByKey(name), transform);
                return particle;
            }
        }
        Debug.LogWarning($"[ParticlePool] 对象池找不到 name: {name}");
        return null;
    }

    /// <summary>
    /// 丢回粒子池
    /// </summary>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    public void ReturnPool(string key, GameObject obj)
    {
        if (obj == null)
            return;

        key = key.Replace("(Clone)", "").Trim();
        if (!poolDict.TryGetValue(key, out Queue<GameObject> pool))
            return;

        // 注意，有可能出现同一个实例被重复引用，必须进行检测，不然会出现问题！！！
        foreach (var item in pool)
        {
            if (ReferenceEquals(item, obj))
            {
                Debug.LogWarning($"[Pool] 实例 {obj.name} {obj.GetInstanceID()} 已在池中，拒绝重复入池");
                return;
            }
        }
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    /// <summary>
    /// 通过关键字从对象池列表获取预制体，用于在对象池空了时临时新建一个物体。
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private GameObject GetPrefabByKey(string key)
    {
        foreach (var data in particlesDataList)
        {
            if (data.particle.name == key)
                return data.particle;
        }
        return null;
    }
}
