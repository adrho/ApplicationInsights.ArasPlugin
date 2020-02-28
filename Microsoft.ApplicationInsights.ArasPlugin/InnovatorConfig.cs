using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Microsoft.ApplicationInsights.ArasPlugin
{
	public static class InnovatorConfig
	{
		private static XDocument xDocument;
		private static string ConfigFilePath;
		private static DateTime LastConfigModifiedTime;
		static InnovatorConfig()
		{
			try
			{
				string executingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
				ConfigFilePath = GetPath(executingDirectory, "InnovatorServerConfig.xml");
				CheckToReLoadConfig();
			}
			catch
			{
				//ignore exceptions
			}
		}
		public static string GetOperatingParamValue(string key)
		{
			string xPath = "/Innovator/operating_parameter[@key='{0}']";
			return GetValue(string.Format(xPath, key));
		}

		private static string GetAttributeValue(string xPath, string attributeName)
		{
			//reload the config if modified
			CheckToReLoadConfig();

			string value = string.Empty;
			if (null != xDocument)
			{
				var element = xDocument.XPathSelectElements(xPath).FirstOrDefault();
				if (null != element && null != element.Attribute(attributeName))
				{
					value = element.Attribute(attributeName).Value;
				}
			}
			return value;
		}

		private static string GetPath(string folderPath, string searchFile)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
			foreach (var file in directoryInfo.GetFiles())
			{
				if (string.Equals(file.Name, searchFile, StringComparison.OrdinalIgnoreCase))
				{
					return file.FullName;
				}
			}

			directoryInfo = Directory.GetParent(folderPath);
			if (null == directoryInfo)
				return "";
			return GetPath(Directory.GetParent(folderPath).FullName, searchFile);
		}

		public static DateTime GetModifiedTime(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException(string.Format("{0} file doesn't exist.", filePath));
			return File.GetLastWriteTimeUtc(filePath);
		}

		private static void LoadConfig()
		{
			xDocument = XDocument.Load(ConfigFilePath);
			LastConfigModifiedTime = GetModifiedTime(ConfigFilePath);
		}

		private static void CheckToReLoadConfig()
		{
			try
			{
				if (File.Exists(ConfigFilePath))
				{
					//get the last modified time
					DateTime modifiedTime = GetModifiedTime(ConfigFilePath);
					if (modifiedTime != LastConfigModifiedTime)
					{
						LoadConfig();
					}
				}
			}
			catch
			{
				//ignore exceptions
			}
		}

		private static string GetValue(string xPathQuery)
		{
			//reload the config if modified
			CheckToReLoadConfig();

			string value = string.Empty;
			if (null != xDocument)
			{
				var element = xDocument.XPathSelectElements(xPathQuery).FirstOrDefault();
				if (null != element && null != element.Attribute("value"))
				{
					value = element.Attribute("value").Value;
				}
			}
			return value;
		}
	}
}
