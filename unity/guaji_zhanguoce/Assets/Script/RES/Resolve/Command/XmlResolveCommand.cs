using UnityEngine;


    /// <summary>
    /// xml解析。解析为XmlNode
    /// </summary>
    public class XmlResolveCommand : BaseResolve
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
                                succed(ab, new XMLParser().Parse(System.Text.UTF8Encoding.UTF8.GetString((res as TextAsset).bytes)));
                            }
                        }
                        try
                        {
                            if (ab != null)
                                ab.Unload(false);
                        }
                        catch (System.Exception ex)
                        {
                            Logger.LogError("XmlResolveCommand Unload Error:" + ex.Message);
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
                        succed(null, new XMLParser().Parse(System.Text.UTF8Encoding.UTF8.GetString((baseRes as TextAsset).bytes)));
                    }

                    baseRes = null;
                }
                base.Reset();
            }
            catch (System.Exception ex)
            {
                if (error != null)
                    error("XmlResolve Exception:" + ex.Message);
                else if (succed != null)
                    succed(null, null);
                base.Reset();
            }
        }
    }

