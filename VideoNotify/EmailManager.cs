﻿using System;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace VideoNotifyBiz
{
    public class EmailManager
    {
        #region 邮件发送类
        public class SMTP
        {
            #region Fields

            private string mMailFrom;
            private string mMailDisplyName;
            private string[] mMailTo;
            private string[] mMailCc;
            private string[] mMailBcc;
            private string mMailSubject;
            private string mMailBody;
            private string[] mMailAttachments;
            private string mSMTPServer;
            private int mSMTPPort;
            private string mSMTPUsername;
            private string mSMTPPassword;
            private bool mSMTPSSL;
            private MailPriority mPriority = MailPriority.Normal;
            private bool mIsBodyHtml = false;
            bool mailSent = false;

            #endregion

            #region Properties

            /// <summary>
            /// 发件人地址
            /// </summary>
            public string MailFrom
            {
                set { mMailFrom = value; }
                get { return mMailFrom; }
            }

            /// <summary>
            /// 显示的名称
            /// </summary>
            public string MailDisplyName
            {
                set { mMailDisplyName = value; }
                get { return mMailDisplyName; }
            }

            /// <summary>
            /// 收件人地址
            /// </summary>
            public string[] MailTo
            {
                set { mMailTo = value; }
                get { return mMailTo; }
            }

            /// <summary>
            /// 抄送
            /// </summary>
            public string[] MailCc
            {
                set { mMailCc = value; }
                get { return mMailCc; }
            }

            /// <summary>
            /// 密件抄送
            /// </summary>
            public string[] MailBcc
            {
                set { mMailBcc = value; }
                get { return mMailBcc; }
            }

            /// <summary>
            /// 邮件主题
            /// </summary>
            public string MailSubject
            {
                set { mMailSubject = value; }
                get { return mMailSubject; }
            }

            /// <summary>
            /// 邮件正文
            /// </summary>
            public string MailBody
            {
                set { mMailBody = value; }
                get { return mMailBody; }
            }

            /// <summary>
            /// 附件
            /// </summary>
            public string[] MailAttachments
            {
                set { mMailAttachments = value; }
                get { return mMailAttachments; }
            }

            /// <summary>
            /// SMTP 服务器
            /// </summary>
            public string SMTPServer
            {
                set { mSMTPServer = value; }
                get { return mSMTPServer; }
            }

            /// <summary>
            /// 发送端口号(默认为 25)
            /// </summary>
            public int SMTPPort
            {
                set { mSMTPPort = value; }
                get { return mSMTPPort; }
            }

            /// <summary>
            /// 用户名
            /// </summary>
            public string SMTPUsername
            {
                set { mSMTPUsername = value; }
                get { return mSMTPUsername; }
            }

            /// <summary>
            /// 密码
            /// </summary>
            public string SMTPPassword
            {
                set { mSMTPPassword = value; }
                get { return mSMTPPassword; }
            }

            /// <summary>
            /// 是否使用安全套接字层 (SSL) 加密连接
            /// 默认为 false
            /// </summary>
            public Boolean SMTPSSL
            {
                set { mSMTPSSL = value; }
                get { return mSMTPSSL; }
            }

            /// <summary>
            /// 邮件的优先级
            /// </summary>
            public MailPriority Priority
            {
                get { return mPriority; }
                set { mPriority = value; }
            }

            /// <summary>
            /// 示邮件正文是否为 Html 格式的值
            /// </summary>
            public bool IsBodyHtml
            {
                get { return mIsBodyHtml; }
                set { mIsBodyHtml = value; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// 邮件发送类
            /// 主机信息从配置文件中获取
            /// 参考:ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.chs/dv_fxgenref/html/54f0f153-17e5-4f49-afdc-deadb940c9c1.htm
            /// </summary>
            /// <param name="mailFrom">发件人地址</param>
            /// <param name="mailTo">收件人地址</param>
            /// <param name="mailSubject">邮件主题</param>
            /// <param name="mailBody">邮件正文</param>
            public SMTP(string mailTo, string mailSubject, string mailBody)
            {

                mMailFrom = ConfigManager.Mail.MailFrom;
                mMailDisplyName = ConfigManager.Mail.MailFromName;
                mSMTPServer = ConfigManager.Mail.SmtpServer;
                mSMTPPort = ConfigManager.Mail.SmtpPort;
                mSMTPUsername = ConfigManager.Mail.MailUserName;
                mSMTPPassword = ConfigManager.Mail.MailUserPassword;

                mSMTPSSL = ConfigManager.Mail.SmtpSSL;
                mIsBodyHtml = true;

                mMailTo = mailTo.Split(';');
                mMailCc = null;
                mMailBcc = null;
                mMailSubject = mailSubject;
                mMailBody = mailBody;
                mMailAttachments = null;

            }

            public SMTP()
            {

                mMailFrom = ConfigManager.Mail.MailFrom;
                mMailDisplyName = ConfigManager.Mail.MailFromName;
                mSMTPServer = ConfigManager.Mail.SmtpServer;
                mSMTPPort = ConfigManager.Mail.SmtpPort;
                mSMTPUsername = ConfigManager.Mail.MailUserName;
                mSMTPPassword = ConfigManager.Mail.MailUserPassword;

                mSMTPSSL = ConfigManager.Mail.SmtpSSL;
                mIsBodyHtml = true;

                mMailCc = null;
                mMailBcc = null;
                mMailAttachments = null;

            }
            #endregion

            #region Methods

            /// <summary>
            /// 同步发送邮件
            /// </summary>
            /// <returns></returns>
            public Boolean Send()
            {
                return SendMail(false, null);
            }

            public Boolean Send(string mailTo, string mailSubject, string mailBody)
            {
                mMailTo = mailTo.Split(';');
                mMailCc = null;
                mMailBcc = null;
                mMailSubject = mailSubject;
                mMailBody = mailBody;
                return SendMail(false, null);
            }

            /// <summary>
            /// 异步发送邮件
            /// </summary>
            /// <param name="userState">异步任务的唯一标识符</param>
            /// <returns></returns>
            public void SendAsync(object userState)
            {
                SendMail(true, userState);
            }

            /// <summary>
            /// 发送邮件
            /// </summary>
            /// <param name="isAsync">是否异步发送邮件</param>
            /// <param name="userState">异步任务的唯一标识符，当 isAsync 为 True 时必须设置该属性， 当 isAsync 为 False 时可设置为 null</param>
            /// <returns></returns>
            private Boolean SendMail(bool isAsync, object userState)
            {
                if (!ConfigManager.Mail.IsSendEMail)
                    return false;

                #region 设置属性值
                string[] mailCcs = mMailCc;
                string[] mailBccs = mMailBcc;
                string[] attachments = mMailAttachments;
                // build the email message
                MailMessage oMailMessage = new MailMessage();
                MailAddress oMailAddress = new MailAddress(mMailFrom, mMailDisplyName);
                oMailMessage.From = oMailAddress;

                if (mMailTo != null)
                {
                    foreach (string mailto in mMailTo)
                    {
                        if (!string.IsNullOrEmpty(mailto))
                        {
                            oMailMessage.To.Add(mailto);
                        }
                    }
                }

                if (mailCcs != null)
                {
                    foreach (string cc in mailCcs)
                    {
                        if (!string.IsNullOrEmpty(cc))
                        {
                            oMailMessage.CC.Add(cc);
                        }
                    }
                }

                if (mailBccs != null)
                {
                    foreach (string bcc in mailBccs)
                    {
                        if (!string.IsNullOrEmpty(bcc))
                        {
                            oMailMessage.Bcc.Add(bcc);
                        }
                    }
                }

                if (attachments != null)
                {
                    foreach (string file in attachments)
                    {
                        if (!string.IsNullOrEmpty(file))
                        {
                            Attachment att = new Attachment(file);
                            oMailMessage.Attachments.Add(att);
                        }
                    }
                }

                if (mIsBodyHtml)
                {
                    mMailBody += "<br><font color=white>" + System.Net.Dns.GetHostName() + "</font>";
                }
                else
                {
                    mMailBody += " \r\n " + System.Net.Dns.GetHostName();
                }


                oMailMessage.Subject = mMailSubject;
                oMailMessage.Body = mMailBody;
                oMailMessage.Priority = mPriority;
                oMailMessage.IsBodyHtml = mIsBodyHtml;

                // Smtp Client
                SmtpClient SmtpMail = new SmtpClient(mSMTPServer, mSMTPPort);
                SmtpMail.Credentials = new NetworkCredential(mSMTPUsername, mSMTPPassword);
                SmtpMail.EnableSsl = mSMTPSSL;
                SmtpMail.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

                #endregion

                try
                {
                    if (!isAsync)
                    {
                        SmtpMail.Send(oMailMessage);
                        mailSent = true;
                    }
                    else
                    {
                        userState = (userState == null) ? Guid.NewGuid() : userState;
                        SmtpMail.SendAsync(oMailMessage, userState);
                    }
                }
                catch (Exception)
                {
                    mailSent = false;
                }

                return mailSent;
            }

            private void SendCompletedCallback(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
            {
                // Get the unique identifier for this asynchronous operation.
                String token = (string)e.UserState;

                if (e.Cancelled)
                {
                    Console.WriteLine("[{0}] Send canceled.", token);
                    mailSent = false;
                }
                if (e.Error != null)
                {
                    Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
                    mailSent = false;
                }
                else
                {
                    Console.WriteLine("Message sent.");
                    mailSent = false;
                }

                mailSent = true;
            }

            public void AddAttachment(string fileName)
            {
                List<string> files = new List<string>();
                if (mMailAttachments != null)
                {
                    for (int i = 0; i < mMailAttachments.Length; i++)
                    {
                        files.Add(mMailAttachments[i]);
                    }
                }
                files.Add(fileName);
                mMailAttachments = files.ToArray();
            }

            #endregion
        }

        #endregion

        #region 邮件接收类
        /// <summary>
        /// 邮件接收类
        /// </summary>
        public class POP3
        {
            #region Fields

            string POPServer;
            string mPOPUserName;
            string mPOPPass;
            int mPOPPort;
            NetworkStream ns;
            StreamReader sr;

            #endregion

            #region Constructors

            /// <summary>
            /// POP3
            /// </summary>
            /// <param name="server">POP3服务器名称</param>
            /// <param name="userName">用户名</param>
            /// <param name="password">用户密码</param>
            public POP3(string server, string userName, string password)
                : this(server, 110, userName, password)
            {
            }

            /// <summary>
            /// POP3
            /// </summary>
            /// <param name="server">POP3服务器名称</param>
            /// <param name="port">端口号</param>
            /// <param name="userName">用户名</param>
            /// <param name="password">用户密码</param>
            public POP3(string server, int port, string userName, string password)
            {
                POPServer = server;
                mPOPUserName = userName;
                mPOPPass = password;
                mPOPPort = port;
            }

            #endregion

            #region Methods

            #region Public

            /// <summary>
            /// 获得新邮件数量
            /// </summary>
            /// <returns>新邮件数量</returns>
            public int GetNumberOfNewMessages()
            {
                byte[] outbytes;
                string input;

                try
                {
                    Connect();

                    input = "stat" + "\r\n";
                    outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
                    ns.Write(outbytes, 0, outbytes.Length);
                    string resp = sr.ReadLine();
                    string[] tokens = resp.Split(new Char[] { ' ' });

                    Disconnect();

                    return Convert.ToInt32(tokens[1]);
                }
                catch
                {
                    return -1;
                }
            }

            /// <summary>
            /// 获取新邮件内容
            /// </summary>
            /// <param name="subj">邮件主题</param>
            /// <returns>新邮件内容</returns>
            public List<MailMessage> GetNewMessages(string subj)
            {

                int newcount;
                List<MailMessage> newmsgs = new List<MailMessage>();

                try
                {
                    newcount = GetNumberOfNewMessages();
                    Connect();

                    for (int n = 1; n < newcount + 1; n++)
                    {
                        List<string> msglines = GetRawMessage(n);
                        string msgsubj = GetMessageSubject(msglines);
                        if (msgsubj.CompareTo(subj) == 0)
                        {
                            MailMessage msg = new MailMessage();
                            msg.Subject = msgsubj;
                            msg.From = new MailAddress(GetMessageFrom(msglines));
                            msg.Body = GetMessageBody(msglines);
                            newmsgs.Add(msg);
                            DeleteMessage(n);
                        }
                    }

                    Disconnect();
                    return newmsgs;
                }
                catch
                {
                    return newmsgs;
                }
            }

            /// <summary>
            /// 获取新邮件内容
            /// </summary>
            /// <param name="nIndex">新邮件索引</param>
            /// <returns>新邮件内容</returns>
            public MailMessage GetNewMessages(int nIndex)
            {
                int newcount;
                MailMessage msg = new MailMessage();

                try
                {
                    newcount = GetNumberOfNewMessages();
                    Connect();
                    int n = nIndex + 1;

                    if (n < newcount + 1)
                    {
                        List<string> msglines = GetRawMessage(n);
                        string msgsubj = GetMessageSubject(msglines);


                        msg.Subject = msgsubj;
                        msg.From = new MailAddress(GetMessageFrom(msglines));
                        msg.Body = GetMessageBody(msglines);
                    }

                    Disconnect();
                    return msg;
                }
                catch
                {
                    return null;
                }
            }

            #endregion

            #region Private

            private bool Connect()
            {
                TcpClient sender = new TcpClient(POPServer, mPOPPort);
                byte[] outbytes;
                string input;

                try
                {
                    ns = sender.GetStream();
                    sr = new StreamReader(ns);

                    sr.ReadLine();
                    input = "user " + mPOPUserName + "\r\n";
                    outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
                    ns.Write(outbytes, 0, outbytes.Length);
                    sr.ReadLine();

                    input = "pass " + mPOPPass + "\r\n";
                    outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
                    ns.Write(outbytes, 0, outbytes.Length);
                    sr.ReadLine();
                    return true;

                }
                catch
                {
                    return false;
                }
            }

            private void Disconnect()
            {
                string input = "quit" + "\r\n";
                Byte[] outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
                ns.Write(outbytes, 0, outbytes.Length);
                ns.Close();
            }

            private List<string> GetRawMessage(int messagenumber)
            {
                Byte[] outbytes;
                string input;
                string line = "";

                input = "retr " + messagenumber.ToString() + "\r\n";
                outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
                ns.Write(outbytes, 0, outbytes.Length);

                List<string> msglines = new List<string>();
                do
                {
                    line = sr.ReadLine();
                    msglines.Add(line);
                } while (line != ".");
                msglines.RemoveAt(msglines.Count - 1);

                return msglines;
            }

            private string GetMessageSubject(List<string> msglines)
            {
                string[] tokens;
                IEnumerator msgenum = msglines.GetEnumerator();
                while (msgenum.MoveNext())
                {
                    string line = (string)msgenum.Current;
                    if (line.StartsWith("Subject:"))
                    {
                        tokens = line.Split(new Char[] { ' ' });
                        return tokens[1].Trim();
                    }
                }
                return "None";
            }

            private string GetMessageFrom(List<string> msglines)
            {
                string[] tokens;
                IEnumerator msgenum = msglines.GetEnumerator();
                while (msgenum.MoveNext())
                {
                    string line = (string)msgenum.Current;
                    if (line.StartsWith("From:"))
                    {
                        tokens = line.Split(new Char[] { '<' });
                        return tokens[1].Trim(new Char[] { '<', '>' });
                    }
                }
                return "None";
            }

            private string GetMessageBody(List<string> msglines)
            {
                string body = "";
                string line = " ";
                IEnumerator msgenum = msglines.GetEnumerator();

                while (line.CompareTo("") != 0)
                {
                    msgenum.MoveNext();
                    line = (string)msgenum.Current;
                }

                while (msgenum.MoveNext())
                {
                    body = body + (string)msgenum.Current + "\r\n";
                }
                return body;
            }

            private void DeleteMessage(int messagenumber)
            {
                Byte[] outbytes;
                string input;

                try
                {
                    input = "dele " + messagenumber.ToString() + "\r\n";
                    outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
                    ns.Write(outbytes, 0, outbytes.Length);
                }
                catch
                {
                    return;
                }

            }

            #endregion

            #endregion
        }

        #endregion
    }
}
