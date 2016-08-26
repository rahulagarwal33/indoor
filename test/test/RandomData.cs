using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Fingerprint
{
	public class RandomData
	{
		Random rand = new Random(unchecked((int) (DateTime.Now.Ticks)));
		HashSet<string> createdMac = new HashSet<string>();
		List<string> modelList = new List<string>();
		string generateNextMac()
		{
			bool bDone = false;
			string mac = "";
			while(!bDone)
			{
				int a = rand.Next(0, 256);
				int b = rand.Next(0, 256);
				int c = rand.Next(0, 256);
				int d = rand.Next(0, 256);
				int e = rand.Next(0, 256);
				int f = rand.Next(0, 256);
				mac = a.ToString("X2") + ":" + a.ToString("X2") + ":" + b.ToString("X2") + ":" + d.ToString("X2") + ":" + e.ToString("X2") + ":" + f.ToString("X2");
				if (!createdMac.Contains(mac))
				{
					bDone = true;
				}
			}
			return mac;
		}
		string generateNextModel()
		{
			if (modelList.Count > 0)
			{
				return modelList[rand.Next(0, modelList.Count - 1)];
			}
			return "";
		}
		double nextMaxRadius()
		{
			return 20 + 10 * rand.NextDouble();
		}
		double nextMinRSSI()
		{
			return -90 + 3 * rand.NextDouble();
		}
		double nextMaxRSSI()
		{
			return -30 + 3 * rand.NextDouble();
		}
		Vector nextPostion(Vector range)
		{
			Vector v = new Vector(range.x * rand.NextDouble(), range.y * rand.NextDouble(), range.z * rand.NextDouble());
			return v;
		}
		void initModelList()
		{
			modelList.Add("Beetel");
			modelList.Add("Cisco");
			modelList.Add("Netgear");
			modelList.Add("DLink");
		}
		public void generateData(int numRouter, Vector range, string filename)
		{
			XmlDocument xmlDoc = new XmlDocument();
			XmlElement routersElem = xmlDoc.CreateElement("Routers");
			xmlDoc.AppendChild(routersElem);
			{ XmlAttribute attrib = xmlDoc.CreateAttribute("StartSampleIndex"); attrib.Value = "0"; routersElem.Attributes.Append(attrib); }
			initModelList();
			for (int i = 0; i < numRouter; ++i)
			{
				string mac = generateNextMac();
				string model = generateNextModel();
				double maxRadius = nextMaxRadius();
				double minRSSi = nextMinRSSI();
				double maxRSSI = nextMaxRSSI();
				Vector pos = nextPostion(range);
				XmlElement routerElem = xmlDoc.CreateElement("Router");
				{ XmlAttribute attrib = xmlDoc.CreateAttribute("MAC"); attrib.Value = mac; routerElem.Attributes.Append(attrib); }
				{ XmlAttribute attrib = xmlDoc.CreateAttribute("Model"); attrib.Value = model; routerElem.Attributes.Append(attrib); }
				{ XmlAttribute attrib = xmlDoc.CreateAttribute("MaxRadius"); attrib.Value = maxRadius.ToString(); routerElem.Attributes.Append(attrib); }
				{ XmlAttribute attrib = xmlDoc.CreateAttribute("MinRSSI"); attrib.Value = minRSSi.ToString(); routerElem.Attributes.Append(attrib); }
				{ XmlAttribute attrib = xmlDoc.CreateAttribute("MaxRSSI"); attrib.Value = maxRSSI.ToString(); routerElem.Attributes.Append(attrib); }
				{ XmlAttribute attrib = xmlDoc.CreateAttribute("Position"); attrib.Value = pos.ToString(); routerElem.Attributes.Append(attrib); }
				routersElem.AppendChild(routerElem);
			}
			FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
			XmlWriterSettings setting = new XmlWriterSettings();
			setting.Indent = true;
			XmlWriter writer = XmlWriter.Create(fs, setting);
			xmlDoc.WriteTo(writer);
			writer.Flush();
			fs.Close();
		}
	}
}
