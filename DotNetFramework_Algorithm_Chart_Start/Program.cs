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

            string hsImagePath = Directory.GetCurrentDirectory() + "../../../images/hs/";

            string doubleTopImagePath = Directory.GetCurrentDirectory() + "../../../images/doubleTop/";

            string doubleBottomImagePath = Directory.GetCurrentDirectory() + "../../../images/doubleBottom/";

            string tripleTopImagePath = Directory.GetCurrentDirectory() + "../../../images/tripleTop/";

            string tripleBottomImagePath = Directory.GetCurrentDirectory() + "../../../images/tripleBottom/";

            string bullishRectangleImagePath = Directory.GetCurrentDirectory() + "../../../images/bullishRectangle/";

            string bearishRectangleImagePath = Directory.GetCurrentDirectory() + "../../../images/bearishRectangle/";

            // if any images from previous runs is in the directory associated with a pattern, it is deleted
            // else create the directory as we will need it to hold our chart images

            clearImageDir(ihsImagePath);

            clearImageDir(hsImagePath);

            clearImageDir(doubleTopImagePath);

            clearImageDir(doubleBottomImagePath);

            clearImageDir(tripleTopImagePath);

            clearImageDir(tripleBottomImagePath);

            clearImageDir(bullishRectangleImagePath);

            clearImageDir(bearishRectangleImagePath);

            // declare a stock data object to hold all the stock data for one ticker
            stockDataObj stock_dataobj = new stockDataObj()
            {
                outputTxtFile = outputTxtFile,
                JSONfilename = JSONfilename
            };

            searchForPattern(patterns.ihs, tickers, stock_dataobj, startDate, endDate);

            fixJSONdoc(JSONfilename);

            File.AppendAllText(JSONfilename, "\t],\n");

            searchForPattern(patterns.hs, tickers, stock_dataobj, startDate, endDate);

            fixJSONdoc(JSONfilename);

            File.AppendAllText(JSONfilename, "\t],\n");

            searchForPattern(patterns.doubleTop, tickers, stock_dataobj, startDate, endDate);

            fixJSONdoc(JSONfilename);

            File.AppendAllText(JSONfilename, "\t],\n");

            searchForPattern(patterns.doubleBottom, tickers, stock_dataobj, startDate, endDate);

            fixJSONdoc(JSONfilename);

            File.AppendAllText(JSONfilename, "\t],\n");

            searchForPattern(patterns.tripleTop, tickers, stock_dataobj, startDate, endDate);

            fixJSONdoc(JSONfilename);

            File.AppendAllText(JSONfilename, "\t],\n");

            searchForPattern(patterns.tripleBottom, tickers, stock_dataobj, startDate, endDate);

            fixJSONdoc(JSONfilename);

            File.AppendAllText(JSONfilename, "\t],\n");

            searchForPattern(patterns.bullishRectangle, tickers, stock_dataobj, startDate, endDate);
             
            fixJSONdoc(JSONfilename);

            File.AppendAllText(JSONfilename, "\t],\n");

            searchForPattern(patterns.bearishRectangle, tickers, stock_dataobj, startDate, endDate);

            fixJSONdoc(JSONfilename);

            File.AppendAllText(JSONfilename, "\t]\n");


            // finish up the json file 
            File.AppendAllText(JSONfilename, "\n}");

            // program has finished execution
            Console.WriteLine("All Done :) press any key to exit");
            Console.ReadKey();
        }

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

    class stockDataObj
    {
        public string outputTxtFile { get; set; } //outputtxtfile is the same as declared in main
        public string JSONfilename { get; set; } // jsonfilename is the same as declared in main
        public int idCount = 1; // idcount is used to index each entry into the chartdata.json file
        public IReadOnlyList<Candle> historic_data; // historic_data holds the market data from yahoo finance of a particular ticker
        public string companyName = ""; // companyname is used to hold the tickers associated company name
        public bool writtenIHS = false;
        public bool writtenHS = false;
        public bool writtendoubleTop = false;
        public bool writtendoubleBottom = false;
        public bool writtentripleTop = false;
        public bool writtentripleBottom = false;
        public bool writtenbullishRectangle = false;
        public bool writtenbearishRectangle = false;

        // getStockData accepts the ticker symbol, the startDate (6 months ago from today) and the endDate (today)
        // this is an asynchronous function meaning the code does not necessarily run sequentially
        // asynchronous code is noted by the await ... 
        public async Task<int> getStockData(string symbol, DateTime startDate, DateTime endDate, patterns pattern)
        {
            // asynchronous code must be placed in a try catch block, as there is no gurantee it will run correctly
            try
            {
                // Yahoo.GetHistoricalAsync gets the historical data for a particular ticker symbol from the passed start and end dates
                historic_data = await Yahoo.GetHistoricalAsync(symbol, startDate, endDate, Period.Daily);

                // Yahoo.symbols(symbol) can return a variety of different fields (properties) associated with a particular ticker
                // the return is in the form of a IReadOnlyDictionary so the returned value must be indexed by key-value pairs
                var security = await Yahoo.Symbols(symbol).Fields(Field.LongName).QueryAsync();

                // tick holds the data associated with the particular ticker symbol
                var tick = security[symbol];

                // companyname holds the value of the dictionary at key Field.LongName
                companyName = tick[Field.LongName];

                if (pattern == patterns.ihs)
                {
                    // initalize the json file for the ihs object data
                    if (!writtenIHS)
                    {
                        File.AppendAllText(JSONfilename, "\t\"ihs\":[\n");
                        writtenIHS = true;
                    }

                    // intervalformcheck checks for the formation of a pattern from the historical data of the ticker symbol
                    // the return is an int, intervalformcheck does not necessarily need to return anything but it helps to know when it is done
                    IHS_IntervalFormCheck(symbol);
                }
                if (pattern == patterns.hs)
                {
                    if (!writtenHS)
                    {
                        File.AppendAllText(JSONfilename, "\t\"hs\":[\n");
                        writtenHS = true;
                    }

                    HS_IntervalFormCheck(symbol);
                }
                if (pattern == patterns.doubleTop)
                {
                    if (!writtendoubleTop)
                    {
                        File.AppendAllText(JSONfilename, "\t\"doubleTop\":[\n");
                        writtendoubleTop = true;
                    }

                    DoubleTop_IntervalFormCheck(symbol);
                }

                //File.AppendAllText(JSONfilename, "\t]\n");

                if (pattern == patterns.doubleBottom)
                {
                    if (!writtendoubleBottom)
                    {
                        File.AppendAllText(JSONfilename, "\t\"doubleBottom\":[\n");
                        writtendoubleBottom = true;
                    }

                    DoubleBottom_IntervalFormCheck(symbol);
                }

                if (pattern == patterns.tripleTop)
                {
                    if (!writtentripleTop)
                    {
                        File.AppendAllText(JSONfilename, "\t\"tripleTop\":[\n");
                        writtentripleTop = true;
                    }

                    TripleTop_IntervalFormCheck(symbol);
                }

                if (pattern == patterns.tripleBottom)
                {
                    if (!writtentripleBottom)
                    {
                        File.AppendAllText(JSONfilename, "\t\"tripleBottom\":[\n");
                        writtentripleBottom = true;
                    }

                    TripleBottom_IntervalFormCheck(symbol);
                }

                if (pattern == patterns.bullishRectangle)
                {
                    if (!writtenbullishRectangle)
                    {
                        File.AppendAllText(JSONfilename, "\t\"bullishRectangle\":[\n");
                        writtenbullishRectangle = true;
                    }

                    BullishRectangle_IntervalFormCheck(symbol);
                }

                if (pattern == patterns.bearishRectangle)
                {
                    if (!writtenbearishRectangle)
                    {
                        File.AppendAllText(JSONfilename, "\t\"bearishRectangle\":[\n");
                        writtenbearishRectangle = true;
                    }

                    BearishRectangle_IntervalFormCheck(symbol);
                }
            }
            catch
            {
                // catch only executes if there was a failure in the asynchronous function calls
                Console.WriteLine("Failed to get data on symbol " + symbol);
                File.AppendAllText(outputTxtFile, "Failed to get data on symbol " + symbol + "\n");
            }

            // return 1 regardless of what happens because then the next ticker symbol can be analyzed
            return 1;
        }


        //Inverted Head and shoulders Algo, check is performed in reverse (meaning from end date to start date) to get the most recent pattern
        public int IHS_IntervalFormCheck(string symbol)
        {

            decimal thisTime; // used to hold the price at the current index of the historic data
            decimal nextTime; // used to hold the price of the next point of evaluation for the historic data
            decimal rightshoulder; // used to hold the price of the right shoulder of the pattern
            decimal leftshoulder; // used to hold the price of the left shoulder of the pattern
            decimal head; // used to hold the price of the head of the pattern (this will be the lowest price)

            // check for a pattern formation by using different intervals of time
            // this check starts with a 1 day interval and will increment as long as no pattern is found
            // if no pattern is found after a 15 day interval check, then no pattern was found
            for (int i = 1; i < 15; i++)
            {
                // start with the most recent entry in the historical data and work backwards
                for (int j = historic_data.Count - 1; j > (i * 6); j--)
                {
                    // initalize this time to the most recent entry 
                    thisTime = historic_data.ElementAt(j).Close;

                    // initalize next time to the most recent entry minus the interval
                    nextTime = historic_data.ElementAt(j - i).Close;
                    if (thisTime > nextTime) // if there is a dip in price...
                    {
                        thisTime = nextTime; // set thistime equal to nexttime
                        nextTime = historic_data.ElementAt(j - (i * 2)).Close; // set nexttime to the next price point depending on the interval
                        if (thisTime < nextTime) // if the price goes up...
                        {
                            rightshoulder = thisTime; // we know that the right shoulder would be thistime based on how the pattern forms
                            thisTime = nextTime;
                            nextTime = historic_data.ElementAt(j - (i * 3)).Close;
                            if (thisTime > nextTime) // if there is a dip in price...
                            {
                                thisTime = nextTime;
                                nextTime = historic_data.ElementAt(j - (i * 4)).Close;
                                if (thisTime < nextTime) // if the price goes up...
                                {
                                    head = thisTime; // we know that the head would be thistime based on how the pattern forms
                                    thisTime = nextTime;
                                    nextTime = historic_data.ElementAt(j - (i * 5)).Close;
                                    if (thisTime > nextTime) // if there is a dip in price ...
                                    {
                                        thisTime = nextTime;
                                        nextTime = historic_data.ElementAt(j - (i * 6)).Close;
                                        if (thisTime < nextTime) // if the price goes up ...
                                        {
                                            leftshoulder = thisTime; // we know that the leftshoulder would be thistime based on how the pattern forms

                                            // the following if statement ensures that the left and right shoulders are within 3% of one another and
                                            // the head is the minimum price for the timespan being evaluated
                                            if ((Math.Max(leftshoulder, rightshoulder) / Math.Min(leftshoulder, rightshoulder)) < Convert.ToDecimal(1.01) && head < Math.Min(leftshoulder, rightshoulder))
                                            {
                                                // at this point we know we have found an ihs pattern 
                                                Console.WriteLine("IHS Pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days");
                                                File.AppendAllText(outputTxtFile, "IHS Pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days\n");

                                                // need to generate a chart image for the pattern found.
                                                // this can be done by passing 
                                                // the ticker symbol, j which is the most recent element index of the historic data,
                                                // and the interval the pattern was found for i.
                                                // the chart generation function returns the reference string for the chartdata.json file
                                                string jsonImagePath = genChartReversed(patterns.ihs, symbol, j, i, 6);

                                                // stockObj objects hold the specific information for the stock symbol
                                                // we only need this when a pattern is found, that is why it is located here
                                                stockObj ihsobj = new stockObj()
                                                {
                                                    id = idCount,
                                                    symbol = symbol,
                                                    startdate = historic_data.ElementAt(j - (i * 6)).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j - (i * 6)).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j - (i * 6)).DateTime.Year.ToString(),
                                                    enddate = historic_data.ElementAt(j).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j).DateTime.Year.ToString(),
                                                    interval = i,
                                                    name = companyName,
                                                    image = jsonImagePath
                                                };

                                                // increment the idcount to start analysis of the next ticker symbol
                                                idCount++;

                                                // store the json serialized data of the stock object to the chartdata.json file
                                                File.AppendAllText(JSONfilename, "\t\t" + JsonConvert.SerializeObject(ihsobj) + ",\n");

                                                // return so that the next ticker symbol can begin its analysis
                                                return i;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // if no pattern is found, move on to the next ticker symbol
            return -1;
        }

        // head and shoulders algo, check is performed in reverse (meaning from end date to start date) to get the most recent pattern
        public int HS_IntervalFormCheck(string symbol)
        {
            decimal thisTime; // used to hold the price at the current index of the historic data
            decimal nextTime; // used to hold the price of the next point of evaluation for the historic data
            decimal rightshoulder; // used to hold the price of the right shoulder of the pattern
            decimal leftshoulder; // used to hold the price of the left shoulder of the pattern
            decimal head; // used to hold the price of the head of the pattern (this will be the lowest price)

            // check for a pattern formation by using different intervals of time
            // this check starts with a 1 day interval and will increment as long as no pattern is found
            // if no pattern is found after a 15 day interval check, then no pattern was found
            for (int i = 1; i < 15; i++)
            {
                // start with the most recent entry in the historical data and work backwards
                for (int j = historic_data.Count - 1; j > (i * 6); j--)
                {
                    // initalize this time to the most recent entry 
                    thisTime = historic_data.ElementAt(j).Close;

                    // initalize next time to the most recent entry minus the interval
                    nextTime = historic_data.ElementAt(j - i).Close;
                    if (thisTime < nextTime) // if the price goes up...
                    {
                        thisTime = nextTime; // set thistime equal to nexttime
                        nextTime = historic_data.ElementAt(j - (i * 2)).Close; // set nexttime to the next price point depending on the interval
                        if (thisTime > nextTime) // if there is a dip in price...
                        {
                            rightshoulder = thisTime; // we know that the right shoulder would be thistime based on how the pattern forms
                            thisTime = nextTime;
                            nextTime = historic_data.ElementAt(j - (i * 3)).Close;
                            if (thisTime < nextTime) // if the price goes up...
                            {
                                thisTime = nextTime;
                                nextTime = historic_data.ElementAt(j - (i * 4)).Close;
                                if (thisTime > nextTime) // if there is a dip in price...
                                {
                                    head = thisTime; // we know that the head would be thistime based on how the pattern forms
                                    thisTime = nextTime;
                                    nextTime = historic_data.ElementAt(j - (i * 5)).Close;
                                    if (thisTime < nextTime) // if the price goes up...
                                    {
                                        thisTime = nextTime;
                                        nextTime = historic_data.ElementAt(j - (i * 6)).Close;
                                        if (thisTime > nextTime) // if there is a dip in price
                                        {
                                            leftshoulder = thisTime; // we know that the leftshoulder would be thistime based on how the pattern forms

                                            // the following if statement ensures that the left and right shoulders are within 3% of one another and
                                            // the head is the maximum price for the timespan being evaluated
                                            if ((Math.Max(leftshoulder, rightshoulder) / Math.Min(leftshoulder, rightshoulder)) < Convert.ToDecimal(1.01) && head > Math.Max(leftshoulder, rightshoulder))
                                            {
                                                // at this point we know we have found an hs pattern 
                                                Console.WriteLine("HS Pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days");
                                                File.AppendAllText(outputTxtFile, "HS Pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days\n");

                                                // need to generate a chart image for the pattern found.
                                                // this can be done by passing 
                                                // the ticker symbol, j which is the most recent element index of the historic data,
                                                // and the interval the pattern was found for i.
                                                // the chart generation function returns the reference string for the chartdata.json file
                                                string jsonImagePath = genChartReversed(patterns.hs, symbol, j, i, 6);

                                                // stockObj objects hold the specific information for the stock symbol
                                                // we only need this when a pattern is found, that is why it is located here
                                                stockObj hsobj = new stockObj()
                                                {
                                                    id = idCount,
                                                    symbol = symbol,
                                                    startdate = historic_data.ElementAt(j - (i * 6)).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j - (i * 6)).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j - (i * 6)).DateTime.Year.ToString(),
                                                    enddate = historic_data.ElementAt(j).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j).DateTime.Year.ToString(),
                                                    interval = i,
                                                    name = companyName,
                                                    image = jsonImagePath
                                                };

                                                // increment the idcount to start analysis of the next ticker symbol
                                                idCount++;

                                                // store the json serialized data of the stock object to the chartdata.json file
                                                File.AppendAllText(JSONfilename, "\t\t" + JsonConvert.SerializeObject(hsobj) + ",\n");

                                                // return so that the next ticker symbol can begin its analysis
                                                return i;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // if no pattern is found, move on to the next ticker symbol
            return -1;
        }

        public int DoubleTop_IntervalFormCheck(string symbol)
        {
            decimal thisTime;
            decimal nextTime;
            decimal firstTop;
            decimal secondTop;
            decimal middleBottom;
            decimal pointSix; // refer to the pattern_image_identifiers folder and the double_top image to understand where this point (and other numeric points) is on the pattern

            for (int i = 1; i < 15; i++)
            {
                for (int j = historic_data.Count - 1; j > (i * 6); j--)
                {
                    thisTime = historic_data.ElementAt(j).Close;

                    nextTime = historic_data.ElementAt(j - i).Close;
                    if (thisTime < nextTime) // thistime is point 7
                    {
                        thisTime = nextTime;
                        nextTime = historic_data.ElementAt(j - (i * 2)).Close;
                        if (thisTime < nextTime) // thistime is point 6
                        {
                            pointSix = thisTime;
                            thisTime = nextTime;
                            nextTime = historic_data.ElementAt(j - (i * 3)).Close;
                            if (thisTime > nextTime) //thistime is point 5
                            {
                                secondTop = thisTime;
                                thisTime = nextTime;
                                nextTime = historic_data.ElementAt(j - (i * 4)).Close;
                                if (thisTime < nextTime) //thistime is point 4
                                {
                                    middleBottom = thisTime;
                                    thisTime = nextTime;
                                    nextTime = historic_data.ElementAt(j - (i * 5)).Close;
                                    if (thisTime > nextTime) // thistime is point 3
                                    {
                                        firstTop = thisTime;
                                        thisTime = nextTime;
                                        nextTime = historic_data.ElementAt(j - (i * 6)).Close;
                                        if (thisTime > nextTime && (Math.Max(firstTop, secondTop) / Math.Min(firstTop, secondTop)) < Convert.ToDecimal(1.005) &&
                                            (Math.Max(thisTime, middleBottom) / Math.Min(thisTime, middleBottom)) < Convert.ToDecimal(1.005) && 
                                            (Math.Max(middleBottom, pointSix) / Math.Min(middleBottom, pointSix)) < Convert.ToDecimal(1.005)) // thistime is point 2
                                        {
                                            //pattern found

                                            Console.WriteLine("Double Top Pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days");
                                            File.AppendAllText(outputTxtFile, "Double Top Pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days\n");

                                            string jsonImagePath = genChartReversed(patterns.doubleTop, symbol, j, i, 6);

                                            stockObj doubleTopObj = new stockObj()
                                            {
                                                id = idCount,
                                                symbol = symbol,
                                                startdate = historic_data.ElementAt(j - (i * 6)).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j - (i * 6)).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j - (i * 6)).DateTime.Year.ToString(),
                                                enddate = historic_data.ElementAt(j).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j).DateTime.Year.ToString(),
                                                interval = i,
                                                name = companyName,
                                                image = jsonImagePath
                                            };

                                            idCount++;

                                            File.AppendAllText(JSONfilename, "\t\t" + JsonConvert.SerializeObject(doubleTopObj) + ",\n");

                                            return i;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return -1;
        }

        public int DoubleBottom_IntervalFormCheck(string symbol)
        {
            decimal thisTime;
            decimal nextTime;
            decimal firstBottom;
            decimal secondBottom;
            decimal middleTop;
            decimal pointSix; // refer to the pattern_image_identifiers folder and the double_bottom image to understand where this (and other numeric) point is on the pattern
            for (int i = 1; i < 15; i++)
            {
                for (int j = historic_data.Count - 1; j > (i * 6); j--)
                {
                    thisTime = historic_data.ElementAt(j).Close;
                    nextTime = historic_data.ElementAt(j - i).Close;
                    if (thisTime > nextTime) // thistime is point 7
                    {
                        thisTime = nextTime;
                        nextTime = historic_data.ElementAt(j - (i * 2)).Close;
                        if (thisTime > nextTime) // thistime is point 6
                        {
                            pointSix = thisTime;
                            thisTime = nextTime;
                            nextTime = historic_data.ElementAt(j - (i * 3)).Close;
                            if (thisTime < nextTime) // thistime is point 5
                            {
                                secondBottom = thisTime;
                                thisTime = nextTime;
                                nextTime = historic_data.ElementAt(j - (i * 4)).Close;
                                if (thisTime > nextTime) // thistime is point 4
                                {
                                    middleTop = thisTime;
                                    thisTime = nextTime;
                                    nextTime = historic_data.ElementAt(j - (i * 5)).Close;
                                    if (thisTime < nextTime) // thistime is point 3
                                    {
                                        firstBottom = thisTime;
                                        thisTime = nextTime;
                                        nextTime = historic_data.ElementAt(j - (i * 6)).Close;
                                        if (thisTime < nextTime && (Math.Max(firstBottom, secondBottom) / Math.Min(firstBottom, secondBottom)) < Convert.ToDecimal(1.01) && 
                                            (Math.Max(thisTime, middleTop) / Math.Min(thisTime, middleTop)) < Convert.ToDecimal(1.01) && 
                                            (Math.Max(middleTop, pointSix) / Math.Min(middleTop, pointSix)) < Convert.ToDecimal(1.01))
                                        {
                                            // pattern found
                                            Console.WriteLine("Double Bottom Pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days");
                                            File.AppendAllText(outputTxtFile, "Double Bottom Pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days\n");
                                            string jsonImagePath = genChartReversed(patterns.doubleBottom, symbol, j, i, 6);
                                            stockObj doubleBottomObj = new stockObj()
                                            {
                                                id = idCount,
                                                symbol = symbol,
                                                startdate = historic_data.ElementAt(j - (i * 6)).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j - (i * 6)).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j - (i * 6)).DateTime.Year.ToString(),
                                                enddate = historic_data.ElementAt(j).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j).DateTime.Year.ToString(),
                                                interval = i,
                                                name = companyName,
                                                image = jsonImagePath
                                            };
                                            idCount++;
                                            File.AppendAllText(JSONfilename, "\t\t" + JsonConvert.SerializeObject(doubleBottomObj) + ",\n");
                                            return i;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return -1;
        }

        public int TripleTop_IntervalFormCheck(string symbol)
        {
            decimal thisTime;
            decimal nextTime;
            decimal firstTop;
            decimal secondTop;
            decimal thirdTop;
            decimal firstBottom;
            decimal secondBottom;
            decimal pointEight;
            for (int i = 1; i < 15; i++)
            {
                for (int j = historic_data.Count - 1; j > (i * 8); j--)
                {
                    thisTime = historic_data.ElementAt(j).Close;
                    nextTime = historic_data.ElementAt(j - i).Close;
                    if (thisTime < nextTime) // thistime is point 9
                    {
                        thisTime = nextTime;
                        nextTime = historic_data.ElementAt(j - (i * 2)).Close;
                        if (thisTime < nextTime) // thisttime is point 8
                        {
                            pointEight = thisTime;
                            thisTime = nextTime;
                            nextTime = historic_data.ElementAt(j - (i * 3)).Close;
                            if (thisTime > nextTime) // thistime is point 7
                            {
                                thirdTop = thisTime;
                                thisTime = nextTime;
                                nextTime = historic_data.ElementAt(j - (i * 4)).Close;
                                if (thisTime <  nextTime) //thistime is point 6
                                {
                                    secondBottom = thisTime;
                                    thisTime = nextTime;
                                    nextTime = historic_data.ElementAt(j - (i * 5)).Close;
                                    if (thisTime > nextTime) //thistime is point 5
                                    {
                                        secondTop = thisTime;
                                        thisTime = nextTime;
                                        nextTime = historic_data.ElementAt(j - (i * 6)).Close;
                                        if (thisTime < nextTime) //thistime is point 4
                                        {
                                            firstBottom = thisTime;
                                            thisTime = nextTime;
                                            nextTime = historic_data.ElementAt(j - (i * 7)).Close;
                                            if (thisTime > nextTime) //thisttime is point 3 
                                            {
                                                firstTop = thisTime;
                                                thisTime = nextTime;
                                                nextTime = historic_data.ElementAt(j - (i * 8)).Close;
                                                if (thisTime > nextTime && (Math.Max(firstTop, secondTop) / Math.Min(firstTop, secondTop)) < Convert.ToDecimal(1.02) && 
                                                    (Math.Max(secondTop, thirdTop) / Math.Min(secondTop, thirdTop)) < Convert.ToDecimal(1.02) &&
                                                    (Math.Max(firstBottom, secondBottom) / Math.Min(firstBottom, secondBottom)) < Convert.ToDecimal(1.02) && 
                                                    (Math.Max(secondBottom, pointEight) / Math.Min(secondBottom, pointEight)) < Convert.ToDecimal(1.02))
                                                {
                                                    // pattern found
                                                    Console.WriteLine("Triple Top pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days");
                                                    File.AppendAllText(outputTxtFile, "Triple Top pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days\n");
                                                    string jsonImagePath = genChartReversed(patterns.tripleTop, symbol, j, i, 8);
                                                    stockObj tripleTopObj = new stockObj()
                                                    {
                                                        id = idCount,
                                                        symbol = symbol,
                                                        startdate = historic_data.ElementAt(j - (i * 8)).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j - (i * 8)).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j - (i * 8)).DateTime.Year.ToString(),
                                                        enddate = historic_data.ElementAt(j).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j).DateTime.Year.ToString(),
                                                        interval = i,
                                                        name = companyName,
                                                        image = jsonImagePath
                                                    };

                                                    idCount++;
                                                    File.AppendAllText(JSONfilename, "\t\t" + JsonConvert.SerializeObject(tripleTopObj) + ",\n");
                                                    return i;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    
                    }
                }
            }
            return -1;
        }

        public int TripleBottom_IntervalFormCheck(string symbol)
        {
            decimal thisTime;
            decimal nextTime;
            decimal firstBottom;
            decimal secondBottom;
            decimal thirdBottom;
            decimal firstTop;
            decimal secondTop;
            decimal pointEight;
            for (int i = 1; i < 15; i++)
            {
                for (int j = historic_data.Count - 1; j > (i * 8); j--)
                {
                    thisTime = historic_data.ElementAt(j).Close;
                    nextTime = historic_data.ElementAt(j - i).Close;
                    if (thisTime > nextTime) //thistime is point 9
                    {
                        thisTime = nextTime;
                        nextTime = historic_data.ElementAt(j - (i * 2)).Close;
                        if(thisTime > nextTime) // thistime is point 8
                        {
                            pointEight = thisTime;
                            thisTime = nextTime;
                            nextTime = historic_data.ElementAt(j - (i * 3)).Close;
                            if (thisTime < nextTime) // thistime is point 7
                            {
                                thirdBottom = thisTime;
                                thisTime = nextTime;
                                nextTime = historic_data.ElementAt(j - (i * 4)).Close;
                                if (thisTime > nextTime) //thistime is point 6
                                {
                                    secondTop = thisTime;
                                    thisTime = nextTime;
                                    nextTime = historic_data.ElementAt(j - (i * 5)).Close;
                                    if (thisTime < nextTime) //thistime is point 5
                                    {
                                        secondBottom = thisTime;
                                        thisTime = nextTime;
                                        nextTime = historic_data.ElementAt(j - (i * 6)).Close;
                                        if (thisTime > nextTime) // thistime is point 4
                                        {
                                            firstTop = thisTime;
                                            thisTime = nextTime;
                                            nextTime = historic_data.ElementAt(j - (i * 7)).Close;
                                            if (thisTime < nextTime) // thistime is point 3
                                            {
                                                firstBottom = thisTime;
                                                thisTime = nextTime;
                                                nextTime = historic_data.ElementAt(j - (i * 8)).Close;
                                                if (thisTime < nextTime && (Math.Max(firstBottom, secondBottom) / Math.Min(firstBottom, secondBottom)) < Convert.ToDecimal(1.02) &&
                                                    (Math.Max(secondBottom, thirdBottom) / Math.Min(secondBottom, thirdBottom)) < Convert.ToDecimal(1.02) &&
                                                    (Math.Max(firstTop, secondTop) / Math.Min(firstTop, secondTop)) < Convert.ToDecimal(1.02) && 
                                                    (Math.Max(secondTop, pointEight) / Math.Min(secondTop, pointEight)) < Convert.ToDecimal(1.02))
                                                {
                                                    //pattern found
                                                    Console.WriteLine("Triple Bottom pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days");
                                                    File.AppendAllText(outputTxtFile, "Triple Bottom pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days\n");
                                                    string jsonImagePath = genChartReversed(patterns.tripleBottom, symbol, j, i, 8);
                                                    stockObj tripleBottomObj = new stockObj()
                                                    {
                                                        id = idCount,
                                                        symbol = symbol,
                                                        startdate = historic_data.ElementAt(j - (i * 8)).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j - (i * 8)).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j - (i * 8)).DateTime.Year.ToString(),
                                                        enddate = historic_data.ElementAt(j).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j).DateTime.Year.ToString(),
                                                        interval = i,
                                                        name = companyName,
                                                        image = jsonImagePath
                                                    };

                                                    idCount++;
                                                    File.AppendAllText(JSONfilename, "\t\t" + JsonConvert.SerializeObject(tripleBottomObj) + ",\n");
                                                    return i;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return -1;
        }


        public int BullishRectangle_IntervalFormCheck(string symbol)
        {
            decimal thisTime;
            decimal nextTime;
            decimal secondTop;
            decimal thirdTop;
            decimal firstBottom;
            decimal secondBottom;
            decimal thirdBottom;
            decimal pointEight;
            for (int i = 1; i < 15; i++)
            {
                for(int j = historic_data.Count - 1; j > (i * 8); j--)
                {
                    thisTime = historic_data.ElementAt(j).Close;
                    nextTime = historic_data.ElementAt(j - i).Close;
                    if (thisTime > nextTime) //thistime is point 9
                    {
                        thisTime = nextTime;
                        nextTime = historic_data.ElementAt(j - (i * 2)).Close;
                        if (thisTime > nextTime) // thistime is point 8
                        {
                            pointEight = thisTime;
                            thisTime = nextTime;
                            nextTime = historic_data.ElementAt(j - (i * 3)).Close;
                            if (thisTime < nextTime) // thistime is point 7
                            {
                                thirdBottom = thisTime;
                                thisTime = nextTime;
                                nextTime = historic_data.ElementAt(j - (i * 4)).Close;
                                if (thisTime > nextTime) // thistime is point 6
                                {
                                    thirdTop = thisTime;
                                    thisTime = nextTime;
                                    nextTime = historic_data.ElementAt(j - (i * 5)).Close;
                                    if (thisTime < nextTime) // thistime is point 5
                                    {
                                        secondBottom = thisTime;
                                        thisTime = nextTime;
                                        nextTime = historic_data.ElementAt(j - (i * 6)).Close;
                                        if (thisTime > nextTime) // thistime is point 4
                                        {
                                            secondTop = thisTime;
                                            thisTime = nextTime;
                                            nextTime = historic_data.ElementAt(j - (i * 7)).Close;
                                            if (thisTime < nextTime) // thistime is point 3
                                            {
                                                firstBottom = thisTime;
                                                thisTime = nextTime;
                                                nextTime = historic_data.ElementAt(j - (i * 8)).Close;
                                                if (thisTime > nextTime && (Math.Max(thisTime, secondTop) / Math.Min(thisTime, secondTop)) < Convert.ToDecimal(1.02) &&
                                                    (Math.Max(secondTop, thirdTop) / Math.Min(secondTop, thirdTop)) < Convert.ToDecimal(1.02) &&
                                                    (Math.Max(thirdTop, pointEight) / Math.Min(thirdTop, pointEight)) < Convert.ToDecimal(1.02) &&
                                                    (Math.Max(firstBottom, secondBottom) / Math.Min(firstBottom, secondBottom)) < Convert.ToDecimal(1.02) &&
                                                    (Math.Max(secondBottom, thirdBottom) / Math.Min(secondBottom, thirdBottom)) < Convert.ToDecimal(1.02)) // thistime is point 2
                                                {
                                                    //pattern found
                                                    Console.WriteLine("Bullish Rectangle pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days");
                                                    File.AppendAllText(outputTxtFile, "Bullish Rectangle pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days");
                                                    string jsonImagePath = genChartReversed(patterns.bullishRectangle, symbol, j, i, 8);
                                                    stockObj bullishRectangle = new stockObj()
                                                    {
                                                        id = idCount,
                                                        symbol = symbol,
                                                        startdate = historic_data.ElementAt(j - (i * 8)).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j - (i * 8)).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j - (i * 8)).DateTime.Year.ToString(),
                                                        enddate = historic_data.ElementAt(j).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j).DateTime.Year.ToString(),
                                                        interval = i,
                                                        name = companyName,
                                                        image = jsonImagePath
                                                    };

                                                    idCount++;
                                                    File.AppendAllText(JSONfilename, "\t\t" + JsonConvert.SerializeObject(bullishRectangle) + ",\n");
                                                    return i;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return -1;
        }

        public int BearishRectangle_IntervalFormCheck(string symbol)
        {
            decimal thisTime;
            decimal nextTime;
            decimal firstTop;
            decimal secondTop;
            decimal thirdTop;
            decimal secondBottom;
            decimal thirdBottom;
            decimal pointEight;
            for (int i = 1; i < 15; i++)
            {
                for (int j = historic_data.Count - 1; j > (i * 8); j--)
                {
                    thisTime = historic_data.ElementAt(j).Close;
                    nextTime = historic_data.ElementAt(j - i).Close;
                    if (thisTime < nextTime) // thistime is point 9
                    {
                        thisTime = nextTime;
                        nextTime = historic_data.ElementAt(j - (i * 2)).Close;
                        if (thisTime < nextTime) //thistime is point 8
                        {
                            pointEight = thisTime;
                            thisTime = nextTime;
                            nextTime = historic_data.ElementAt(j - (i * 3)).Close;
                            if (thisTime > nextTime) // thistime is point 7
                            {
                                thirdTop = thisTime;
                                thisTime = nextTime;
                                nextTime = historic_data.ElementAt(j - (i * 4)).Close;
                                if (thisTime < nextTime) //thistime is point 6
                                {
                                    thirdBottom = thisTime;
                                    thisTime = nextTime;
                                    nextTime = historic_data.ElementAt(j - (i * 5)).Close;
                                    if (thisTime > nextTime) // thistime is point 5
                                    {
                                        secondTop = thisTime;
                                        thisTime = nextTime;
                                        nextTime = historic_data.ElementAt(j - (i * 6)).Close;
                                        if (thisTime < nextTime) // thistime is point 4
                                        {
                                            secondBottom = thisTime;
                                            thisTime = nextTime;
                                            nextTime = historic_data.ElementAt(j - (i * 7)).Close;
                                            if (thisTime > nextTime) // thistime is point 3
                                            {
                                                firstTop = thisTime;
                                                thisTime = nextTime;
                                                nextTime = historic_data.ElementAt(j - (i * 8)).Close;
                                                if (thisTime < nextTime && (Math.Max(thisTime, secondBottom) / Math.Min(thisTime, secondBottom)) < Convert.ToDecimal(1.02) && 
                                                   (Math.Max(secondBottom, thirdBottom) / Math.Min(secondBottom, thirdBottom)) < Convert.ToDecimal(1.02) &&
                                                   (Math.Max(thirdBottom, pointEight) / Math.Min(thirdBottom, pointEight)) < Convert.ToDecimal(1.02) &&
                                                   (Math.Max(firstTop, secondTop) / Math.Min(firstTop, secondTop)) < Convert.ToDecimal(1.02) && 
                                                   (Math.Max(secondTop, thirdTop) / Math.Min(secondTop, thirdTop)) < Convert.ToDecimal(1.02))
                                                {
                                                    // pattern found
                                                    Console.WriteLine("Bearish Rectangle pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days");
                                                    File.AppendAllText(outputTxtFile, "Bearish Rectangle pattern found for symbol: " + symbol + " on: " + historic_data.ElementAt(j).DateTime.Date + " with interval: " + i + " days");
                                                    string jsonImagePath = genChartReversed(patterns.bearishRectangle, symbol, j, i, 8);
                                                    stockObj bearishRectangle = new stockObj()
                                                    {
                                                        id = idCount,
                                                        symbol = symbol,
                                                        startdate = historic_data.ElementAt(j - (i * 8)).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j - (i * 8)).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j - (i * 8)).DateTime.Year.ToString(),
                                                        enddate = historic_data.ElementAt(j).DateTime.Month.ToString() + "/" + historic_data.ElementAt(j).DateTime.Day.ToString() + "/" + historic_data.ElementAt(j).DateTime.Year.ToString(),
                                                        interval = i,
                                                        name = companyName,
                                                        image = jsonImagePath
                                                    };

                                                    idCount++;
                                                    File.AppendAllText(JSONfilename, "\t\t" + JsonConvert.SerializeObject(bearishRectangle) + ",\n");
                                                    return i;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return -1;
        }

        public string genChartReversed(patterns pattern, string symbol, int endIndex, int interval, int amountOfPoints)
        {
            // initalize some file path strings...
            string imagePath = "null";
            string jsonImagePath = "null";

            // if it is an ihs pattern, set the strings accordingly
            if (pattern == patterns.ihs)
            {
                imagePath = Directory.GetCurrentDirectory() + "../../../images/ihs/" + symbol + ".png";
                jsonImagePath = "/images/ihs/" + symbol + ".png";
            }
            else if (pattern == patterns.hs)
            {
                imagePath = Directory.GetCurrentDirectory() + "../../../images/hs/" + symbol + ".png";
                jsonImagePath = "/images/hs/" + symbol + ".png";
            }
            else if (pattern == patterns.doubleTop)
            {
                imagePath = Directory.GetCurrentDirectory() + "../../../images/doubleTop/" + symbol + ".png";
                jsonImagePath = "/images/doubleTop/" + symbol + ".png";
            }
            else if (pattern == patterns.doubleBottom)
            {
                imagePath = Directory.GetCurrentDirectory() + "../../../images/doubleBottom/" + symbol + ".png";
                jsonImagePath = "/images/doubleBottom/" + symbol + ".png";
            }
            else if (pattern == patterns.tripleTop)
            {
                imagePath = Directory.GetCurrentDirectory() + "../../../images/tripleTop/" + symbol + ".png";
                jsonImagePath = "/images/tripleTop/" + symbol + ".png";
            }
            else if (pattern == patterns.tripleBottom)
            {
                imagePath = Directory.GetCurrentDirectory() + "../../../images/tripleBottom/" + symbol + ".png";
                jsonImagePath = "/images/tripleBottom/" + symbol + ".png";
            }
            else if (pattern == patterns.bullishRectangle)
            {
                imagePath = Directory.GetCurrentDirectory() + "../../../images/bullishRectangle/" + symbol + ".png";
                jsonImagePath = "/images/bullishRectangle/" + symbol + ".png";
            }
            else if (pattern == patterns.bearishRectangle)
            {
                imagePath = Directory.GetCurrentDirectory() + "../../../images/bearishRectangle/" + symbol + ".png";
                jsonImagePath = "/images/bearishRectangle/" + symbol + ".png";
            }
            // create a new chart object
            Chart chart = new Chart();

            // add a chart area that is used to chart to
            ChartArea CA = chart.ChartAreas.Add("A1");

            // add a series that holds the price data of the ticker symbol for the timeframe of the pattern
            Series S1 = chart.Series.Add("S1");

            // declare a chartpricelist to hold the price values at the different dates (depending on the interval the pattern was found for)
            List<decimal> chartPriceList = new List<decimal>();

            // declare the chart type to be a line chart
            S1.ChartType = SeriesChartType.Line;

            // add the prices to the chart and the chartpricelist, this can be done in any order but this is
            // done in the order of the furthest in the past point to the most recent point for simplicity
            for(int i = amountOfPoints; i >= 0; i--)
            {
                S1.Points.AddXY(historic_data.ElementAt(endIndex - (interval * i)).DateTime.Date, historic_data.ElementAt(endIndex - (interval * i)).Close);
                chartPriceList.Add(historic_data.ElementAt(endIndex - (interval * i)).Close);
            }

            // specify some aesthetic properties for the chart
            // such as background color, alignment, fontstyle/weight, axis names
            chart.BackColor = Color.Transparent;
            CA.BackColor = Color.Transparent;
            CA.AxisX.TitleAlignment = StringAlignment.Center;
            CA.AxisY.TitleAlignment = StringAlignment.Center;
            CA.AxisX.TitleFont = new Font("Arial", 15, FontStyle.Bold);
            CA.AxisY.TitleFont = new Font("Arial", 15, FontStyle.Bold);
            CA.AxisX.Title = "Date";
            CA.AxisY.Title = "Price ($)";

            // determine the minimum price of the chart so we can determine what the min and max values should be on the chart axis'
            decimal minPrice = chartPriceList.ElementAt(0);
            decimal maxPrice = chartPriceList.ElementAt(0);

            for (int j = 0; j < chartPriceList.Count; j++)
            {
                if (chartPriceList.ElementAt(j) < minPrice)
                {
                    minPrice = chartPriceList.ElementAt(j);
                }
                if (chartPriceList.ElementAt(j) > maxPrice)
                {
                    maxPrice = chartPriceList.ElementAt(j);
                }
            }

            // set the chart's min and max to 1% greater/less than the max/min
            // this helps to utilize the entire chart area, or give it a 'zoomed in' look
            CA.AxisY.Minimum = Math.Round(Convert.ToDouble(minPrice) * 0.99, 2);
            CA.AxisY.Maximum = Math.Round(Convert.ToDouble(maxPrice) * 1.01, 2);

            // output some of this data to the console 
            Console.WriteLine("Amount of entries evaluated: " + chartPriceList.Count);
            Console.WriteLine("Min price: " + minPrice);
            Console.WriteLine("Max price: " + maxPrice);

            // more aesthetic properties set
            chart.AntiAliasing = AntiAliasingStyles.Graphics;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            chart.Size = new Size(1000, 800);
            chart.Titles.Add(symbol);
            chart.Titles.ElementAt(0).Font = new Font("Arial", 25, FontStyle.Bold);
            chart.Series["S1"].BorderWidth = 5;

            // declare the image to be a png image type
            chart.SaveImage(imagePath, ChartImageFormat.Png);

            // clear the list of prices (dont necessarily have to do this but its good practice)
            chartPriceList.Clear();

            //return the jsonimagepath to be output to the chartdata.json file
            return jsonImagePath;
        }
    }





    // stockObj class for holding the specific information of a ticker symbol when a pattern is found
    public class stockObj
    {
        public int id { get; set; }
        public string symbol { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public int interval { get; set; }
        public string name { get; set; }
        public string image { get; set; }
    }
}

