using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Fingerprint
{
	public class Maze
	{
		public Random rand = new Random();
		public double Width = 1000;
		public double Length = 1000;
		public double Height = 20;
		public int sampleID = 0;
		double gridW, gridL, gridH;
		Dictionary<Int64, List<Router>> routers = new Dictionary<Int64, List<Router>>();


		FingerPrintData generateNextFingerprint()
		{
			Vector randomPos = new Vector(Width * rand.NextDouble(), Height * rand.NextDouble(), Length * rand.NextDouble());
			Int64 llref = getLLRef(randomPos);
			List<Router> lstRouters = getRouterList(llref);
			FingerPrintData data = null;
			if (lstRouters != null)
			{
				data = new FingerPrintData();
				foreach (Router r in lstRouters)
				{
					FingerPrintData.Data d = new FingerPrintData.Data();
					d.lat = randomPos.z;
					d.lon = randomPos.x;
					d.mac = r.mac;
					d.model = r.model;
					d.rssi = r.RSSI(randomPos);
					d.sampleID = sampleID;
					data.lstData.Add(d);
				}
			}
			++sampleID;
			return data;
		}

		public Vector distance(Vector vec1, Vector vec2)
		{
			Vector v = vec1 - vec2;
			return v;
		}
		public void createGrid(double dw, double dl, double dh)
		{
			gridW = dw;
			gridL = dl;
			gridH = dh;
		}
		public void addRouter(XmlNode routerData)
		{
			Router router = new Router(this, routerData);
			List<Int64> lstKey = getLLRefList(router.pos, router.maxRadius);
			foreach (Int64 k in lstKey)
			{
				insertRouter(k, router);
			}
		}
		public List<Int64> getLLRefList(Vector v, double radius)
		{
			List<Int64> lstKey = new List<Int64>();
			int nW = (int)(Width / gridW);
			int nL = (int)(Length/ gridL);
			int nH = (int)(Height / gridH);

			int startnW = (int)(v.x	/ gridW);
			int startnL = (int)(v.z/ gridL);
			int startnH = (int)(v.y / gridH);

			int offsetNW = (int)(radius / gridW);
			int offsetNL = (int)(radius / gridL);
			int offsetNH = (int)(radius / gridH);

			for (int w = startnW - offsetNW - 1; w < startnW + offsetNW + 1; ++w)
			{
				for (int l = startnL - offsetNL - 1; l < startnL + offsetNL + 1; ++l)
				{
					//for (int h = startnH - offsetNH - 1; h < startnH + offsetNH + 1; ++h)
					{
						int h = 0;
						if (w >= 0 && w < nW && l >= 0 && l < nL && h >= 0 && h < nH)
						{
							Int64 key = getLLRef(new Vector(w * gridW, h * gridH, l * gridL));
							lstKey.Add(key);
						}
					}
				}
			}
			return lstKey;
		}
		public List<Router> getRouterList(Int64 key)
		{
			List<Router> lst = null;
			routers.TryGetValue(key, out lst);
			return lst;
		}
		public void insertRouter(Int64 key, Router r)
		{
			List<Router> lst = null;
			if (!routers.TryGetValue(key, out lst))
			{
				lst = new List<Router>();
				lst.Add(r);
				routers.Add(key, lst);
			}
			else
				lst.Add(r);
		}
		public Int64 getLLRef(Vector pos)
		{
			Int64 nW = (Int64)(pos.x / gridW);
			Int64 nL = (Int64)(pos.z / gridL);
			Int64 nH = (Int64)(pos.y / gridH);
			Int64 key = (nW << 32 | nL);
			return key;
		}
	}
	public class FingerPrintData
	{
		public class Data
		{
			public string mac;
			public string model;
			public double lat;
			public double lon;
			public double rssi;
			public int sampleID;
		};
		public List<Data> lstData = new List<Data>();
	}
}
