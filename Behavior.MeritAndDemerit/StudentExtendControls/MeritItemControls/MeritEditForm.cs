using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar;
using FISCA.DSAUtil;
using SmartSchool.Common;
using Framework;
using JHSchool.Legacy;
using Framework.Feature;
using JHSchool.Behavior.Legacy;
using JHSchool.Behavior.Editor;
using JHSchool.Data;
using FISCA.LogAgent;

namespace JHSchool.Behavior.MeritAndDemerit
{
    /// <summary>
    /// 新增或修改獎勵資料的畫面。
    /// 修改時，因為只能修改一個學生的某一筆獎勵資料，所以只要傳入一個 MeritRecordEditor 物件即可
    /// 新增時，可以同時對多位學生增加相同的獎勵紀錄，所以要傳入多位學生的資料，但不傳入MeritRecordEditor 物件，此物件會在儲存時由每位學生記錄取得。
    /// </summary>
    public partial class MeritEditForm : FISCA.Presentation.Controls.BaseForm
    {
        private List<K12.Data.StudentRecord> _students;
        private ErrorProvider _errorProvider;
        private K12.Data.MeritRecord _meritRecordEditor;
        private Dictionary<string, string> ResonDic = new Dictionary<string, string>();

        //Log
        private Dictionary<string, string> DicBeforeData = new Dictionary<string, string>();

        /// <summary>
        /// Constructor，新增時使用。
        /// </summary>
        /// <param name="students"></param>
        public MeritEditForm(List<K12.Data.StudentRecord> students)
        {
            #region 新增
            this._students = students;
            Initialize();
            dateTimeInput1.Value = DateTime.Today;
            dateTimeInput2.Value = DateTime.Today;
            Text = "獎勵管理";
            if (this._students.Count > 1)
            {
                Text = string.Format("獎勵管理 【 新增：{0} ... 等共 {1} 位 】", this._students[0].Name, this._students.Count.ToString()); ;
            }
            else if (this._students.Count == 1)
            {
                Text = string.Format("獎勵管理 【 新增：{0} 】", this._students[0].Name); ;
            }
            #endregion
        }

        public MeritEditForm(List<K12.Data.StudentRecord> students, string SchoolYear, string Semester)
        {
            #region 新增
            this._students = students;

            #region 建構子
            InitializeComponent();

            _errorProvider = new ErrorProvider();

            //學年度
            intSchoolYear.Value = int.Parse(SchoolYear);
            intSemester.Value = int.Parse(Semester);
            #endregion


            dateTimeInput1.Value = DateTime.Today;
            dateTimeInput2.Value = DateTime.Today;
            Text = "獎勵管理";
            if (this._students.Count > 1)
            {
                Text = string.Format("獎勵管理 【 新增：{0} ... 等共 {1} 位 】", this._students[0].Name, this._students.Count.ToString()); ;
            }
            else if (this._students.Count == 1)
            {
                Text = string.Format("獎勵管理 【 新增：{0} 】", this._students[0].Name); ;
            }
            #endregion
        }

        //多載建構子
        public MeritEditForm(List<K12.Data.StudentRecord> students, string SchoolYear, string Semester,bool LockMode)
        {
            #region 新增
            this._students = students;

            #region 建構子
            InitializeComponent();

            _errorProvider = new ErrorProvider();

            //學年度
            intSchoolYear.Value = int.Parse(SchoolYear);
            intSemester.Value = int.Parse(Semester);
            #endregion

            //LockMode狀態鎖住學年度學期
            if (LockMode)
            {
                intSchoolYear.Enabled = false;
                intSemester.Enabled = false;
            }

            dateTimeInput1.Value = DateTime.Today;
            dateTimeInput2.Value = DateTime.Today;
            Text = "獎勵管理";
            if (this._students.Count > 1)
            {
                Text = string.Format("獎勵管理 【 新增：{0} ... 等共 {1} 位 】", this._students[0].Name, this._students.Count.ToString()); ;
            }
            else if (this._students.Count == 1)
            {
                Text = string.Format("獎勵管理 【 新增：{0} 】", this._students[0].Name); ;
            }
            #endregion
        }

