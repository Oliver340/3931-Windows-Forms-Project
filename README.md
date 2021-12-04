# 3931-Windows-Forms-Project
Topics to go over:
Performance (we have the graph only draw what's on screen, rather than the entire wave)

How to build:
Extract our files to a folder, then double click on the solution
It should be able to run out of the box

In the event that you are unable to select anything, add these lines to Form1.Designer.cs (for whatever reason, it removed these line every time we made a change):
  Line 110:
    this.WaveChart.SelectionRangeChanged += chart_SelectionRangeChanged;
  Line 281:
    this.freqChart.SelectionRangeChanged += chart2_SelectionRangeChanged;
