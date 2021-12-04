# 3931-Windows-Forms-Project
Topics to go over:
Performance (we have the graph only draw what's on screen, rather than the entire wave)
When zooming out it only adds data on the right instead of redrawing the entire thing, as well as zooming in it only deletes what is not going to be on screen anymore.

How to build:
Extract our files to a folder, then double click on the solution
It should be able to run out of the box

In the event that you are unable to select anything, add these lines to Form1.Designer.cs (for whatever reason, it removed these line every time we made a change):
  Line 110:
    this.WaveChart.SelectionRangeChanged += chart_SelectionRangeChanged;
  Line 281:
    this.freqChart.SelectionRangeChanged += chart2_SelectionRangeChanged;

Note for filtering: If you try filtering make sure to select a part of the wave and click one of the windowing options or DFT (not Open DFT). Then the start of your select on the chart below will be your filter cut off, then whereever the selection ends will be the length of the filter. Next press high or low pass filter and click apply to apply to the wave data. You can now press play and hear the results, select an area and DFT if you would like to see the filtered data, note the changed values on the left.
