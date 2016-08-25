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
				double rssi = (maxRSSI + (maxRSSI - minRSSI) / (maxRadius) * dist) + 5 * maze.rand.NextDouble();
				return rssi;
			}
		}
	}
}
