namespace VideoNotifyBiz
{
    public abstract class VideoNotify
    {
        public abstract string GetSiteValueByXpath(string siteUrl,string xPath);
        public abstract bool SendNotify(int notifyType);
    }
}
