using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;
using System.IO.Compression;
using ScottPlot;
namespace ClassLib
{
    public static class ReportGeneration
    {
        private static Film film;
        private static string filePath;
        public static void Run()
        {
            GenerateHorisontalBarGraph(film, filePath);
            string zipPath = @".\..\data\ReportExample.docx";
            string extractPath = @".\..\data\example";

            ZipFile.ExtractToDirectory(zipPath,extractPath);



            XElement root = XElement.Load(@".\..\data\example\word\document.xml");
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                {"title", "EOM"},
                {"year", "HOOOOOOOO"}
            };
        
            FindAndReplace(root, dict);
            root.Save(@".\..\data\example\word\document.xml");
            ZipFile.CreateFromDirectory(@".\..\data\example", @".\..\data\Report.docx");
        }
        public static void SetData(Film filmToGraph, string path)
        {
            film = filmToGraph;
            filePath = path;
        }
        private static void GenerateHorisontalBarGraph(Film film, string filePath)
        {
            var plt = new ScottPlot.Plot(600, 400);
            Random rand = new Random(0);
            int pointCount = 10;
            double[] xs = DataGen.Consecutive(pointCount);
            double[] ys = MakeYAsixs(film, pointCount);
            plt.PlotBar(xs, ys,horizontal: true);
            plt.Grid(lineStyle: LineStyle.Dot);
            string[] labels = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"};
            plt.YTicks(xs, labels);
            plt.SaveFig(filePath + @"\graph.png");

        }
        private static double[] MakeYAsixs(Film film, int count)
        {
            double[] newYs = new double[count];
            for(int i = 0; i < film.reviews.Count; i++)
            {
                int rating = film.reviews[i].rating;
                newYs[rating-1] +=1;
            }
            return newYs;
        }
        private static void FindAndReplace(XElement node, Dictionary<string, string> dict)
        {
            if (node.FirstNode != null
                && node.FirstNode.NodeType == XmlNodeType.Text)
            {
                string replacement;
                if (dict.TryGetValue(node.Value, out replacement))
                {
                    node.Value = replacement;
                }
            }
            foreach (XElement el in node.Elements())
            {
                FindAndReplace(el, dict);
            }
        }

    }
}