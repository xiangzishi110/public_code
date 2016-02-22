using UnityEngine;

   public class RESShaderTool : MonoBehaviour
    {
   
       void Start() 
       {
           Execution();
       }

       private void Execution() 
       {
           if (RES.resManageType == RESManageType.ASSETBUNDLE)
           {
               if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
               {
                   foreach (var item in GetComponentsInChildren<Renderer>())
                   {
                       foreach (var mater in item.materials)
	                    {
                            mater.shader = Shader.Find(mater.shader.name);
	                    }  
                   } 
               }
            
           }
          
       }

    }

