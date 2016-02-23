using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.IO;

/// <summary>
/// Dll脚本更新
/// 目前只支持android
/// </summary>
public class AppStarter : MonoBehaviour 
{
    /// <summary>
    /// 是否运行dll代码
    /// </summary>
    public bool isRunFromDll = false;

    /// <summary>
    /// true:assetbundle;false:resources
    /// </summary>
    public bool resAssetBundle = false;

    /// <summary>
    /// dll资源打包路径
    /// </summary>
    private string[] abList = { "Dll/GameModules.assetbundle"};
    /// <summary>
    /// 当前加载序号
    /// </summary>
    private int index = 0;

    public static AppStarter instance;

    void Awake()
    {
        instance = this;
    }

	void Start ()
    {
        ///////////////////////////////////////////////
        ///因为在buildIn里面没法调用到SdkUtil,所以写到这里直接调用android部分
        ///将来ios版本也要加入相应的代码来处理sdk初始化，方能正常运行在线更新
        ///////////////////////////////////////////////
       // bool needWaitForInitApp=SDKHelper.initApp();
        //if (needWaitForInitApp == false)
        //{
             startUpdate();
        //}
    }
    public void startUpdate()
    {
       // ResOnlineUpdate update = GetComponent<ResOnlineUpdate>();
        //if (update)
        //{
            //update.InitExecution(ExeRun);
        //}

        ExeRun(false);
    }

    void ExeRun(bool isbool)
    {
        if (isRunFromDll)
        {
            print("[H]--> dll load start");
            StartCoroutine(Load());
        }
        else
        {
           gameObject.AddComponent<GameInitialize>();
        }
    }
    IEnumerator Load()
    {
        string fileName = abList[index];
        string url = FileLoadPath.GetResFullPath(fileName);
        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {
            Logger.Log(www.error);
        }
        else
        {
            AssetBundle bundle = www.assetBundle;
            //解密
            if (AESTool.NeedDecrypt(www.bytes))
            {
                bundle = AESTool.DecryptCreateFromMemory(www.bytes);
            }

            TextAsset txt = bundle.mainAsset as TextAsset;
            print("[H]--> load dll name:" + fileName);
            byte[] dllBytes = txt.bytes;
            Assembly asm = Assembly.Load(dllBytes);
            print("[H]--> load asm name:" + asm.FullName);
        }
        index++;
        if(index < abList.Length)
        {            
            StartCoroutine(Load());
        }
        else
        {
            print("[H]--> all dll load ok");
            Assembly asm = Assembly.Load("GameModules");
            //绑定游戏入口类，执行游戏逻辑
            Type type = asm.GetType("GameInitialize");            
            this.gameObject.AddComponent(type);
            print("[H]--> dll run ok");
        }
    }
}
