using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace JHSchool.Behavior.MeritAndDemerit
{
    public class tool
    {
        public static FISCA.UDT.AccessHelper _A = new FISCA.UDT.AccessHelper();

        public static FISCA.Data.QueryHelper _Q = new FISCA.Data.QueryHelper();

        /// <summary>
        /// 取得常用備註欄位資料
        /// 0是懲戒 1 是獎勵
        /// </summary>
        public static List<string> GerRemarkTitle(string type)
        {
            List<string> remarkList = new List<string>();
            remarkList.Add("");
            DataTable dt = _Q.Select(string.Format(@"select DISTINCT(remark) from discipline 
where merit_flag={0} and remark is not null order by remark", type));

            foreach (DataRow row in dt.Rows)
            {
                string remark = "" + row["remark"];
                remarkList.Add(remark);
            }
            return remarkList;
        }
    }
}
