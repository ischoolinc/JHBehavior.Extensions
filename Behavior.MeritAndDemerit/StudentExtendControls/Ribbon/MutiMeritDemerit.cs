using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.DSAUtil;
using System.Xml;
using Framework.Feature;
using FISCA.LogAgent;

namespace JHSchool.Behavior.MeritAndDemerit
{
    public partial class MutiMeritDemerit : BaseForm
    {
        string _DemeritOrMerit;

        Dictionary<string, string> ReasonDic = new Dictionary<string, string>();

        StringBuilder sb = new StringBuilder();

        /// <summary>
        /// 傳入獎勵或懲戒字串,以決定模式
        /// </summary>
        /// <param name="DemeritOrMerit"></param>
        public MutiMeritDemerit(string DemeritOrMerit)
        {
            InitializeComponent();

            _DemeritOrMerit = DemeritOrMerit;
        }

        //Load
        private void MutiMeritDemerit_Load(object sender, EventArgs e)
        {
            List<string> cols = new List<string>() { "大功", "小功", "嘉獎", "大過", "小過", "警告" };
            Campus.Windows.DataGridViewImeDecorator dec = new Campus.Windows.DataGridViewImeDecorator(this.dataGridViewX1, cols);

            //comboBoxEx1.DisplayMember = "Key";
            //comboBoxEx1.ValueMember = "Value";
            integerInput1.Text = School.DefaultSchoolYear;
            integerInput2.Text = School.DefaultSemester;
            dateTimeInput1.Text = DateTime.Now.ToShortDateString();
            dateTimeInput2.Text = DateTime.Now.ToShortDateString();
            KeyValuePair<string, string> fkvp = new KeyValuePair<string, string>("", "");
            comboBoxEx1.Items.Add(fkvp);

            if (_DemeritOrMerit == "獎勵")
            {
                List<string> remarkList = tool.GerRemarkTitle("1");
                cbRemark.Items.AddRange(remarkList.ToArray());

                #region 獎勵
                DSResponse dsrsp = Config.GetDisciplineReasonList();
                foreach (XmlElement element in dsrsp.GetContent().GetElements("Reason"))
                {
                    if (element.GetAttribute("Type") == "獎勵")
                    {
                        string k = element.GetAttribute("Code") + "-" + element.GetAttribute("Description");
                        string v = element.GetAttribute("Description");
                        KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(k, v);
                        if (!ReasonDic.ContainsKey(element.GetAttribute("Code")))
                        {
                            ReasonDic.Add(element.GetAttribute("Code"), v);
                        }
                        //KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(k, v);
                        comboBoxEx1.Items.Add(kvp);
                    }
                }
                #endregion
            }
            else //獎勵是預設動作
            {
                #region 懲戒
                labelX1.Text = "懲戒日期";
                labelX6.Text = "大過";
                labelX7.Text = "小過";
                labelX8.Text = "警告";
                this.Text = "多人懲戒快速登錄";

                List<string> remarkList = tool.GerRemarkTitle("0");
                cbRemark.Items.AddRange(remarkList.ToArray());

                DSResponse dsrsp = Config.GetDisciplineReasonList();
                foreach (XmlElement element in dsrsp.GetContent().GetElements("Reason"))
                {
                    if (element.GetAttribute("Type") == "懲戒")
                    {
                        string k = element.GetAttribute("Code") + "-" + element.GetAttribute("Description");
                        string v = element.GetAttribute("Description");
                        KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(k, v);
                        if (!ReasonDic.ContainsKey(element.GetAttribute("Code")))
                        {
                            ReasonDic.Add(element.GetAttribute("Code"), v);
                        }
                        //KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(k, v);
                        comboBoxEx1.Items.Add(kvp);
                    }
                }

                //dataGridViewX1.Columns.Clear();
                //SetColumn("班級", 60, true);
                //SetColumn("座號", 60, true);
                //SetColumn("姓名", 100, true);
                //SetColumn("大過", 60, true);
                //SetColumn("小過", 60, true);
                //SetColumn("警告", 60, true);
                //SetColumn("事由", 280, true); 

                Column7.HeaderText = "大過";
                Column8.HeaderText = "小過";
                Column9.HeaderText = "警告";
                #endregion
            }

            comboBoxEx1.DisplayMember = "Key";
            comboBoxEx1.ValueMember = "Value";
            comboBoxEx1.SelectedIndex = 0;

            #region 填入學生
            List<K12.Data.StudentRecord> StudentList = K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);
            StudentList = SortClassIndex.K12Data_StudentRecord(StudentList); //sort

