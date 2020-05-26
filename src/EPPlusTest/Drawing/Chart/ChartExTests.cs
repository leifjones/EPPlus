﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.ChartEx;
using OfficeOpenXml.Drawing.Chart.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EPPlusTest.Drawing.Chart
{
    [TestClass]
    public class ChartExTests : TestBase
    {
        static ExcelPackage _pck;
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _pck = OpenPackage("ChartEx.xlsx", true);
        }
        [ClassCleanup]
        public static void Cleanup()
        {
            SaveAndCleanup(_pck);
        }
        [TestMethod]
        public void ReadChartEx()
        {
            using (var p = OpenTemplatePackage("Chartex.xlsx"))
            {
                var chart1 = (ExcelChartEx)p.Workbook.Worksheets[0].Drawings[0];
                var chart2 = (ExcelChartEx)p.Workbook.Worksheets[0].Drawings[1];
                var chart3 = (ExcelChartEx)p.Workbook.Worksheets[0].Drawings[2];

                Assert.IsNotNull(chart1.Fill);
                Assert.IsNotNull(chart1.PlotArea);
                Assert.IsNotNull(chart1.Legend);
                Assert.IsNotNull(chart1.Title);
                Assert.IsNotNull(chart1.Title.Font);

                Assert.IsInstanceOfType(chart1.Series[0].DataDimensions[0], typeof(ExcelChartExStringData));
                Assert.AreEqual(eStringDataType.Category, ((ExcelChartExStringData)chart1.Series[0].DataDimensions[0]).Type);
                Assert.AreEqual("_xlchart.v1.0", chart1.Series[0].DataDimensions[0].Formula);
                Assert.IsInstanceOfType(chart1.Series[0].DataDimensions[1], typeof(ExcelChartExNumericData));
                Assert.AreEqual(eNumericDataType.Value, ((ExcelChartExNumericData)chart1.Series[0].DataDimensions[1]).Type);
                Assert.AreEqual("_xlchart.v1.2", chart1.Series[0].DataDimensions[1].Formula);

                Assert.IsInstanceOfType(chart1.Series[1].DataDimensions[0], typeof(ExcelChartExStringData));
                Assert.AreEqual("_xlchart.v1.0", chart1.Series[1].DataDimensions[0].Formula);
                Assert.IsInstanceOfType(chart1.Series[1].DataDimensions[1], typeof(ExcelChartExNumericData));
                Assert.AreEqual("_xlchart.v1.4", chart1.Series[1].DataDimensions[1].Formula);

            }
        }
        [TestMethod]
        public void AddSunburstChart()
        {
            var ws = _pck.Workbook.Worksheets.Add("Sunburst");
            AddHierarkiData(ws);
            var chart = ws.Drawings.AddExtendedChart("Sunburst1", eChartExType.Sunburst);
            var serie = chart.Series.Add("Sunburst!$A$2:$C$17", "Sunburst!$D$2:$D$17");
            chart.SetPosition(2, 0, 15, 0);
            chart.SetSize(1600, 900);
            serie.DataLabel.Position = eLabelPosition.Center;   
            serie.DataLabel.ShowCategory = true;
            serie.DataLabel.ShowValue=true;
            var dp=serie.DataPoints.Add(2);
            dp.Fill.Style = eFillStyle.PatternFill;
            dp.Fill.PatternFill.PatternType = eFillPatternStyle.DashDnDiag;
            dp.Fill.PatternFill.BackgroundColor.SetRgbColor(Color.Red);
            dp.Fill.PatternFill.ForegroundColor.SetRgbColor(Color.DarkGray);
            chart.StyleManager.SetChartStyle(ePresetChartStyle.SunburstChartStyle7);

            Assert.IsInstanceOfType(chart, typeof(ExcelSunburstChart));
            Assert.AreEqual(0, chart.Axis.Length);
            Assert.IsNull(chart.XAxis);
            Assert.IsNull(chart.YAxis);
            
        }
        [TestMethod]
        public void AddTreemapChart()
        {
            var ws = _pck.Workbook.Worksheets.Add("Treemap");
            AddHierarkiData(ws);
            var chart = ws.Drawings.AddExtendedChart("Treemap", eChartExType.Treemap);
            var serie = chart.Series.Add("Treemap!$A$2:$C$17", "Treemap!$D$2:$D$17");
            chart.SetPosition(2, 0, 15, 0);
            chart.SetSize(1600, 900);
            serie.DataLabel.Position = eLabelPosition.Center;
            serie.DataLabel.ShowCategory = true;
            serie.DataLabel.ShowValue = true;
            serie.DataLabel.ShowSeriesName = true;
            chart.StyleManager.SetChartStyle(ePresetChartStyle.TreemapChartStyle9);
            Assert.IsInstanceOfType(chart, typeof(ExcelTreemapChart));
        }
        [TestMethod]
        public void AddBoxWhiskerChart()
        {
            var ws = _pck.Workbook.Worksheets.Add("BoxWhisker");    
            AddHierarkiData(ws);
            var chart = ws.Drawings.AddBoxWhiskerChart("BoxWhisker");
            var serie = chart.Series.Add("BoxWhisker!$A$2:$C$17", "BoxWhisker!$D$2:$D$17");
            chart.SetPosition(2, 0, 15, 0);
            chart.SetSize(1600, 900);
            chart.StyleManager.SetChartStyle(ePresetChartStyle.BoxWhiskerChartStyle3);

            Assert.IsInstanceOfType(chart, typeof(ExcelBoxWhiskerChart));
            Assert.AreEqual(2, chart.Axis.Length);
            Assert.IsNotNull(chart.XAxis);
            Assert.IsNotNull(chart.YAxis);

            Assert.IsFalse(serie.ShowMeanLine);
            Assert.IsTrue(serie.ShowMeanMarker);
            Assert.IsTrue(serie.ShowOutliers);
            Assert.IsFalse(serie.ShowNonOutliers);

            Assert.AreEqual(eQuartileMethod.Exclusive, serie.QuartileMethod);
        }
        [TestMethod]
        public void AddHistogramChart()
        {
            var ws = _pck.Workbook.Worksheets.Add("Histogram");
            AddHierarkiData(ws);
            var chart = ws.Drawings.AddHistogramChart("Histogram");
            var serie = chart.Series.Add("Histogram!$A$2:$C$17", "Histogram!$D$2:$D$17");
            serie.Binning.Underflow = 1;
            serie.Binning.OverflowAutomatic = true;
            serie.Binning.Count = 3;
            chart.SetPosition(2, 0, 15, 0);
            chart.SetSize(1600, 900);
            chart.StyleManager.SetChartStyle(ePresetChartStyle.HistogramChartStyle2);

            Assert.IsInstanceOfType(chart, typeof(ExcelHistogramChart));
        }
        [TestMethod]
        public void AddParetoChart()
        {
            var ws = _pck.Workbook.Worksheets.Add("Pareto");
            AddHierarkiData(ws);
            var chart = ws.Drawings.AddHistogramChart("Pareto", true);
            var serie = chart.Series.Add("Pareto!$A$2:$C$17", "Pareto!$D$2:$D$17");
            chart.SetPosition(2, 0, 15, 0);
            chart.SetSize(1600, 900);

            Assert.IsInstanceOfType(chart, typeof(ExcelHistogramChart));
            Assert.IsNotNull(serie.ParetoLine);
            serie.ParetoLine.Fill.Style = eFillStyle.SolidFill;
            serie.ParetoLine.Fill.SolidFill.Color.SetRgbColor(Color.FromArgb(128,255,0,0),true);
            serie.ParetoLine.Effect.SetPresetShadow(ePresetExcelShadowType.OuterBottomRight);
            Assert.AreEqual(eChartType.Pareto, chart.ChartType);
            chart.StyleManager.SetChartStyle(ePresetChartStyle.HistogramChartStyle4);
        }
        [TestMethod]
        public void AddWaterfallChart()
        {
            var ws = _pck.Workbook.Worksheets.Add("Waterfall");
            AddHierarkiData(ws);
            var chart = ws.Drawings.AddWaterfallChart("Waterfall");
            var serie = chart.Series.Add("Waterfall!$A$2:$C$17", "Waterfall!$D$2:$D$17");
            chart.SetPosition(2, 0, 15, 0);
            chart.SetSize(1600, 900);
            var dt = chart.Series[0].DataPoints.Add(15);
            dt.SubTotal = true;
            dt = serie.DataPoints.Add(0);
            dt.SubTotal = true;            
            dt= serie.DataPoints.Add(4);
            dt.Fill.Style = eFillStyle.SolidFill;
            dt.Fill.SolidFill.Color.SetSchemeColor(eSchemeColor.Accent2);
            dt = serie.DataPoints.Add(2);
            dt.Fill.Style = eFillStyle.SolidFill;
            dt.Fill.SolidFill.Color.SetSchemeColor(eSchemeColor.Accent4);

            dt= serie.DataPoints[0];
            dt.Border.Fill.Style = eFillStyle.GradientFill;
            dt.Border.Fill.GradientFill.Colors.AddRgb(0, Color.Green);
            dt.Border.Fill.GradientFill.Colors.AddRgb(40, Color.Blue);
            dt.Border.Fill.GradientFill.Colors.AddRgb(70, Color.Red);
            dt.Fill.Style = eFillStyle.SolidFill;
            dt.Fill.SolidFill.Color.SetSchemeColor(eSchemeColor.Accent1);

            chart.StyleManager.SetChartStyle(ePresetChartStyle.HistogramChartStyle4);

            Assert.IsInstanceOfType(chart, typeof(ExcelWaterfallChart));
            Assert.AreEqual(4,serie.DataPoints.Count);
            Assert.IsTrue(serie.DataPoints[0].SubTotal);
            Assert.AreEqual(eFillStyle.GradientFill, serie.DataPoints[0].Border.Fill.Style);
            Assert.AreEqual(3, serie.DataPoints[0].Border.Fill.GradientFill.Colors.Count);
            Assert.AreEqual(eFillStyle.SolidFill, serie.DataPoints[0].Fill.Style);
            Assert.AreEqual(eSchemeColor.Accent1, serie.DataPoints[0].Fill.SolidFill.Color.SchemeColor.Color);
            Assert.IsTrue(serie.DataPoints[15].SubTotal);
        }
        [TestMethod]
        public void AddFunnelChart()
        {
            var ws = _pck.Workbook.Worksheets.Add("Funnel");
            AddHierarkiData(ws);
            var chart = ws.Drawings.AddFunnelChart("Funnel");
            var serie = chart.Series.Add("Funnel!$A$2:$C$17", "Funnel!$D$2:$D$17");
            chart.SetPosition(2, 0, 15, 0);
            chart.SetSize(1600, 900);
        }
        [TestMethod]
        public void AddRegionMapChart()
        {
            var ws = _pck.Workbook.Worksheets.Add("RegionMap");
            AddGeoData(ws);
            var chart = ws.Drawings.AddRegionMapChart("RegionMap");
            var serie = chart.Series.Add("RegionMap!$A$2:$B$11", "RegionMap!$C$2:$C$11");
            serie.Region = new CultureInfo("sv");
            serie.Language = new CultureInfo("sv-SE");
            serie.Colors.NumberOfColors = eNumberOfColors.ThreeColor;
            serie.Colors.MinColor.Color.SetSchemeColor(eSchemeColor.Dark1);
            serie.Colors.MinColor.ValueType = eColorValuePositionType.Number;
            serie.Colors.MinColor.PositionValue = 22;
            serie.Colors.MidColor.ValueType = eColorValuePositionType.Percent;
            serie.Colors.MidColor.PositionValue = 50.11;
            serie.Colors.MaxColor.ValueType = eColorValuePositionType.Extreme;
            serie.Colors.MaxColor.Color.SetRgbColor(Color.Red);
            serie.DataLabel.Border.Width = 1;
            serie.ViewedRegionType = eGeoMappingLevel.DataOnly;
            serie.ProjectionType = eProjectionType.Miller;
            chart.Legend.Add();
            chart.Legend.Position = eLegendPosition.Left;
            chart.Legend.PositionAlignment = ePositionAlign.Center;
            chart.Title.Text = "Sweden Region Map";
            chart.SetPosition(2, 0, 15, 0);
            chart.SetSize(1600, 900);

            Assert.AreEqual("sv", serie.Region.TwoLetterISOLanguageName);
            Assert.AreEqual("sv-SE", serie.Language.Name);
        }
        private class SalesData
        {
            public string Continent { get; set; }
            public string Country { get; set; }
            public string State { get; set; }
            public double Sales { get; set; }

        }
        private class GeoData
        {
            public string Country { get; set; }
            public string State { get; set; }
            public double Sales { get; set; }

        }

        private void AddHierarkiData(ExcelWorksheet ws)
        {

            var l = new List<SalesData>
            {
                new SalesData{ Continent="Europe", Country="Sweden", State = "Stockholm", Sales = 154 },
                new SalesData{ Continent="Asia", Country="Vietnam", State = "Ho Chi Minh", Sales= 88 },
                new SalesData{ Continent="Europe", Country="Sweden", State = "Västerås", Sales = 33 },
                new SalesData{ Continent="Asia", Country="Japan", State = "Tokyo", Sales= 534 },
                new SalesData{ Continent="Europe", Country="Germany", State = "Frankfurt", Sales = 109 },
                new SalesData{ Continent="Asia", Country="Vietnam", State = "Hanoi", Sales= 322 },
                new SalesData{ Continent="Asia", Country="Japan", State = "Osaka", Sales= 88 },
                new SalesData{ Continent="North America", Country="Canada", State = "Vancover", Sales= 99 },
                new SalesData{ Continent="Asia", Country="China", State = "Peking", Sales= 205 },
                new SalesData{ Continent="North America", Country="Canada", State = "Toronto", Sales= 138 },
                new SalesData{ Continent="Europe", Country="France", State = "Lyon", Sales = 185 },
                new SalesData{ Continent="North America", Country="USA", State = "Boston", Sales= 155 },
                new SalesData{ Continent="Europe", Country="France", State = "Paris", Sales = 127 },
                new SalesData{ Continent="North America", Country="USA", State = "New York", Sales= 330 },
                new SalesData{ Continent="Europe", Country="Germany", State = "Berlin", Sales = 210 },
                new SalesData{ Continent="North America", Country="USA", State = "San Fransico", Sales= 411 },
            };

            ws.Cells["A1"].LoadFromCollection(l, true, OfficeOpenXml.Table.TableStyles.Medium12);
        }
    private void AddGeoData(ExcelWorksheet ws)
    {

        var l = new List<GeoData>
            {
                new GeoData{ Country="Sweden", State = "Stockholm", Sales = 154 },
                new GeoData{ Country="Sweden", State = "Jämtland", Sales = 55 },
                new GeoData{ Country="Sweden", State = "Västerbotten", Sales = 44},
                new GeoData{ Country="Sweden", State = "Dalarna", Sales = 33 },
                new GeoData{ Country="Sweden", State = "Uppsala", Sales = 22 },
                new GeoData{ Country="Sweden", State = "Skåne", Sales = 47 },
                new GeoData{ Country="Sweden", State = "Halland", Sales = 88 },
                new GeoData{ Country="Sweden", State = "Norrbotten", Sales = 99 },
                new GeoData{ Country="Sweden", State = "Västra Götaland", Sales = 120 },
                new GeoData{ Country="Sweden", State = "Södermanland", Sales = 57 },
            };

        ws.Cells["A1"].LoadFromCollection(l, true, OfficeOpenXml.Table.TableStyles.Medium12);
    }
    }
}

