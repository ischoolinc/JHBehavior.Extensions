using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using SmartSchool.StudentRelated.RibbonBars.AttendanceEditor;
using System.Xml;
using DevComponents.DotNetBar;
using SmartSchool.Common;
using Framework;
using JHSchool.Legacy;
using Framework.Feature;
using JHSchool.Behavior.Legacy;
using JHSchool.Behavior.Editor;
using FISCA.DSAUtil;
using JHSchool.Data;
using FISCA.LogAgent;
//using SmartSchool.ApplicationLog;

namespace JHSchool.Behavior.MeritAndDemerit
{
    /// <summary>
    /// 新增或修改懲戒資料的畫面。
    /// 修改時，因為只能修改一個學生的某一筆獎懲資料，所以只要傳入一個 DemeritRecordEditor 物件即可
    /// 新增時，可以同時對多位學生增加相同的懲戒紀錄，所以要傳入多位學生的資料，但不傳入DemeritRecordEditor 物件，此物件會在儲存時由每位學生記錄取得。
    /// </summary>
    public partial class DemeritEditForm : FISCA.Presentation.Controls.BaseForm
    {
        private List<JHStudentRecord> _students;
        private ErrorProvider _errorProvider;
        private JHDemeritRecord _demeritRecordEditor;
        private Dictionary<string, string> dicReason = new Dictionary<string, string>();

        private Dictionary<string, string> ResonDic = new Dictionary<string, string>();

        //Log
        private Dictionary<string, string> DicBeforeData = new Dictionary<string, string>();

        /// <summary>
        /// Constructor，新增時使用。
        /// </summary>
        /// <param name="students"></param>
        public DemeritEditForm(List<JHStudentRecord> students)
        {
            #region 新增
            this._students = students;
            Initialize();
            dateTimeInput1.Value = DateTime.Today;
            dateTimeInput2.Value = DateTime.Today;
            if (this._students.Count > 1)
            {
                Text = string.Format("懲戒管理 【 新增：{0} ... 等共 {1} 位 】", this._students[0].Name, this._students.Count.ToString()); ;
            }
            else if (this._students.Count == 1)
            {
                Text = string.Format("懲戒管理 【 新增：{0} 】", this._students[0].Name); ;
            } 
            #endregion
        }

