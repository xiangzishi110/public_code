using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// 获取内存信息相关接口
/// </summary>
public static class MemoryTool
{
    private const float mMBSize = 1024f * 1024f;

    /// <summary>
    /// 获取当前系统未内存,单位MB
    /// </summary>
    public static float GetSystemUnusedMemory()
    {
		Logger.LogError("请添加需要的获取内存的实现");
		return 0f;//SDKHelper.GetSystemUnusedMemory();

    }

    /// <summary>
    /// 获取当前分配内存,单位MB
    /// </summary>
    public static float GetTotalAllocatedMemory()
    {
        return (float)(Profiler.GetTotalAllocatedMemory() / mMBSize);
    }

    /// <summary>
    /// 获取当前申请内存,单位MB
    /// </summary>
    public static float GetTotalReservedMemory()
    {
        return (float)(Profiler.GetTotalReservedMemory() / mMBSize);
    }

    /// <summary>
    /// 获取当前未使用申请内存,单位MB
    /// </summary>
    public static float GetTotalUnusedReservedMemory()
    {
        return (float)(Profiler.GetTotalUnusedReservedMemory() / mMBSize);
    }

    /// <summary>
    /// 获取当前Momo堆内存,单位MB
    /// </summary>
    public static float GetMonoHeapSize()
    {
        return (float)(Profiler.GetMonoHeapSize() / mMBSize);
    }

    /// <summary>
    /// 获取当前Momo使用内存,单位MB
    /// </summary>
    public static float GetMonoUsedSize()
    {
        return (float)(Profiler.GetMonoUsedSize() / mMBSize);
    }
}
