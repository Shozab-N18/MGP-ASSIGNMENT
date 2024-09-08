using System;
using ScottPlot.WinForms;
using Data;
using ScottPlot;
using Conditions;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Processor;

namespace Forms
{
    public class DataPlotForm : Form
    {
        private FormsPlot dataPlot;
        private List<RaceCarData> data;
        private List<ChannelCondition> ChannelConditions;
        private List<ScottPlot.Color> colors = new List<ScottPlot.Color>
        {
            Colors.Blue, Colors.Green, Colors.Purple, Colors.Orange, Colors.Red, Colors.Brown, Colors.Pink, Colors.Gray, Colors.Black
        };
        private List<string> analysisResults = new List<string>();
        private bool showAllPlots = false;
        private Button togglePlotButton;
        private string fileName;

        public DataPlotForm(List<RaceCarData> data, List<ChannelCondition> channelConditions, List<string> analysisResults, string fileName)
        {
            this.data = data;
            this.fileName = fileName;
            this.ChannelConditions = channelConditions;
            this.analysisResults = analysisResults;
            this.Size = new Size(1200, 800);
            this.Location = new Point(0, 0);

            InitializeComponents();
            UpdatePlotDisplay();
        }

        private void InitializeComponents()
        {
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = System.Windows.Forms.Orientation.Vertical,
                SplitterWidth = 20,
                IsSplitterFixed = false,
                SplitterDistance = (int)(this.ClientSize.Width * 0.7),
                BackColor = System.Drawing.Color.LightGray,
                Panel2MinSize = 0
            };

            dataPlot = new FormsPlot { Dock = DockStyle.Fill };
            splitContainer.Panel1.Controls.Add(dataPlot);

            var analysisPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var headerLabel = new System.Windows.Forms.Label
            {
                Text = "Analysis of Results",
                Font = new Font("Arial", 16, System.Drawing.FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 40
            };

            var analysisTextBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 12),
                BorderStyle = BorderStyle.None,
                BackColor = System.Drawing.Color.White,
                Text = string.Join(Environment.NewLine + Environment.NewLine, analysisResults)
            };

            togglePlotButton = new Button
            {
                Text = showAllPlots ? "Show Critical Plots" : "Show All Plots",
                Dock = DockStyle.Bottom,
                Height = 40,
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleCenter
            };
            togglePlotButton.Click += TogglePlotButton_Click;

            analysisPanel.Controls.Add(analysisTextBox);
            analysisPanel.Controls.Add(headerLabel);
            analysisPanel.Controls.Add(togglePlotButton);

            splitContainer.Panel2.Controls.Add(analysisPanel);

            this.Controls.Add(splitContainer);
        }

        private void AddConditionLineForChannel(ChannelCondition condition, ScottPlot.Plottables.Scatter scatter, ScottPlot.Color color)
        {
            scatter.LineColor = color;
            scatter.MarkerColor = color;

            scatter.FillY = true;
            scatter.FillYValue = condition.ConditionValue;

            var conditionLine = dataPlot.Plot.Add.HorizontalLine(condition.ConditionValue);
            conditionLine.LinePattern = LinePattern.Dotted;
            conditionLine.Color = color;
            conditionLine.ExcludeFromLegend = true;

            switch (condition.Operator)
            {
                case ComparisonOperator.LessThan:
                case ComparisonOperator.LessThanOrEqualTo:
                    scatter.FillYBelowColor = color.WithAlpha(.4);
                    scatter.FillYAboveColor = Colors.Transparent;
                    break;
                case ComparisonOperator.GreaterThan:
                case ComparisonOperator.GreaterThanOrEqualTo:
                    scatter.FillYBelowColor = Colors.Transparent;
                    scatter.FillYAboveColor = color.WithAlpha(.4);
                    break;
                case ComparisonOperator.EqualTo:
                    conditionLine.LinePattern = LinePattern.Solid;
                    scatter.FillYBelowColor = Colors.Transparent;
                    scatter.FillYAboveColor = Colors.Transparent;
                    break;
                case ComparisonOperator.NotEqualTo:
                    conditionLine.LinePattern = LinePattern.Dashed;
                    scatter.FillYBelowColor = color.WithAlpha(.4);
                    scatter.FillYAboveColor = color.WithAlpha(.4);
                    break;
            }
            var op = ComparisonOperatorHelper.GetSymbol(condition.Operator);
            conditionLine.Text = $"y {op} {condition.ConditionValue}";
        }

        private void TogglePlotButton_Click(object sender, EventArgs e)
        {
            showAllPlots = !showAllPlots;
            togglePlotButton.Text = showAllPlots ? "Show Critical Plots" : "Show All Plots";

            UpdatePlotDisplay();
        }
        
        private void UpdatePlotDisplay()
        {
            dataPlot.Plot.Clear();
            
            List<RaceCarData> filteredData = showAllPlots ? data : FilterDataByConditions(data);
            
            var channels = filteredData
                .GroupBy(record => record.Channel)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Add scatter plots for each channel
            var customColorCount = 0;
            foreach (var channelNumber in channels.Keys)
            {
                var channelData = channels[channelNumber];

                double[] dataX = channelData.Select(record => record.Time).ToArray();
                double[] dataY = channelData.Select(record => record.Value).ToArray();

                var scatter = dataPlot.Plot.Add.Scatter(dataX, dataY);
                scatter.LegendText = $"Channel {channelNumber}";
                scatter.MarkerShape = MarkerShape.FilledDiamond;
                scatter.MarkerSize = 10;
                scatter.LineWidth = 1;

                var condition = ChannelConditions.Find(c => c.Channel == channelNumber);
                if (condition != null)
                {
                    AddConditionLineForChannel(condition, scatter, colors[customColorCount % colors.Count]);
                    customColorCount++;
                }
            }

            dataPlot.Plot.XLabel("Time");
            dataPlot.Plot.YLabel("Value");
            dataPlot.Plot.Title($"Data from {fileName}");

            dataPlot.Refresh();
        }

        private List<RaceCarData> FilterDataByConditions(List<RaceCarData> data)
        {
            return data.Where(record => ChannelConditions.Any(condition => condition.Channel == record.Channel)).ToList();
        }
    }
}
