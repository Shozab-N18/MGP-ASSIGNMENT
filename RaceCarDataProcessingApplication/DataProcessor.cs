using System;
using Forms;
using Data;
using Conditions;
using System.Windows.Forms.Design;

namespace Processor 
{
    public class DataProcessor
    {
        private List<ChannelCondition> channelConditions = new List<ChannelCondition>
        {
            new ChannelCondition(2, -0.5, ComparisonOperator.LessThan), // Channel 2 < -0.5
            new ChannelCondition(7, 0, ComparisonOperator.LessThan)     // Channel 7 < 0
        };

        private List<RaceCarData> data = new List<RaceCarData>();

        private List<String> analysisResults = new List<String>();

        private string fileName;

        public void ProcessData()
        {
            // Channel 7 = Channel 5 – Channel 4
            CalculateNewChannelData(newChannel: 7, ch1: 5, ch2: 4, op: (x, y) => x - y);
            AnalyseData();
            PlotData();
        }

        public void ReadData(string path)
        {
            data.Clear();
            analysisResults.Clear();
            var lines = File.ReadLines(path).Skip(1); // Skip table header line
            
            foreach (var row in lines)
            {
                try 
                {
                    var column = row.Split('\t');
                    var time = double.Parse(column[0]);
                    var value = double.Parse(column[1]);
                    var outing = int.Parse(column[2]);
                    var channel = int.Parse(column[3]);
                    
                    if (double.IsNaN(time) || double.IsNaN(value)) continue;
                    data.Add(new RaceCarData(time, value, outing, channel));
                }
                catch (Exception e)
                {
                    throw new Exception($"Error reading data. {e.Message}");
                }
            }

            fileName = Path.GetFileName(path);
        }

        public void AnalyseData()
        {
            // Determine when conditions are first satisfied 
            foreach (var condition in channelConditions)
            {
                FindWhenConditionIsFirstMet(condition);
            }
            
            // Determine when all conditions are met at the same time
            FindWhenAllConditionsAreFirstSimultaneouslyMet();
        }

        public void FindWhenConditionIsFirstMet(ChannelCondition condition)
        {
            var channel = condition.Channel;
            var conditionValue = condition.ConditionValue;

            // Find data points where condition is met and sort by time to find the first occurrence
            var channelData = data.FindAll((record) => record.Channel == channel && condition.IsConditionMet(record.Value));
            channelData.Sort((a, b) => a.Time.CompareTo(b.Time));

            var op = ComparisonOperatorHelper.GetSymbol(condition.Operator);

            if (channelData.Count != 0) 
            {
                analysisResults.Add($"Channel {channel} {op} {conditionValue} condition first satisfied at time {channelData[0].Time} seconds.");
            } 
            else 
            {
                analysisResults.Add($"No time found where the condition Channel {channel} {op} {conditionValue} is satisfied.");
            }
        }

        public void FindWhenAllConditionsAreFirstSimultaneouslyMet()
        {
            var channelDataDictionary = new Dictionary<int, List<RaceCarData>>();
            foreach(var condition in channelConditions)
            {
                var channelData = data.FindAll((record) => record.Channel == condition.Channel && condition.IsConditionMet(record.Value));
                channelData.Sort((a, b) => a.Time.CompareTo(b.Time));

                channelDataDictionary[condition.Channel] = channelData;
            }

            bool conditionMet = false;

            // Extract time points from the first channel's data to compare against
            var referenceChannel = channelConditions.First().Channel;
            var referenceData = channelDataDictionary[referenceChannel];

            foreach (var referenceRecord in referenceData)
            {
                bool allConditionsMet = true;

                // Check if all other channels have a record with the same time
                foreach (var condition in channelConditions.Where(c => c.Channel != referenceChannel))
                {
                    if (!channelDataDictionary[condition.Channel].Any(record => record.Time == referenceRecord.Time))
                    {
                        allConditionsMet = false;
                        break;
                    }
                }

                if (allConditionsMet)
                {
                    analysisResults.Add($"All conditions first satisfied simultaneously at time {referenceRecord.Time} seconds.");
                    conditionMet = true;
                    break;
                }
            }

            if (!conditionMet)
            {
                analysisResults.Add("No simultaneous occurrence of all conditions found.");
            }
        }

        public void PlotData()
        {
            // var dataToPlot = data.FindAll((record) => record.Channel == 2 || record.Channel == 7);
            
            Application.EnableVisualStyles();
            Application.Run(new DataPlotForm(data, channelConditions, analysisResults, fileName));
        }

        public void CalculateNewChannelData(int newChannel, int ch1, int ch2, Func<double, double, double> op)
        {
            var ch1Data = data.FindAll((record) => record.Channel == ch1);
            var ch2Data = data.FindAll((record) => record.Channel == ch2);

            for(int i = 0; i < ch1Data.Count; i++) 
            {
                var ch1Time = ch1Data[i].Time;
                var ch1Value = ch1Data[i].Value;
                var ch2Value = ch2Data[i].Value;

                var newValue = op(ch1Value, ch2Value);
                data.Add(new RaceCarData(ch1Time, newValue, ch1Data[i].Outing, newChannel));
            }
        }

        public List<String> GetAnalysisResults()
        {
            return analysisResults;
        }

        public List<RaceCarData> GetData()
        {
            return data;
        }
    }
}
