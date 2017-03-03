using System.Data.SqlClient;
using System.Diagnostics;
using TTools.Properties;

namespace TTools.Domain
{
    public class BeautyGsContext
    {
        //Beauty社のDBから受注情報をロードする
        public void ExecuteSQL(string sqlStr)
        {
            //Setting読み込み
            string dbServer = Settings.Default.BeautyDbIP;
            string dbName = Settings.Default.BeautyDbName;
            string dbUser = Settings.Default.BeautyDbUser;
            string dbPassword = Settings.Default.BeautyDbPass;

            //接続文字列
            string conString =
                "Data Source = " + dbServer + ";" +
                "Initial Catalog = " + dbName + ";" +
                "User ID = " + dbUser + ";" +
                "Password = " + dbPassword;

            using (var connection = new SqlConnection(conString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = sqlStr;
                Debug.WriteLine(sqlStr);

                if (Settings.Default.DebugMode == true) return;
                command.ExecuteNonQuery();
            }
        }
    }
}
