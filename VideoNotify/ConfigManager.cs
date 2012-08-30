using System;
using System.Configuration;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;

namespace VideoNotifyBiz
{
	/// <summary>
	/// Summary description for AppConfig.
	/// </summary>
	public class ConfigManager
	{

		private ConfigManager()
		{
		}

		public static string ConnectionString
		{
			get
			{
				return ConfigurationManager.AppSettings["ConString"];
			}
		}

		public static object GetSectionByName(string sectionName)
		{
			CacheManager configCache = CacheFactory.GetCacheManager();
			ConfigurationSection site = configCache.GetData(sectionName) as ConfigurationSection;
			if (site == null)
			{
				ConfigurationFileMap fileMap = new ConfigurationFileMap();
				string configFileName = GetAbsoluteFilePath(ConfigurationManager.AppSettings["CustomConfigFile"]);
				fileMap.MachineConfigFilename = configFileName;
				Configuration config = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
				site = config.GetSection(sectionName) as ConfigurationSection;

				configCache.Add(sectionName, site, CacheItemPriority.Normal, null, new FileDependency(configFileName));
			}
			return site;

		}

		 #region 计算文件的绝对路径, 在类库中调用，一般是不能使用Server.MapPath的时候
		public static string GetAbsoluteFilePath(string filePath)
		{
            string file = filePath;
            if (!filePath.Substring(1, 1).Equals(":")
                && !filePath.StartsWith("\\"))
            {
                file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            }
            return file.Replace("/", "\\");
		}
#endregion

		public static MailSection Mail
		{
			get
			{
				return (MailSection)GetSectionByName("MailSection");
			}
		}
	}

	public class MailSection : ConfigurationSection
	{
		[ConfigurationProperty("MailFrom")]
		public string MailFrom
		{
			set
			{
				this["MailFrom"] = value;
			}
			get
			{
				return (string)this["MailFrom"];
			}
		}

		[ConfigurationProperty("MailFromName")]
		public string MailFromName
		{
			set
			{
				this["MailFromName"] = value;
			}
			get
			{
				return (string)this["MailFromName"];
			}
		}

		[ConfigurationProperty("SmtpServer")]
		public string SmtpServer
		{
			set
			{
				this["SmtpServer"] = value;
			}
			get
			{
				return (string)this["SmtpServer"];
			}
		}

		[ConfigurationProperty("SmtpPort")]
		public int SmtpPort
		{
			set
			{
				this["SmtpPort"] = value;
			}
			get
			{
				return (int)this["SmtpPort"];
			}
		}

		[ConfigurationProperty("SmtpSSL")]
		public bool SmtpSSL
		{
			set
			{
				this["SmtpSSL"] = value;
			}
			get
			{
				return (bool)this["SmtpSSL"];
			}
		}

		[ConfigurationProperty("MailUserName")]
		public string MailUserName
		{
			set
			{
				this["MailUserName"] = value;
			}
			get
			{
				return (string)this["MailUserName"];
			}
		}

		[ConfigurationProperty("MailUserPassword")]
		public string MailUserPassword
		{
			set
			{
				this["MailUserPassword"] = value;
			}
			get
			{
				return (string)this["MailUserPassword"];
			}
		}

		[ConfigurationProperty("IsSendEMail")]
		public bool IsSendEMail
		{
			set
			{
				this["IsSendEMail"] = value;
			}
			get
			{
				return (bool)this["IsSendEMail"];
			}
		}
	}
}
