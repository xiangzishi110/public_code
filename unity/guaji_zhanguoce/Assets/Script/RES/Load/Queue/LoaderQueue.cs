using UnityEngine;
using System.Collections.Generic;

    /// <summary>
    /// 加载队列,
    /// 普通加载队列,优先加载。
    /// 闲时加载队列,普通加载队列全部加载完成后，再加载
    /// </summary>
    public class LoaderQueue : MonoBehaviour
    {


    /// <summary>
    /// 普通加载队列
    /// </summary>
    private List<BatchProxy> normalQueue;
    /// <summary>
    /// 闲时加载队列
    /// </summary>
    private List<BatchProxy> idleQueue;
    /// <summary>
    /// 当前正在进行加载的数量
    /// </summary>
    private int _loadingCount;

    public LoaderQueue()
    {
        normalQueue = new List<BatchProxy>();
        idleQueue = new List<BatchProxy>();
    }
    /// <summary>
    /// 开始加载
    /// </summary>
    /// <param name="proxy"></param>
    /// <param name="queue"></param>
    public void AddProxy(BatchProxy proxy, LoadQueueEnum queue)
    {
        if (queue == LoadQueueEnum.Normal)
        {
            PushToQueue(normalQueue, proxy);
        }
        else
        {
            PushToQueue(idleQueue, proxy);
        }

        CheckQueue();
    }

    private void PushToQueue(List<BatchProxy> list, BatchProxy proxy)
    {
        list.Add(proxy);
    }
    /// <summary>
    ///加载
    /// </summary>
    private void CheckQueue()
    {
        while (_loadingCount < RES.QueueMaxNum)
        {
            BatchProxy proxy;
            if (normalQueue.Count > 0)
            {
                proxy = normalQueue[0];
                normalQueue.RemoveAt(0);
                proxy.AddDelegate(ProxyLoadComplete, null, null);
                proxy.Load();
                _loadingCount++;
            }
            else if (idleQueue.Count > 0)
            {
                proxy = idleQueue[0];
                idleQueue.RemoveAt(0);
                proxy.AddDelegate(ProxyLoadComplete, null, null);
                proxy.Load();
                _loadingCount++;
            }
            else
            {
                return;
            }
        }
    }

    /// <summary>
    /// 加载队列完成失败都执行此回调
    /// </summary>
    /// <param name="loadBases"></param>
    private void ProxyLoadComplete()
    {
        //Logger.LogWarning("LoaderQueue 加载队列完成：" + normalQueue.Count);
        _loadingCount--;
        CheckQueue();
    }
    }

