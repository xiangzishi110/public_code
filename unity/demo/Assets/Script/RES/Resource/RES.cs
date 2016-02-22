using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 资源加载管理器
/// </summary>
public class RES : MonoBehaviour
{

	public static GameObject main;

	public static LoaderQueue loaderQueue;

	public static ResourceConfig resourceConfig;

	public static ABRecManage abRecManage;
	/// <summary>
	/// 加载进度条委托
	/// </summary>
	public static RESProgressUIDelegate resProgressUIDele;

	/// <summary>
	/// 执行加载进度条回调事件
	/// </summary>
	/// <param name="hashCode"></param>
	/// <param name="progress"></param>
	public static void RESProgressUINotify(int hashCode,float progress)
	{
		if (resProgressUIDele != null)
		{
			resProgressUIDele(hashCode, progress);
		}
	}

	/// <summary>
	/// 默认资源管理模型
	/// </summary>
	public static RESManageType resManageType = RESManageType.ASSETBUNDLE;

	/// <summary>
	/// 加载队列最多数量
	/// </summary>
	public static int QueueMaxNum = 15;
	/// <summary>
	/// 监听加载10秒，无反应默认错误处理
	/// </summary>
	public static float LoadErrorMaxTime = 10f;

	///// <summary>
	///// UI公共图集
	///// </summary>
	//private static List<UIAtlas> _UIPublicAtlas;

	/// <summary>
	/// 初始化
	/// </summary>
	public static void CheckContext() 
	{
		if (main == null)
		{
			main = new GameObject("RES_Main");

			resManageType = AppStarter.instance.resAssetBundle ? RESManageType.ASSETBUNDLE : RESManageType.RESOURCES;

			main.AddComponent<DontDestroy>();

			main.AddComponent<RES>();

			loaderQueue= main.AddComponent<LoaderQueue>();

			abRecManage = main.AddComponent<ABRecManage>();

			resourceConfig = new ResourceConfig();

			initDefaultDResolve();

		}
	   
	}
	public static void reset()
	{
		AbRecComplete = false;
		resProgressUIDele = null;//去掉loading回调委托
		resResolveDic = new Dictionary<string, System.Type>();
		if (main != null)
		{
			GameObject.Destroy(main);
			main = null;
		}
		
	}


	private static RES_INIT_COMPLETE initCompleteCallBack;

	/// <summary>
	/// 初始化RES模块
	/// </summary>
	/// <param name="p_configPath">资源配置表地址</param>
	/// <param name="initCallBack">初始化完成后的回调方法</param>
	public static void Init(RES_INIT_COMPLETE initCallBack)
	{

		CheckContext();

		initCompleteCallBack = initCallBack;

		//加载UI公共图集资源
		List<string> pathList = new List<string>();
		foreach (string path in RESUIDefine.PublicUIAtlasName)
		{
			pathList.Add(RESUIDefine.UIPublicPath + path);
		}
		LoadByURL(RES.GetLoadBasesByResPathOrName(pathList, RESFormat.PREFAB), LoadPublicAtlasFinish);
	}

	private static void LoadPublicAtlasFinish()
	{
		//if( null == _UIPublicAtlas )
		//{
		//    _UIPublicAtlas = new List<UIAtlas>();
		//}
		//_UIPublicAtlas.Clear();

		//foreach( string szAtlasName in RESUIDefine.PublicUIAtlasName )
		//{
		//    string szFullPath = RESUIDefine.UIPublicPath + szAtlasName;

		//    System.Object resObj = RES.GetRES(szFullPath);
		//    if (null == resObj)
		//    {
		//        continue;
		//    }

		//    GameObject atlasObj = resObj as GameObject;
		//    if (null == atlasObj)
		//    {
		//        continue;
		//    }

		//    UIAtlas atlas = atlasObj.GetComponent<UIAtlas>();
		//    _UIPublicAtlas.Add(atlas);
		//}

		//
		LoadBase loabBase = new LoadBase("ResourceConfig", "Config/ResourceConfig", RESFormat.XML, resManageType);
		LoadByURL(loabBase, initComplete);
	}
	private static void initComplete()
	{
		XMLNode xml = GetRES("Config/ResourceConfig",RESFormat.XML) as XMLNode;

		if (xml == null)
		{
			Logger.LogError("ResourceConfig null");
		}
		else
		{
			//int autoLoadBy = int.Parse(xml.GetValue("root>0>@loadBy"));


			//resManageType = (autoLoadBy == 1 ? RESManageType.ASSETBUNDLE : RESManageType.RESOURCES);


			resourceConfig.init(xml);
		}
		abRecManage.Init(AbRecCall);
	}

	public static bool AbRecComplete = false;
	private static void AbRecCall()
	{
		if (initCompleteCallBack != null)
		{

			initCompleteCallBack();
		}
	}

	static public UIAtlas GetUIPublicAtlas( string szAtlasName )
	{
		//if( null == _UIPublicAtlas )
		//{
		//    return null;
		//}

		//foreach (UIAtlas atlas in _UIPublicAtlas )
		//{
		//    if( atlas.name == szAtlasName )
		//    {
		//        return atlas;
		//    }
		//}
		
		return null;
	}

