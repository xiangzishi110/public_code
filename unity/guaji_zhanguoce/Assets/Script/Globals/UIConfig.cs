using UnityEngine;
using System.Collections;

/// <summary>
/// UI 全局配置
/// </summary>
public class UIConfig
{
    /// <summary>
    /// UI 缓存池最大容量
    /// </summary>
    public static  int mLimitCount = 5;
    /// <summary>
    /// 需要隐藏MainScene的UI列表
    /// </summary>
    public static string[] mHideMainSceneUINameList = new string[]
	{
		"ShopUI",
	};

}


