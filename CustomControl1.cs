using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.entities;
namespace WindowsFormsApp1
{
    public partial class CustomControl1 : Control
    {
        private List<entities.Component> _data;
        public List<entities.Component> Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public CustomControl1() { }
        public CustomControl1(List<entities.Component> components)
        {
            InitializeComponent();

            ResizeRedraw = true;
            Data = components;

        }

        private void CustomControl1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            //get the drawing area
            Rectangle clipRectangle = e.ClipRectangle;

            //determine the width of the bars
            var barWidth = clipRectangle.Width / Data.Count;
            //compute the maximum bar height
            var maxBarHeight = clipRectangle.Height * 0.8;
            //compute the scaling factor based on the maximum value that we want to represent
            var scalingFactor = maxBarHeight / Data.Max(x => x.numberComp);

            Brush redBrush = new SolidBrush(Color.Red);

            for (int i = 0; i < Data.Count; i++)
            {
                var barHeight = Data[i].numberComp * scalingFactor;

                graphics.FillRectangle(
                    redBrush,
                    i * barWidth,
                    (float)(clipRectangle.Height - barHeight),
                    (float)(0.8 * barWidth),
                    (float)barHeight);
                string label = Data[i].compName; // Assuming Data[i].Label contains the label for the bar
                SizeF labelSize = graphics.MeasureString(label, this.Font);
                float labelX = i * barWidth + (barWidth - labelSize.Width) / 2; // Center the label horizontally
                float labelY = (float)(clipRectangle.Height - barHeight) - labelSize.Height - 5; // Place the label above the bar with some offset
                graphics.DrawString(label, this.Font, Brushes.Black, labelX, labelY);
            }

        }
    }
}

