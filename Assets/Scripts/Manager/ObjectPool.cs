using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameEvents.OnReturnToPool += ReturnToPool;
    }

    private void OnDisable()
    {
        GameEvents.OnReturnToPool -= ReturnToPool;
    }

    [System.Serializable]
    public class PoolData
    {
        public GameObject prefab;
        public int initialSize;
    }

    // 字典存储物体队列
    private readonly Dictionary<string, Queue<GameObject>> poolDict = new();
    private readonly List<GameObject> activeObject = new();

    /// <summary>
    /// 从对象池里获取指定 key 对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public GameObject GetFromPool(string key)
    {
        Debug.Log($"[ObjectPool] 尝试从字典中获取对象: {key}");
        // 尝试从字典中获取对象
        if (poolDict.TryGetValue(key, out Queue<GameObject> pool))
        {
            if (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                obj.SetActive(true);
                activeObject.Add(obj);
                return obj;
            }
            else
            {
                Debug.LogWarning($"[{key}] 池子空了，现场创建");
                // 创建新对象
                GameObject prefab = GetPrefabByKey(key);
                if (prefab != null)
                {
                    GameObject newObj = Instantiate(prefab, transform);
                    activeObject.Add(newObj);
                    return newObj;
                }
            }
        }
        else
        {
            var tempObjects = new List<GameObject>();
            // 新建一个对象池
            Queue<GameObject> newPool = new();
            // 默认新建 5 个进池
            for (int i = 0; i < 5; i++)
            {
                Debug.Log($"{key} 对应的预制体：{GetPrefabByKey(key)}");
                // 新建对象
                GameObject obj = Instantiate(GetPrefabByKey(key), transform);
                tempObjects.Add(obj);
                newPool.Enqueue(obj);
            }
            // 将该对象池存入字典
            poolDict[key] = newPool;
            Debug.Log($"对象池初始化：{key}");
            foreach (var obj in tempObjects)
            {
                obj.SetActive(false);
            }
            GameObject returnObj = newPool.Dequeue();
            returnObj.SetActive(true);
            activeObject.Add(returnObj);
            return returnObj;
        }
        Debug.Log("报告！对象池真没招了！");
        return null;
    }

    /// <summary>
    /// 丢回对象池
    /// </summary>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    private void ReturnToPool(string key, GameObject obj)
    {
        Debug.Log($"{key} 物体准备返回对象池");
        if (obj == null)
        {
            Debug.LogWarning("警告：调用返回对象池方法时传入的对象为空！");
            return;
        }

        key = key.Replace("(Clone)", "").Trim();
        if (!poolDict.TryGetValue(key, out Queue<GameObject> pool))
        {
            Debug.LogWarning($"警告：对象池字典中不存在对象 {key}");
            return;
        }

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
        activeObject.Remove(obj);
        Debug.Log($"{key} 物体返回对象池成功");
    }

    /// <summary>
    /// 通过关键字从对象池列表获取预制体，用于在对象池空了时临时新建一个物体。
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private GameObject GetPrefabByKey(string key)
    {
        GameObject gameObject;
        // 在提示列表中查找
        gameObject = Resources.Load<GameObject>($"Prefabs/PlantTip/{key.Replace("(Clone)", "").Trim()}");
        // 在植物列表中查找
        if (gameObject == null)
            gameObject = Resources.Load<GameObject>($"Prefabs/Plant/{key.Replace("(Clone)", "").Trim()}");
        // 在僵尸提示列表中查找
        if (gameObject == null)
            gameObject = Resources.Load<GameObject>($"Prefabs/Zombie/{key.Replace("(Clone)", "").Trim()}");
        // 在根列表中查找
        if (gameObject == null)
            gameObject = Resources.Load<GameObject>($"Prefabs/{key.Replace("(Clone)", "").Trim()}");
        return gameObject;
    }

    /// <summary>
    /// 将所有取出的对象返回对象池
    /// </summary>
    public void ReturnAll()
    {
        if (activeObject == null || activeObject.Count == 0)
            return;

        var tempList = new List<GameObject>(activeObject);
        tempList.ForEach(i => ReturnToPool(i.name, i));
    }
}
