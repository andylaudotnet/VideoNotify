using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using VideoNotifyBiz;

namespace WFVideoNotify
{
    public partial class Form1 : Form
    {
        private static object locker = new object();
        private static YoukuVideoNotify YVN;
        private static int ok1Count;
        private static int ok2Count;
        private static int errorCount;
        private static int indexNum;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            skinEngine1.SkinFile = "MSN.ssk";
            CmbInit();
            for (int i = 1; i <= 10; i++)
            {
                CmbThreadCount.Items.Add(i);
            }
            CmbThreadCount.SelectedIndex = 4;
        }
        private void btnGetValue_Click(object sender, EventArgs e)
        {
            btnGetValue.Enabled = false;
            YVN = new YoukuVideoNotify();
            ok1Count = 0;
            ok2Count = 0;
            errorCount = 0;
            indexNum = 0;
            CmbInit();
            txtResult.AppendText(DateTime.Now.ToString("HH:mm:ss")+"开始读取优酷用户最新视频...\r\n");
            for (int i = 0; i <int.Parse(CmbThreadCount.SelectedItem.ToString()); i++)
            {
                new Thread(BeginGet) { IsBackground = true}.Start();
            }
            //EmailManager.SMTP smtp = new EmailManager.SMTP();
            //smtp.Send("andylee@swdc.com.cn", "新视频通知", txtResult.Text);
        }
        private void BeginGet()
        {
            try
            { 
                lock (locker)
                {
                        for (int i = 0; i < cmbWebUrl.Items.Count; i++)
                        {
                            ListItem selectedItem = (ListItem)cmbWebUrl.Items[i];
                            string value = selectedItem.Value;
                            string text = selectedItem.Text;
                            if (int.Parse(value) == 1|| btnGetValue.Enabled)
                                continue;
                            try
                            {
                                indexNum++;
                                txtResult.AppendText("****************线程" + Thread.CurrentThread.ManagedThreadId+ "正在读取第" + indexNum.ToString() + "个用户数据****************\r\n");
                                txtResult.AppendText(YVN.GetSiteValueByXpath(text, "/html/body/div/div/div[2]/div/div[4]/div/div/div[2]/div/div/div/h3/a") + "\r\n");
                                cmbWebUrl.Items.Remove(selectedItem);
                                ListItem newItem = new ListItem("1", text);
                                ok1Count++;
                            }
                            catch
                            {
                                try
                                {
                                    txtResult.AppendText(YVN.GetSiteValueByXpath(text, "/html/body/div/div/div[2]/div/div[5]/div/div/div[2]/div/div/div/h3/a") + "\r\n");
                                    cmbWebUrl.Items.Remove(selectedItem);
                                    ListItem newItem = new ListItem("1", text);
                                    ok2Count++;
                                }
                                catch (Exception ex)
                                {
                                    txtResult.AppendText("读取数据" + text + "发生异常：" + ex.Message + "\r\n");
                                    cmbWebUrl.Items.Remove(selectedItem);
                                    ListItem newItem = new ListItem("1", text);
                                    errorCount++;
                                }
                            }
                            Thread.Sleep(1);
                            if (cmbWebUrl.Items.Count == indexNum)
                            {
                                if (!btnGetValue.Enabled)
                                {
                                    txtResult.AppendText("\r\n"+DateTime.Now.ToString("HH:mm:ss")+"用户最新视频数据读取完毕!\r\n(表达式1匹配成功" + ok1Count.ToString() + "条，表达式2匹配成功" + ok2Count.ToString() + "条，失败" + errorCount.ToString() + "条)\r\n\r\n");
                                    btnGetValue.Enabled = true;
                                }
                                return;
                            }
                     }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("线程" + Thread.CurrentThread.Name + "读取第" + indexNum.ToString() + "条数据时出现异常:\r\n" + ex.Message);
                btnGetValue.Enabled = true;
            }
        }

        private void CmbInit()
        {
            string videoList = @"
http://u.youku.com/user_show/id_UNjI4NjAxMDA=.html|
http://u.youku.com/user_show/id_UMzMxOTY5MjQ=.html|
http://u.youku.com/user_show/uid_TaobaoUED|
http://u.youku.com/user_show/id_UMzM5MzAzNDUy.html|
http://u.youku.com/user_show/id_UMjExNDIwNTY=.html|
http://u.youku.com/user_show/id_UMzIxMDM0Mjg=.html|
http://u.youku.com/user_show/id_UODU4MzYzNDA=.html|
http://u.youku.com/user_show/id_UODk2ODA=.html|
http://u.youku.com/user_show/id_UNTA3MzAwODA=.html|
http://u.youku.com/user_show/id_UNTEyMzE3MTI=.html|
http://u.youku.com/user_show/id_UMTAzMDAyMzA0.html|
http://u.youku.com/user_show/id_UODU1ODc1NTI=.html|
http://u.youku.com/user_show/id_UMzI2ODgzNzg4.html|
http://u.youku.com/user_show/id_UMTE0NjM0NTU2.html|
http://u.youku.com/user_show/id_UMTE2NTY5MTAw.html|
http://u.youku.com/user_show/id_UMTAwNjQwNDc2.html|
http://u.youku.com/user_show/id_UMjIwNTg0MTY=.html|
http://u.youku.com/user_show/id_UMTQ0MDkwMzM2.html|
http://u.youku.com/user_show/id_UMTQzNjk2NDA=.html|
http://u.youku.com/user_show/id_UMjA3NTk4ODQ=.html|
http://u.youku.com/user_show/id_UMjY5MTg1MDU2.html|
http://u.youku.com/user_show/id_UMTg1NDM5NDA=.html|
http://u.youku.com/user_show/id_UMjA3NTk4ODQ=.html|
http://u.youku.com/user_show/id_UODUzOTI5MDQ=.html";
            string[] list = videoList.Split('|');
            List<ListItem> items = new List<ListItem>();
            for (int i = 0; i < list.Length; i++)
            {
                items.Add(new ListItem("0", list[i]));
            }
            //cmbWebUrl.Items.Clear();
            cmbWebUrl.DataSource = items;
            cmbWebUrl.DisplayMember = "Text";
            cmbWebUrl.ValueMember = "Value";
        }
    }
}
