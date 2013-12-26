using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace JHSchool.Behavior.MeritAndDemerit
{
    /// <summary>
    /// 代表目前使用者的相關權限資訊。
    /// </summary>
    public static class Permissions
    {
        public static string 獎勵資料項目 { get { return "JHSchool.Student.Detail0030"; } }
        public static bool 獎勵資料項目權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[獎勵資料項目].Executable;
            }
        }

        public static string 懲戒資料項目 { get { return "JHSchool.Student.Detail0040"; } }
        public static bool 懲戒資料項目權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[懲戒資料項目].Executable;
            }
        }

        public static string 獎懲學期統計 { get { return "JHSchool.Student.Detail0035"; } }
        public static bool 獎懲學期統計權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[獎懲學期統計].Executable;
            }
        }

        /// <summary>
        /// 取得是否有「獎勵」權限。
        /// </summary>
        public static bool Merit { get { return User.Acl["JHSchool.Student.Ribbon0060"].Executable; } }

        /// <summary>
        /// 取得是否有「懲戒」權限。
        /// </summary>
        public static bool Demerit { get { return User.Acl["JHSchool.Student.Ribbon0080"].Executable; } }


    }
}
