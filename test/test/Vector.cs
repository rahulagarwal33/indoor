using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fingerprint
{
	public class Vector
	{
		public double x;
		public double y;
		public double z;

		public Vector(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public double magnitude()
		{
			return Math.Sqrt(x * x + y * y + z * z);
		}
		public void SetFromString(string s)
		{
			List<string> l = s.Split(',').ToList();
			if (l.Count == 3)
			{
				x = Double.Parse(l[0]);
				y = Double.Parse(l[1]);
				z = Double.Parse(l[2]);
			}
		}
		public void converPosToLL(out double lat, out double lon)
		{
			lon = x / 1852.3 / 60;
			lat = z / 1852.3 / 60;
		}
		public static Vector fromLL(double lat, double lon, double y)
		{
			Vector v = new Vector(lon * 60 * 1852.3, y, lat * 60 * 1852.3);
			return v;
		}

		public static Vector operator -(Vector lhs, Vector rhs)
		{
			Vector v = new Vector(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
			return v;
		}
		public static Vector operator +(Vector lhs, Vector rhs)
		{
			Vector v = new Vector(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
			return v;
		}
		public override string ToString()
		{
			string s = x.ToString() + "," + y.ToString() + "," + z.ToString();
			return s;
		}
	}
}
