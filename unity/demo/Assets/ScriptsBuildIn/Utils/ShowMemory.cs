using UnityEngine;
using System.Collections;

public class ShowMemory : MonoBehaviour
{
    GUIStyle mStyle;
    public const float m_KBSize = 1024f * 1024f;

    void Start()
    {
        mStyle = new GUIStyle();
        mStyle.normal.textColor = Color.green;
        mStyle.fontSize = 20;
    }

    void OnGUI()
    {

        GUI.Label(new Rect(10, 10, 800, 100), GetMemoryInfo(), mStyle);
    }
   
    public string GetMemoryInfo()
    {
        float totalSysUnusedMemory = MemoryTool.GetSystemUnusedMemory();
        float totalMemory = MemoryTool.GetTotalAllocatedMemory();
        float totalReservedMemory = MemoryTool.GetTotalReservedMemory();
        float totalUnusedReservedMemory = MemoryTool.GetTotalUnusedReservedMemory();
        float monoHeapSize = MemoryTool.GetMonoHeapSize();
        float monoUsedSize = MemoryTool.GetMonoUsedSize();

        return string.Format("TotalSysUnusedMemory: {0}MB \nTotalAllocatedMemory：{1}MB  \nTotalReservedMemory：{2}MB \nTotalUnusedReservedMemory:{3}MB \nMonoHeapSize:{4}MB \nMonoUsedSize:{5}MB", 
            totalSysUnusedMemory.ToString("f2"),
            totalMemory.ToString("f2"), 
            totalReservedMemory.ToString("f2"), 
            totalUnusedReservedMemory.ToString("f2"), 
            monoHeapSize.ToString("f2"), 
            monoUsedSize.ToString("f2"));

    }
}
