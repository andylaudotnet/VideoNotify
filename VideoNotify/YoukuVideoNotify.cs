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
            string username = "";
            try
            {
                username = doc.DocumentNode.SelectSingleNode("/html/body/div/div/div[2]/div/div/div/div/div/h1").InnerText.Replace("的视频空间","");
            }
            catch { }
            var videoName = username + "最新视频：" + doc.DocumentNode.SelectSingleNode(xPath).InnerHtml;
            var videoHref = "视频地址：" + doc.DocumentNode.SelectSingleNode(xPath).Attributes["href"].Value;
            return videoName+"\r\n"+videoHref;
        }
        public override bool SendNotify(int notifyType)
        {
            return true;
        }
    }
}
