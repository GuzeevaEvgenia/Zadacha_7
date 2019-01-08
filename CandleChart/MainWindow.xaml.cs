using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SciChart.Charting.Model.DataSeries;
using MyLibrary;
using SciChart.Charting.DrawingTools.TradingAnnotations.ViewModels;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Examples.ExternalDependencies.Data;
using System.Timers;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Visuals.Axes;
using DataManager = MyLibrary.DataManager;
using SciChart.Charting.Common.Helpers;
using SciChart.Data.Model;

namespace CandleChart
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Queue<Candle> candles;
        private List<Candle> candlesList;
        private OhlcDataSeries<DateTime, double> dataSeries = new OhlcDataSeries<DateTime, double>();
        private Thread newThread;

        private void CandlestickChartExampleView_OnLoaded(object sender, RoutedEventArgs e)
        {
            
            sciChart.RenderableSeries[0].DataSeries = dataSeries;
            xlsxrb.IsChecked = true;
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            newThread = new Thread(() => AddCandlesTest());
            newThread.IsBackground = true;
            newThread.Start();
            StartButton.IsEnabled = false;
            xlsxrb.IsEnabled = false;
            jsonrb.IsEnabled = false;
        }

        private void AddFractal(int currentIndex)
        {
            int size = 20;

            var qqq = Fractals.Calculate(candlesList, currentIndex);

            if (qqq.Item1)
            {
                CustomAnnotation annotation = null;

                if (qqq.Item2 == Fractals.Type.Top)
                {
                    var uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "up.png", UriKind.RelativeOrAbsolute);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = uri;
                    bi.EndInit();

                    Image arrow = new Image()
                    {
                        Source = bi,
                        Width = size,
                        Height = size
                    };
                    annotation = new CustomAnnotation()
                    {
                        X1 = currentIndex,
                        Y1 = candlesList[currentIndex].High,
                        Content = arrow,
                        ContentTemplate = (DataTemplate)sciChart.Resources["AnnotationTemplate"],
                        VerticalAnchorPoint = VerticalAnchorPoint.Bottom,
                        HorizontalAnchorPoint = HorizontalAnchorPoint.Center
                    };
                }
                else if (qqq.Item2 == Fractals.Type.Bottom)
                {
                    var uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "down.png", UriKind.RelativeOrAbsolute);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = uri;
                    bi.EndInit();
                    Image arrow = new Image()
                    {
                        Source = bi,
                        Width = size,
                        Height = size
                    };
                    annotation = new CustomAnnotation()
                    {
                        X1 = currentIndex,
                        Y1 = candlesList[currentIndex].Low,
                        Content = arrow,
                        ContentTemplate = (DataTemplate)sciChart.Resources["AnnotationTemplate"],
                        VerticalAnchorPoint = VerticalAnchorPoint.Top,
                        HorizontalAnchorPoint = HorizontalAnchorPoint.Center
                    };
                }
                sciChart.Annotations.Add(annotation);
            }
        }

        private void AddCandlesTest()
        {
            while (candles.Count > 0)
            {
                Thread.Sleep(500);

                Candle current = candles.Dequeue();
                dataSeries.Append(
                    current.Date,
                    (double)current.Open,
                    (double)current.High,
                    (double)current.Low,
                    (double)current.Close);
                if (dataSeries.Count == 1)
                    sciChart.Dispatcher.Invoke(() => sciChart.ZoomExtents());
                sciChart.Dispatcher.Invoke(() => Fractal(current));
                sciChart.Dispatcher.Invoke(() => RefreshData());
            }
        }

        private void Xlsxrb_OnChecked(object sender, RoutedEventArgs e)
        {
            candles = DataManager.GetCandlesFromXLSX("prices.xlsx");
            candlesList = candles.ToList();
        }

        private void Jsonrb_OnChecked(object sender, RoutedEventArgs e)
        {
            candles = DataManager.GetCandlesFromJSON("prices.json");
            candlesList = candles.ToList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (newThread != null && newThread.IsAlive)
            {
                newThread.Abort();
                StartButton.IsEnabled = true;
            }
        }

        private void RefreshData()
        {
            sciChart.RenderableSeries[0].DataSeries = dataSeries;
            AxisX.VisibleRange = new IndexRange(dataSeries.Count - 20, dataSeries.Count - 1);
        }

        private void Fractal(Candle c)
        {
            if (dataSeries.Count > 4)
                AddFractal(candlesList.IndexOf(c) - 2);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            xlsxrb.IsEnabled = true;
            jsonrb.IsEnabled = true;
            sciChart.Annotations.Clear();
            dataSeries.Clear();
        }
    }
}
