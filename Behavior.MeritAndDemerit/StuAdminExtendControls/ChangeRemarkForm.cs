using FISCA.LogAgent;
using FISCA.Presentation.Controls;
using K12.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JHSchool.Behavior.MeritAndDemerit
{
    public partial class ChangeRemarkForm : BaseForm
    {
        public string ChangeText = "";
        List<DisciplineRecord> _helper;

        public ChangeRemarkForm(List<DisciplineRecord> helper)
        {
            InitializeComponent();


            List<string> remarkList = tool.GerRemarkTitle(helper[0].MeritFlag);
            cbRemark.Items.AddRange(remarkList.ToArray());

            _helper = helper;
            cbRemark.Text = _helper[0].Remark;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult dr = MsgBox.Show("確認儲存備註修改?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);

            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                ChangeText = cbRemark.Text.Trim();
                StringBuilder sb_log = new StringBuilder();

                foreach (DisciplineRecord each in _helper)
                {
                    string ClassName = each.Student.Class != null ? each.Student.Class.Name : "";
                    string SeatNo = each.Student.SeatNo.HasValue ? each.Student.SeatNo.Value.ToString() : "";
                    sb_log.Append(string.Format("班級「{0}」學生「{1}」座號「{2}」", ClassName, each.Student.Name, SeatNo));

                    string name = "";
                    if (each.MeritFlag == "1")
                        name = "獎勵";
                    else if (each.MeritFlag == "0")
                        name = "懲戒";
                    else
                        name = "留查";

                    sb_log.AppendLine(string.Format("{0}日期「{1}」備註「{2}」修改為「{3}」", name, each.OccurDate.ToShortDateString(), each.Remark, ChangeText));

                    each.Remark = ChangeText;
                }

                try
                {
                    Discipline.Update(_helper);
                    sb_log.AppendLine("\n共" + _helper.Count + "筆資料");
                    MsgBox.Show("儲存完成");
                    ApplicationLog.Log("獎懲批次修改", "修改", sb_log.ToString());
                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存發生錯誤:\n" + ex.Message);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
