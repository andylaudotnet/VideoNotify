using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace VideoNotifyBiz
{
    public class YoukuVideoNotify : VideoNotify        
    {
        public override string GetSiteValueByXpath(string siteUrl, string xPath)
        {
            WebClient wc = new WebClient {
                Encoding = Encoding.GetEncoding("utf-8")
            };
            var html = wc.DownloadString(siteUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var videoName = "视频名称：" + doc.DocumentNode.SelectSingleNode(xPath).InnerHtml;
            var videoHref = "视频地址：" + doc.DocumentNode.SelectSingleNode(xPath).Attributes["href"].Value;
            return videoName+"\r\n"+videoHref;
        }
        public override bool SendNotify(int notifyType)
        {
            return true;
        }
    }
}