	#region 资源缓存

	/// <summary>
	/// 资源缓存
	/// key:
	///  Resources模式是 resPath,例如:Prefab/UI/Arena/ArenaUI
	///  AssetBundel模型是 resPath +“_”+文件类型，例如:Prefab/UI/Arena/ArenaUI_prefab
	/// </summary>
	private static Dictionary<string, IRESCache> resCache = new Dictionary<string, IRESCache>();



	/// <summary>
	/// 获取加载解析成功后缓存资源
	/// </summary>
	/// <param name="resPathOrName">资源相对路径不带后缀名或者ResourceConfig.xml中的name</param>
	/// <param name="resType"></param>
	/// <returns></returns>
	public static System.Object GetRES(string resPathOrName, string resType = RESFormat.PREFAB)
	{
		//配置文件 name
		if (ResourceConfigExits(resPathOrName))
		{
			return resourceConfig.GetItemOrGroup(resPathOrName).GetContent();
		}
		else
		{

			//相对路径

			//AssetBundle 模式
			if (resCache.ContainsKey(resPathOrName + "_" + resType))
			{
				return resCache[resPathOrName + "_" + resType].Res();
			}

			//Resouces 模式
			if (resCache.ContainsKey(resPathOrName))
			{
				return resCache[resPathOrName].Res();
			}

			Logger.LogError("RES.GetRES null :"+resPathOrName);

			return null;
		}
	 
	}
	/// <summary>
	/// 获取实例化之后并关联RESRecommend的GameObject
	/// 必须先加载
	/// </summary>
	/// <param name="resPathOrName">name或相对路径的唯一标签</param>
	/// <returns></returns>
	public static GameObject GetRESGo(string resPathOrName)
	{
		System.Object ob=null;
		bool bAssetMode = false;
		//配置文件 name
		if (ResourceConfigExits(resPathOrName))
		{
		  ob=resourceConfig.GetItemOrGroup(resPathOrName).GetContent();
		}
		else
		{
			//相对路径
			//Resouces 模式
			if (resCache.ContainsKey(resPathOrName))
			{
				ob= resCache[resPathOrName].Res();
			}
			//AssetBundle 模式
			if (resCache.ContainsKey(resPathOrName + "_" + RESFormat.PREFAB))
			{
				ob = resCache[resPathOrName + "_" + RESFormat.PREFAB].Res();
				bAssetMode = true;
			}
		}

		if (ob == null)
		{
			Logger.LogError("获取实例化之后并关联RESRecommend的GameObject Error : " + resPathOrName);
			return null;
		}
		 

		GameObject go = Instantiate(ob as Object) as GameObject;

		AddRESRecommend(go, new RESBase(resPathOrName, RESFormat.PREFAB));

		if (bAssetMode)
		{
			System.Object assetObj = resCache[resPathOrName + "_" + RESFormat.PREFAB].BaseRes();
			if (assetObj is AssetBundle)
			{
				AssetBundle ab = assetObj as AssetBundle;
				if (null != ab)
				{
					//Debug.LogError("Unload : " + resPathOrName);
					ab.Unload(false);
				}
			}
		}

		return go;
	}
	/// <summary>
	/// 判断是否已经缓存资源了。true,存在；
	/// ResourceConfig.xml中的name只能是item中的name
	/// </summary>
	/// <param name="resKey"></param>
	/// <returns></returns>
	public static bool ResExist(string resPathOrName, string resType,RESManageType resmanageType)
	{
		//配置文件 name
		if (ResourceConfigExits(resPathOrName) && resourceConfig.GetItemOrGroup(resPathOrName) is ResourceItem)
		{
			ResourceItem resource = resourceConfig.GetItemOrGroup(resPathOrName) as ResourceItem;
			//Debug.Log("判断 是否已经缓存资源 配置文件 : " + resPathOrName + " ; " + resource.path+" ; "+resource.type);
			return ResExist(resource.path, resource.type,resource.loadBy);
			
		}
		else
		{
			//相对路径

			//AssetBundle 模式
			if (resmanageType == RESManageType.ASSETBUNDLE)
			{
				//Debug.Log("AssetBundle模式是否存在缓存资源  : " + resPathOrName + "_" + resType + " ; " + resCache.ContainsKey(resPathOrName + "_" + resType));
				return resCache.ContainsKey(resPathOrName + "_" + resType);
			}
			else
			{
				//Resouces 模式

			   // Logger.Log("Resouces模式是否存在缓存资源  : " + resPathOrName + " ; " + resCache.ContainsKey(resPathOrName));
				return resCache.ContainsKey(resPathOrName);
			}
			
		}
	}

	
	/// <summary>
	/// 判断是否已经缓存资源了。true,存在；
	/// ResourceConfig.xml中的name只能是item中的name
	/// </summary>
	/// <param name="resKey"></param>
	/// <returns></returns>
	public static bool ResExist(string resPathOrName, string resType)
	{
		//配置文件 name
		if (ResourceConfigExits(resPathOrName) && resourceConfig.GetItemOrGroup(resPathOrName) is ResourceItem)
		{
			ResourceItem resource = resourceConfig.GetItemOrGroup(resPathOrName) as ResourceItem;
			//Debug.Log("判断 是否已经缓存资源 配置文件 : " + resPathOrName + " ; " + resource.path + " ; " + resource.type);
			return ResExist(resource.path, resource.type);

		}
		else
		{
			//相对路径
			//AssetBundle 模式
			if (resCache.ContainsKey(resPathOrName + "_" + resType))
			{
			   //Logger.Log("AssetBundle模式已经缓存资源  : " + resPathOrName + "_" + resType);
				return true;
			}
			else if (resCache.ContainsKey(resPathOrName))
			{
				//Resouces 模式
			   //Logger.Log("Resouces模式已经缓存资源  : " + resPathOrName);
				return true;
		   }
			
			//Debug.Log("没有缓存资源  : " + resPathOrName + ";" + resType);
			return false;
		}
	}
   