            foreach (K12.Data.StudentRecord student in StudentList)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);
                row.Tag = student;
                row.Cells[0].Value = student.Class != null ? student.Class.Name : "";
                row.Cells[1].Value = student.SeatNo.HasValue ? student.SeatNo.Value.ToString() : "";
                row.Cells[2].Value = student.StudentNumber;
                row.Cells[3].Value = student.Name;
                dataGridViewX1.Rows.Add(row);
            }
            #endregion

            textBoxX3.Focus();
        }

        //儲存
        private void buttonX2_Click(object sender, EventArgs e)
        {
            if (CheckIntError())
            {
                MsgBox.Show("輸入獎懲隻數型態錯誤,請重新修正後再儲存!!");
                return;
            }

            if (CheckReasonError())
            {
                DialogResult dr = MsgBox.Show("部份學生資料未輸入事由,是否繼續儲存作業?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1);

                if (dr == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            if (_DemeritOrMerit == "獎勵")
            {
                List<K12.Data.MeritRecord> MeritList = GetMeritList();
                try
                {
                    K12.Data.Merit.Insert(MeritList);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("新增獎勵失敗\n" + ex.Message);
                    return;
                }
                ApplicationLog.Log("學務系統.獎勵登錄", "獎勵快速登錄", sb.ToString());
                MsgBox.Show("新增獎勵成功!");
            }
            else //懲戒
            {
                List<K12.Data.DemeritRecord> DemeritList = GetDemeritList();
                try
                {
                    K12.Data.Demerit.Insert(DemeritList);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("新增獎勵失敗\n" + ex.Message);
                    return;
                }
                ApplicationLog.Log("學務系統.懲戒登錄", "懲戒快速登錄", sb.ToString());
                MsgBox.Show("新增懲戒成功!");
            }
            this.Close();
        }

        //取得獎勵資料
        private List<K12.Data.MeritRecord> GetMeritList()
        {
            sb.AppendLine("批次獎勵快速登錄作業");
            sb.AppendLine("學年度「" + integerInput1.Value.ToString() + "」");
            sb.AppendLine("學期「" + integerInput2.Value.ToString() + "」");
            sb.AppendLine("獎勵日期：" + dateTimeInput1.Value.ToShortDateString());
            sb.AppendLine("詳細資料：");

            List<K12.Data.MeritRecord> MeritList = new List<K12.Data.MeritRecord>();
            //每一位學生的獎懲資料
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {

                K12.Data.StudentRecord student = (K12.Data.StudentRecord)row.Tag;
                K12.Data.MeritRecord mr = new K12.Data.MeritRecord();

                mr.RefStudentID = student.ID; //學生ID
                mr.SchoolYear = integerInput1.Value; //學年度
                mr.Semester = integerInput2.Value; //學期

                mr.MeritA = int.Parse(string.IsNullOrEmpty("" + row.Cells[4].Value) ? "0" : "" + row.Cells[4].Value); //大功
                mr.MeritB = int.Parse(string.IsNullOrEmpty("" + row.Cells[5].Value) ? "0" : "" + row.Cells[5].Value);  //小功
                mr.MeritC = int.Parse(string.IsNullOrEmpty("" + row.Cells[6].Value) ? "0" : "" + row.Cells[6].Value);  //嘉獎
                if (mr.MeritA + mr.MeritB + mr.MeritC <= 0)
                {
                    continue;
                }
                mr.Remark = "" + row.Cells[7].Value; //備註 2019/12/24新增
                mr.Reason = "" + row.Cells[8].Value;

                mr.OccurDate = dateTimeInput1.Value; //獎勵日期
                mr.RegisterDate = dateTimeInput2.Value; //登錄日期

                MeritList.Add(mr);

                sb.AppendLine("學生「" + student.Name + "」"
                    + "大功「" + row.Cells[4].Value + "」"
                    + "小功「" + row.Cells[5].Value + "」"
                    + "嘉獎「" + row.Cells[6].Value + "」"
                    + "備註「" + row.Cells[7].Value + "」"
                    + "事由「" + row.Cells[8].Value + "」");
            }

            return MeritList;
        }

        //取得懲戒資料
        private List<K12.Data.DemeritRecord> GetDemeritList()
        {
            sb.AppendLine("批次懲戒快速登錄作業");
            sb.AppendLine("學年度「" + integerInput1.Value.ToString() + "」");
            sb.AppendLine("學期「" + integerInput2.Value.ToString() + "」");
            sb.AppendLine("懲戒日期：" + dateTimeInput1.Value.ToShortDateString());
            sb.AppendLine("詳細資料：");

            List<K12.Data.DemeritRecord> DemeritList = new List<K12.Data.DemeritRecord>();
            //每一位學生的獎懲資料
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                K12.Data.StudentRecord student = (K12.Data.StudentRecord)row.Tag;
                K12.Data.DemeritRecord mr = new K12.Data.DemeritRecord();

                mr.RefStudentID = student.ID; //學生ID
                mr.SchoolYear = integerInput1.Value; //學年度
                mr.Semester = integerInput2.Value; //學期

                mr.DemeritA = int.Parse(string.IsNullOrEmpty("" + row.Cells[4].Value) ? "0" : "" + row.Cells[4].Value); //大過
                mr.DemeritB = int.Parse(string.IsNullOrEmpty("" + row.Cells[5].Value) ? "0" : "" + row.Cells[5].Value); //小過
                mr.DemeritC = int.Parse(string.IsNullOrEmpty("" + row.Cells[6].Value) ? "0" : "" + row.Cells[6].Value); //警告
                if (mr.DemeritA + mr.DemeritB + mr.DemeritC <= 0)
                {
                    continue;
                }

                mr.Remark = "" + row.Cells[7].Value; //備註 2019/12/24新增
                mr.Reason = "" + row.Cells[8].Value;

                mr.OccurDate = dateTimeInput1.Value; //懲戒日期
                mr.RegisterDate = dateTimeInput2.Value; //登錄日期

                DemeritList.Add(mr);

                sb.AppendLine("學生「" + student.Name + "」"
                + "大過「" + row.Cells[4].Value + "」"
                + "小過「" + row.Cells[5].Value + "」"
                + "警告「" + row.Cells[6].Value + "」"
                + "備註「" + row.Cells[7].Value + "」"
                + "事由「" + row.Cells[8].Value + "」");
            }

            return DemeritList;
        }

        /// <summary>
        /// 檢查事由是否輸入
        /// </summary>
        /// <returns></returns>
        private bool CheckReasonError()
        {
            bool returnTrue = false;

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (("" + row.Cells[8].Value).Trim() == "")
                {
                    returnTrue = true;
                }
            }
            return returnTrue;
        }

        /// <summary>
        /// 檢查獎/懲支數是否為int型態
        /// </summary>
        /// <returns></returns>
        private bool CheckIntError()
        {
            bool returnTrue = false;

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ColumnIndex > 3 && cell.ColumnIndex < 7)
                    {
                        if (!intParse("" + cell.Value))
                        {
                            returnTrue = true;
                        }
                    }
                }
            }
            return returnTrue;
        }

        //離開
        private void buttonX3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Changed事件

        private void comboBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)comboBoxEx1.SelectedItem;
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                row.Cells[8].Value = kvp.Value;
            }
        }


        private void comboBoxEx1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                comboBoxEx1.Focus();

                string comText = comboBoxEx1.Text;
                comText = comText.Remove(0, comText.IndexOf('-') + 1);

                string reasonValue = GetReason(comText);

                foreach (DataGridViewRow row in dataGridViewX1.Rows)
                {
                    row.Cells[8].Value = reasonValue;
                }
            }
        }

        private void comboBoxEx1_TextChanged(object sender, EventArgs e)
        {
            string comText = comboBoxEx1.Text;
            comText = comText.Remove(0, comText.IndexOf('-') + 1);

            string reasonValue = GetReason(comText);

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                row.Cells[8].Value = reasonValue;
            }
        }

        private string GetReason(string comText)
        {
            string reasonValue = "";
            List<string> list = new List<string>();
            string[] reasonList = comText.Split(',');
            foreach (string each in reasonList)
            {
                string each1 = each.Replace("\r\n", "");
                if (ReasonDic.ContainsKey(each1))
                {
                    list.Add(ReasonDic[each1]);
                }
                else
                {
                    list.Add(each1);
                }
            }

            reasonValue = string.Join(",", list);
            return reasonValue;
        }

        private void textBoxX1_TextChanged(object sender, EventArgs e)
        {
            if (intParse(textBoxX1.Text))
            {
                foreach (DataGridViewRow row in dataGridViewX1.Rows)
                {
                    row.Cells[4].Value = textBoxX1.Text;
                }
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(textBoxX1, "輸入內容非數字!!");
            }
        }

        private void textBoxX2_TextChanged(object sender, EventArgs e)
        {
            if (intParse(textBoxX2.Text))
            {
                foreach (DataGridViewRow row in dataGridViewX1.Rows)
                {
                    row.Cells[5].Value = textBoxX2.Text;
                }
                errorProvider2.Clear();
            }
            else
            {
                errorProvider2.SetError(textBoxX2, "輸入內容非數字!!");
            }

        }

        private void textBoxX3_TextChanged(object sender, EventArgs e)
        {
            if (intParse(textBoxX3.Text))
            {
                foreach (DataGridViewRow row in dataGridViewX1.Rows)
                {
                    row.Cells[6].Value = textBoxX3.Text;
                }
                errorProvider3.Clear();
            }
            else
            {
                errorProvider3.SetError(textBoxX3, "輸入內容非數字!!");
            }
        }

        #endregion

        /// <summary>
        /// 傳入學生,依學生班級座號排序
        /// </summary>
        public int SortStudent(K12.Data.StudentRecord x, K12.Data.StudentRecord y)
        {
            K12.Data.StudentRecord student1 = x;
            K12.Data.StudentRecord student2 = y;

            string ClassName1 = student1.Class != null ? student1.Class.Name : "";
            ClassName1 = ClassName1.PadLeft(5, '0');
            string ClassName2 = student2.Class != null ? student2.Class.Name : "";
            ClassName2 = ClassName2.PadLeft(5, '0');

            string Sean1 = student1.SeatNo.HasValue ? student1.SeatNo.Value.ToString() : "";
            Sean1 = Sean1.PadLeft(3, '0');
            string Sean2 = student2.SeatNo.HasValue ? student2.SeatNo.Value.ToString() : "";
            Sean2 = Sean2.PadLeft(3, '0');

            ClassName1 += Sean1;
            ClassName2 += Sean2;

            return ClassName1.CompareTo(ClassName2);
        }

        private void dataGridViewX1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //不是標題列
            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                //缺曠數量
                if (e.ColumnIndex > 3 && e.ColumnIndex < 7)
                {
                    DataGridViewCell cell = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    if (intParse("" + cell.Value))
                    {
                        cell.ErrorText = "";
                    }
                    else
                    {
                        cell.ErrorText = "內容非數字!!";
                    }
                }

                //事由替換
                if (e.ColumnIndex == 8)
                {
                    DataGridViewCell cell = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    cell.Value = GetReason("" + cell.Value);
                }
            }
        }

        private bool intParse(string CellValue)
        {
            int TextIndex;
            if (int.TryParse(CellValue, out TextIndex) || string.IsNullOrEmpty(CellValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void cbRemark_TextChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                row.Cells[7].Value = cbRemark.Text;
            }
        }
    }
}
