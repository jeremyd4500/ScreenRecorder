using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScreenRecorder
{
    class AppMethods : Main
    {

        public string jsonPath = "../../Resources/AppSettings.json";
        public string[] positions1 = { "Center" };
        public string[] positions2 = { "Left", "Right" };
        public string[] positions3 = { "Left", "Center", "Right" };
        public int monitorCount;
        public int frameRate;
        public int quality;

        public void AddMonitors()
        {
            MonitorGrid.Children.Clear();

            RowDefinition row = new RowDefinition();
            MonitorGrid.RowDefinitions.Add(row);

            AddMonitorHeader("Monitor", 70, 0);
            AddMonitorHeader("Resolution", 180, 1);
            AddMonitorHeader("Position", 85, 2);
            AddMonitorHeader("Capture", 60, 3);

            for (int i = 1; i <= monitorCount; i++)
            {
                row = new RowDefinition();
                Label name = new Label();

                row.Height = new GridLength(22);
                MonitorGrid.RowDefinitions.Add(row);

                AddMonitorName(i);
                AddMonitorResolution(i);
                AddMonitorRecord(i);
            }

            if (monitorCount == 1)
            {
                AddMonitorPosition(positions1);
            }
            else if (monitorCount == 2)
            {
                AddMonitorPosition(positions2);
            }
            else
            {
                AddMonitorPosition(positions3);
            }

        }

        public void AddMonitorHeader(string title, int width, int columnIndex)
        {
            ColumnDefinition column = new ColumnDefinition();
            Label name = new Label();

            column.Width = new GridLength(width);
            MonitorGrid.ColumnDefinitions.Add(column);

            name.Content = title;
            name.Padding = new Thickness(0);
            name.Margin = new Thickness(0);
            name.HorizontalContentAlignment = HorizontalAlignment.Center;
            name.VerticalContentAlignment = VerticalAlignment.Center;
            name.BorderBrush = Brushes.Black;
            name.BorderThickness = new Thickness(1);
            name.Background = Brushes.Gray;
            Grid.SetRow(name, 0);
            Grid.SetColumn(name, columnIndex);
            MonitorGrid.Children.Add(name);
        }

        public void AddMonitorName(int rowIndex)
        {
            Label name = new Label();
            name.Content = rowIndex.ToString();
            name.Padding = new Thickness(0);
            name.Margin = new Thickness(0);
            name.HorizontalContentAlignment = HorizontalAlignment.Center;
            name.VerticalContentAlignment = VerticalAlignment.Center;
            name.BorderBrush = Brushes.Black;
            name.BorderThickness = new Thickness(.5);
            Grid.SetRow(name, rowIndex);
            Grid.SetColumn(name, 0);
            MonitorGrid.Children.Add(name);
        }

        public void AddMonitorPosition(string[] positions)
        {
            for (int i = 1; i <= monitorCount; i++)
            {
                Label position = new Label();
                position.Content = positions[i - 1];
                position.Padding = new Thickness(0);
                position.Margin = new Thickness(0);
                position.HorizontalContentAlignment = HorizontalAlignment.Center;
                position.VerticalContentAlignment = VerticalAlignment.Center;
                position.BorderBrush = Brushes.Black;
                position.BorderThickness = new Thickness(.5);
                Grid.SetRow(position, i);
                Grid.SetColumn(position, 2);
                MonitorGrid.Children.Add(position);
            }
        }

        public void AddMonitorRecord(int rowIndex)
        {
            RadioButton selected = new RadioButton();
            Grid.SetRow(selected, rowIndex);
            Grid.SetColumn(selected, 3);
            selected.HorizontalAlignment = HorizontalAlignment.Center;
            selected.VerticalAlignment = VerticalAlignment.Center;
            MonitorGrid.Children.Add(selected);
        }

        public void AddMonitorResolution(int rowIndex)
        {
            ComboBox resolution = new ComboBox();
            Grid.SetRow(resolution, rowIndex);
            Grid.SetColumn(resolution, 1);
            resolution.Items.Add("1280 x 720");
            resolution.Items.Add("1440 x 900");
            resolution.Items.Add("1600 x 900");
            resolution.Items.Add("1920 x 1080");
            resolution.Items.Add("2048 x 1080");
            resolution.Items.Add("2560 x 1440");
            resolution.Items.Add("3840 x 2160");
            resolution.SelectedIndex = 3;
            resolution.HorizontalContentAlignment = HorizontalAlignment.Center;
            resolution.VerticalContentAlignment = VerticalAlignment.Center;
            MonitorGrid.Children.Add(resolution);
        }

        public void UpdateFields()
        {
            monitorCount = Globals.settings.MonitorCount;
            frameRate = Globals.settings.FrameRate;
            quality = Globals.settings.Quality;

            RecordingLocation.Text = Globals.settings.RecordingLocation;
            MonitorCount.SelectedIndex = monitorCount - 1;
            FrameRateSelection.SelectedItem = frameRate;
            QualitySelection.SelectedItem = quality;

            AddMonitors();
        }

        public void UpdateJsonFile()
        {
            string updatedFile = JsonConvert.SerializeObject(Globals.settings);
            File.WriteAllText(jsonPath, updatedFile);
        }
    }
}
