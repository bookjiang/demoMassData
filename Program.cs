using demoMassData.Models;
using Microsoft.Extensions.FileProviders;
using System;
using System.Data;
using System.IO;
using demoMassData.utils;
using MySql.Data.MySqlClient;
using Dapper;

namespace demoMassData
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (var dbContext = new DemoDbContext())
            //{

            //    dbContext.company.Add(new Company
            //    {
            //        name = "星城科技",
            //        address = "湖南长沙雨花区"
            //    });

            //    dbContext.SaveChanges();

            //    Console.WriteLine("All Company in database:");
            //    foreach (var company in dbContext.company)
            //    {
            //        Console.WriteLine("{0}-{1}", company.name, company.address);
            //    }
            //    Console.ReadKey();
            //}
            //Console.WriteLine("Hello World!");
            //var provider = new PhysicalFileProvider("E:\\download");
            //var fileInfo = provider.GetFileInfo("test\\*.jpg");
            //var fileInfo = provider.GetDirectoryContents("test");
            //Console.WriteLine(fileInfo.ToString());


            MySqlBulkLoaderHelper mySqlBulkLoaderHelper = new MySqlBulkLoaderHelper();
            //string path = "E:\\download\\test";
            string path = "E:\\download\\bdd100k\\images\\100k\\train";
            //string connectionString = "server=localhost;uid=root;pwd=226713;database=test;charset=utf8;SslMode=None;allowLoadLocalInfile=true;";
            string connectionString = "server=218.244.154.17;uid=root;pwd=Wzq@226713;database=test;charset=utf8;SslMode=None;allowLoadLocalInfile=true;";




            //Console.WriteLine(dataTable.Rows.Count);
            //DataTable dataTable = mySqlBulkLoaderHelper.fileToDataTable(path);
            DateTime beforeDT = System.DateTime.Now;
            //int count=mySqlBulkLoaderHelper.BulkInsert(connectionString, dataTable);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    //conn.Execute("update test1 set type='.png' where type='.jpg';");
                    int count =conn.Execute("delete from test1 where type='.jpg1';");
                    //int count=conn.Execute("update test1 set type='.png' where id in (select id from (select * from test1 order by id asc limit 0,30000) as tt) ;");
                    //conn.Execute("update test1 set type=case type where '.jpg'")
                    //int count = conn.Execute("update test1 set type= case type when '.png' then '.png2' when '.png1' then '.png3' when '.jpg' then '.jpg1' end where id!=0");

                    conn.Close();
                    Console.WriteLine("数据量为：" + count + "/n");


                }
                catch (MySqlException ex)
                {
                    throw ex;
                }
            }



            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforeDT);

            Console.WriteLine("DateTime costed for insert function is: {0}ms/n", ts.TotalMilliseconds);




        }
    }
}
