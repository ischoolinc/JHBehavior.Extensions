using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using System.Xml;
using JHSchool.Behavior.BusinessLogic;
using FISCA.LogAgent;
using Framework.Security;
using FCode = Framework.Security.FeatureCodeAttribute;
using Framework;
using FISCA.DSAUtil;
using Campus.Windows;

namespace JHSchool.Behavior.MeritAndDemerit
{
    [FCode("JHSchool.Student.Detail0035", "獎懲學期統計")]
    public partial class DisciplineUnifytForm : BaseForm
    {
        BackgroundWorker BGW = new BackgroundWorker();
        bool BkWBool = false;
        bool _LockMode = false; //學年度學期鎖定模式

        List<K12.Data.MeritRecord> MeritList = new List<K12.Data.MeritRecord>();
        List<K12.Data.DemeritRecord> DemeirtList = new List<K12.Data.DemeritRecord>();
        List<AutoSummaryRecord> AutoRecord = new List<AutoSummaryRecord>();
        K12.Data.MoralScoreRecord MSRecord;

        private FeatureAce _UserPermission;

        int MeritA;
        int MeritB;
        int MeritC;
        int DemeritA;
        int DemeritB;
        int DemeritC;

        Color SetColor = Color.LightCyan;

        string _StudentID;
        int _SchoolYear;
        int _Semester;


        public DisciplineUnifytForm(string StudentID, int SchoolYear, int Semester, FeatureAce UserPermission)
        {
            InitializeComponent();

            _StudentID = StudentID;
            _SchoolYear = SchoolYear;
            _Semester = Semester;

            _UserPermission = UserPermission;
        }

        //多載建構子 by Cloud
        public DisciplineUnifytForm(string StudentID, int SchoolYear, int Semester, FeatureAce UserPermission,bool LockMode)
        {
            InitializeComponent();

            _StudentID = StudentID;
            _SchoolYear = SchoolYear;
            _Semester = Semester;

            _UserPermission = UserPermission;
            _LockMode = LockMode;
        }

        private void DemeritUnifytForm_Load(object sender, EventArgs e)
        {
            List<string> cols1 = new List<string>() { "大功", "小功", "嘉獎" };
            DataGridViewImeDecorator dec1 = new DataGridViewImeDecorator(this.dgvMerit, cols1);

            List<string> cols2 = new List<string>() { "大過", "小過", "警告" };
            DataGridViewImeDecorator dec2 = new DataGridViewImeDecorator(this.dgvDemerit, cols2);

            lbHelp1.Text = _SchoolYear + "學年度　第" + _Semester + "學期　獎勵 / 懲戒記錄";

            BGW.DoWork += new DoWorkEventHandler(BgW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgW_RunWorkerCompleted);

            K12.Data.Merit.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(DataChanged);
            K12.Data.Merit.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(DataChanged);
            K12.Data.Merit.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(DataChanged);
            K12.Data.Demerit.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(DataChanged);
            K12.Data.Demerit.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(DataChanged);
            K12.Data.Demerit.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(DataChanged);
            K12.Data.MoralScore.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(DataChanged);
            K12.Data.MoralScore.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(DataChanged);
            K12.Data.MoralScore.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(DataChanged);

            BGW.RunWorkerAsync();
        }

        /// <summary>
        /// 總合更新內容
        /// </summary>
        void DataChanged(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (BGW.IsBusy)
            {
                BkWBool = true;
            }
            else
            {
                lockForm(false);
                BGW.RunWorkerAsync();
            }
        }

        private void lockForm(bool IsTrueOrFalse)
        {

            btnMeritNew.Enabled = IsTrueOrFalse;
            btnMeritEdit.Enabled = IsTrueOrFalse;
            btnMeritDelete.Enabled = IsTrueOrFalse;
            btnSaveMeritStatistics.Enabled = IsTrueOrFalse;
            dgvMerit.Enabled = IsTrueOrFalse;

            btnDemeritNew.Enabled = IsTrueOrFalse;
            btnDemeritEdit.Enabled = IsTrueOrFalse;
            btnDemeritClear.Enabled = IsTrueOrFalse;
            btnDemeritDelete.Enabled = IsTrueOrFalse;
            btnSaveMeritStatistics.Enabled = IsTrueOrFalse;
            dgvDemerit.Enabled = IsTrueOrFalse;

            if (IsTrueOrFalse)
            {
                this.Text = "獎勵 / 懲戒學期統計";
            }
            else
            {
                this.Text = "更新資料中...";
            }
        }