	/// <summary>
	/// 添加资源缓存
	/// </summary>
	/// <param name="baseRes">原始资源</param>
	/// <param name="res">解析后资源</param>
	/// <param name="loadBase">加载资源封装数据</param>
	public static void AddRESCache(System.Object baseRes, System.Object res, LoadBase loadBase)
	{
		   //Logger.Log("添加资源缓存:" + loadBase.resPath + "; " + loadBase.resType + ";" + loadBase.resManageType +" ; "+ loadBase.resKey);
		if (loadBase == null )
		{
			Logger.LogError("RES AddRESCache LoadBase null");
			return;
		}
		if (string.IsNullOrEmpty(loadBase.resKey))
		{
			Logger.LogError("RES AddRESCache LoadBase.resKey null");
			return;
		}
		if (res==null)
		{
			Logger.LogError("RES AddRESCache res null:"+loadBase.resKey);
			return;
		}
			if (resCache.ContainsKey(loadBase.resKey))
			{
				 Logger.Log("Error 资源缓存已经存在了:" + loadBase.resKey+"; "+loadBase.resManageType);
			}
			else
			{
				IRESCache cache = new RESCache();
				cache.Init(loadBase.resKey, loadBase.resType, baseRes, res, loadBase.resManageType);
				resCache[loadBase.resKey] = cache;
				//Logger.Log(" 资源缓存:" +loadBase.resPath + "; " + loadBase.resType + ";" + loadBase.resManageType + " ; " + loadBase.resKey+" ; 个数: "+resCache.Count);


				//AssetBundle资源，引用其它资源引用数量先自动加1
				if (loadBase.resManageType == RESManageType.ASSETBUNDLE)
				{
					if (abRecManage.RecExist(loadBase.resPath))
					{
						foreach (var item in abRecManage.GetABRec(loadBase.resPath).GetRecAssets)
						{
							//Logger.Log("AssetBundle资源，引用其它资源引用数量先自动加1: " + item.resPath + "_" + item.resType + " ; ");
							if (item == null)
							{
								Logger.LogError("RES AddRESCache abRecManage GetRecAssets null:" + loadBase.resKey);
							}
							else
							{
								AddRESCacheRecommend(item.resPath, item.resType, 1);
							}
					
						}
					}
				}
			}
		
		
	}
	/// <summary>
	/// 减少资源缓存引用
	/// </summary>
	/// <param name="resKey"></param>
	/// <param name="num"></param>
	public static void SubRESCacheRecommend(string resKey,string resType,int num = 1)
	{
		//配置文件 name
		if (ResourceConfigExits(resKey) && resourceConfig.GetItemOrGroup(resKey) is ResourceItem)
		{
			ResourceItem resource = resourceConfig.GetItemOrGroup(resKey) as ResourceItem;

			SubRESCacheRecommend(resource.path, resource.type);
		}
		else
		{
			//相对路径
		   
			//AssetBundle模式
			if (resCache.ContainsKey(resKey + "_" + resType))
			{
				//Logger.Log("减少资源缓存引用数量 前 : " + resKey + "_" + resType + " ; " + resCache[resKey + "_" + resType].GetRecommend());

				if (resCache[resKey + "_" + resType].SubRecommend(num))
				{
					resCache.Remove(resKey + "_" + resType);

					//Logger.Log("减少资源缓存引用数量 为 0 释放内存了: " + resCache.ContainsKey(resKey + "_" + resType) + "  ; " + resCache.Count);
				   
					//销毁时，引用其它资源的引用数量自动减1。
					if (abRecManage.RecExist(resKey))
					{
						//Logger.Log("减少引用到其它资源缓存引用数量 前 : " + resKey + "_" + resType);

						foreach (var item in abRecManage.GetABRec(resKey).GetRecAssets)
						{
							//Logger.Log("释放内存,引用其它资源的引用数量自动减1 前 : " + item.resPath + "_" + item.resType + " ; ");
							if (item == null)
							{
								Logger.LogError("RES SubRESCacheRecommend AB GetRecAssets null:" + resKey+";"+resType);
							}
							else
							{
								SubRESCacheRecommend(item.resPath, item.resType);
							}
						}
					}
					 
				}
				else
				{
					 //Logger.Log("减少资源缓存引用数量 后 : " + resKey + "_" + resType + " ; " + resCache[resKey + "_" + resType].GetRecommend());
				}

				if (abRecManage.RecExist(resKey))
				{
					//Logger.Log("减少引用到其它资源缓存引用数量 前 : " + resKey + "_" + resType);

					foreach (var item in abRecManage.GetABRec(resKey).GetRecAssets)
					{
						//Logger.Log("减少引用到其它资源缓存引用数量 前 : " + item.resPath + "_" + item.resType + " ; ");
						if (item == null)
						{
							Logger.LogError("RES SubRESCacheRecommend abRecManage GetRecAssets null:" + resKey+";"+resType);
						}
						else
						{
							SubRESCacheRecommend(item.resPath, item.resType);
						}
					}
				}
			}
			else if (resCache.ContainsKey(resKey))
				{
					//Resources模式

					//Logger.Log("减少资源缓存引用数量 前 : " + resKey + " ; " + resCache[resKey].GetRecommend());
					if (resCache[resKey].SubRecommend(num))
					{
						//Logger.Log("Resources模式 减少资源缓存引用数量 为 0 释放内存了: " + resKey + " ; " + resCache.ContainsKey(resKey)+"  ; "+resCache.Count);

						resCache.Remove(resKey);
					}
					else
					{
						//Logger.Log("Resources模式 减少资源缓存引用数量 后 : " + resKey + " ; " + resCache[resKey].GetRecommend());
					}
				}
		}
		
	}
	/// <summary>
	/// 增加资源缓存引用
	/// </summary>
	/// <param name="resKey"></param>
	/// <param name="num"></param>
	public static void AddRESCacheRecommend(string resKey,string resType, int num = 1)
	{
	   
		//配置文件 name
		if (ResourceConfigExits(resKey) && resourceConfig.GetItemOrGroup(resKey) is ResourceItem)
		{
			ResourceItem resource = resourceConfig.GetItemOrGroup(resKey) as ResourceItem;
		 
			AddRESCacheRecommend(resource.path, resource.type);
			
		}
		else
		{
			//相对路径

					   
			//AssetBundle模式
			if (resCache.ContainsKey(resKey + "_" + resType))
			{
			   //Logger.Log("增加资源缓存引用数量 前 : " + resKey + "_" + resType + " ; " + resCache[resKey + "_" + resType].GetRecommend());
				resCache[resKey + "_" + resType].AddRecommend(num);
			   //Logger.Log("增加资源缓存引用数量 后 : " + resKey + "_" + resType + " ; " + resCache[resKey + "_" + resType].GetRecommend());

			   if (abRecManage.RecExist(resKey))
			   {
				   //Debug.Log("增加引用到其它资源缓存引用数量 前 : " + resKey+"_"+resType);

				   foreach (var item in abRecManage.GetABRec(resKey).GetRecAssets)
				   {
					   //Logger.Log("增加引用到其它资源缓存引用数量 前 : " + item.resPath + "_" + item.resType + " ; ");
					   if (item == null)
					   {
						   Logger.LogError("RES AddRESCacheRecommend abRecManage GetRecAssets null :" + resKey + ";" + resType);
					   }
					   else
					   {
						   AddRESCacheRecommend(item.resPath, item.resType, num);
					   }
				   }
			   }
			}
			else if (resCache.ContainsKey(resKey))
				{
					//Resources模式
				   //Logger.Log("Resources模式 增加资源缓存引用数量 前 : " + resKey + " ; " + resCache[resKey].GetRecommend());
					resCache[resKey].AddRecommend(num);
				   //Logger.Log("Resources模式 增加资源缓存引用数量 后 : " + resKey + " ; " + resCache[resKey].GetRecommend());
				}
		}
	   
	}