        /// <summary>
        /// Constructor，修改時使用
        /// </summary>
        /// <param name="_demeritRecordEditor"></param>
        public MeritEditForm(K12.Data.MeritRecord meritRecordEditor, Framework.Security.FeatureAce permission)
        {
            #region 修改
            this._meritRecordEditor = meritRecordEditor;

            #region Log用

            if (meritRecordEditor.MeritA.HasValue)
            {
                DicBeforeData.Add("大功", meritRecordEditor.MeritA.Value.ToString());
            }
            if (meritRecordEditor.MeritB.HasValue)
            {
                DicBeforeData.Add("小功", meritRecordEditor.MeritB.Value.ToString());
            }
            if (meritRecordEditor.MeritC.HasValue)
            {
                DicBeforeData.Add("嘉獎", meritRecordEditor.MeritC.Value.ToString());
            }
            DicBeforeData.Add("事由", meritRecordEditor.Reason);

            DicBeforeData.Add("備註", meritRecordEditor.Remark);
            #endregion

            this._students = new List<K12.Data.StudentRecord>();
            this._students.Add(K12.Data.Student.SelectByID(meritRecordEditor.RefStudentID));

            Initialize();

            if (permission.Editable)
                Text = string.Format("獎勵管理 【 修改：{0}，{1} 】", Student.Instance[meritRecordEditor.RefStudentID].Name, meritRecordEditor.OccurDate.ToShortDateString());
            else
                Text = string.Format("獎勵管理 【 檢視：{0}，{1} 】", Student.Instance[meritRecordEditor.RefStudentID].Name, meritRecordEditor.OccurDate.ToShortDateString());

            btnSave.Visible = permission.Editable;
            cboReasonRef.Enabled = permission.Editable;
            intSchoolYear.Enabled = permission.Editable;
            intSemester.Enabled = permission.Editable;
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
            intSchoolYear.Value = int.Parse(K12.Data.School.DefaultSchoolYear);
            intSemester.Value = int.Parse(K12.Data.School.DefaultSemester);
            #endregion
        }

