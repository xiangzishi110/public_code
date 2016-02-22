using UnityEngine;

   public class BytesResolveCommand : BaseResolve
    {
       public override void Resolve(System.Object baseRes, RESResolveSuccedDelegate succed, RESResolveProgressDelegate progress, RESResolveErrorDelegate error)
       {
           try
           {

               base.IsActivity = ActivityEnum.Activity;
               if (baseRes is AssetBundle)
               {
                   AssetBundle ab = baseRes as AssetBundle;
                   if (ab == null)
                   {
                       if (progress != null)
                       {
                           progress(1f);
                       }

                       if (error != null)
                           error("Error AssetBundle null ");
                   }
                   else
                   {
                       UnityEngine.Object res = ab.mainAsset;

                       if (progress != null)
                       {
                           progress(1f);
                       }

                       if (res == null)
                       {
                           if (error != null)
                               error("Error AssetBundle mainAsset null ");
                       }
                       else
                       {
                           if (succed != null)
                           {
                               succed(ab, (res as TextAsset).bytes);
                           }
                       }

                       try
                       {
                           if (ab != null)
                               ab.Unload(false);
                       }
                       catch (System.Exception ex)
                       {
                           Logger.LogError("BytesResolveCommand Unload Error:" + ex.Message);
                       }
                   }


               }
               else
               {
                   //Resources.Load 返回的原始资源
                   if (progress != null)
                   {
                       progress(1f);
                   }
                   if (succed != null)
                   {
                       succed(null, (baseRes as TextAsset).bytes);
                   }

                   baseRes = null;
               }
               base.Reset();
           } 
           catch (System.Exception ex)
           {
               if (error != null)
                   error("BytesResolve Exception:" + ex.Message);
               else if (succed != null)
                   succed(null, null);
               base.Reset();
           }
       }
    }

