using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace WindowsFormsApplication1
{
	public partial class Form1 : Form
	{
		public static int sampleID = 0;
		public Form1()
		{
			InitializeComponent();
		}
		string sendPostData(string url, string data)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			using(StreamWriter writer = new StreamWriter(httpWebRequest.GetRequestStream()))
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
		List<Data> createData()
		{
			List<Data> data = new List<Data>();
			Random rand = new Random();
			for (int i = 0; i < 5; ++i)
			{
				Data d = new Data();
				d.mac = "AA:BB:CC:DD:EE:" + rand.Next(255).ToString("X2");
				d.rssi = -rand.NextDouble() * 50;
				d.lat = (10 * 60 * 1852.3 + 500 * rand.NextDouble()) / (60 * 1852.3);
				d.lon = (15 * 60 * 1852.3 + 500 * rand.NextDouble()) / (60 * 1852.3);
				d.sampleID = sampleID;
				data.Add(d);
			}
			++sampleID;
			return data;
		}
		private void button1_Click(object sender, EventArgs e)
		{
			List<Data> data = createData();
			var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data);
			string result = sendPostData("http://localhost:8888/indoor/index.php", json);
			int k = 1;
		}
	}
	public class Data
	{
		public string mac;
		public string model;
		public double lat;
		public double lon;
		public double rssi;
		public int sampleID;
	};
}
