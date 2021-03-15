using Microsoft.Extensions.FileProviders;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace demoMassData.utils
{
    class MySqlBulkLoaderHelper
    {


        public  DataTable fileToDataTable(string filePath)
        {

            //var provider = new PhysicalFileProvider("E:\\download\\test");
            DataTable dataTable = new DataTable("test1");
            DataColumn dataColumn1 = new DataColumn("fileName", typeof(string));
            DataColumn dataColumn2 = new DataColumn("type", typeof(string));
            DataColumn dataColumn3 = new DataColumn("length", typeof(int));
            DataColumn dataColumn4 = new DataColumn("ceateTime", typeof(DateTime));
            dataTable.Columns.Add(dataColumn1);
            dataTable.Columns.Add(dataColumn2);
            dataTable.Columns.Add(dataColumn3);
            dataTable.Columns.Add(dataColumn4);
            DirectoryInfo direcInfo = new DirectoryInfo(filePath);
            if (direcInfo != null && direcInfo.Exists)
            {
                foreach (var item in direcInfo.GetFiles())
                {

                    dataTable.Rows.Add(item.Name, item.Extension, item.Length, System.DateTime.Now);
                    //Console.WriteLine(item.FullName+item.Name+item.Extension+item.Length);
                   

                }
            }
            Console.WriteLine("数据量为：" + dataTable.Rows.Count + "/n");
            return dataTable;

        }






        /// <summary>
        ///将DataTable转换为标准的CSV
        /// </summary>
        /// <param name="table">数据表</param>
        /// <returns>返回标准的CSV</returns>
        private  string DataTableToCsv(DataTable table)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else if (colum.DataType == typeof(DateTime) || colum.DataType == typeof(DateTime?))
                    {
                        sb.Append(string.IsNullOrEmpty(row[colum].ToString()) ? "1970-01-01 00:00:00" : row[colum].ToString());
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }


            return sb.ToString();
        }

        /// <summary>
        ///大批量数据插入,返回成功插入行数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="table">数据表</param>
        /// <returns>返回成功插入行数</returns>
        public  int BulkInsert(string connectionString, DataTable table)
        {
            if (string.IsNullOrEmpty(table.TableName)) throw new Exception("请给DataTable的TableName属性附上表名称");
            if (table.Rows.Count == 0) return 0;
            int insertCount = 0;
            //string tmpPath = Path.GetTempFileName();
            string tmpPath = "E:\\test.tmp";
            string csv = DataTableToCsv(table);
            File.WriteAllText(tmpPath, csv);
            // MySqlTransaction tran = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    //tran = conn.BeginTransaction();
                    MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                    {
                        CharacterSet = "utf8mb4",
                        Local=true,
                        FieldTerminator = ",",
                        FieldQuotationCharacter = '"',
                        EscapeCharacter = '"',
                        LineTerminator = "\r\n",
                        FileName = tmpPath,
                        NumberOfLinesToSkip = 0,
                        TableName = table.TableName,
                    };
                    bulk.Columns.AddRange(table.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToArray());
                    //var columns = table.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToList();
                    //bulk.Columns.AddRange(table.);
                    insertCount = bulk.Load();
                    // tran.Commit();
                    Console.WriteLine("test");
                    conn.Close();

                }
                catch (MySqlException ex)
                {
                    // if (tran != null) tran.Rollback();
                    conn.Close();
                    File.Delete(tmpPath);

                    throw ex;
                }
            }
            File.Delete(tmpPath);
            return insertCount;
        }

    }
}
