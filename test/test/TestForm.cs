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
using System.Xml;
using System.Threading;

namespace Fingerprint
{
	public partial class TestForm : Form
	{
		Vector queryPoint = new Vector(0, 0, 0);
		Vector returnPoint = new Vector(0, 0, 0);

		Maze maze = new Maze();
		Server server = new Server();
		public static int sampleID = 0;
		bool bExit = false;
		int numFingerprintUploaded = 0;
		int numFingerprintError = 0;
		ManualResetEvent allDone = new ManualResetEvent(false);
		public TestForm()
		{
			InitializeComponent();
		}
		private void generateFingerprints()
		{
			Vector pos = maze.generateNextPos();

			while(!bExit)
			{
				int r = maze.rand.Next(7);
				if (r == 3 || r == 5)
					pos = maze.generateNextPos();
				FingerPrintData data = maze.generateNextFingerprint(pos);
				if(data != null)
				{
					bool bRet = server.uploadFingerprint(data);
					if (bRet)
						numFingerprintUploaded++;
					else
						numFingerprintError++;
				}
				BeginInvoke((Action<int>)((int x) =>
				{
					this.Text = "Fingerprint Uploaded: " + numFingerprintUploaded + ", Error: " + numFingerprintError;
				}), 0);
				Thread.Sleep(100);
			}
			allDone.Set();
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			picBox.MouseWheel += new MouseEventHandler(picBox_MouseWheel);
			picBox.MouseClick += new MouseEventHandler(picBox_MouseClick);
			RandomData data = new RandomData();
			//data.generateData(5000, new Vector(1000, 10, 1000), "data.xml");
			maze.createGrid(10, 10, 10);
			maze.loadRouters("data.xml");

			Thread t = new Thread(() => generateFingerprints());
			t.Start();
		}

		void picBox_MouseClick(object sender, MouseEventArgs e)
		{
			picBox.Focus();
		}

		void picBox_MouseWheel(object sender, MouseEventArgs e)
		{
			float x = (float)picBox.Size.Width + e.Delta * 1.0f;
			float y = (float)picBox.Size.Height + e.Delta * 1.0f;
			if(x <= panel1.Width)
				x = panel1.Width;
			if(y <= panel1.Height)
				y = panel1.Height;
			picBox.Size = new Size((int)x, (int)y);
			picBox.Invalidate();
		}

		private void picBox_Paint(object sender, PaintEventArgs e)
		{
			Size sz = picBox.Size;
			Size mazeSize = maze.size();
			float marginX = 50;
			float marginY = 50;
			float scaleX = (float)((sz.Width - marginX)  * 1.0 / mazeSize.Width);
			float scaleY = (float)((sz.Height - marginY) * 1.0 / mazeSize.Height);
			SizeF szFinal = new SizeF((float)sz.Width - marginX, (float)sz.Height - marginY);
			float offsetX = marginX / 2;
			float offsetY = marginY / 2;
			e.Graphics.TranslateTransform(offsetX, offsetY);
			e.Graphics.ScaleTransform(scaleX, scaleY);
			Pen p = new Pen(Color.Green, 0.01f);
			e.Graphics.DrawRectangle(p, (int)queryPoint.x - 1, (int)queryPoint.z - 1, 2, 2);
			Pen p1 = new Pen(Color.Red, 0.01f);
			e.Graphics.DrawRectangle(p1, (int)returnPoint.x - 1, (int)returnPoint.z - 1, 2, 2);
			maze.draw(e.Graphics);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			bExit = true;
			allDone.WaitOne();
			maze.saveRouters("data.xml");
		}

		private void picBox_Click(object sender, EventArgs e)
		{
			Size sz = picBox.Size;
			Size mazeSize = maze.size();
			float marginX = 50;
			float marginY = 50;
			float scaleX = (float)((sz.Width - marginX) * 1.0 / mazeSize.Width);
			float scaleY = (float)((sz.Height - marginY) * 1.0 / mazeSize.Height);
			SizeF szFinal = new SizeF((float)sz.Width - marginX, (float)sz.Height - marginY);
			float offsetX = marginX / 2;
			float offsetY = marginY / 2;
			float x = (float)((MouseEventArgs)e).X;
			float y = (float)((MouseEventArgs)e).Y;

			//convert these to maze point
			x -= offsetX;
			y -= offsetY;
			x /= scaleX;
			y /= scaleY;
		
			Vector v = new Vector(x, 0, y);
			queryPoint = v;
			FingerPrintData data = maze.generateNextFingerprint(v);
			Vector pos = server.getPosition(data);
			returnPoint = pos;
			picBox.Invalidate();
		}

	}
}
