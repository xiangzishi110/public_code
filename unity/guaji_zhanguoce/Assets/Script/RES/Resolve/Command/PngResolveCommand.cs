
using UnityEngine;




    public class PngResolveCommand : BaseResolve
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
                                succed(ab, res as Texture2D);
                            }
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
                        succed(baseRes, baseRes as Texture2D);
                    }

                }
                base.Reset();
            }
            catch (System.Exception ex)
            {
                if (error != null)
                    error("PngResolve Exception:" + ex.Message);
                else if (succed != null)
                    succed(null, null);
                base.Reset();
            }
        }
    }