	/// <summary>
	/// 添加资源缓存引用,RESRecommend
	/// 资源缓存引用单列，一个GameObject只能有一个
	/// </summary>
	/// <param name="go">资源所有关联的GameObject</param>
	/// <param name="resKey">资源标签,相对路径</param>
	public static void AddRESRecommend(GameObject go, RESBase resKey)
	{
		if (go.GetComponent<RESRecommend>() == null)
		{
			go.AddComponent<RESRecommend>();
		}
		go.GetComponent<RESRecommend>().Init(resKey);
	}

	/// <summary>
	/// 直接释放资源内存
	/// </summary>
	/// <param name="resKey"></param>
	public static void ReleaseResCache(string resKey,string resType)
	{
		
		 //配置文件 name
		if (ResourceConfigExits(resKey) && resourceConfig.GetItemOrGroup(resKey) is ResourceItem)
		{
			ResourceItem resource = resourceConfig.GetItemOrGroup(resKey) as ResourceItem;

			ReleaseResCache(resource.path, resource.type);

		}
		else
		{
			//相对路径

			//Resources模式
			if (resCache.ContainsKey(resKey))
			{
				resCache[resKey].Release();
				Logger.Log("Res 直接释放资源内存:" + resKey);
				resCache.Remove(resKey);

			}
			//AssetBundle模式
			if (resCache.ContainsKey(resKey + "_" + resType))
			{
				//引用数量 多加 1,加载成功引用其它资源自动加1；
				int num = resCache[resKey + "_" + resType].GetRecommend() + 1;
				Logger.Log("AB 直接释放资源内存:" + resKey + "_" + resType + " : " + num);
				resCache[resKey + "_" + resType].Release();
			
				resCache.Remove(resKey + "_" + resType);

				if (abRecManage.RecExist(resKey))
				{
				   //Logger.Log("AB  直接释放资源内存,所引用的资源引用数量自动减1  ");
					foreach (var item in abRecManage.GetABRec(resKey).GetRecAssets)
					{
						Logger.Log("AB 直接释放资源内存,所引用的资源引用数量自动减  ： " +num +" ; "+ item.resPath);
						if (item == null)
						{
							Logger.LogError("RES ReleaseResCache abRecManage GetRecAssets null :" + resKey+";"+resType);
						}
						else
						{
							SubRESCacheRecommend(item.resPath, item.resType, num);
						}
					}
				}
			}
		}
	   
	}

