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
		public bool uploadFingerprint(FingerPrintData data)
		{
			if(data != null)
			{
				string dataStr = data.ToString();
				string result = sendPostData(url + "/uploadFingerprint.php", dataStr);
				Dictionary<string, Object> obj = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize <Dictionary<string, Object>>(result);
				if (obj != null && obj.Keys.Count == 3)
				{
					if ((obj["InsertRouter"] is bool) && (obj["InsertData"] is bool) && (obj["InsertLLRef"]) is bool)
					{
						if ((bool)obj["InsertRouter"] && (bool)obj["InsertData"] && (bool)obj["InsertLLRef"])
						{
							return true;
						}
					}
					return false;
				}
				else
				{
					return false;
				}
			}
			return true;
		}
		public Vector getPosition(FingerPrintData data)
		{
			if (data != null)
			{
				string dataStr = data.ToString();
				string result = sendPostData(url + "/queryPosition.php", dataStr);
				try
				{
					PosData ll = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<PosData>(result);
					Vector v = Vector.fromLL(ll.lat, ll.y, ll.lon);
					return v;
				}
				catch (System.Exception ex)
				{
					
				}
			}
			return null;
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
	public class PosData
	{
		public double lat;
		public double lon;
		public double y;
		public double accuracy;
	}
}
