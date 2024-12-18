using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXFReaderNET;
using DXFReaderNET.Entities;
using Microsoft.Win32;
using System.IO;
namespace DXFSplitIntoParts
{
    public partial class Form1 : Form
    {


        private Vector2 panPointStart = Vector2.Zero;
        internal enum FunctionsEnum
        {
            None,
            ZoomWindow1,
            ZoomWindow2,
        }
        private FunctionsEnum CurrentFunction = FunctionsEnum.None;
        private Vector2 p1 = Vector2.Zero;
        private Vector2 p2 = Vector2.Zero;
        private const string ProgramName = "DXFSplitIntoParts";
        private string CurrentLoadDXFPath = Application.StartupPath;
        private string CurrentSaveDXFPath = Application.StartupPath;
        public Form1()
        {
            InitializeComponent();
        }



        private void newDrawingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dxfReaderNETControl1.NewDrawing();
            ResetTab();
            tabPage1.Text = "Drawing";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadRegistry();
            this.Text = "DXFReader.NET - " + ProgramName;
            dxfReaderNETControl1.NewDrawing();
            dxfReaderNETControl1.CustomCursor = CustomCursorType.CrossHair;
            toolStripStatusLabel1.Text = "";
            dxfReaderNETControl1.Dock = DockStyle.Fill;
            tabControl1.Dock = DockStyle.Fill;
        }

        private void loadDXFFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "DXF";
            openFileDialog1.Filter = "DXF|*.dxf";
            openFileDialog1.FileName = "";
            openFileDialog1.InitialDirectory = CurrentLoadDXFPath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CurrentLoadDXFPath = Path.GetDirectoryName(openFileDialog1.FileName);

