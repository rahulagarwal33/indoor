using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Fingerprint
{
	public class Server
	{
		public string url = "http://localhost:8888/indoor";
		public void uploadFingerprint(FingerPrintData data)
		{
			if(data != null)
			{
				string dataStr = data.ToString();
				string result = sendPostData(url + "/index.php", dataStr);
			}
		}
		string sendPostData(string url, string data)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			using (StreamWriter writer = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				writer.Write(data);
				writer.Flush();
				writer.Close();
			}
			HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
			string result = "";
			using (StreamReader reader = new StreamReader(response.GetResponseStream()))
			{
				result = reader.ReadToEnd();
			}
			return result;
		}
	}
}
