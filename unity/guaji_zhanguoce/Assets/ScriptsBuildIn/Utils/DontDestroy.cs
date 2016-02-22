using UnityEngine;


/// <summary>
/// 切换场景不销毁
/// </summary>
public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}