	/// <summary>
	/// 释放所有资源
	/// </summary>
	public static void ReleaseAll() 
	{
		// Logger.LogError("释放所有资源 <<<<<< ");
		if (resCache != null)
		{
			foreach (var item in resCache.Values)
			{
				if (item != null)
				{
					// Logger.LogError("释放资源 <<<<<< " +item.ResKey());

					item.Release();
				}
				  
			}

			resCache.Clear();
		}
	 
	}

	#endregion 资源缓存


	#region 加载资源模块

	/// <summary>
	/// 根据资源相对路径不带后缀名或resourceconfig中的name，获取加载任务集合。
	/// 所有资源都是为同类型，加载模式都相同。
	/// 返回默认数量为0
	/// </summary>
	/// <param name="resPathOrName"></param>
	/// <returns></returns>
	public static LoadBase[] GetLoadBasesByResPathOrName(List<string> resPathOrName,string resType,RESManageType resManageType = RESManageType.AUTO)
	{
		if (resPathOrName == null || resPathOrName.Count == 0)
		{
			Logger.LogError("RES GetLoadBasesByResPathOrName resPathOrName null");
			return new LoadBase[0];
		}
		Dictionary<string, LoadBase> loadBases = new Dictionary<string, LoadBase>();

		LoadBase loadBase = null;
		foreach (var resKey in resPathOrName)
		{
			// 是否是ResourceConfig 中的name
			if (ResourceConfigExits(resKey))
			{
				foreach (var item in GetLoadBase(resKey))
				{
					if (loadBases.ContainsKey(item.resKey) == false)
						loadBases.Add(item.resKey, item);
				}
			}
			else
			{
				// url 资源相对路径 加载
				loadBase = GetLoadBase(resKey, resType, resManageType);
				if (loadBase != null && loadBases.ContainsKey(loadBase.resKey) == false)
					loadBases.Add(loadBase.resKey, loadBase);
			}
		}
	   

		return new List<LoadBase>(loadBases.Values).ToArray();
	}
	/// <summary>
	/// 根据ReourceItem,获取对应的加载任务
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public static LoadBase GetLoadBase(ResourceItem item)
	{
		if (item == null)
			return null;

		LoadBase loadBase = null;
		if (string.IsNullOrEmpty(item.suffix))
		{
			loadBase = new LoadBase(item.Name, item.path, item.type, item.loadBy);
		}
		else
		{
			loadBase = new LoadBase(item.Name, item.path, item.type, RESTool.LoadAbPathByAbPath(item.path,item.suffix, item.loadBy), item.loadBy);
		}
		return loadBase;
	}
	/// <summary>
	///获取加载任务
	/// ResManageType，为默认
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public static LoadBase GetLoadBase(string resPath, string resType, RESManageType resManageType = RESManageType.AUTO)
	{
		if (resManageType == RESManageType.AUTO)
		{
			resManageType = RES.resManageType;
		}
		return new LoadBase(RESTool.GetFileName(resPath), resPath, resType, resManageType);
	}
	/// <summary>
	/// 根据ResourceGroup,获取对应的加载任务集合
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public static LoadBase[] GetLoadBase(ResourceGroup group)
	{
		if (group == null)
		{
			Logger.LogError("RES GetLoadBase group null");
			return new LoadBase[0];
		}
		LoadBase loadBase = null;
		Dictionary<string, LoadBase> loadBases = new Dictionary<string, LoadBase>();

		foreach (var resourceItem in group.children)
		{
			loadBase = GetLoadBase(resourceItem as ResourceItem);

			if (loadBase != null && loadBases.ContainsKey(loadBase.resKey) == false)
				loadBases.Add(loadBase.resKey, loadBase);
		}
		loadBase = null;
		return new List<LoadBase>(loadBases.Values).ToArray();

	}
	/// <summary>
	/// 根据ResourceConfig 中的name, 获取对应的加载任务集合
	/// 不存在返回 null
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static LoadBase[] GetLoadBase(string name)
	{

		IResource resource = resourceConfig.GetItemOrGroup(name);

		if (resource == null)
		{
			return null;
		}

		if (resource is ResourceGroup)
		{
			return GetLoadBase(resource as ResourceGroup);
		 
		}
		else
		{
			return new LoadBase[] { GetLoadBase(resource as ResourceItem) };
		}

	}
	/// <summary>
	/// 获取加载场景LoadBase,场景必须是AssetBundle
	/// 缓存key为sceneName_unity3d不是ConstData.ScenePath + sceneName_unity3d
	/// </summary>
	/// <param name="sceneName"></param>
	/// <returns></returns>
	public static LoadBase GetSceneLoadBase(string sceneName)
	{
		if (resManageType == RESManageType.ASSETBUNDLE)
		{
			LoadBase loadBase = new LoadBase(sceneName,sceneName, RESFormat.UNITY3D, RESTool.LoadAbPathByPath(RESUIDefine.ScenePath + sceneName, RESFormat.UNITY3D, RESManageType.ASSETBUNDLE, RESFormat.UNITY3D), RESManageType.ASSETBUNDLE);
			return loadBase;
		}
		else
		{
			return null;
		}
	}
	

