
using System;
using System.Windows.Forms;

namespace _3931_Project_windows_forms
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.button1 = new System.Windows.Forms.Button();
            this.WaveChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            ((System.ComponentModel.ISupportInitialize)(this.WaveChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 32);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // WaveChart
            // 
            this.WaveChart.AccessibleName = "WaveChart";
            chartArea1.Name = "ChartArea1";
            this.WaveChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.WaveChart.Legends.Add(legend1);
            this.WaveChart.Location = new System.Drawing.Point(12, 41);
            this.WaveChart.Name = "WaveChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "chartSeries";
            this.WaveChart.Series.Add(series1);
            this.WaveChart.Size = new System.Drawing.Size(941, 392);
            this.WaveChart.TabIndex = 1;
            this.WaveChart.Text = "WaveChart";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(35, 439);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(104, 56);
            this.trackBar1.TabIndex = 3;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            //
            // Cursor1
            //
            this.WaveChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WaveChart_MouseDown);
            this.WaveChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.WaveChart_MouseMove);
            this.WaveChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.WaveChart_MouseUp);
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Location = new System.Drawing.Point(12, 64);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(21, 329);
            this.vScrollBar1.TabIndex = 4;
            this.vScrollBar1.Value = 50;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1164, 586);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.WaveChart);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.WaveChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataVisualization.Charting.Chart WaveChart;
        private System.Windows.Forms.DataVisualization.Charting.Cursor CursorX;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
    }
}

