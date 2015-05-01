using cferc_website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace cferc_website.Repository
{
    public class repoDownloadData
    {
        //private Data Entities object 
        private MartinezDBEntities db;

        //constructor initializes entities object
        public repoDownloadData(MartinezDBEntities pContext)
        {
            this.db = pContext;
        }

        public MartinezDBEntities context()
        {
            return db;
        }

        //Measures Dropdown menu

        public IQueryable getMeasure()
        {
            var measureToSend = from measure in db.measures
                                select new
                                {
                                    measureID = measure.measureID,
                                    measureName = measure.measureName
                                };
            return measureToSend;
        }

        public IQueryable getIndustry(measure pMeasure)
        {

            var query = (from records in db.series
                         where records.measureID == pMeasure.measureID
                         select new { industryResult = records.industry })
                         .Distinct();

            var t = (from val in query
                        select new { industryID = val.industryResult.industryID, industryName = val.industryResult.industryName });

            return t;
        }

        public IQueryable getArea(measure pMeasure, industry pIndustry)
        {
            var seriesQuery = (from records in db.series
                               where records.measureID == pMeasure.measureID &&
                               records.industryID == pIndustry.industryID
                               select new { areaResult = records.area })
                               .Distinct();
            var query = (from val in seriesQuery
                         select new { areaID = val.areaResult.areaID, areaName = val.areaResult.areaName });
            return query;

        }

        public IQueryable getYears(measureIndustryArea pData)
        {
            //create list of areaIds to compare to in following linq query
            List<string> areaIds = new List<string>();
            foreach (var i in pData.area)
            {
                areaIds.Add(i.areaID);
            }

           
            var query = (from records in db.series
                         where records.measureID == pData.measureID &&
                         records.industryID == pData.industryID &&
                         areaIds.Contains(records.areaID)
                         select new { beginYear = records.beginYear, endYear = records.endYear })
                         .Distinct();
            return query;
        }

        public List<dataOutput> getData(inputData pData)
        {
            //create list of areaIds for linq query
            List<string> areaIds = new List<string>();

            foreach (var i in pData.area)
            {
                areaIds.Add(i.areaID);
            }

            int beginYear = Convert.ToInt32(pData.beginYear);
            int endYear = Convert.ToInt32(pData.endYear);
            //initialize list object
            List<dataOutput> output = new List<dataOutput>();

            //query series entity
            var data = (from records in db.series
                        where records.measureID == pData.measureID &&
                        records.industryID == pData.industryID &&
                        areaIds.Contains(records.areaID)
                        select records);

            //must loop through query objects that are enumerables: this one loops through each series returned above
            foreach (var i in data)
            {
                
                //create another enumerable with a data record for each series object
                var test = (from t in i.data_table
                            where t.year >= beginYear &&
                            t.year <= endYear
                            select new { year = t.year, val = t.value });
                //loop through data records and add to list
                foreach (var j in test)
                {
                    dataOutput temp = new dataOutput();
                    temp.areaName = i.area.areaName;
                    temp.year = j.year;
                    temp.val = j.val;
                    output.Add(temp);
                }
                
            }

            return output;
            
            
        }
        

    }

    public class measureIndustry
    {
        public string measureID {get; set;}
        public string measureName { get; set; }
        public string industryID { get; set; }
        public string industryName { get; set; }


    }

    public class measureIndustryArea
    {
        public string measureID { get; set; }
        public string measureName { get; set; }
        public string industryID { get; set; }
        public string industryName { get; set; }
        public List<area> area { get; set; }


    }

    public class inputData
    {
        public string measureID { get; set; }
        public string measureName { get; set; }
        public string industryID { get; set; }
        public string industryName { get; set; }
        public List<area> area { get; set; }
        public string beginYear { get; set; }
        public string endYear { get; set; }
    }

    public class inputDataForDownload
    {
        public string measureID { get; set; }
        public string measureName { get; set; }
        public string industryID { get; set; }
        public string industryName { get; set; }
        public string area { get; set; }
        public string beginYear { get; set; }
        public string endYear { get; set; }
    }

    public class dataOutput
    {
        public string areaName { get; set; }
        public int? year { get; set; }
        public long? val { get; set; }
 
    }

}