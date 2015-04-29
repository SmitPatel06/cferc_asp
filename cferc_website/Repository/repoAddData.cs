using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cferc_website.Models;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace cferc_website.Repository
{
    public class repoAddData
    {

        private MartinezDBEntities db;

        public repoAddData(MartinezDBEntities pContext)
        {
            this.db = pContext;
        }

        public void checkArea(area pArea)
        {
            db.areas.Add(pArea);
            db.SaveChanges();
        }

        public void checkIndustry(industry pIndustry)
        {
            db.industries.Add(pIndustry);
            db.SaveChanges();
        }

        public void checkMeasure(measure pMeasure)
        {
            db.measures.Add(pMeasure);
            db.SaveChanges();
        }

        public void checkSeries(series pSeries)
        {
            series addedSeries = new series();
            addedSeries.areaID = pSeries.areaID;
            addedSeries.measureID = pSeries.measureID;
            addedSeries.industryID = pSeries.industryID;
            addedSeries.seriesID = pSeries.seriesID;
            addedSeries.beginYear = pSeries.beginYear;
            addedSeries.beginPeriod = pSeries.beginPeriod;
            addedSeries.endYear = pSeries.endYear;
            addedSeries.endPeriod = pSeries.endPeriod;
            addedSeries.blsTable = pSeries.blsTable;


            db.series.Add(addedSeries);
            db.SaveChanges();

            //addData ****needs Multiple Active Result set on the web.config file.
            foreach (data_table val in pSeries.data_table)
            {
                db.data_table.Add(val);
                db.SaveChanges();
            }



        }

        public MartinezDBEntities context()
        {
            return db;

        }

        public string insertFromCsv(string fileName)
        {

            string[] strArray;

            string firstLine = string.Empty;

            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            StreamReader sr = new StreamReader(fileName);

            string headerline = sr.ReadLine();
            firstLine = sr.ReadLine();
            strArray = r.Split(firstLine);

            area newArea = new area();
            measure newMeasure = new measure();
            industry newIndustry = new industry();
            series newSeries = new series();



            newArea.areaID = strArray[2];
            newArea.areaName = strArray[1];
            newMeasure.measureID = strArray[4];
            newMeasure.measureName = strArray[3];
            newIndustry.industryID = strArray[6];
            newIndustry.industryName = strArray[5];

            newSeries.areaID = newArea.areaID;
            newSeries.measureID = newMeasure.measureID;
            newSeries.industryID = newIndustry.industryID;
            newSeries.seriesID = newArea.areaID + strArray[0];
            newSeries.beginYear = "NA";
            newSeries.beginPeriod = "NA";
            newSeries.endYear = "NA";
            newSeries.endPeriod = "NA";
            newSeries.blsTable = strArray[0];

            db.areas.Add(newArea);
            db.industries.Add(newIndustry);
            db.measures.Add(newMeasure);
            db.series.Add(newSeries);
            db.SaveChanges();

            //start reading from 2nd line in CSV
            foreach (var line in File.ReadAllLines(fileName, Encoding.GetEncoding(1250)).Skip(1))
            {
                strArray = r.Split(line);
                //assign split string to variables

                data_table newData = new data_table();



                int year = Convert.ToInt32(strArray[7]);
                long value = Convert.ToInt64(strArray[8]);



                newData.seriesID = newSeries.seriesID;
                newData.period = "NA";
                newData.year = year;
                newData.value = value;



                db.data_table.Add(newData);
                db.SaveChanges();






            }
            return "Updated Database";


        }

    }
}