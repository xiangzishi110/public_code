using UnityEngine;
using System.Collections;


/// <summary>
/// UI 层级关系配置
/// </summary>
public class UILayerPosition
{
    /// <summary>
    /// UIManger 最大UIPanel Depth 2000
    /// </summary>
    public static int UIMaxUIDepth = 2000;
    /// <summary>
    ///链接网络提示UI，ngui pnale depth
    /// </summary>
    public static int LinkNetWorkTishiUIDepth = 4000;
    /// <summary>
    /// 链接网络提示UI，localPosition
    /// </summary>
    public static Vector3 LinkNetWorkTishiUIPositon = new Vector3(0, 0, -4000f);

}