	/// <summary>
	/// ResouceConfig.xml 是否包含此name
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static bool ResourceConfigExits(string name)
	{
		return resourceConfig.ResourceExits(name);
	}
	/// <summary>
	/// 单个加载资源，根据ResourceConfig中的配置name
	/// </summary>
	/// <param name="name">资源名称</param>
	/// <param name="succed">加载成功回调</param>
	/// <param name="progress">加载进度回调</param>
	/// <param name="error">加载错误回调</param>
	/// <param name="showProgressUI">是否显示进度条，默认Disable不显示</param>
	/// <param name="queue">加载队列模式，默认普通加载</param>
	public static void LoadByName(string name, RESSuccedDelegate succed = null, ActivityEnum showProgressUI = ActivityEnum.Disable, LoadQueueEnum queue = LoadQueueEnum.Normal, RESProgressDelegate progress = null, RESErrorDelegate error = null)
	{
		
		if (ResourceConfigExits(name)==false)
		{
			Logger.LogError("ResourceConfig 不存在:" + name);
			if (error != null)
			{
				error(null, "ResourceConfig 不存在:" + name);
			}
		 
			return;
		}
		   //第一步：由 name 获取 实际的LoadBase
	   LoadBase[]  loadBases=   GetLoadBase(name);
		if(loadBases==null || loadBases.Length==0)
		{
				Logger.LogError("ResourceConfig 不存在:" + name);
		}
		else
		{
			//测试日志
			/*
			foreach (var item in loadBases)
			{
				Logger.LogError("ResourceConfig loadBase:" + item.resPath+" ; "+item.resType);
			}
			*/
		   LoadByURL(loadBases, succed, showProgressUI, queue, progress, error);
		}
		
	}



