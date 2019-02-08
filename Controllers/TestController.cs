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
q1.customers_id as id,
    q1.accountcode,
   
    COALESCE(q1.ht,' ') as HTDate,
    COALESCE(q2.name,' ') as name,
    --COALESCE(to_number(regexp_replace(q2.age, '[^0-9]', '')),0) AS AGE,
    COALESCE(date_part('year',age(q2.dob)) ,0) AS AGE,
    COALESCE(q2.genurl,' ') as contact,
    COALESCE(initcap(q2.clcity),' ') as City,
    'Not Available' as district,
    COALESCE(q2.sourcecategory,' ') as sourcecategory,
    COALESCE(q3.noofgrafts,0) as grafts,
    COALESCE(q3.totalrate,0) as rate,
    COALESCE(ROUND(q3.totalrate/NULLIF(q3.noofgrafts,0),0),0) as RPG,
    COALESCE(q4.Nurse,' ') as nurses,
    COALESCE(q5.Doctor,' ') as doctors,  
    COALESCE(q1.doctorsinstructions,' ') as doctor_instr,
    COALESCE(q1.patientinstructions,' ') as patient_instr
FROM
(
    SELECT
    LS.customers_id,
        LS.accountcode,regexp_replace(
        string_agg(to_char(dateofsurgery,'dd/mm/yyyy'),',') , '([^,]+)(,\1)+', '\1') as ht,
        string_agg(LS.doctorsinstructions,',') doctorsinstructions,
        string_agg(LS.patientinstructions,',') patientinstructions
    FROM
        leadsurgerydetails LS
    WHERE
        LS.dateofsurgery BETWEEN '01/01/2018' and '12/01/2018'
        AND
        LS.prehead=50
    GROUP BY
        LS.customers_id,LS.accountcode
)q1
LEFT JOIN
(
    SELECT
        CV.id,CV.name,CV.age,CV.genurl,CV.sourcecategory,CV.clcity,CV.dob
    FROM
        customers CV
    WHERE
       CV.prehead=50
)q2
ON q1.customers_id = q2.id
LEFT JOIN
(
    SELECT
        LR.customers_id,LR.noofgrafts,LR.totalrate
    FROM
        leadratedetails LR
)q3
ON q1.customers_id = q3.customers_id
LEFT JOIN
(
    SELECT
        LS.customers_id,regexp_replace(string_agg(CV.name,','), '([^,]+)(,\1)+', '\1') nurse
    FROM
        nurses CV,leadsurgerystaffdetails LS
    WHERE
        LS.nurses_id = CV.id
    GROUP BY  
        LS.customers_id
)q4
ON q1.customers_id = q4.customers_id
LEFT JOIN
(
    SELECT
        LS.customers_id,regexp_replace(string_agg(CV.name,',') , '([^,]+)(,\1)+', '\1') doctor
    FROM
        doctors CV,leadsurgerystaffdetails LS
    WHERE
        LS.doctors_id = CV.id
    GROUP BY  
        LS.customers_id
)q5
ON q1.customers_id = q5.customers_id
;"};
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