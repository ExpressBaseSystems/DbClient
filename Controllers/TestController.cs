using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using test.Models;

namespace test
{
    public class Result
    {
        public List<string> type = new List<string>();
        public List<List<string>> result = new List<List<string>>();
    }
   public class Query
    {
        public string qstring;
        public List<string> explain = new List<string>();
        public int rowno;
        public Result resultobj=new Result();
        // public List<JProperty> jsonobj = new List<JProperty>();
    }
    public class TestController : Controller
    {
        
        NpgsqlConnection conn,connection;
        NpgsqlDataReader dataReader;

        public IActionResult Test()
        {  
            return View();
        }

        public NpgsqlDataReader executeQuery(string sql)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            return (cmd.ExecuteReader());
        }
        public void SqlQuery(string sql)
        {
            conn.Open();
            var x = JsonConvert.DeserializeObject(sql).ToString();
            dataReader = executeQuery(x);
            while (dataReader.Read())
            {

            }
            connection.Close();
        }

        [HttpPost]
        public Query GetExplain()//string querytext, string queryparam)
        {
            List<string> dataitem = new List<string>();
            int counter = 0;
            Query q1 = null;
            int flag = 0;
            string json=null;
            var d=new List<ExecutionPlan>();
            try
            {
                conn = new NpgsqlConnection("Server=35.200.147.143;User Id=interns_november2018_admin;" +
                                "Password=ruwPcqyM;Database=interns_november2018;");
                conn.Open();
                q1 = new Query() { qstring= @"SELECT 
Distinct
    q1.customers_id as id,
    q2.accountcode,
    replace(initcap(q2.longname),'Hoc ','') AS CENTER,
    q2.consdate AS DOC,
    q1.trdate AS DOCLO,
    COALESCE(q4.ht,' ') AS HTDATE,
    COALESCE(q2.name,' ') AS NAME,
    COALESCE(q1.status,' ') AS STATUS,
    COALESCE(q2.typeofcustomer,' ') AS SERVICE,
    --COALESCE(to_number(regexp_replace(q2.age, '[^0-9]', '')),0) AS AGE,
    --EXTRACT(YEAR FROM age(q2.dob)) Age,
    COALESCE(date_part('year',age(dob)) ,0) AS AGE,
    COALESCE(q2.genurl,' ') AS MOBILE,
    COALESCE(q2.clcity,' ') AS City,
    q2.district AS DISTRICT,
    CASE WHEN q2.customertype='1' THEN 'Yes' ELSE 'No' END as NRI,
    COALESCE(q2.sourcecategory,' ') AS sourcecategory,
    COALESCE(q2.subcategory,' ') AS SOURCESUBCAT,
    COALESCE(q2.noofgrafts,0) AS GRAFTS,
    COALESCE(q2.totalrate,0) AS RATE,
    COALESCE(q5.advanceamount,0) AS ADVANCE,
    COALESCE(q3.doctor,' ') AS CONSULTEDDR,
    COALESCE(q1.createdby,' ') AS CLOSEDBY,
    q2.trdate as DOE,
q2.eb_loc_id
   
FROM

(
SELECT
    x.customers_id,x.trdate,x.status,y.createdby
FROM
(
        SELECT
    xx.customers_id,xx.trdate,xx.status
FROM
(
    SELECT
        customers_id, MIN(trdate) AS trdate, MAX(status) AS status
FROM
        leaddetails
WHERE
        (lower(status)='closed' OR lower(status)='ht done')  
GROUP BY customers_id
)xx
WHERE
    to_char(trdate,'mm/yyyy')=('01-2018')
    )x
    LEFT JOIN
    (
        SELECT
    customers_id,string_agg(distinct createdby,',') as createdby,trdate--,status
FROM
        leaddetails
        where
        status = 'Closed'
        GROUP BY customers_id,trdate
    )y ON x.customers_id=y.customers_id AND x.trdate = y.trdate
)q1
LEFT JOIN
    (
        SELECT
            LR.consdate,CV.name,CV.age,CV.genurl,CV.sourcecategory,CV.subcategory,CV.typeofcustomer,loc.longname,loc.id as eb_loc_id,LR.noofgrafts,LR.totalrate,CV.accountcode,
            CV.clcity,CV.customertype,CV.dob,CV.id,CV.trdate,CV.district
        FROM
            customers CV,eb_locations loc,leadratedetails LR
        WHERE
             CV.eb_loc_id = loc.id AND CV.id = LR.customers_id
    )q2
ON
q1.customers_id = q2.id
LEFT JOIN
(
    SELECT
        LS1.customers_id,D.name as Doctor
    FROM
        doctors D, leadratedetails LS1
    WHERE
        D.id=LS1.consultingdoctor
)q3
ON
q1.customers_id=q3.customers_id
LEFT JOIN
(SELECT
        LS.customers_id,string_agg(to_char( LS. dateofsurgery,'dd-Mon-yy'),',')as ht
        --regexp_replace(listagg(to_char(LS. dateofsurgery,'Mon-dd'),',') WITHIN GROUP (ORDER BY LS. dateofsurgery), '([^,]+)(,\1)+', '\1') as ht    
    FROM
        leadsurgerydetails LS
    GROUP BY
        LS.customers_id
)q4
ON q1.customers_id=q4.customers_id
LEFT JOIN
(
      SELECT
        LP.customers_id,sum(LP.advanceamount ) as advanceamount,MAX(LP.trdate)
    FROM
        leadpaymentdetails LP
    Group By
       LP.customers_id
)q5
ON q1.customers_id =q5.customers_id
--WHERE
--(CASE WHEN -1=-1 THEN eb_loc_id>0 ELSE eb_loc_id=ANY(string_to_array(:eb_location_id,',')::int[]) END)
-- eb_loc_id=ANY(string_to_array(:eb_location_id,',')::int[])
ORDER BY name;
 " };
                // command.Parameters.Add(":id", NpgsqlTypes.NpgsqlDbType.Integer);
                // command.Parameters[":id"].Value = 2;
                string sql2 = @"explain (format json, analyze on) " + q1.qstring + ";" +q1.qstring;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                NpgsqlDataReader dr = executeQuery(sql2);
                watch.Stop();
                //TimeSpan start = DateTime.Now.TimeOfDay;
                while (dr.Read())
                {
                    json = json + dr[0].ToString();
                    q1.explain.Add(dr[0].ToString());
                    d = JsonConvert.DeserializeObject<List<ExecutionPlan>>(json);
                }
               
                //q1.explain.Add("Start Time : " + start);
                //dr.NextResult();
                //int n = dr.FieldCount;
                
                //while (dr.Read())
                //{
                //    List<string> sublist = new List<string>();
                //    for (int i = 0; i < n; i++)
                //    {
                //        sublist.Add(dr[i].ToString());
                //    }
                //    q1.resultobj.result.Add(sublist);
                //    if (flag ==0 )
                //    {
                //        for (int i = 0; i < n; i++)
                //        {
                //            q1.resultobj.type.Add(dr.GetName(i));
                //        }
                //        flag = 1;
                //    }
                //    counter++;
                //}
                //TimeSpan end = DateTime.Now.TimeOfDay;
                //TimeSpan t = end - start;
                //int exec_time = (int)t.Milliseconds;
                //q1.explain.Add("End Time : " + end);
                q1.explain.Add("Execution Time : "  + watch.ElapsedMilliseconds+"ms");
                dr.Close();
                q1.rowno = counter;
                //NpgsqlCommand command = new NpgsqlCommand("insert into queryplan(startcost,totalcost,rows,loops,plan_time,exec_time) values(" + d[0].Plan.ActualStartupTime + "," + d[0].Plan.ActualTotalTime + "," + d[0].Plan.ActualRows + "," + d[0].Plan.ActualLoops + "," + d[0].PlanningTime + "," + d[0].ExecutionTime + ")",conn);
                //int num=command.ExecuteNonQuery();
                
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured",e);
            }
            return q1;
        }
        
    }
}