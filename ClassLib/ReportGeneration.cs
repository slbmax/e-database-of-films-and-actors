using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;
using System.IO.Compression;
using ScottPlot;
using System.IO;
namespace ClassLib
{
    public static class ReportGeneration
    {
        private static Film film;
        private static UserRepository repo;
        private static string filePath;
        private static string filmRating;
        public static void Run()
        {
            Review highestRatingReview = GetHighestReviewRating();
            if(highestRatingReview.content == null)
            {
                highestRatingReview.content = "";
                highestRatingReview.createdAt = DateTime.Now;
                highestRatingReview.rating = 0;
            }
            Review lowestRatingReview = GetLowestReviewRating();
            if(lowestRatingReview.content == null)
            {
                lowestRatingReview.content = "";
                lowestRatingReview.createdAt = DateTime.Now;
                lowestRatingReview.rating = 0;
            }
            string zipPath = @".\..\data\ReportExample.docx";
            string extractPath = @".\..\data\example";
            ZipFile.ExtractToDirectory(zipPath,extractPath);

            XElement root = XElement.Load(@".\..\data\example\word\document.xml");
        
            FindAndReplace(root, highestRatingReview, lowestRatingReview);
            root.Save(@".\..\data\example\word\document.xml");

            File.Delete(@".\..\data\example\word\media\image1.png");
            GenerateHorisontalBarGraph(film, @".\..\data\example\word\media");

            ZipFile.CreateFromDirectory(@".\..\data\example",filePath +@"\Report "+ DateTime.Now.ToString().Replace(":", ".")+".docx");
            

            Directory.Delete(@".\..\data\example",true);
        }
        public static void SetData(Film filmToGraph, string path, double rating, UserRepository userRepo)
        {
            film = filmToGraph;
            filePath = path;
            filmRating = rating == 0 ? "no rating yet" : rating.ToString();
            repo = userRepo;
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
            plt.SaveFig(filePath + @"\image1.png");

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
        private static void FindAndReplace(XElement node, Review highest, Review lowest)
        {
            if (node.FirstNode != null
                && node.FirstNode.NodeType == XmlNodeType.Text)
            {
                switch (node.Value)
                {
                    case "A": node.Value = ""; break;
                    case "title": node.Value = $"{film.title}"; break;
                    case "year": node.Value = $"{film.releaseYear}"; break;
                    case "amountActors": node.Value = $"{film.actors.Length}"; break;
                    case "amountReviews": node.Value = $"{film.reviews.Count}"; break;
                    case "rating": node.Value = $"{filmRating}"; break;
                    case "contentHighest":
                        if(highest.content == "") 
                            node.Value = "-"; 
                        else
                            node.Value = $"{highest.content}";
                        break;
                    case "ratingHighest": 
                        if(highest.content == "") 
                            node.Value = "-"; 
                        else
                            node.Value = $"{highest.rating}";
                        break;
                    case "createdHighest":
                        if(highest.content == "") 
                            node.Value = "-"; 
                        else
                            node.Value = $"{highest.createdAt.ToString()}";
                        break;
                    case "userHighest": 
                        if(highest.content == "") 
                            node.Value = "-"; 
                        else
                        {
                            string username = repo.GetById(highest.user_id).username;
                            node.Value = $"{username}";
                        }
                        break;
                    case "contentLowest":
                        if(lowest.content == "") 
                            node.Value = "-"; 
                        else
                            node.Value = $"{lowest.content}";
                        break;
                    case "ratingLowest": 
                        if(lowest.content == "") 
                            node.Value = "-"; 
                        else
                            node.Value = $"{lowest.rating}";
                        break;
                    case "createdLowest": 
                        if(lowest.content == "") 
                            node.Value = "-"; 
                        else
                            node.Value = $"{lowest.createdAt}";
                        break;
                    case "userLowest" :
                        if(lowest.content == "") 
                            node.Value = "-"; 
                        else
                        {
                            string username = repo.GetById(lowest.user_id).username;
                            node.Value = $"{username}";
                        }
                        break;
                }
            }
            foreach (XElement el in node.Elements())
            {
                FindAndReplace(el, highest, lowest);
            }
        }
        private static Review GetHighestReviewRating()
        {
            Review review = new Review();
            review.rating = 0;
            for(int i = 0; i<film.reviews.Count; i++)
            {
                if(film.reviews[i].rating > review.rating)
                    review = film.reviews[i];
            }
            return review;
        }
        private static Review GetLowestReviewRating()
        {
            Review review = new Review();
            review.rating = 11;
            for(int i = 0; i<film.reviews.Count; i++)
            {
                if(film.reviews[i].rating < review.rating)
                    review = film.reviews[i];
            }
            return review;
        }

    }
}