using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Drawing;
using Newtonsoft.Json; //used to easily serialize the desired output data into json format
using YahooFinanceApi; // used to access the stock securities data from yahoo finance

namespace DotNetFramework_Algorithm_Chart_Start
{
    class Program
    {
        static void Main(string[] args)
        {
            // initalizations...
            string outputTxtFile = Directory.GetCurrentDirectory() + "../../../output.txt"; // used to output console data to a persistent file
            string JSONfilename = Directory.GetCurrentDirectory() + "../../../chartdata.json"; // used to output json data to a persistent file, later transferred to server

            // tickers used to find patterns for
            string[] tickers = { "PLTR", "LI", "TLRY", "UAVS", "NEPT", "SNDL", "FSKR", "RIDE",
            "RIOT", "HYLN", "APHA", "SPLP", "LMND", "WNW", "LAZR", "CRSR", "CNI"};

            DateTime endDate = DateTime.Today; // the end date is the most recent day, which is today
            DateTime startDate = DateTime.Today.AddMonths(-6); // the start date is 6 months prior to today

            File.WriteAllText(outputTxtFile, ""); // clears the outputtxtfile from previous runs
            // File.WriteAllText(JSONfilename, "{\n\t\"ihs\":[\n"); 
            // clears the output json data from previous runs 
            File.WriteAllText(JSONfilename, "{\n");

            string imagesFolderPath = Directory.GetCurrentDirectory() + "../../../images/";
            // if the images folder does not exist, create it as we will need it to store pattern folders which contain the chart images
            if (!Directory.Exists(imagesFolderPath))
            {
                DirectoryInfo di = new DirectoryInfo(imagesFolderPath);
                di.Create();
            }

            string ihsImagePath = Directory.GetCurrentDirectory() + "../../../images/ihs/"; // declares the path for ihs image storage

            string hsImagePath = Directory.GetCurrentDirectory() + "../../../images/hs/"; // path for hs image storage
            
            string doubleTopImagePath = Directory.GetCurrentDirectory() + "../../../images/doubleTop/"; // path for doubletop image storage
            
            string doubleBottomImagePath = Directory.GetCurrentDirectory() + "../../../images/doubleBottom/"; // path for double bottom image storage
            
            string tripleTopImagePath = Directory.GetCurrentDirectory() + "../../../images/tripleTop/"; // path for triple top image storage

            string tripleBottomImagePath = Directory.GetCurrentDirectory() + "../../../images/tripleBottom/"; // path for triple bottom image storage

            string bullishRectangleImagePath = Directory.GetCurrentDirectory() + "../../../images/bullishRectangle/"; // path for bullish rectangle image storage

            string bearishRectangleImagePath = Directory.GetCurrentDirectory() + "../../../images/bearishRectangle/"; // path for bearish rectangle image storage

            // imagepathslist holds all the image paths for the various patterns
            List<string> imagePathsList = new List<string>();
            imagePathsList.Add(ihsImagePath); imagePathsList.Add(hsImagePath);
            imagePathsList.Add(doubleTopImagePath); imagePathsList.Add(doubleBottomImagePath);
            imagePathsList.Add(tripleTopImagePath); imagePathsList.Add(tripleBottomImagePath);
            imagePathsList.Add(bullishRectangleImagePath); imagePathsList.Add(bearishRectangleImagePath);

            // if any images from previous runs is in the directory associated with a pattern, it is deleted
            // else create the directory as we will need it to hold our chart images
            // this is what the clearImageDir() call does.
            for (int i = 0; i < imagePathsList.Count; i++)
            {
                clearImageDir(imagePathsList[i]);
            }

            // declare a stock data object to hold all the stock data for one ticker
            stockDataObj stock_dataobj = new stockDataObj()
            {
                outputTxtFile = outputTxtFile,
                JSONfilename = JSONfilename
            };

            List<patterns> patternsList = new List<patterns>();
            patternsList.Add(patterns.ihs); patternsList.Add(patterns.hs);
            patternsList.Add(patterns.doubleTop); patternsList.Add(patterns.doubleBottom);
            patternsList.Add(patterns.tripleTop); patternsList.Add(patterns.tripleBottom);
            patternsList.Add(patterns.bullishRectangle);

            // searchForPattern will search the historical market data, trying to find a case 
            // that exhibits the specified pattern 
            for (int i = 0; i < patternsList.Count; i++)
            {
                searchForPattern(patternsList[i], tickers, stock_dataobj, startDate, endDate);

                // the json document has a few syntactical errors after being printed to
                // that is why the fixJsonDoc() call is made to correct these errors
                fixJSONdoc(JSONfilename);

                // this is the final step in printing to the json doc for the specified pattern
                File.AppendAllText(JSONfilename, "\t],\n");
            }

            // the final pattern is a little special because the json document cannot have a comma
            // after the closing bracket ]
            searchForPattern(patterns.bearishRectangle, tickers, stock_dataobj, startDate, endDate);

            fixJSONdoc(JSONfilename);

            File.AppendAllText(JSONfilename, "\t]\n");

            // finish up the json file 
            File.AppendAllText(JSONfilename, "\n}");

            // program has finished execution
            Console.WriteLine("All Done :) press any key to exit");
            Console.ReadKey();
        }

        // clearImageDir simply clears any contents of the folder
        // passed as an argument
        public static void clearImageDir(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                DirectoryInfo di = new DirectoryInfo(folderPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(folderPath);
                di.Create();
            }
        }

        // searchForPattern looks at the historical market data of the passed tickers
        // and analyzes the data for a specific pattern also passed as an argument
        public static void searchForPattern(patterns pattern, string[] tickers, stockDataObj stock_dataobj, DateTime startDate, DateTime endDate)
        {
            int i = 0;
            // iterate through each ticker to check for a pattern
            while (i < tickers.Length)
            {
                // awaiter holds the return value of the asynchronous task getStockData
                var awaiter = stock_dataobj.getStockData(tickers[i], startDate, endDate, pattern);

                // this if statement forces the main() to wait until the current ticker is done being evaluated
                // when the result of 1 is returned, move to the next ticker
                if (awaiter.Result == 1)
                {
                    i++;
                }
            }
            return;
        }

        // fixJsonDoc takes care of a few syntactical errors
        // produced by the newtonsoft Json api call. 
        public static void fixJSONdoc(string JSONfilename)
        {
            string[] jsonFileLines = File.ReadAllLines(JSONfilename);
            if (jsonFileLines[jsonFileLines.Length - 1].Contains(','))
            {
                string temp = jsonFileLines[jsonFileLines.Length - 1];
                string temp2 = "";
                for (int j = 0; j < temp.Length - 1; j++)
                {
                    temp2 += temp[j];
                }
                jsonFileLines[jsonFileLines.Length - 1] = temp2;
            }
            File.WriteAllLines(JSONfilename, jsonFileLines);
        }

    }

    // enum of the patterns we are looking for
    public enum patterns
    {
        ihs,
        hs,
        doubleTop,
        doubleBottom,
        tripleTop,
        tripleBottom,
        bullishRectangle,
        bearishRectangle
    }


}

