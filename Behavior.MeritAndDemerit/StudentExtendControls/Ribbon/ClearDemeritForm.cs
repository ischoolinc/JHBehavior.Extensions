using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.DSAUtil;
using System.Xml;
using DevComponents.DotNetBar;
using SmartSchool.Common;
using Framework;
using JHSchool.Behavior.Editor;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using FISCA.LogAgent;

namespace JHSchool.Behavior.MeritAndDemerit
{
    public partial class ClearDemeritForm : FISCA.Presentation.Controls.BaseForm
    {
        private JHDemeritRecord editor;

        public ClearDemeritForm(JHDemeritRecord cdr)
        {
            InitializeComponent();

            this.editor = cdr;

            //設定銷過作業標題
            this.Text = string.Format("銷過作業【 {0} , {1} 】", JHStudent.SelectByID(cdr.RefStudentID).Name, cdr.OccurDate.ToShortDateString());

            //銷過日期預設為今天
            dateTimeInput1.Value = DateTime.Today;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DateTime dt;
            if (!DateTime.TryParse(dateTimeInput1.Text,out dt))
            {
                FISCA.Presentation.Controls.MsgBox.Show("請輸入正確時間格式!!");
                return;
            }

            this.editor.ClearDate = dateTimeInput1.Value;
            this.editor.ClearReason = this.txtDescription.Text;
            this.editor.Cleared = "是";
            try
            {
                JHDemerit.Update(this.editor);
                this.Close();
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("銷過失敗： \n" + ex.Message );
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("學生「" + editor.Student.Name + "」");
            sb.Append("日期「" + editor.OccurDate.ToShortDateString() + "」");
            sb.AppendLine("懲戒資料已被銷過。");
            sb.AppendLine("詳細資料：");
            if (editor.ClearDate.HasValue)
            {
                sb.AppendLine("銷過日期「" + editor.ClearDate.Value.ToShortDateString() + "」");
            }
            else
            {
                sb.AppendLine("銷過日期「」");
            }
            sb.AppendLine("銷過事由「" + editor.ClearReason + "」");
            ApplicationLog.Log("學務系統.懲戒資料", "銷過作業", "student", editor.Student.ID, sb.ToString());
            FISCA.Presentation.Controls.MsgBox.Show("銷過成功");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //離開
            this.Close();
        }
    }
}
