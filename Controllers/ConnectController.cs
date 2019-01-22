using System;
using Npgsql;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Newtonsoft.Json;

namespace PgSql
{

    public class TablesDict : Dictionary<string, MyTable>
    {

    }

    public class MyColumn
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public string ColumnKey { get; set; }
        public string ColumnTable { get; set; }
    }

    //public class MyColumnCollection: List<MyColumn>
    //{

    //}

    public class MyTable //: List<MyColumn>
    {
        public string Name { get; set; }
        public string Schema { get; set; }
        public List<string> Index = new List<string>();
        public Sequence Sequence = new Sequence();
        public List<MyColumn> Columns = new List<MyColumn>();
    }


    public class Sequence
    {

    }

    public class row
    {
        public int count { get; set; }
        public List<string> column = new List<string>();
    }


    public class ConnectController : Controller
    {
        List<string> dataItems = new List<string>();
        List<MyTable> tableName = new List<MyTable>();
        TablesDict Table = new TablesDict();
        List<object> Row = new List<object>();
        NpgsqlConnection connection;
        NpgsqlDataReader dataReader;
        NpgsqlDataReader dataReader1;
        string connstring = "Server=35.200.147.143; Port=5432; User Id=interns_november2018_admin; Password=ruwPcqyM; Database=interns_november2018; Pooling=true;";
        
        string key = "", table = "";
        int count = 0;


        public IActionResult Connect()
        {
            try
            {
                connection = new NpgsqlConnection(connstring);
                connection.Open();
                string sql = @"
                    SELECT
                        tablename, schemaname, indexname
                    FROM
                        pg_indexes
                    WHERE
                        schemaname != 'pg_catalog' AND 
                        schemaname != 'information_schema';

                    SELECT 
                        table_name, column_name, data_type 
                    FROM 
                        information_schema.columns 
                    WHERE 
                        table_schema = 'public' AND
                        table_name = any(SELECT
                        tablename
                    FROM
                        pg_indexes
                    WHERE
                        schemaname != 'pg_catalog' AND 
                        schemaname != 'information_schema');

                    SELECT 
	                c.conname AS constraint_name,
                    c.contype AS constraint_type,
                    tbl.relname AS tabless,
                    ARRAY_AGG(col.attname
                    ORDER BY

                        u.attposition)
                    AS columns,
                        pg_get_constraintdef(c.oid) AS definition
                    FROM pg_constraint c
                    JOIN LATERAL UNNEST(c.conkey) WITH
                    ORDINALITY AS u(attnum, attposition) ON TRUE
                    JOIN pg_class tbl ON tbl.oid = c.conrelid
                    JOIN pg_namespace sch ON sch.oid = tbl.relnamespace
                    JOIN pg_attribute col ON(col.attrelid = tbl.oid AND col.attnum = u.attnum)
                    GROUP BY constraint_name, constraint_type, tabless, definition
                    ORDER BY tabless;
                ";

                
                dataReader = ExecuteQuery(sql);
                dataReader1 = dataReader;
                //dataReader1 = dataReader;
                //dataReader1.Read();
                while (dataReader.Read())
                {
                    MyTable tab = new MyTable()
                    {
                        Name = dataReader[0].ToString(),
                        Schema = dataReader[1].ToString()
                    };
                    //dataItems.Add(dataReader[2].ToString());
                    // tab.Index = dataItems;
                    if (Table.ContainsKey(dataReader[0].ToString()))
                    {
                        // dataItems.Clear();
                        //tab.Index.Add(dataReader[2].ToString());
                        Table[dataReader1[0].ToString()].Index.Add(dataReader1[2].ToString());
                        continue;
                    }
                    else
                    {
                        Table.Add(dataReader[0].ToString(), tab);
                        //dataItems.Clear();
                    }
                    Table[dataReader1[0].ToString()].Index.Add(dataReader1[2].ToString());
                }
                dataReader.NextResult();
                while (dataReader.Read())
                {
                    //Table[dataReader[0].ToString()].Columns.Add(col);
                    MyColumn col = new MyColumn()
                    {
                        ColumnName = dataReader[1].ToString(),
                        ColumnType = dataReader[2].ToString()
                    };
                    Table[dataReader[0].ToString()].Columns.Add(col);

                }
                dataReader.NextResult();
                while (dataReader.Read())
                {
                    ArrayList column = new ArrayList
                    {
                        dataReader[3]
                    };
                    var t = dataReader[3];
                    var col = column[0];
                    var x = (col as string[])[0];
                    string constName = dataReader[0].ToString();
                    string[] st = constName.Split("_");
                    string _name = st[st.Length - 1];
                    if (_name.Equals("pkey"))
                    {
                        foreach(MyColumn obj in Table[dataReader[2].ToString()].Columns)
                        {
                            if (obj.ColumnName.Equals(x))
                            {
                                obj.ColumnKey = "Primary key";
                            }
                        }
                    }
                    if (_name.Equals("fkey"))
                    {
                        foreach (MyColumn obj in Table[dataReader[2].ToString()].Columns)
                        {
                            if (obj.ColumnName.Equals(x))
                            {
                                obj.ColumnKey = "Foreign key";
                                string definition = dataReader[4].ToString();
                                string[] df = definition.Split("REFERENCES ");
                                obj.ColumnTable = df[df.Length - 1];
                            }
                        }
                    }
                    if (_name.Equals("uniquekey"))
                    {
                        foreach (MyColumn obj in Table[dataReader[2].ToString()].Columns)
                        {
                            if (obj.ColumnName.Equals(x))
                            {
                                obj.ColumnKey = "Unique key";
                            }
                        }
                    }
                }

                //foreach (Table dataItem in dataItems)
                //{
                //    //List<string> dataItem = new List<string>();
                //    string sql1 = "select * from pg_indexes where tablename = '" + dataItem.Name + "'";
                //    dataReader = ExecuteQuery(sql1);

                //    while (dataReader.Read())
                //    {
                //        dataItem.Index.Add(dataReader[2].ToString());
                //    }
                //    dataReader.Close();
                //    string sql2 = "SELECT column_name, data_type FROM information_schema.columns WHERE table_name = '" + dataItem.Name + "' and table_schema = 'public'";
                //    dataReader1 = ExecuteQuery(sql2);
                //    while (dataReader1.Read())
                //    {
                //        string sql3 = "SELECT tc.constraint_name,kcu.column_name,ccu.table_name AS foreign_table_name,ccu.column_name AS foreign_column_name FROM information_schema.table_constraints AS tc JOIN information_schema.key_column_usage AS kcu ON tc.constraint_name = kcu.constraint_name AND tc.table_schema = kcu.table_schema JOIN information_schema.constraint_column_usage AS ccu ON ccu.constraint_name = tc.constraint_name AND ccu.table_schema = tc.table_schema WHERE tc.table_name = '" + dataItem.Name + "'";
                //        dataReader = ExecuteQuery(sql3);
                //        while (dataReader.Read())
                //        {
                //            //dataItem.Add(dataReader1[0].ToString() + "      " + dataReader1[2].ToString());
                //            string constName = dataReader[0].ToString();
                //            string[] st = constName.Split("_")
                //            string _name = st[st.Length - 1];
                //            if (_name.Equals("pkey"))
                //            {
                //                key = "Primary Key";
                //            }
                //            if (_name.Equals("fkey"))
                //            {
                //                key = "Foerign Key";
                //                table = dataReader[0].ToString();
                //            }
                //            dataItem.Columns.Add(new Column() {
                //                Name = dataReader1[0].ToString(),
                //                CType = dataReader1[1].ToString(),
                //                CKey = key,
                //                CTable = table
                //            });
                //        }
                //        dataReader.Close();
                //    }
                //    dataReader1.Close();


                //string sql4 = "select relname from pg_class where relkind = 'S'";
                //dataReader1 = Func(sql4);
                //while (dataReader1.Read())
                //{
                //    dataItem.Add(dataReader1[0].ToString());
                //}
                //dataReader1.Close();
                //foreach ( string m in dataItem)
                //string sql5 = "select tab.relname as tabname, attr.attname as column from pg_class as seq join pg_depend as dep on (seq.relfilenode = dep.objid) join pg_class as tab on (dep.refobjid = tab.relfilenode) join pg_attribute as attr on (attr.attnum = dep.refobjsubid and attr.attrelid = dep.refobjid) where seq.relname = '"+i+"'; ";
                //dataReader1 = Func(sql5);
                //while (dataReader1.Read())
                //{
                //    dataItem.Add(dataReader1[0].ToString() + "      " + dataReader1[2].ToString());
                //}
                //d.Add(i, dataItem);
                // dataReader1.Close();
                //}
                //connection.Close();
                ViewBag.Tables = Table;
                ViewBag.TablesDict_S = JsonConvert.SerializeObject(Table);
                // ViewData["obj"] = tab;
                //  ViewBag.tableitem = dataindex;
            }
            catch (Exception msg)
            {
                Console.Write(msg.Message);
            }
            connection.Close();
            return View();
        }

        public NpgsqlDataReader ExecuteQuery(string sql)
        { 
            NpgsqlCommand command = new NpgsqlCommand(sql, connection);            
            return command.ExecuteReader();
        }

        public Boolean SqlQuery(string sql)
        {
            connection = new NpgsqlConnection(connstring);
            connection.Open();
            var x = JsonConvert.DeserializeObject(sql).ToString();
            dataReader = ExecuteQuery(x);
            int count = dataReader.FieldCount;
            row r = new row() { count = this.count };
            while (dataReader.Read())
            {
                for (int i=0;i<count;i++)
                {
                    r.column.Add(dataReader[i].ToString());
                }
                Row.Add(r);
            }
            connection.Close();
            ViewBag.Row = Row;
            return true;
        }
    }


}