        private void MeritEditor_Load(object sender, EventArgs e)
        {
            #region Load

            List<string> remarkList = tool.GerRemarkTitle("1");
            cbRemark.Items.AddRange(remarkList.ToArray());

            //取得獎勵的代碼和原因清單，並放到 事由代碼 的下拉式方塊中。
            DSResponse dsrsp = Config.GetDisciplineReasonList();
            cboReasonRef.SelectedItem = null;
            cboReasonRef.Items.Clear();
            DSXmlHelper helper = dsrsp.GetContent();
            KeyValuePair<string, string> fkvp = new KeyValuePair<string, string>("", "");
            cboReasonRef.Items.Add(fkvp);

            foreach (XmlElement element in helper.GetElements("Reason"))
            {
                if (element.GetAttribute("Type") == "獎勵")
                {
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
            if (this._meritRecordEditor != null)
            {
                txtReason.Text = _meritRecordEditor.Reason;
                txt1.Text = _meritRecordEditor.MeritA.ToString();
                txt2.Text = _meritRecordEditor.MeritB.ToString();
                txt3.Text = _meritRecordEditor.MeritC.ToString();
                intSchoolYear.Text = _meritRecordEditor.SchoolYear.ToString();
                intSemester.Text = _meritRecordEditor.Semester.ToString();

                dateTimeInput1.Value = _meritRecordEditor.OccurDate;

                if (_meritRecordEditor.RegisterDate != null)
                {
                    dateTimeInput2.Value = _meritRecordEditor.RegisterDate.Value;
                }
                else
                {
                    dateTimeInput2.Text = "";
                }

                cbRemark.Text = _meritRecordEditor.Remark;
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


            if (this._meritRecordEditor == null) //新增
            {

                List<K12.Data.MeritRecord> LogMeritList = new List<K12.Data.MeritRecord>();
                try
                {
                    LogMeritList = Insert();
                    K12.Data.Merit.Insert(LogMeritList);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("新增獎勵記錄時發生錯誤: \n" + ex.Message);
                    return;
                }

                if (_students.Count == 1)
                {
                    #region 單筆新增Log
                    StringBuilder sb = new StringBuilder();
                    sb.Append("學生「" + LogMeritList[0].Student.Name + "」");
                    sb.Append("日期「" + LogMeritList[0].OccurDate.ToShortDateString() + "」");
                    sb.AppendLine("新增一筆獎勵資料。");
                    sb.AppendLine("詳細資料：");
                    if (LogMeritList[0].MeritA.HasValue)
                    {
                        sb.Append("大功「" + LogMeritList[0].MeritA.Value.ToString() + "」");
                    }
                    if (LogMeritList[0].MeritB.HasValue)
                    {
                        sb.Append("小功「" + LogMeritList[0].MeritB.Value.ToString() + "」");
                    }
                    if (LogMeritList[0].MeritC.HasValue)
                    {
                        sb.Append("嘉獎「" + LogMeritList[0].MeritC.Value.ToString() + "」");
                    }
                    sb.AppendLine("獎勵事由「" + LogMeritList[0].Reason + "」");
                    sb.AppendLine("備註「" + LogMeritList[0].Remark + "」");
                    ApplicationLog.Log("學務系統.獎勵資料", "新增學生獎勵資料", "student", _students[0].ID, sb.ToString());
                    #endregion
                    FISCA.Presentation.Controls.MsgBox.Show("新增獎勵資料成功!");
                }
                else if (_students.Count > 1)
                {
                    #region 批次新增Log
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("批次新增獎勵資料");
                    sb.Append("日期「" + LogMeritList[0].OccurDate.ToShortDateString() + "」");
                    sb.AppendLine("共「" + LogMeritList.Count + "」名學生，");
                    sb.AppendLine("詳細資料：");
                    if (LogMeritList[0].MeritA.HasValue)
                    {
                        sb.Append("大功「" + LogMeritList[0].MeritA.Value.ToString() + "」");
                    }
                    if (LogMeritList[0].MeritB.HasValue)
                    {
                        sb.Append("小功「" + LogMeritList[0].MeritB.Value.ToString() + "」");
                    }
                    if (LogMeritList[0].MeritC.HasValue)
                    {
                        sb.Append("嘉獎「" + LogMeritList[0].MeritC.Value.ToString() + "」");
                    }
                    sb.AppendLine("獎勵事由「" + LogMeritList[0].Reason + "」");
                    sb.AppendLine("備註「" + LogMeritList[0].Remark + "」");
                    sb.AppendLine("學生詳細資料：");
                    foreach (K12.Data.MeritRecord each in LogMeritList)
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

                    ApplicationLog.Log("學務系統.獎勵資料", "批次新增學生獎勵資料", sb.ToString());
                    #endregion
                    FISCA.Presentation.Controls.MsgBox.Show("批次新增獎勵資料成功!");
                }
            }
            else //修改
            {
                try
                {
                    Modify();
                    K12.Data.Merit.Update(this._meritRecordEditor);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("修改獎勵記錄時發生錯誤: \n" + ex.Message);
                    return;
                }

                #region 修改Log
                StringBuilder sb = new StringBuilder();
                sb.Append("學生「" + this._meritRecordEditor.Student.Name + "」");
                sb.AppendLine("日期「" + this._meritRecordEditor.OccurDate.ToShortDateString() + "」獎勵資料已修改。");
                sb.AppendLine("詳細資料：");
                sb.AppendLine("大功「" + DicBeforeData["大功"] + "」變更為「" + this._meritRecordEditor.MeritA.Value + "」");
                sb.AppendLine("小功「" + DicBeforeData["小功"] + "」變更為「" + this._meritRecordEditor.MeritB.Value + "」");
                sb.AppendLine("嘉獎「" + DicBeforeData["嘉獎"] + "」變更為「" + this._meritRecordEditor.MeritC.Value + "」");
                sb.AppendLine("獎勵事由「" + DicBeforeData["事由"] + "」變更為「" + this._meritRecordEditor.Reason + "」");
                sb.AppendLine("備註「" + DicBeforeData["備註"] + "」變更為「" + this._meritRecordEditor.Remark + "」");
                ApplicationLog.Log("學務系統.獎勵資料", "修改學生獎勵資料", "student", this._meritRecordEditor.Student.ID, sb.ToString());
                #endregion
                FISCA.Presentation.Controls.MsgBox.Show("修改獎勵資料成功!");
            }

            this.Close();
            #endregion
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboReasonRef_SelectedIndexChanged(object sender, EventArgs e)
        {
            KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)cboReasonRef.SelectedItem;
            txtReason.Text = kvp.Value;
        }

        private string GetTextValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "0";
            return text;
        }

        //private string chengDateTime(DateTime x)
        //{
        //    if (x == null)
        //        return "";
        //    string time = x.ToString();
        //    int y = time.IndexOf(' ');
        //    return time.Remove(y);
        //}

        private void Modify()
        {
            //把畫面的資料填回 MeritRecordEditor 物件中
            this.FillDataToEditor(this._meritRecordEditor);
        }

        private List<K12.Data.MeritRecord> Insert()
        {
            List<K12.Data.MeritRecord> newEditors = new List<K12.Data.MeritRecord>();

            //對所有學生，都準備好相關的 MeritRecordEditor物件
            foreach (K12.Data.StudentRecord sr in this._students)
            {
                K12.Data.MeritRecord dre = new K12.Data.MeritRecord();
                dre.RefStudentID = sr.ID;
                this.FillDataToEditor(dre);
                newEditors.Add(dre);
            }
            return newEditors;
        }

        //把畫面資料填到 Editor 中。因為新增和修改模式都會有這些程式碼，所已抽出來成為一個函數，以避免程式碼重複。
        private void FillDataToEditor(K12.Data.MeritRecord editor)
        {
            //把畫面的資料填回 MeritRecordEditor 物件中

            editor.SchoolYear = intSchoolYear.Value;
            editor.Semester = intSemester.Value;
            editor.MeritA = ChangeInt(txt1.Text);
            editor.MeritB = ChangeInt(txt2.Text);
            editor.MeritC = ChangeInt(txt3.Text);
            editor.Reason = txtReason.Text;
            editor.Remark = cbRemark.Text;
            editor.OccurDate = dateTimeInput1.Value;
            if (dateTimeInput2.Text != "")
            {
                editor.RegisterDate = dateTimeInput2.Value;
            }
        }

        //處理文字轉數字
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