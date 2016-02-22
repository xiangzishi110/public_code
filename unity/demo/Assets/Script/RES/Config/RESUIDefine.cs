using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class RESUIDefine 
{
    /// <summary>
    /// 公共图集列表
    /// </summary>
    public static List<string> PublicUIAtlasName = new List<string>() 
    { 
        "PublicButton",
        "PublicImage",
        "PublicVipNum",
        "PublicBackground",
    };

    /// <summary>
    /// UI路径
    /// </summary>
    public static string UIPath = "Resources/UI/UIPrefabs/";

    /// <summary>
    /// UI公共图集路径
    /// </summary>
    public static string UIPublicPath = "UI/UIAtlas/Public/";
	
	/// <summary>
	/// 场景目录: Scene/
	/// </summary>
	public const string ScenePath = "Scene/";


    
}