                SaveRegistry();
                ResetTab();
                dxfReaderNETControl1.ReadDXF(openFileDialog1.FileName);
                tabPage1.Text = Path.GetFileName(openFileDialog1.FileName);
                dxfReaderNETControl1.NormalizeEntities(dxfReaderNETControl1.DXF.Entities);
                dxfReaderNETControl1.ZoomCenter();
            }
        }


        private void saveDXFFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = "dxf";
            saveFileDialog1.Filter = "DXF|*.dxf";
            saveFileDialog1.FileName = "drawing.dxf";
            openFileDialog1.InitialDirectory = CurrentSaveDXFPath;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CurrentSaveDXFPath = Path.GetDirectoryName(saveFileDialog1.FileName);
                dxfReaderNETControl1.WriteDXF(saveFileDialog1.FileName);
                SaveRegistry();
            }
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveRegistry();
            System.Environment.Exit(0);
        }

        private void aboutDXFReaderNETComponentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dxfReaderNETControl1.About();
        }




        private void LoadRegistry()
        {

            Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey(ProgramName);


            this.Width = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "wWidth", Screen.PrimaryScreen.Bounds.Width - 40);
            this.Height = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "wHeight", Screen.PrimaryScreen.Bounds.Height - 60);
            this.Left = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "wLeft", 20);
            this.Top = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "wTop", 20);

            if (this.Left <= 20 || this.Top <= 20)
            {
                this.Left = 20;
                this.Top = 20;
            }
            if (this.Width <= 40 || this.Height <= 60)
            {
                this.Width = Screen.PrimaryScreen.Bounds.Width - 40;
                this.Height = Screen.PrimaryScreen.Bounds.Height - 60;
            }
            if (this.Left + this.Width > Screen.PrimaryScreen.Bounds.Width)
            {
                this.Width = Screen.PrimaryScreen.Bounds.Width - this.Left - 40;

            }
            if (this.Top + this.Height > Screen.PrimaryScreen.Bounds.Height)
            {
                this.Height = Screen.PrimaryScreen.Bounds.Height - this.Top - 60;

            }
            string mWindowState = Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "mWindowState", "").ToString();

            if (mWindowState == "Maximized") this.WindowState = FormWindowState.Maximized;

            dxfReaderNETControl1.HighlightEntityOnHover = Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "HighlightEntityOnHover", false).ToString());
            dxfReaderNETControl1.ContinuousHighlight = Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ContinuousHighlight", false).ToString());
            dxfReaderNETControl1.HighlightNotContinuous = Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "HighlightNotContinuous", false).ToString());
            dxfReaderNETControl1.HighlightGrabPoints = Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "HighlightGrabPoints", false).ToString());

            dxfReaderNETControl1.ShowAxes = Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowAxes", false).ToString());
            dxfReaderNETControl1.ShowBasePoint = Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowBasePoint", false).ToString());
            dxfReaderNETControl1.ShowLimits = Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowLimits", false).ToString());
            dxfReaderNETControl1.ShowExtents = Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowExtents", false).ToString());

            dxfReaderNETControl1.ShowGrid = Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowGrid", true).ToString());
            dxfReaderNETControl1.ShowGridRuler = Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowGridRuler", false).ToString());

            dxfReaderNETControl1.AntiAlias = Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "AntiAlias", false).ToString());

            dxfReaderNETControl1.ForeColor = Color.FromArgb((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ForeColor", Color.White.ToArgb()));
            dxfReaderNETControl1.BackColor = Color.FromArgb((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "BackColor", Color.FromArgb(33, 40, 48).ToArgb()));

            dxfReaderNETControl1.HighlightMarkerColor = Color.FromArgb((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "HighlightMarkerColor", Color.FromArgb(255, 255, 0).ToArgb()));
            dxfReaderNETControl1.HighlightColor = Color.FromArgb((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "HighlightColor", Color.FromArgb(255, 0, 0).ToArgb()));
            dxfReaderNETControl1.AxisXColor = Color.FromArgb((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "AxisXColor", Color.FromArgb(72, 38, 43).ToArgb()));
            dxfReaderNETControl1.AxisYColor = Color.FromArgb((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "AxisYColor", Color.FromArgb(34, 102, 39).ToArgb()));
            dxfReaderNETControl1.AxisZColor = Color.FromArgb((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "AxisZColor", Color.FromArgb(32, 35, 175).ToArgb()));
            dxfReaderNETControl1.AxesColor = Color.FromArgb((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "AxesColor", Color.FromArgb(255, 0, 0).ToArgb()));
            dxfReaderNETControl1.GridColor = Color.FromArgb((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "GridColor", Color.FromArgb(38, 45, 55).ToArgb()));




            dxfReaderNETControl1.ObjectOsnapMode = (ObjectOsnapTypeFlags)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ObjectOsnapMode", 0);


            dxfReaderNETControl1.GridDisplay = (GridDisplayType)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "GridDisplay", 0);

            dxfReaderNETControl1.PlotRotation = (PlotOrientationType)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotRotation", 0);
            dxfReaderNETControl1.PlotRendering = (PlotRenderingType)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotRendering", 0);
            dxfReaderNETControl1.PlotMode = (PlotModeType)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotMode", 0);
            dxfReaderNETControl1.PlotUnits = (PlotUnitsType)Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotUnits", 0);

            CurrentLoadDXFPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "CurrentLoadDXFPath", CurrentLoadDXFPath).ToString();
            CurrentSaveDXFPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "CurrentSaveDXFPath", CurrentSaveDXFPath).ToString();




            dxfReaderNETControl1.PlotMarginTop = float.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotMarginTop", 0.0F).ToString(), System.Globalization.CultureInfo.CurrentCulture);
            dxfReaderNETControl1.PlotMarginBottom = float.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotMarginBottom", 0.0F).ToString(), System.Globalization.CultureInfo.CurrentCulture);
            dxfReaderNETControl1.PlotMarginLeft = float.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotMarginLeft", 0.0F).ToString(), System.Globalization.CultureInfo.CurrentCulture);
            dxfReaderNETControl1.PlotMarginRight = float.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotMarginRight", 0.0F).ToString(), System.Globalization.CultureInfo.CurrentCulture);


            Vector2 po = new Vector2(float.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotOffsetX", 0.0F).ToString(), System.Globalization.CultureInfo.CurrentCulture), float.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotOffsetY", 0.0F).ToString(), System.Globalization.CultureInfo.CurrentCulture));
            dxfReaderNETControl1.PlotOffset = po;

            dxfReaderNETControl1.PlotScale = float.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotScale", 1.0F).ToString(), System.Globalization.CultureInfo.CurrentCulture);




            dxfReaderNETControl1.PlotPenWidth = float.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotPenWidth", 0.0F).ToString(), System.Globalization.CultureInfo.CurrentCulture);

            dxfReaderNETControl1.CursorSelectionSize = int.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "CursorSelectionSize", 3).ToString(), System.Globalization.CultureInfo.CurrentCulture);
            dxfReaderNETControl1.RubberBandPenWidth = int.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "RubberBandPenWidth", 0).ToString(), System.Globalization.CultureInfo.CurrentCulture);
            dxfReaderNETControl1.ZoomInOutPercent = float.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ZoomInOutPercent", 50).ToString(), System.Globalization.CultureInfo.CurrentCulture);




        }

        private void SaveRegistry()
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "CurrentLoadDXFPath", CurrentLoadDXFPath);

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "CurrentSaveDXFPath", CurrentSaveDXFPath);



            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "GridDisplay", System.Convert.ToInt32(dxfReaderNETControl1.GridDisplay));
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "HighlightEntityOnHover", dxfReaderNETControl1.HighlightEntityOnHover);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ContinuousHighlight", dxfReaderNETControl1.ContinuousHighlight);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "HighlightNotContinuous", dxfReaderNETControl1.HighlightNotContinuous);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "HighlightGrabPoints", dxfReaderNETControl1.HighlightGrabPoints);


            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowAxes", dxfReaderNETControl1.ShowAxes);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowBasePoint", dxfReaderNETControl1.ShowBasePoint);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowLimits", dxfReaderNETControl1.ShowLimits);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowExtents", dxfReaderNETControl1.ShowExtents);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowGrid", dxfReaderNETControl1.ShowGrid);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ShowGridRuler", dxfReaderNETControl1.ShowGridRuler);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "AntiAlias", dxfReaderNETControl1.AntiAlias);








            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ForeColor", dxfReaderNETControl1.ForeColor.ToArgb());
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "BackColor", dxfReaderNETControl1.BackColor.ToArgb());
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "HighlightMarkerColor", dxfReaderNETControl1.HighlightMarkerColor.ToArgb());
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "HighlightColor", dxfReaderNETControl1.HighlightColor.ToArgb());
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "AxisXColor", dxfReaderNETControl1.AxisXColor.ToArgb());
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "AxisYColor", dxfReaderNETControl1.AxisYColor.ToArgb());
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "AxisZColor", dxfReaderNETControl1.AxisZColor.ToArgb());
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "AxesColor", dxfReaderNETControl1.AxesColor.ToArgb());
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "GridColor", dxfReaderNETControl1.GridColor.ToArgb());

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "wWidth", this.Width);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "wHeight", this.Height);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "wLeft", this.Left);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "wTop", this.Top);

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "mWindowState", this.WindowState);


            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotRotation", System.Convert.ToInt32(dxfReaderNETControl1.PlotRotation));
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotRendering", System.Convert.ToInt32(dxfReaderNETControl1.PlotRendering));
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotMode", System.Convert.ToInt32(dxfReaderNETControl1.PlotMode));
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotUnits", System.Convert.ToInt32(dxfReaderNETControl1.PlotUnits));

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotMarginTop", dxfReaderNETControl1.PlotMarginTop);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotMarginBottom", dxfReaderNETControl1.PlotMarginBottom);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotMarginLeft", dxfReaderNETControl1.PlotMarginLeft);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotMarginRight", dxfReaderNETControl1.PlotMarginRight);

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotOffsetX", dxfReaderNETControl1.PlotOffset.X);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotOffsetY", dxfReaderNETControl1.PlotOffset.Y);

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotScale", dxfReaderNETControl1.PlotScale);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "PlotPenWidth", dxfReaderNETControl1.PlotPenWidth);

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "CursorSelectionSize", dxfReaderNETControl1.CursorSelectionSize);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "RubberBandPenWidth", dxfReaderNETControl1.RubberBandPenWidth);

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ZoomInOutPercent", dxfReaderNETControl1.ZoomInOutPercent);



            Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + ProgramName, "ObjectOsnapMode", System.Convert.ToInt32(dxfReaderNETControl1.ObjectOsnapMode));


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveRegistry();
        }

        private void splitIntoPartsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetTab();
            this.Cursor = Cursors.WaitCursor;
            List<DxfDocument> myDocList = dxfReaderNETControl1.SplitIntoParts(dxfReaderNETControl1.DXF.Entities);

            int n = 0;
            foreach (DxfDocument dxf in myDocList)
            {

                n++;
                TabPage tp = new TabPage("Part" + n.ToString());
                tabControl1.TabPages.Add(tp);
                DXFReaderNETControl dxfControl = new DXFReaderNETControl();

                tp.Controls.Add(dxfControl);


                dxfControl.Dock = DockStyle.Fill;
                dxfControl.ShowAxes = false;
                dxfControl.ShowGrid = true;

                dxfControl.DXF = dxf;

                Vector3 ExtMin = new Vector3();
                Vector3 ExtMax = new Vector3();
                MathHelper.EntitiesExtensions(dxfControl.DXF.Entities, out ExtMin, out ExtMax);


                dxfControl.ZoomWindow(ExtMin.ToVector2(), ExtMax.ToVector2());

            }
            this.Cursor = Cursors.Default;

        }
        private void ResetTab()
        {
            for (int k = tabControl1.TabPages.Count - 1; k >= 1; k--)
                tabControl1.TabPages.Remove(tabControl1.TabPages[k]);
        }
        private void saveAllPartsToolStripMenuItem_Click(object sender, EventArgs e)
        {


            folderBrowserDialog1.SelectedPath = CurrentSaveDXFPath;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                CurrentSaveDXFPath = folderBrowserDialog1.SelectedPath;
                for (int k = 1; k < tabControl1.TabPages.Count; k++)
                {
                    string dxfFileName = folderBrowserDialog1.SelectedPath + "\\" + Path.GetFileName(dxfReaderNETControl1.FileName).ToLower().Replace(".dxf", "_" + tabControl1.TabPages[k].Text + ".dxf");
                    ((DXFReaderNETControl)tabControl1.TabPages[k].Controls[0]).WriteDXF(dxfFileName);
                }
            }

        }

        private void drawingInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int tIndex = tabControl1.SelectedIndex;


            ((DXFReaderNETControl)tabControl1.TabPages[tIndex].Controls[0]).ShowDrawingInfo();

        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            int tIndex = tabControl1.SelectedIndex;
            if (tIndex > 0)
            {

                ((DXFReaderNETControl)tabControl1.TabPages[tIndex].Controls[0]).ZoomCenter();
            }

        }
    }
}

