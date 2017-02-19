using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTools.Properties;
using TTools.Models;

namespace TTools.Domain
{
    public class TpicsDbContext
    {
        public DispatchObservableCollection<VendorItem> Load()
        {
            //Setting読み込み
            string dbServer = Settings.Default.TpicsDbIP;
            string dbName = Settings.Default.TpicsDbName;
            string dbUser = Settings.Default.TpicsDbUser;
            string dbPassword = Settings.Default.TpicsDbPass;

            //接続文字列
            string conString =
                "Data Source = " + dbServer + ";" +
                "Initial Catalog = " + dbName + ";" +
                "User ID = " + dbUser + ";" +
                "Password = " + dbPassword;

            //SQL文字列
            string sqlString = "SELECT * FROM dbo.XSECT";

            //DataTableへ読み込み
            DataTable dataTable;
            using (SqlConnection con = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand(sqlString, con);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);

                dataTable = new DataTable();

                sqlDataAdapter.Fill(dataTable);
            }


            //ObservableCollectionにマッピングする
            DispatchObservableCollection<VendorItem> items = new DispatchObservableCollection<VendorItem>();
            foreach (DataRow row in dataTable.Rows)
            {
                items.Add(new VendorItem
                {
                    Id = row["BUMO"] as string,
                    Name = row["BNAME"] as string,
                    Tel = row["TEL"] as string,
                    Fax = row["FAX"] as string,
                    Zip = row["ZIP"] as string,
                    Adr = row["ADR1"] as string,
                    Mail = row["MAIL"] as string,
                });
            }

            return items;
        }
    }
}