        void BgW_DoWork(object sender, DoWorkEventArgs e)
        {

            List<SchoolYearSemester> SchoolSemesterList = new List<SchoolYearSemester>();

            //取得獎勵
            MeritList = K12.Data.Merit.SelectBySchoolYearAndSemester(_StudentID, _SchoolYear, _Semester);
            MeritList.Sort(new Comparison<K12.Data.MeritRecord>(SchoolYearComparerMerit));

            //取得懲戒
            DemeirtList = K12.Data.Demerit.SelectBySchoolYearAndSemester(_StudentID, _SchoolYear, _Semester);
            DemeirtList.Sort(new Comparison<K12.Data.DemeritRecord>(SchoolYearComparerDemerit));

            SchoolYearSemester SchoolSemester = new SchoolYearSemester(_SchoolYear, _Semester);
            //取得自動統計
            AutoRecord = AutoSummary.Select(new string[] { _StudentID }, new SchoolYearSemester[] { SchoolSemester });

            //取得日常生活表現
            MSRecord = K12.Data.MoralScore.SelectBySchoolYearAndSemester(_StudentID, _SchoolYear, _Semester);

        }

        private int SchoolYearComparerMerit(K12.Data.MeritRecord x, K12.Data.MeritRecord y)
        {
            return y.OccurDate.CompareTo(x.OccurDate);
        }

        private int SchoolYearComparerDemerit(K12.Data.DemeritRecord x, K12.Data.DemeritRecord y)
        {
            return y.OccurDate.CompareTo(x.OccurDate);
        }

        void BgW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (BkWBool)
            {
                BkWBool = false;
                BGW.RunWorkerAsync();
                return;
            }

            lockForm(true);

            BingDataMerit();

            BingDataDemerit();

            BingDataMoralScore();
        }

        /// <summary>
        /// 獎勵資料
        /// </summary>
        private void BingDataMerit()
        {
            listViewMerit.Items.Clear();
            MeritA = 0;
            MeritB = 0;
            MeritC = 0;

            foreach (K12.Data.MeritRecord each in MeritList)
            {
                ListViewItem itms = new ListViewItem(each.OccurDate.ToShortDateString());

                if (each.MeritA.HasValue)
                {
                    itms.SubItems.Add(each.MeritA.Value.ToString());
                    MeritA += each.MeritA.Value;
                }
                else
                {
                    itms.SubItems.Add("");
                }
                if (each.MeritB.HasValue)
                {
                    itms.SubItems.Add(each.MeritB.Value.ToString());
                    MeritB += each.MeritB.Value;
                }
                else
                {
                    itms.SubItems.Add("");
                }
                if (each.MeritC.HasValue)
                {
                    itms.SubItems.Add(each.MeritC.Value.ToString());
                    MeritC += each.MeritC.Value;
                }
                else
                {
                    itms.SubItems.Add("");
                }

                itms.SubItems.Add(each.Reason);
                itms.SubItems.Add(each.RegisterDate.HasValue ? each.RegisterDate.Value.ToShortDateString() : "");
                itms.SubItems.Add(each.Remark);
                itms.Tag = each;
                listViewMerit.Items.Add(itms);
            }
        }

        /// <summary>
        /// 懲戒資料
        /// </summary>
        private void BingDataDemerit()
        {
            listViewDemerit.Items.Clear();
            DemeritA = 0;
            DemeritB = 0;
            DemeritC = 0;

            foreach (K12.Data.DemeritRecord each in DemeirtList)
            {
                ListViewItem itms = new ListViewItem(each.OccurDate.ToShortDateString());
                if (each.DemeritA.HasValue)
                {
                    itms.SubItems.Add(each.DemeritA.Value.ToString());
                    if (each.Cleared != "是")
                    {
                        DemeritA += each.DemeritA.Value;
                    }

                }
                else
                {
                    itms.SubItems.Add("");
                }
                if (each.DemeritB.HasValue)
                {
                    itms.SubItems.Add(each.DemeritB.Value.ToString());
                    if (each.Cleared != "是")
                    {
                        DemeritB += each.DemeritB.Value;
                    }
                }
                else
                {
                    itms.SubItems.Add("");
                }
                if (each.DemeritC.HasValue)
                {
                    itms.SubItems.Add(each.DemeritC.Value.ToString());
                    if (each.Cleared != "是")
                    {
                        DemeritC += each.DemeritC.Value;
                    }
                }
                else
                {
                    itms.SubItems.Add("");
                }
                itms.SubItems.Add(each.Reason);
                itms.SubItems.Add(each.Cleared);
                itms.SubItems.Add(each.ClearDate.HasValue ? each.ClearDate.Value.ToShortDateString() : "");
                itms.SubItems.Add(each.ClearReason);
                itms.SubItems.Add(each.RegisterDate.HasValue ? each.RegisterDate.Value.ToShortDateString() : "");
                itms.SubItems.Add(each.Remark);
                itms.Tag = each;
                listViewDemerit.Items.Add(itms);
            }
        }