	/// <summary>
	/// 批量加载资源
	/// </summary>
	/// <param name="loadBases">加载资源封装数据数组</param>
	/// <param name="succed">加载成功回调</param>
	/// <param name="progress">加载进度回调</param>
	/// <param name="error">加载错误回调</param>
	/// <param name="showProgressUI">是否显示进度条，默认Disable不显示</param>
	/// <param name="queue">加载队列模式，默认普通加载</param>
	public static void LoadByURL(LoadBase[] loadBases, RESSuccedDelegate succed = null, ActivityEnum showProgressUI = ActivityEnum.Disable, LoadQueueEnum queue = LoadQueueEnum.Normal,RESProgressDelegate progress = null, RESErrorDelegate error = null)
	{
		if(loadBases==null || loadBases.Length==0)
		{
			Logger.LogError("资源加载任务为 null");
			if (succed != null)
			{
				succed();
			}

			return;
		}

		CheckContext();

		BatchProxy batchProxy = GetBattchProxy();

		batchProxy.AddDelegate(succed, progress, error);

		batchProxy.UpdateLoadInfo(GetTrueLoadBaseByUrl(loadBases), showProgressUI);

		loaderQueue.AddProxy(batchProxy, queue);

	}
	/// <summary>
	/// 获取真正的url LoadBase 加载封装数据包
	/// </summary>
	/// <param name="loadBases"></param>
	/// <returns></returns>
	public static LoadBase[] GetTrueLoadBaseByUrl(LoadBase[] loadBases)
	{
		Dictionary<string, LoadBase> loadBaseDic = new Dictionary<string, LoadBase>();

		foreach (var item in loadBases)
		{
			if (RecExits(item.resPath, item.resType, item.resManageType))
			{
				foreach (var recAssetsLoad in GetRecAssetsLoadBase(item.resPath))
				{
					if (recAssetsLoad != null && !loadBaseDic.ContainsKey(recAssetsLoad.resKey))
					{
						//Logger.Log("添加加载引用资源 ： " + recAssetsLoad.resPath + "; " + recAssetsLoad.resType + " ; " + recAssetsLoad.resKey);
						loadBaseDic.Add(recAssetsLoad.resKey, recAssetsLoad);
					}
				}
			}
			else
			{
				if (item != null && !loadBaseDic.ContainsKey(item.resKey))
				{
					//Logger.Log("添加加载本身资源 ： " + item.resPath + "; " + item.resType + " ; " + item.resKey);
					loadBaseDic.Add(item.resKey, item);
				}
			}
		}
		return new List<LoadBase>( loadBaseDic.Values ).ToArray();
	}
	/// <summary>
	/// 单个加载资源
	/// </summary>
	/// <param name="loadBase">加载资源封装数据</param>
	/// <param name="succed">加载成功回调</param>
	/// <param name="progress">加载进度回调</param>
	/// <param name="error">加载错误回调</param>
	/// <param name="showProgressUI">是否显示进度条，默认Disable不显示</param>
	/// <param name="queue">加载队列模式，默认普通加载</param>
	public static void LoadByURL(LoadBase loadBase, RESSuccedDelegate succed = null, ActivityEnum showProgressUI = ActivityEnum.Disable, LoadQueueEnum queue = LoadQueueEnum.Normal, RESProgressDelegate progress = null, RESErrorDelegate error = null)
	{
		if (loadBase == null)
		{
			Logger.LogError("LoadByURL 资源加载任务为 null");
			if (succed != null)
			{
				succed();
			}

			return;
		}
		CheckContext();
		LoadBase[] loadBases=null;
		if (RecExits(loadBase.resPath,loadBase.resType,loadBase.resManageType))
		{
			
			Dictionary<string, LoadBase> loadBaseDic = new Dictionary<string, LoadBase>();

			foreach (var recAssetsLoad in GetRecAssetsLoadBase(loadBase.resPath))
			{
				if (recAssetsLoad != null && !loadBaseDic.ContainsKey(recAssetsLoad.resKey))
				{
				  //  Logger.Log("添加加载 recAssetsLoad ： " + recAssetsLoad.resPath + "; " + recAssetsLoad.resType + " ; " + recAssetsLoad.resKey);
					loadBaseDic.Add(recAssetsLoad.resKey, recAssetsLoad);
				}
			}
			if(loadBaseDic.Count>0)
				loadBases = new List<LoadBase>(loadBaseDic.Values).ToArray();
		}
		else
		{
		   
			loadBases = new LoadBase[1] { loadBase };
		}
		if (loadBase == null || loadBases.Length==0)
		{
			Logger.LogError("LoadByURL loadBases 资源加载任务为 null");
			if (succed != null)
			{
				succed();
			}

			return;
		}
		BatchProxy batchProxy = GetBattchProxy();

		batchProxy.AddDelegate(succed, progress, error);


		batchProxy.UpdateLoadInfo(loadBases, showProgressUI);

		loaderQueue.AddProxy(batchProxy, queue);
	}

	/// <summary>
	/// 判断资源是否引用了其它资源。true，引用了
	/// </summary>
	/// <param name="loadBase"></param>
	/// <returns></returns>
	public static bool RecExits(string resPath,string resType ,RESManageType  resManageType)
	{
		// 加载任务是否会引用其它资源
		if (resManageType == RESManageType.ASSETBUNDLE && resType == RESFormat.PREFAB)
		{
			return abRecManage.RecExist(resPath);
		}
		return false;
	}
	/// <summary>
	/// 获取引用其它资源,包含本身资源的加载 LoadBase
	/// </summary>
	/// <param name="loadBase"></param>
	/// <returns></returns>
	public static LoadBase[] GetRecAssetsLoadBase(string resPath)
	{
				ABRec abRec = abRecManage.GetABRec(resPath);
				if (abRec != null)
				{
					List<LoadBase> loadBases = new List<LoadBase>();
			  
					foreach (var item in abRec.abInfoRecAssets)
					{
						loadBases.Add(new LoadBase(item.resName, item.resPath, item.resType, RESManageType.ASSETBUNDLE));
					}
					return loadBases.ToArray();
				}
			return null;
	}
	/// <summary>
	/// 获取空闲的加载批量代理
	/// </summary>
	/// <returns></returns>
	public static BatchProxy GetBattchProxy()
	{
		/*
		BatchProxy[] batchProxylist =main.GetComponentsInChildren<BatchProxy>();
		foreach (BatchProxy item in batchProxylist)
		{
			if (item.GetAcitivity==ActivityEnum.Disable)
			{
				return item;
			}
		}
		 */
		return main.AddComponent<BatchProxy>();;
	}
	/// <summary>
	/// 获取空闲的加载器
	/// </summary>
	/// <param name="resManageType"></param>
	/// <returns></returns>
	public static ILoad GetLoader(RESManageType resManageType)
	{
		//Resources加载
		if (resManageType == RESManageType.RESOURCES)
		{
			/*
			foreach (var item in main.GetComponentsInChildren<ResLoadCommand>())
			{
				if (item.IsActivity == ActivityEnum.Disable)
				{
					return item;
				}
			}
			*/
			return main.AddComponent<ResLoadCommand>();
		}
		else
		{
			//www 加载
			/*
			foreach (var item in main.GetComponentsInChildren<ABLoadCommand>())
			{
				if (item.IsActivity == ActivityEnum.Disable)
				{
					return item;
				}
			}
			*/
			return main.AddComponent<ABLoadCommand>();
		}
	}