        public DemeritEditForm(List<JHStudentRecord> students, string SchoolYear, string Semester)
        {
            #region 新增
            this._students = students;

            #region 建構子
            InitializeComponent();

            _errorProvider = new ErrorProvider();

            //學年度
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) - 4);
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) - 3);
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) - 2);
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) - 1);
            int SchoolYearSelectIndex = cboSchoolYear.Items.Add(SchoolYear);
            cboSchoolYear.SelectedIndex = SchoolYearSelectIndex;
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) + 1);
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) + 2);
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) + 3);
            //學期
            cboSemester.Items.Add(1);
            cboSemester.Items.Add(2);
            cboSemester.SelectedIndex = Semester == "1" ? 0 : 1;

            #endregion

            dateTimeInput1.Value = DateTime.Today;
            dateTimeInput2.Value = DateTime.Today;
            if (this._students.Count > 1)
            {
                Text = string.Format("懲戒管理 【 新增：{0} ... 等共 {1} 位 】", this._students[0].Name, this._students.Count.ToString()); ;
            }
            else if (this._students.Count == 1)
            {
                Text = string.Format("懲戒管理 【 新增：{0} 】", this._students[0].Name); ;
            }
            #endregion
        }

        //多載建構子
        public DemeritEditForm(List<JHStudentRecord> students, string SchoolYear, string Semester,bool LockMode)
        {
            #region 新增
            this._students = students;

            #region 建構子
            InitializeComponent();

            _errorProvider = new ErrorProvider();

            //學年度
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) - 4);
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) - 3);
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) - 2);
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) - 1);
            int SchoolYearSelectIndex = cboSchoolYear.Items.Add(SchoolYear);
            cboSchoolYear.SelectedIndex = SchoolYearSelectIndex;
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) + 1);
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) + 2);
            cboSchoolYear.Items.Add(int.Parse(SchoolYear) + 3);
            //學期
            cboSemester.Items.Add(1);
            cboSemester.Items.Add(2);
            cboSemester.SelectedIndex = Semester == "1" ? 0 : 1;

            #endregion

            //LockMode狀態鎖住學年度學期
            if (LockMode)
            {
                cboSchoolYear.Enabled = false;
                cboSemester.Enabled = false;
            }

            dateTimeInput1.Value = DateTime.Today;
            dateTimeInput2.Value = DateTime.Today;
            if (this._students.Count > 1)
            {
                Text = string.Format("懲戒管理 【 新增：{0} ... 等共 {1} 位 】", this._students[0].Name, this._students.Count.ToString()); ;
            }
            else if (this._students.Count == 1)
            {
                Text = string.Format("懲戒管理 【 新增：{0} 】", this._students[0].Name); ;
            }
            #endregion
        }

        /// <summary>
        /// Constructor，修改時使用
        /// </summary>
        /// <param name="_demeritRecordEditor"></param>
        public DemeritEditForm(JHDemeritRecord demeritRecordEditor,Framework.Security.FeatureAce permission)
        {
            #region 修改
            this._demeritRecordEditor = demeritRecordEditor;

            #region Log用

            if (demeritRecordEditor.DemeritA.HasValue)
            {
                DicBeforeData.Add("大過", demeritRecordEditor.DemeritA.Value.ToString());
            }
            if (demeritRecordEditor.DemeritB.HasValue)
            {
                DicBeforeData.Add("小過", demeritRecordEditor.DemeritB.Value.ToString());
            }
            if (demeritRecordEditor.DemeritC.HasValue)
            {
                DicBeforeData.Add("警告", demeritRecordEditor.DemeritC.Value.ToString());
            }
            DicBeforeData.Add("事由", demeritRecordEditor.Reason);
            #endregion

            this._students = new List<JHStudentRecord>();
            this._students.Add(JHStudent.SelectByID(demeritRecordEditor.RefStudentID));

            Initialize();

            if (permission.Editable)
                Text = string.Format("懲戒管理 【 修改：{0}，{1} 】", JHStudent.SelectByID(demeritRecordEditor.RefStudentID).Name, demeritRecordEditor.OccurDate.ToShortDateString());
            else
                Text = string.Format("懲戒管理 【 檢視：{0}，{1} 】", JHStudent.SelectByID(demeritRecordEditor.RefStudentID).Name, demeritRecordEditor.OccurDate.ToShortDateString());

            btnSave.Visible = permission.Editable;
            cboReasonRef.Enabled = permission.Editable;
            cboSchoolYear.Enabled = permission.Editable;
            cboSemester.Enabled = permission.Editable;
            dateTimeInput1.Enabled = permission.Editable;
            dateTimeInput2.Enabled = permission.Editable;

            foreach (Control each in Controls)
            {
                if (each is DevComponents.DotNetBar.Controls.TextBoxX)
                    (each as DevComponents.DotNetBar.Controls.TextBoxX).ReadOnly = !permission.Editable;
            } 
            #endregion
        }

        private void Initialize()
        {
            #region 建構子
            InitializeComponent();

            _errorProvider = new ErrorProvider();

            //學年度
            cboSchoolYear.Items.Add(int.Parse(K12.Data.School.DefaultSchoolYear) - 3);
            cboSchoolYear.Items.Add(int.Parse(K12.Data.School.DefaultSchoolYear) - 2);
            cboSchoolYear.Items.Add(int.Parse(K12.Data.School.DefaultSchoolYear) - 1);
            int SchoolYearSelectIndex = cboSchoolYear.Items.Add(int.Parse(K12.Data.School.DefaultSchoolYear));
            cboSchoolYear.Items.Add(int.Parse(K12.Data.School.DefaultSchoolYear) + 1);
            cboSchoolYear.Items.Add(int.Parse(K12.Data.School.DefaultSchoolYear) + 2);
            cboSchoolYear.SelectedIndex = SchoolYearSelectIndex;
            //學期
            cboSemester.Items.Add(1);
            cboSemester.Items.Add(2);
            cboSemester.SelectedIndex = K12.Data.School.DefaultSemester == "1" ? 0 : 1;
            #endregion
        }

        private void DemeritEditor_Load(object sender, EventArgs e)
        {
            #region Load
            //取得懲戒的代碼和原因清單，並放到 事由代碼 的下拉式方塊中。
            DSResponse dsrsp = Config.GetDisciplineReasonList();
            cboReasonRef.SelectedItem = null;
            cboReasonRef.Items.Clear();
            dicReason.Clear();
            DSXmlHelper helper = dsrsp.GetContent();
            KeyValuePair<string, string> fkvp = new KeyValuePair<string, string>("", "");
            cboReasonRef.Items.Add(fkvp);

            foreach (XmlElement element in helper.GetElements("Reason"))
            {
                if (element.GetAttribute("Type") == "懲戒" || element.GetAttribute("Type") == "懲誡")
                {
                    dicReason.Add(element.GetAttribute("Code"), element.GetAttribute("Description"));
                    string k = element.GetAttribute("Code") + "-" + element.GetAttribute("Description");
                    string v = element.GetAttribute("Description");
                    KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(k, v);
                    cboReasonRef.Items.Add(kvp);

                    if (!ResonDic.ContainsKey("" + element.GetAttribute("Code")))
                    {
                        ResonDic.Add("" + element.GetAttribute("Code"), "" + element.GetAttribute("Description"));
                    }
                }
            }
            cboReasonRef.DisplayMember = "Key";
            cboReasonRef.ValueMember = "Value";
            cboReasonRef.SelectedIndex = 0;

            //如果是修改模式，則把資料填到畫面上。
            if (this._demeritRecordEditor != null)
            {
                txtReason.Text = _demeritRecordEditor.Reason;
                txt1.Text = _demeritRecordEditor.DemeritA.ToString();
                txt2.Text = _demeritRecordEditor.DemeritB.ToString();
                txt3.Text = _demeritRecordEditor.DemeritC.ToString();
                dateTimeInput1.Value = _demeritRecordEditor.OccurDate;
                cboSchoolYear.Text = _demeritRecordEditor.SchoolYear.ToString();
                cboSemester.Text = _demeritRecordEditor.Semester.ToString();

                if (_demeritRecordEditor.RegisterDate.HasValue)
                {
                    int SetIndex = _demeritRecordEditor.RegisterDate.ToString().IndexOf(' ');
                    string SetRegisterDate = _demeritRecordEditor.RegisterDate.ToString().Remove(SetIndex);
                    dateTimeInput2.Value = _demeritRecordEditor.RegisterDate.Value;
                }
                else
                {
                    dateTimeInput2.Text = "";
                }
            }
            #endregion
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region Save
            bool valid = true;
            foreach (Control control in this.Controls)
                if (!string.IsNullOrEmpty(_errorProvider.GetError(control)))
                    valid = false;

            if (!valid)
            {
                FISCA.Presentation.Controls.MsgBox.Show("資料驗證錯誤，請先修正後再行儲存");
                return;
            }

            //檢查使用者是否忘記輸入功過次數。

            int sum = int.Parse(GetTextValue(txt1.Text)) + int.Parse(GetTextValue(txt2.Text)) + int.Parse(GetTextValue(txt3.Text));
            if (sum <= 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("請別忘了輸入功過次數。");
                return;
            }

            if (txtReason.Text.Trim() == "")
            {
                DialogResult dr = FISCA.Presentation.Controls.MsgBox.Show("事由未輸入，是否繼續進行儲存操作？", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
                if (dr == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            if (this._demeritRecordEditor == null) //新增懲戒
            {
                List<JHDemeritRecord> LogDemeritList = new List<JHDemeritRecord>();

                try
                {
                    LogDemeritList = Insert();
                    JHDemerit.Insert(LogDemeritList);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("新增懲戒記錄時發生錯誤: \n" + ex.Message);
                }

                if (_students.Count == 1)
                {
                    #region 單筆新增Log
                    StringBuilder sb = new StringBuilder();
                    sb.Append("學生「" + LogDemeritList[0].Student.Name + "」");
                    sb.Append("日期「" + LogDemeritList[0].OccurDate.ToShortDateString() + "」");
                    sb.AppendLine("新增一筆懲戒資料。");
                    sb.AppendLine("詳細資料：");
                    if (LogDemeritList[0].DemeritA.HasValue)
                    {
                        sb.Append("大過「" + LogDemeritList[0].DemeritA.Value.ToString() + "」");
                    }
                    if (LogDemeritList[0].DemeritB.HasValue)
                    {
                        sb.Append("小過「" + LogDemeritList[0].DemeritB.Value.ToString() + "」");
                    }
                    if (LogDemeritList[0].DemeritC.HasValue)
                    {
                        sb.Append("警告「" + LogDemeritList[0].DemeritC.Value.ToString() + "」");
                    }
                    sb.Append("懲戒事由「" + LogDemeritList[0].Reason + "」");

                    ApplicationLog.Log("學務系統.懲戒資料", "新增學生懲戒資料", "student", _students[0].ID, sb.ToString());
                    #endregion
                    FISCA.Presentation.Controls.MsgBox.Show("新增懲戒資料成功!");
                }
                else if (_students.Count > 1)
                {
                    #region 批次新增Log
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("批次新增懲戒資料");
                    sb.Append("日期「" + LogDemeritList[0].OccurDate.ToShortDateString() + "」");
                    sb.AppendLine("共「" + LogDemeritList.Count + "」名學生，");
                    sb.AppendLine("詳細資料：");
                    if (LogDemeritList[0].DemeritA.HasValue)
                    {
                        sb.Append("大過「" + LogDemeritList[0].DemeritB.Value.ToString() + "」");
                    }
                    if (LogDemeritList[0].DemeritB.HasValue)
                    {
                        sb.Append("小過「" + LogDemeritList[0].DemeritB.Value.ToString() + "」");
                    }
                    if (LogDemeritList[0].DemeritC.HasValue)
                    {
                        sb.Append("警告「" + LogDemeritList[0].DemeritC.Value.ToString() + "」");
                    }
                    sb.AppendLine("懲戒事由「" + LogDemeritList[0].Reason + "」");

                    sb.AppendLine("學生詳細資料：");
                    foreach (JHDemeritRecord each in LogDemeritList)
                    {
                        sb.Append("學生「" + each.Student.Name + "」");

                        if (each.Student.Class != null)
                        {
                            sb.Append("班級「" + each.Student.Class.Name + "」");
                        }
                        else
                        {
                            sb.Append("班級「」");
                        }

                        if (each.Student.SeatNo.HasValue)
                        {
                            sb.AppendLine("座號「" + each.Student.SeatNo.Value.ToString() + "」");
                        }
                        else
                        {
                            sb.AppendLine("座號「」");
                        }
                    }

                    ApplicationLog.Log("學務系統.懲戒資料", "批次新增學生懲戒資料", sb.ToString());
                    #endregion
                    FISCA.Presentation.Controls.MsgBox.Show("批次新增懲戒資料成功!");
                }
            }
            else
            {
                try
                {
                    Modify();
                    JHDemerit.Update(this._demeritRecordEditor);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("修改懲戒記錄時發生錯誤: \n" + ex.Message);
                }

                #region 修改Log
                StringBuilder sb = new StringBuilder();
                sb.Append("學生「" + this._demeritRecordEditor.Student.Name + "」");
                sb.AppendLine("日期「" + this._demeritRecordEditor.OccurDate.ToShortDateString() + "」懲戒資料已修改。");
                sb.AppendLine("詳細資料：");
                sb.AppendLine("大過「" + DicBeforeData["大過"] + "」變更為「" + this._demeritRecordEditor.DemeritA.Value + "」");
                sb.AppendLine("小過「" + DicBeforeData["小過"] + "」變更為「" + this._demeritRecordEditor.DemeritB.Value + "」");
                sb.AppendLine("警告「" + DicBeforeData["警告"] + "」變更為「" + this._demeritRecordEditor.DemeritC.Value + "」");
                sb.AppendLine("懲戒事由「" + DicBeforeData["事由"] + "」變更為「" + this._demeritRecordEditor.Reason + "」");
                ApplicationLog.Log("學務系統.懲戒資料", "修改學生懲戒資料", "student", this._demeritRecordEditor.Student.ID, sb.ToString());
                #endregion
                FISCA.Presentation.Controls.MsgBox.Show("修改懲戒資料成功!");
            }

            this.Close();
            #endregion
        }

        private void cboSchoolYear_Validated(object sender, EventArgs e)
        {
            _errorProvider.SetError(cboSchoolYear, null);
            int i;
            if (!int.TryParse(cboSchoolYear.Text, out i))
                _errorProvider.SetError(cboSchoolYear, "學年度必須為整數數字");
        }

        private void cboSemester_Validated(object sender, EventArgs e)
        {
            _errorProvider.SetError(cboSemester, null);
            if (cboSemester.Text != "1" && cboSemester.Text != "2")
                _errorProvider.SetError(cboSemester, "學期必須填入1或2");
        }

        private void txt1_Validated(object sender, EventArgs e)
        {
            this.Text_Validate(this.txt1);
        }

        private void txt2_Validated(object sender, EventArgs e)
        {
            this.Text_Validate(this.txt2);
        }

        private void txt3_Validated(object sender, EventArgs e)
        {
            this.Text_Validate(this.txt3);
        }

        //將TextBox的除錯動作合為單一處理程序
        private void Text_Validate(DevComponents.DotNetBar.Controls.TextBoxX txt)
        {
            _errorProvider.SetError(txt, null);
            if (string.IsNullOrEmpty(txt.Text))
                return;
            int i = 0;
            if (!int.TryParse(txt.Text, out i))
                _errorProvider.SetError(txt, "必須為整數數字");
            else
            {
                if (i < 0)
                    _errorProvider.SetError(txt, "不能輸入負數");
            }
        }

        //畫面離開
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //事由控制項在被改變時
        private void cboReasonRef_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboReasonRef.SelectedItem == null) return;
            KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)cboReasonRef.SelectedItem;
            txtReason.Text = kvp.Value;
        }

        private string GetTextValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "0";
            return text;
        }

        private void Modify()
        {
            //把畫面的資料填回 DemeritRecordEditor 物件中
            this.FillDataToEditor(this._demeritRecordEditor);
        }

        private List<JHDemeritRecord> Insert()
        {
            List<JHDemeritRecord> newEditors = new List<JHDemeritRecord>();

            //對所有學生，都準備好相關的 DemeritRecordEditor物件
            foreach (JHStudentRecord sr in this._students)
            {
                JHDemeritRecord dre = new JHDemeritRecord();
                dre.RefStudentID = sr.ID;
                this.FillDataToEditor(dre);
                newEditors.Add(dre);
            }

            return newEditors;

        }

        //把畫面資料填到 Editor 中。
        private void FillDataToEditor(JHDemeritRecord editor)
        {
            #region 把畫面資料填到 Editor 中
            //把畫面的資料填回 DemeritRecordEditor 物件中
            editor.SchoolYear = int.Parse(cboSchoolYear.Text);
            editor.Semester = int.Parse(cboSemester.Text);
            editor.DemeritA = ChangeInt(txt1.Text);
            editor.DemeritB = ChangeInt(txt2.Text);
            editor.DemeritC = ChangeInt(txt3.Text);
            editor.Reason = txtReason.Text;

            editor.OccurDate = dateTimeInput1.Value;
            if (dateTimeInput2.Text != "")
            {
                editor.RegisterDate = dateTimeInput2.Value;
            }
            #endregion
        }

        private int ChangeInt(string txt)
        {
            int xParse;
            if (int.TryParse(txt, out xParse))
            {
                return xParse;
            }
            else
            {
                return 0;
            }
        }

        private void cboReasonRef_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtReason.Focus();
                txtReason.Select(txtReason.Text.Length + 1, 0);
            }
        }

        private void txtReason_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                string reasonValue = "";
                List<string> list = new List<string>();
                string[] reasonList = txtReason.Text.Split(',');
                foreach (string each in reasonList)
                {
                    string each1 = each.Replace("\r\n", "");
                    if (ResonDic.ContainsKey(each1))
                    {
                        list.Add(ResonDic[each1]);
                    }
                    else
                    {
                        list.Add(each1);
                    }
                }

                reasonValue = string.Join(",", list);

                txtReason.Text = reasonValue;
            }
        }
    }
}