        /// <summary>
        /// 日常生活表現資料
        /// </summary>
        private void BingDataMoralScore()
        {
            dgvMerit.Rows.Clear();
            dgvDemerit.Rows.Clear();

            //獎勵明細統計
            dgvMerit.Rows.Add(MeritOrDemeritDetailStatistics(true));

            //獎勵非明細統計(InitialSummary)
            dgvMerit.Rows.Add(MeritOrDemeritInitialSummary(true));

            //獎勵學期統計(AutoSummary)
            dgvMerit.Rows.Add(MeritOrDemeritAutoSummary(true));

            dgvMerit.Columns[0].Width = 150;
            dgvMerit.Columns[1].Width = 120;
            dgvMerit.Columns[2].Width = 120;
            dgvMerit.Columns[3].Width = 120;

            //懲戒明細統計
            dgvDemerit.Rows.Add(MeritOrDemeritDetailStatistics(false));

            //懲戒非明細統計(InitialSummary)
            dgvDemerit.Rows.Add(MeritOrDemeritInitialSummary(false));

            //懲戒學期統計(AutoSummary)
            dgvDemerit.Rows.Add(MeritOrDemeritAutoSummary(false));

            dgvDemerit.Columns[0].Width = 150;
            dgvDemerit.Columns[1].Width = 120;
            dgvDemerit.Columns[2].Width = 120;
            dgvDemerit.Columns[3].Width = 120;
        }

        //明細統計
        private DataGridViewRow MeritOrDemeritDetailStatistics(bool IsMeritOrDemerit)
        {
            DataGridViewRow row1 = new DataGridViewRow();
            if (IsMeritOrDemerit)
            {
                row1.CreateCells(dgvMerit); //獎勵Row
                row1.Cells[1].Value = MeritA.ToString();
                row1.Cells[2].Value = MeritB.ToString();
                row1.Cells[3].Value = MeritC.ToString();
            }
            else
            {
                row1.CreateCells(dgvDemerit); //懲戒Row
                row1.Cells[1].Value = DemeritA.ToString();
                row1.Cells[2].Value = DemeritB.ToString();
                row1.Cells[3].Value = DemeritC.ToString();
            }
            row1.ReadOnly = true; //不可更動
            row1.Cells[0].Style.BackColor = SetColor;
            row1.Cells[1].Style.BackColor = SetColor;
            row1.Cells[2].Style.BackColor = SetColor;
            row1.Cells[3].Style.BackColor = SetColor;

            row1.Cells[0].Value = "明細統計";

            return row1;
        }

        //期中統計
        private DataGridViewRow MeritOrDemeritInitialSummary(bool IsMeritOrDemerit)
        {
            DataGridViewRow row2 = new DataGridViewRow();

            if (MSRecord != null)
            {
                SummarySetupObj obj = new SummarySetupObj(MSRecord.InitialSummary);

                if (IsMeritOrDemerit)
                {
                    row2.CreateCells(dgvMerit);
                    row2.Cells[1].Value = obj.MeritA.ToString();
                    row2.Cells[2].Value = obj.MeritB.ToString();
                    row2.Cells[3].Value = obj.MeritC.ToString();
                }
                else
                {
                    row2.CreateCells(dgvDemerit);
                    row2.Cells[1].Value = obj.DemeritA.ToString();
                    row2.Cells[2].Value = obj.DemeritB.ToString();
                    row2.Cells[3].Value = obj.DemeritC.ToString();
                }

                row2.Cells[0].ReadOnly = true;
                row2.Cells[0].Style.BackColor = SetColor;
                row2.Cells[0].Value = "非明細統計";
            }
            else
            {

                if (IsMeritOrDemerit)
                {
                    row2.CreateCells(dgvMerit);
                }
                else
                {
                    row2.CreateCells(dgvDemerit);
                }

                row2.Cells[0].ReadOnly = true;
                row2.Cells[0].Style.BackColor = SetColor;

                row2.Cells[0].Value = "非明細統計";
                row2.Cells[1].Value = "0";
                row2.Cells[2].Value = "0";
                row2.Cells[3].Value = "0";

            }
            return row2;
        }