	/// <summary>
	/// 加载scene assetbundle unity3d,assetbundle模式
	/// </summary>
	/// <param name="sceneName">场景名称</param>
	/// <param name="succed"></param>
	/// <param name="showProgressUI"></param>
	/// <param name="queue"></param>
	/// <param name="progress"></param>
	/// <param name="error"></param>
	public static void LoadSceneUnitye3d(string sceneName, RESSuccedDelegate succed = null, ActivityEnum showProgressUI = ActivityEnum.Disable, LoadQueueEnum queue = LoadQueueEnum.Normal, RESProgressDelegate progress = null, RESErrorDelegate error = null)
	{
		LoadByURL(GetSceneLoadBase(sceneName), succed, showProgressUI, queue, progress, error);
	}

	public static System.Object GetBaseRES(string resPathOrName, string resType = RESFormat.PREFAB)
	{
		//配置文件 name
		if (ResourceConfigExits(resPathOrName))
		{
			return resourceConfig.GetItemOrGroup(resPathOrName).GetBaseContent();
		}
		else
		{

			//相对路径

			//AssetBundle 模式
			if (resCache.ContainsKey(resPathOrName + "_" + resType))
			{
				return resCache[resPathOrName + "_" + resType].BaseRes();
			}

			//Resouces 模式
			if (resCache.ContainsKey(resPathOrName))
			{
				return resCache[resPathOrName].BaseRes();
			}

			Logger.LogError("RES.GetRES null :" + resPathOrName);

			return null;
		}
	}

	/// <summary>
	/// 是否正在加载
	/// </summary>
	public static bool IsLoading( string szKey )
	{
		if( null == main )
		{
			return false;
		}

		BatchProxy[] batchProxylist = main.GetComponentsInChildren<BatchProxy>();
		foreach (BatchProxy item in batchProxylist)
		{
			if (item.GetAcitivity == ActivityEnum.Disable)
			{
				continue;
			}

			if (item.IsInProxy(szKey))
			{
				return true;
			}
		}

		return false;
	}

	#endregion 加载资源模块

	#region 解析模块

	private static Dictionary<string, System.Type> resResolveDic=new Dictionary<string,System.Type> ();
	public static BaseResolve GetResolve(string str)
	{

		CheckContext();

		if (resResolveDic.ContainsKey(str))
		{
			System.Type type = resResolveDic[str];
			/*
			Component[] decodeList = RES.main.GetComponentsInChildren(type);
			foreach (BaseResolve item in decodeList)
			{
				if (item.IsActivity==ActivityEnum.Disable)
				{
					return item;
				}
			}
			*/
			return RES.main.gameObject.AddComponent(type) as BaseResolve;
		}
		else
		{
			Logger.LogError("RES GetDecode Error : " + str + " resResolveDic not contain");
			return null;
		}
	}
	/// <summary>
	/// 添加一个解析器
	/// </summary>
	/// <param name="suffix"></param>
	/// <param name="type"></param>
	public static void addResolve(string suffix, System.Type type)
	{
		if (!resResolveDic.ContainsKey(suffix))
		{
			resResolveDic.Add(suffix, type);
		}
	}
	private static void initDefaultDResolve()
	{
		addResolve(RESFormat.PREFAB, typeof(PrefabResolveCommand));
		addResolve(RESFormat.PNG, typeof(PngResolveCommand));
		addResolve(RESFormat.MAT, typeof(MatResolveCommand));
		addResolve(RESFormat.XML, typeof(XmlResolveCommand));
		addResolve(RESFormat.CSV, typeof(CsvResolveCommand));
		addResolve(RESFormat.TXT, typeof(TxtResolveCommand));
		addResolve(RESFormat.FBX, typeof(FBXResolveCommand));
		addResolve(RESFormat.SHADER, typeof(ShaderResolveCommand));
		addResolve(RESFormat.UNITY3D, typeof(Unity3DResolveCommand));
		addResolve(RESFormat.Bytes, typeof(BytesResolveCommand));
		addResolve(RESFormat.Mp3, typeof(Mp3ResolveCommand));
		addResolve(RESFormat.TTF, typeof(TTFResolveCommand));
	}



	#endregion 解析模块
}


