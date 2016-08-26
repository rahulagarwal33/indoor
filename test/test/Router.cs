using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Fingerprint
{
	public class Router
	{
		Maze maze;

		public Vector pos = new Vector(0, 0, 0);
		public double maxRadius;

		public double minRSSI = -90;
		public double maxRSSI = -30;
		public string mac;
		public string model;

		public Router(Maze m, XmlNode routerData)
		{
			pos.SetFromString(routerData.Attributes["Position"].Value);
			maxRadius = Double.Parse(routerData.Attributes["MaxRadius"].Value);
			minRSSI = Double.Parse(routerData.Attributes["MinRSSI"].Value);
			maxRSSI = Double.Parse(routerData.Attributes["MaxRSSI"].Value);
			mac = routerData.Attributes["MAC"].Value;
			model = routerData.Attributes["Model"].Value;
			this.maze = m;
		}
		public XmlNode save(XmlDocument xmlDoc)
		{
			XmlNode routerElem = xmlDoc.CreateElement("Router");
			{ XmlAttribute attrib = xmlDoc.CreateAttribute("MAC"); attrib.Value = mac; routerElem.Attributes.Append(attrib); }
			{ XmlAttribute attrib = xmlDoc.CreateAttribute("Model"); attrib.Value = model; routerElem.Attributes.Append(attrib); }
			{ XmlAttribute attrib = xmlDoc.CreateAttribute("MaxRadius"); attrib.Value = maxRadius.ToString(); routerElem.Attributes.Append(attrib); }
			{ XmlAttribute attrib = xmlDoc.CreateAttribute("MinRSSI"); attrib.Value = minRSSI.ToString(); routerElem.Attributes.Append(attrib); }
			{ XmlAttribute attrib = xmlDoc.CreateAttribute("MaxRSSI"); attrib.Value = maxRSSI.ToString(); routerElem.Attributes.Append(attrib); }
			{ XmlAttribute attrib = xmlDoc.CreateAttribute("Position"); attrib.Value = pos.ToString(); routerElem.Attributes.Append(attrib); }
			return routerElem;
		}
		public double RSSI(Vector to)
		{
			Vector distVec = maze.distance(to, pos);
			double dist = distVec.magnitude();
			if (dist > maxRadius)
			{
				return -100;
			}
			else
			{
				double rssi = (maxRSSI + (minRSSI - maxRSSI) / (maxRadius) * dist) + 5 * maze.rand.NextDouble();
				return rssi;
			}
		}
	}
}