        //自動統計
        private DataGridViewRow MeritOrDemeritAutoSummary(bool IsMeritOrDemerit)
        {
            DataGridViewRow row3 = new DataGridViewRow();
            foreach (AutoSummaryRecord each in AutoRecord)
            {
                if (each.SchoolYear == _SchoolYear && each.Semester == _Semester)
                {
                    //XmlElement xml3;
                    if (IsMeritOrDemerit)
                    {
                        row3.CreateCells(dgvMerit);
                        //xml3 = (XmlElement)MSRecord.Summary.SelectSingleNode("DisciplineStatistics/Merit");
                    }
                    else
                    {
                        row3.CreateCells(dgvDemerit);
                        //xml3 = (XmlElement)MSRecord.Summary.SelectSingleNode("DisciplineStatistics/Demerit");
                    }
                    row3.ReadOnly = true; //AutoSummery為自動計算
                    row3.Cells[0].Style.BackColor = SetColor;
                    row3.Cells[1].Style.BackColor = SetColor;
                    row3.Cells[2].Style.BackColor = SetColor;
                    row3.Cells[3].Style.BackColor = SetColor;
                    row3.Cells[0].Value = "本學期獎懲統計";

                    if (IsMeritOrDemerit)
                    {
                        row3.Cells[1].Value = each.MeritA.ToString();
                        row3.Cells[2].Value = each.MeritB.ToString();
                        row3.Cells[3].Value = each.MeritC.ToString();
                    }
                    else
                    {
                        row3.Cells[1].Value = each.DemeritA.ToString();
                        row3.Cells[2].Value = each.DemeritB.ToString();
                        row3.Cells[3].Value = each.DemeritC.ToString();
                    }
                }
            }

            return row3;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //新增獎勵
        private void btnMeritNew_Click(object sender, EventArgs e)
        {
            List<K12.Data.StudentRecord> studs = new List<K12.Data.StudentRecord>();
            studs.Add(K12.Data.Student.SelectByID(_StudentID));
            MeritEditForm editForm;
            //判斷是否為LockMode
            if(_LockMode)
            {
                editForm = new MeritEditForm(studs, _SchoolYear.ToString(), _Semester.ToString(),true);  //此編輯表單在新增模式下允許一次對多位學生新增相同的懲戒記錄，所以 Constructor 要傳入學生的集合。
            }
            else
            {
                editForm = new MeritEditForm(studs, _SchoolYear.ToString(), _Semester.ToString());  //此編輯表單在新增模式下允許一次對多位學生新增相同的懲戒記錄，所以 Constructor 要傳入學生的集合。
            }

            editForm.ShowDialog();
        }

        //修改獎勵
        private void btnMeritEdit_Click(object sender, EventArgs e)
        {
            if (listViewMerit.SelectedItems.Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("請先選擇一筆您要修改的資料");
                return;
            }
            if (listViewMerit.SelectedItems.Count > 1)
            {
                FISCA.Presentation.Controls.MsgBox.Show("選擇資料筆數過多，一次只能修改一筆資料");
                return;
            }

            K12.Data.MeritRecord record = (K12.Data.MeritRecord)this.listViewMerit.SelectedItems[0].Tag;
            MeritEditForm editForm = new MeritEditForm(record, _UserPermission); //此編輯表單在修改模式下，一次只能對一位學生的某一筆懲戒記錄進行修改，所以 Constructor 就傳入一個 Editor 物件。
            editForm.ShowDialog();
        }

        //刪除獎勵
        private void btnMeritDelete_Click(object sender, EventArgs e)
        {
            if (listViewMerit.SelectedItems.Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("必須選擇一筆以上資料!!");
                return;
            }

            List<K12.Data.MeritRecord> MeritList = new List<K12.Data.MeritRecord>();

            if (FISCA.Presentation.Controls.MsgBox.Show("確定將刪除所選擇之獎勵資料?", "確認", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            foreach (ListViewItem item in listViewMerit.SelectedItems)
            {
                K12.Data.MeritRecord record = item.Tag as K12.Data.MeritRecord;
                MeritList.Add(record);
            }  

            try
            {
                K12.Data.Merit.Delete(MeritList);
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("刪除獎勵資料失敗!" + ex.Message);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("學生「" + K12.Data.Student.SelectByID(_StudentID).Name + "」");
            foreach (K12.Data.MeritRecord merit in MeritList)
            {
                sb.AppendLine("學年度「" + merit.SchoolYear + "」");
                sb.AppendLine("學期「" + merit.Semester + "」");
                sb.AppendLine("日期「" + merit.OccurDate.ToShortDateString() + "」");

                int a = merit.MeritA.HasValue ? merit.MeritA.Value : 0;
                int b = merit.MeritB.HasValue ? merit.MeritB.Value : 0;
                int c = merit.MeritC.HasValue ? merit.MeritC.Value : 0;
                sb.AppendLine(string.Format("支數「大功：{0} 小功：{1} 嘉獎：{2}」", a, b, c));

                sb.AppendLine("事由「" + merit.Reason + "」");
                sb.AppendLine("備註「" + merit.Remark + "」");
            }
            sb.AppendLine("獎勵資料已被刪除。");

            ApplicationLog.Log("學務系統.獎勵資料", "刪除學生獎勵資料", "student", _StudentID, sb.ToString());
            
            FISCA.Presentation.Controls.MsgBox.Show("刪除獎勵資料成功");
        }

        //新增懲戒
        private void btnDemeritNew_Click(object sender, EventArgs e)
        {
            List<K12.Data.StudentRecord> studs = new List<K12.Data.StudentRecord>();
            studs.Add(K12.Data.Student.SelectByID(_StudentID));
            DemeritEditForm editForm;
            //判斷是否為LockMode
            if(_LockMode)
            {
                editForm = new DemeritEditForm(studs, _SchoolYear.ToString(), _Semester.ToString(), true);  //此編輯表單在新增模式下允許一次對多位學生新增相同的懲戒記錄，所以 Constructor 要傳入學生的集合。
            }
            else
            {
                editForm = new DemeritEditForm(studs, _SchoolYear.ToString(), _Semester.ToString());  //此編輯表單在新增模式下允許一次對多位學生新增相同的懲戒記錄，所以 Constructor 要傳入學生的集合。
            }
            
            editForm.ShowDialog();
        }

        //修改懲戒
        private void btnDemeritEdit_Click(object sender, EventArgs e)
        {
            if (listViewDemerit.SelectedItems.Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("請先選擇一筆您要修改的資料");
                return;
            }
            if (listViewDemerit.SelectedItems.Count > 1)
            {
                FISCA.Presentation.Controls.MsgBox.Show("選擇資料筆數過多，一次只能修改一筆資料");
                return;
            }

            K12.Data.DemeritRecord record = (K12.Data.DemeritRecord)this.listViewDemerit.SelectedItems[0].Tag;
            DemeritEditForm editForm = new DemeritEditForm(record, _UserPermission); //此編輯表單在修改模式下，一次只能對一位學生的某一筆懲戒記錄進行修改，所以 Constructor 就傳入一個 Editor 物件。
            editForm.ShowDialog();
        }

        //刪除懲戒
        private void btnDemeritDelete_Click(object sender, EventArgs e)
        {
            if (listViewDemerit.SelectedItems.Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("必須選擇一筆以上資料!!");
                return;
            }

            List<K12.Data.DemeritRecord> DemeritList = new List<K12.Data.DemeritRecord>();

            if (FISCA.Presentation.Controls.MsgBox.Show("確定將刪除所選擇之懲戒資料?", "確認", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            foreach (ListViewItem item in listViewDemerit.SelectedItems)
            {
                K12.Data.DemeritRecord record = item.Tag as K12.Data.DemeritRecord;
                DemeritList.Add(record);
            }    

            try
            {
                K12.Data.Demerit.Delete(DemeritList);
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("刪除懲戒資料失敗: \n" + ex.Message);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("學生「" + K12.Data.Student.SelectByID(_StudentID).Name + "」");
            foreach (K12.Data.DemeritRecord demerit in DemeritList)
            {
                sb.AppendLine("學年度「" + demerit.SchoolYear + "」");
                sb.AppendLine("學期「" + demerit.Semester + "」");
                sb.AppendLine("日期「" + demerit.OccurDate.ToShortDateString() + "」");

                int a = demerit.DemeritA.HasValue ? demerit.DemeritA.Value : 0;
                int b = demerit.DemeritB.HasValue ? demerit.DemeritB.Value : 0;
                int c = demerit.DemeritC.HasValue ? demerit.DemeritC.Value : 0;
                sb.AppendLine(string.Format("支數「大過：{0} 小過：{1} 警告：{2}」", a, b, c));

                sb.AppendLine("事由「" + demerit.Reason + "」");
                sb.AppendLine("備註「" + demerit.Remark + "」");
            }
            sb.AppendLine("懲戒資料已被刪除。");

            ApplicationLog.Log("學務系統.懲戒資料", "刪除學生懲戒資料", "student", _StudentID, sb.ToString());
            
            FISCA.Presentation.Controls.MsgBox.Show("刪除懲戒資料成功");
        }

        //銷過懲戒
        private void btnDemeritClear_Click(object sender, EventArgs e)
        {
            if (listViewDemerit.SelectedItems.Count == 1)
            {
                K12.Data.DemeritRecord dr = (K12.Data.DemeritRecord)listViewDemerit.SelectedItems[0].Tag;

                if (this.btnDemeritClear.Text == "銷過")
                {
                    ClearDemeritForm editForm = new ClearDemeritForm(dr);
                    editForm.ShowDialog();
                }
                else   //取消銷過
                {
                    if (FISCA.Presentation.Controls.MsgBox.Show("您要將此筆銷過紀錄恢復成未銷過狀態嗎?", "確定", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dr.ClearDate = null;
                        dr.ClearReason = "";
                        dr.Cleared = "";
                        try
                        {
                            K12.Data.Demerit.Update(dr);
                            //MsgBox.Show("取消銷過完成!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            FISCA.Presentation.Controls.MsgBox.Show("取消銷過失敗： \n" + ex.Message);
                            return;
                        }

                        ApplicationLog.Log("學務系統.懲戒資料", "取消銷過", "student", dr.Student.ID, "學生「" + dr.Student.Name + "」於「" + dr.OccurDate.ToShortDateString() + "」日，懲戒資料的「銷過狀態」已被取消。");
                        FISCA.Presentation.Controls.MsgBox.Show("取消銷過成功!");
                    }
                }

            }
        }

        //銷過按鈕控制
        private void listViewDemerit_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnDemeritClear.Enabled = false;

            if (this.listViewDemerit.SelectedItems.Count == 1)
            {
                K12.Data.DemeritRecord dr = (K12.Data.DemeritRecord)this.listViewDemerit.SelectedItems[0].Tag;

                this.btnDemeritClear.Enabled = true;

                this.btnDemeritClear.Text = (string.IsNullOrEmpty(dr.Cleared) ? "銷過" : "取消銷過");
            } 
        
        }

        /// <summary>
        /// 判斷字串
        /// </summary>
        private int ParseInt(string LoadString)
        {
            int nowINT;
            if (int.TryParse(LoadString, out nowINT))
            {
                return nowINT;
            }
            else
            {
                return 0;
            }
        }

        //獎勵儲存(非明細)
        private void btnSaveMeritStatistics_Click(object sender, EventArgs e)
        {
            if (MSRecord != null)
            {
                SummarySetupObj obj = new SummarySetupObj(MSRecord.InitialSummary);
                obj.MeritA = ParseInt("" + dgvMerit.Rows[1].Cells[1].Value);
                obj.MeritB = ParseInt("" + dgvMerit.Rows[1].Cells[2].Value);
                obj.MeritC = ParseInt("" + dgvMerit.Rows[1].Cells[3].Value);
                MSRecord.InitialSummary = obj.GetAllXmlElement();

                //Log
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("修改學生" + "「" + MSRecord.Student.Name + "」非明細獎勵統計值");
                sb.AppendLine("大功「" + dgvMerit.Rows[1].Cells[1].Value + "」");
                sb.AppendLine("小功「" + dgvMerit.Rows[1].Cells[2].Value + "」");
                sb.AppendLine("嘉獎「" + dgvMerit.Rows[1].Cells[3].Value + "」");
               
                #region 有這個物件(註解)
                //if (MSRecord.InitialSummary.InnerXml != "") //如果不是空的
                //{
                //    #region 如果不是空的
                //    if (MSRecord.InitialSummary.SelectSingleNode("DisciplineStatistics") != null)
                //    {
                //        #region DisciplineStatistics
                //        XmlElement xml = (XmlElement)MSRecord.InitialSummary.SelectSingleNode("DisciplineStatistics/Merit");

                //        //用取代的(複雜)
                //        if (xml != null) //有Merit
                //        {
                //            #region Merit
                //            DSXmlHelper helper = new DSXmlHelper();
                //            XmlElement xml2 = helper.AddElement("Merit");

                //            string MeritValueA = "" + dgvMerit.Rows[1].Cells[1].Value;
                //            helper.SetAttribute("Merit", "A", MeritValueA != "" ? MeritValueA : "0");

                //            string MeritValueB = "" + dgvMerit.Rows[1].Cells[2].Value;
                //            helper.SetAttribute("Merit", "B", MeritValueB != "" ? MeritValueB : "0");

                //            string MeritValueC = "" + dgvMerit.Rows[1].Cells[3].Value;
                //            helper.SetAttribute("Merit", "C", MeritValueC != "" ? MeritValueC : "0");

                //            XmlNode XmlN = MSRecord.InitialSummary.OwnerDocument.ImportNode(xml2, true);

                //            MSRecord.InitialSummary.SelectSingleNode("DisciplineStatistics").ReplaceChild(XmlN, xml);
                //            #endregion

                //        }
                //        else //沒有Merit
                //        {
                //            #region Merit
                //            DSXmlHelper helper = new DSXmlHelper();
                //            XmlElement xml2 = helper.AddElement("Merit");

                //            string MeritValueA = "" + dgvMerit.Rows[1].Cells[1].Value;
                //            helper.SetAttribute("Merit", "A", MeritValueA != "" ? MeritValueA : "0");

                //            string MeritValueB = "" + dgvMerit.Rows[1].Cells[2].Value;
                //            helper.SetAttribute("Merit", "B", MeritValueB != "" ? MeritValueB : "0");

                //            string MeritValueC = "" + dgvMerit.Rows[1].Cells[3].Value;
                //            helper.SetAttribute("Merit", "C", MeritValueC != "" ? MeritValueC : "0");

                //            XmlNode XmlN = MSRecord.InitialSummary.OwnerDocument.ImportNode(xml2, true);

                //            MSRecord.InitialSummary.SelectSingleNode("DisciplineStatistics").AppendChild(XmlN);
                //            #endregion
                //        }
                //        #endregion
                //    }
                //    else //沒有DisciplineStatistics,但是可能有AttendanceStatistics
                //    {
                //        XmlElement xml = (XmlElement)MSRecord.InitialSummary.SelectSingleNode("AttendanceStatistics");

                //        if (xml != null) //有AttendanceStatistics,則Inset在後面
                //        {
                //            #region Merit
                //            DSXmlHelper helper = new DSXmlHelper();

                //            XmlElement xml2 = helper.AddElement("DisciplineStatistics");
                //            helper.AddElement("Merit");

                //            string MeritValueA = "" + dgvMerit.Rows[1].Cells[1].Value;
                //            helper.SetAttribute("DisciplineStatistics/Merit", "A", MeritValueA != "" ? MeritValueA : "0");

                //            string MeritValueB = "" + dgvMerit.Rows[1].Cells[2].Value;
                //            helper.SetAttribute("DisciplineStatistics/Merit", "B", MeritValueB != "" ? MeritValueB : "0");

                //            string MeritValueC = "" + dgvMerit.Rows[1].Cells[3].Value;
                //            helper.SetAttribute("DisciplineStatistics/Merit", "C", MeritValueC != "" ? MeritValueC : "0");

                //            XmlNode XmlN = MSRecord.InitialSummary.OwnerDocument.ImportNode(xml2, true);

                //            MSRecord.InitialSummary.InsertAfter(XmlN, xml);
                //            #endregion
                //        }
                //        else //無AttendanceStatistics直接新增
                //        {
                //            #region Merit
                //            DSXmlHelper helper = new DSXmlHelper();

                //            XmlElement xml2 = helper.AddElement("DisciplineStatistics");
                //            helper.AddElement("Merit");

                //            string MeritValueA = "" + dgvMerit.Rows[1].Cells[1].Value;
                //            helper.SetAttribute("Merit", "A", MeritValueA != "" ? MeritValueA : "0");

                //            string MeritValueB = "" + dgvMerit.Rows[1].Cells[2].Value;
                //            helper.SetAttribute("Merit", "B", MeritValueB != "" ? MeritValueB : "0");

                //            string MeritValueC = "" + dgvMerit.Rows[1].Cells[3].Value;
                //            helper.SetAttribute("Merit", "C", MeritValueC != "" ? MeritValueC : "0");

                //            XmlNode XmlN = MSRecord.InitialSummary.OwnerDocument.ImportNode(xml2, true);

                //            MSRecord.InitialSummary.AppendChild(XmlN);
                //            #endregion
                //        }
                //    } 
                //    #endregion
                //}
                //else
                //{
                //    #region 如果是空的
                //    DSXmlHelper helper = new DSXmlHelper(MSRecord.InitialSummary);
                //    helper.AddElement("DisciplineStatistics");
                //    helper.AddElement("DisciplineStatistics", "Merit");

                //    string MeritValueA = "" + dgvMerit.Rows[1].Cells[1].Value;
                //    helper.SetAttribute("DisciplineStatistics/Merit", "A", MeritValueA == "" ? MeritValueA : "0");

                //    string MeritValueB = "" + dgvMerit.Rows[1].Cells[2].Value;
                //    helper.SetAttribute("DisciplineStatistics/Merit", "B", MeritValueB == "" ? MeritValueB : "0");

                //    string MeritValueC = "" + dgvMerit.Rows[1].Cells[3].Value;
                //    helper.SetAttribute("DisciplineStatistics/Merit", "C", MeritValueC == "" ? MeritValueC : "0"); 
                //    #endregion
                //}
                #endregion

                try
                {
                    K12.Data.MoralScore.Update(MSRecord);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("更新,儲存失敗");
                    return;
                }

                ApplicationLog.Log("學務系統.獎懲學期統計", "修改學生非明細獎勵統計", "student", MSRecord.RefStudentID, sb.ToString());
            }
            else
            {
                #region 資料不存在
                //新增InitialSummary
                K12.Data.MoralScoreRecord Jsr = new K12.Data.MoralScoreRecord();
                Jsr.RefStudentID = _StudentID;
                Jsr.SchoolYear = _SchoolYear;
                Jsr.Semester = _Semester;

                //Log
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("新增學生" + "「" + K12.Data.Student.SelectByID(_StudentID).Name + "」非明細獎勵統計值");
                sb.AppendLine("大功「" + dgvMerit.Rows[1].Cells[1].Value + "」");
                sb.AppendLine("小功「" + dgvMerit.Rows[1].Cells[2].Value + "」");
                sb.AppendLine("嘉獎「" + dgvMerit.Rows[1].Cells[3].Value + "」");

                DSXmlHelper helper = new DSXmlHelper("InitialSummary");
                helper.AddElement("DisciplineStatistics");
                helper.AddElement("DisciplineStatistics", "Merit");
                helper.SetAttribute("DisciplineStatistics/Merit", "A", "" + dgvMerit.Rows[1].Cells[1].Value);
                helper.SetAttribute("DisciplineStatistics/Merit", "B", "" + dgvMerit.Rows[1].Cells[2].Value);
                helper.SetAttribute("DisciplineStatistics/Merit", "C", "" + dgvMerit.Rows[1].Cells[3].Value);
                Jsr.InitialSummary = helper.BaseElement;

                try
                {
                    K12.Data.MoralScore.Insert(Jsr);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("新增,儲存失敗");
                    return;
                } 
                #endregion

                ApplicationLog.Log("學務系統.獎懲學期統計", "修改學生非明細獎勵統計", "student", _StudentID, sb.ToString());
            }


            lbHelp2.Text = "說明：白色欄位為可調整內容";
            lbHelp2.ForeColor = Color.Black;
            FISCA.Presentation.Controls.MsgBox.Show("儲存成功");

        }

        //懲戒儲存(非明細)
        private void btnSaveDemeritStatistics_Click(object sender, EventArgs e)
        {
            if (MSRecord != null)
            {
                SummarySetupObj obj = new SummarySetupObj(MSRecord.InitialSummary);
                obj.DemeritA = ParseInt("" + dgvDemerit.Rows[1].Cells[1].Value);
                obj.DemeritB = ParseInt("" + dgvDemerit.Rows[1].Cells[2].Value);
                obj.DemeritC = ParseInt("" + dgvDemerit.Rows[1].Cells[3].Value);
                MSRecord.InitialSummary = obj.GetAllXmlElement();

                //Log
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("修改學生" + "「" + K12.Data.Student.SelectByID(_StudentID).Name + "」非明細懲戒統計值");
                sb.AppendLine("大過「" + dgvMerit.Rows[1].Cells[1].Value + "」");
                sb.AppendLine("小過「" + dgvMerit.Rows[1].Cells[2].Value + "」");
                sb.AppendLine("警告「" + dgvMerit.Rows[1].Cells[3].Value + "」");

                try
                {
                    K12.Data.MoralScore.Update(MSRecord);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("更新,儲存失敗");
                    return;
                }

                ApplicationLog.Log("學務系統.獎懲學期統計", "修改學生非明細懲戒統計", "student", MSRecord.RefStudentID, sb.ToString());
            }
            else
            {
                #region 資料不存在
                //新增InitialSummary
                K12.Data.MoralScoreRecord Jsr = new K12.Data.MoralScoreRecord();
                Jsr.RefStudentID = _StudentID;
                Jsr.SchoolYear = _SchoolYear;
                Jsr.Semester = _Semester;

                //Log
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("新增學生" + "「" + K12.Data.Student.SelectByID(_StudentID).Name + "」非明細懲戒統計值");
                sb.AppendLine("大過「" + dgvMerit.Rows[1].Cells[1].Value + "」");
                sb.AppendLine("小過「" + dgvMerit.Rows[1].Cells[2].Value + "」");
                sb.AppendLine("警告「" + dgvMerit.Rows[1].Cells[3].Value + "」");

                DSXmlHelper helper = new DSXmlHelper("InitialSummary");
                helper.AddElement("DisciplineStatistics");
                helper.AddElement("DisciplineStatistics", "Demerit");
                helper.SetAttribute("DisciplineStatistics/Demerit", "A", "" + dgvDemerit.Rows[1].Cells[1].Value);
                helper.SetAttribute("DisciplineStatistics/Demerit", "B", "" + dgvDemerit.Rows[1].Cells[2].Value);
                helper.SetAttribute("DisciplineStatistics/Demerit", "C", "" + dgvDemerit.Rows[1].Cells[3].Value);
                Jsr.InitialSummary = helper.BaseElement;

                try
                {
                    K12.Data.MoralScore.Insert(Jsr);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("新增,儲存失敗");
                    return;
                }
                #endregion

                ApplicationLog.Log("學務系統.獎懲學期統計", "修改學生非明細懲戒統計", "student", _StudentID, sb.ToString());
            }

            lbHelp3.Text = "說明：白色欄位為可調整內容";
            lbHelp3.ForeColor = Color.Black;
            FISCA.Presentation.Controls.MsgBox.Show("儲存成功");
        }

        private void dgvMerit_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            btnSaveMeritStatistics.Pulse(20);
            lbHelp2.Text = "您已修改資料,請儲存獎勵統計";
            lbHelp2.ForeColor = Color.Red;
        }


        //private int intTryParse(DataGridViewCell cell)
        //{
        //    int x;
        //    if (int.TryParse("" + cell.Value, out x))
        //    {
        //        return x;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        private void dgvDemerit_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            btnSaveDemeritStatistics.Pulse(20);
            lbHelp3.Text = "您已修改資料,請儲存懲戒統計";
            lbHelp3.ForeColor = Color.Red;
        }

        private void dgvMerit_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvMerit.EndEdit();

            ColumnAddInUnify.SetAddCell(dgvMerit, false);

            dgvMerit.BeginEdit(false);
        }

        private void dgvDemerit_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvDemerit.EndEdit();

            ColumnAddInUnify.SetAddCell(dgvDemerit, false);

            dgvDemerit.BeginEdit(false);
        }

        private void listViewMerit_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewMerit.SelectedItems.Count == 1)
            {
                K12.Data.MeritRecord record = (K12.Data.MeritRecord)this.listViewMerit.SelectedItems[0].Tag;
                MeritEditForm editForm = new MeritEditForm(record, _UserPermission); //此編輯表單在修改模式下，一次只能對一位學生的某一筆懲戒記錄進行修改，所以 Constructor 就傳入一個 Editor 物件。
                editForm.ShowDialog();
            }
        }

        private void listViewDemerit_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewDemerit.SelectedItems.Count == 1)
            {
                K12.Data.DemeritRecord record = (K12.Data.DemeritRecord)this.listViewDemerit.SelectedItems[0].Tag;
                DemeritEditForm editForm = new DemeritEditForm(record, _UserPermission); //此編輯表單在修改模式下，一次只能對一位學生的某一筆懲戒記錄進行修改，所以 Constructor 就傳入一個 Editor 物件。
                editForm.ShowDialog();
            }
        }
    }
}
