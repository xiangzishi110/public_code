using UnityEngine;

 public class Unity3DResolveCommand : BaseResolve
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

                    if (progress != null)
                    {
                        progress(1f);
                    }

                    if (succed != null)
                    {
                        succed(ab, ab);
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
                if (error != null)
                    error("Resources no  unity3d  ");

            }
            base.Reset();
        }
        catch (System.Exception ex)
        {
            if (error != null)
                error("Unity3DResolve Exception:" + ex.Message);
            else if (succed != null)
                succed(null, null);
            base.Reset();
        }

    }
}


