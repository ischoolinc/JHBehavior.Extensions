using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Data;
using System.Xml;
using Framework.Security;
using Framework;
using JHSchool.Behavior.BusinessLogic;
using FCode = Framework.Security.FeatureCodeAttribute;

namespace JHSchool.Behavior.MeritAndDemerit
{
    [FCode("JHSchool.Student.Detail0035", "獎懲學期統計")]
    public partial class DisciplineUnifytItem : DetailContentBase
    {
        BackgroundWorker BGW = new BackgroundWorker();
        bool BkWBool = false;
        List<AutoSummaryRecord> AutoSummaryList = new List<AutoSummaryRecord>();

        internal static FeatureAce UserPermission;

        //建構子
        public DisciplineUnifytItem()
        {
            InitializeComponent();

            BGW.DoWork += new DoWorkEventHandler(BkW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BkW_RunWorkerCompleted);

            //獎勵更新時(統計更新)
            JHMerit.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHMerit_AfterDelete);
            JHMerit.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(JHMerit_AfterDelete);
            JHMerit.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHMerit_AfterDelete);

            //懲戒更新時(統計更新)
            JHDemerit.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHMerit_AfterDelete);
            JHDemerit.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(JHMerit_AfterDelete);
            JHDemerit.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHMerit_AfterDelete);

            //轉入補登更新
            JHMoralScore.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHMerit_AfterDelete);
            JHMoralScore.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(JHMerit_AfterDelete);
            JHMoralScore.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHMerit_AfterDelete);

            Group = "獎懲學期統計";

            UserPermission = User.Acl[FCode.GetCode(GetType())];
            if (!UserPermission.Editable)
            {
                this.listView.MouseDoubleClick -= new System.Windows.Forms.MouseEventHandler(this.listView_MouseDoubleClick);
                btnEdit.Enabled = UserPermission.Editable;
                btnNewItem.Enabled = UserPermission.Editable;
            }

        }

        //事件
        void JHMerit_AfterDelete(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(JHMerit_AfterDelete), sender, e);
            }
            else
            {
                if (this.PrimaryKey != "")
                {
                    this.Loading = true;

                    if (BGW.IsBusy)
                    {
                        BkWBool = true;
                    }
                    else
                    {
                        BGW.RunWorkerAsync();
                    }
                }
            }
        }

        //切換學生
        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            if (this.PrimaryKey != "")
            {
                this.Loading = true;

                if (BGW.IsBusy)
                {
                    BkWBool = true;
                }
                else
                {
                    BGW.RunWorkerAsync();
                }
            }
        }

        //(舊)更新畫面
        void Instance_ItemUpdated(object sender, ItemUpdatedEventArgs e)
        {
            if (this.PrimaryKey != "")
            {
                this.Loading = true;

                if (BGW.IsBusy)
                {
                    BkWBool = true;
                }
                else
                {
                    BGW.RunWorkerAsync();
                }
            }
        }

        /// <summary>
        /// 背景模式取得資料
        /// </summary>
        void BkW_DoWork(object sender, DoWorkEventArgs e)
        {
            AutoSummaryList.Clear();

            AutoSummaryList = AutoSummary.Select(new string[] { this.PrimaryKey }, null);

            #region 註解掉
            //List<SchoolYearSemester> SchoolSemesterList = new List<SchoolYearSemester>();

            //foreach (JHMeritRecord each in JHMerit.SelectByStudentIDs(new string[] { this.PrimaryKey }))
            //{
            //    bool IsTrue = true;

            //    foreach (SchoolYearSemester school in SchoolSemesterList)
            //    {
            //        if (school.SchoolYear == each.SchoolYear && school.Semester == each.Semester)
            //        {
            //            IsTrue = false; //如果有重覆的
            //            break;
            //        }
            //    }

            //    if (IsTrue)
            //    {
            //        SchoolYearSemester SchoolSemester = new SchoolYearSemester(each.SchoolYear, each.Semester);
            //        SchoolSemesterList.Add(SchoolSemester);
            //    }
            //}

            //foreach (JHDemeritRecord each in JHDemerit.SelectByStudentIDs(new string[] { this.PrimaryKey }))
            //{
            //    bool IsTrue = true;

            //    foreach (SchoolYearSemester school in SchoolSemesterList)
            //    {
            //        if (school.SchoolYear == each.SchoolYear && school.Semester == each.Semester)
            //        {
            //            IsTrue = false; //如果有重覆的
            //            break;
            //        }
            //    }

            //    if (IsTrue)
            //    {
            //        SchoolYearSemester SchoolSemester = new SchoolYearSemester(each.SchoolYear, each.Semester);
            //        SchoolSemesterList.Add(SchoolSemester);
            //    }
            //}

            //foreach (JHMoralScoreRecord each in JHMoralScore.SelectByStudentIDs(new string[] { this.PrimaryKey }))
            //{
            //    bool IsTrue = true;

            //    foreach (SchoolYearSemester school in SchoolSemesterList)
            //    {
            //        if (school.SchoolYear == each.SchoolYear && school.Semester == each.Semester)
            //        {
            //            IsTrue = false; //如果有重覆的
            //            break;
            //        }
            //    }

            //    if (IsTrue)
            //    {
            //        SchoolYearSemester SchoolSemester = new SchoolYearSemester(each.SchoolYear, each.Semester);
            //        SchoolSemesterList.Add(SchoolSemester);
            //    }
            //} 
            #endregion
        }

        /// <summary>
        /// 背景模式完成
        /// </summary>
        void BkW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (BkWBool)
            {
                BkWBool = false;
                BGW.RunWorkerAsync();
                return;
            }

            BindData();

            this.Loading = false;
        }

        /// <summary>
        /// 更新畫面資料
        /// </summary>
        private void BindData()
        {
            if (!this.SaveButtonVisible && !this.CancelButtonVisible && this.PrimaryKey.Contains(PrimaryKey))
            {
                this.listView.Items.Clear();

                AutoSummaryList.Sort(new Comparison<AutoSummaryRecord>(SortAutoSummary));

                foreach (AutoSummaryRecord each in AutoSummaryList)
                {
                    // 2018/05/11 羿均註解下列程式碼，
                    //if (each.DemeritA + each.DemeritB + each.DemeritC + each.MeritA + each.MeritB + each.MeritC == 0) continue;
                    
                    ListViewItem itms = new ListViewItem(each.SchoolYear.ToString());
                    itms.SubItems.Add(each.Semester.ToString());

                    itms.SubItems.Add(each.MeritA == 0 ? "" : each.MeritA.ToString());
                    itms.SubItems.Add(each.MeritB == 0 ? "" : each.MeritB.ToString());
                    itms.SubItems.Add(each.MeritC == 0 ? "" : each.MeritC.ToString());
                    itms.SubItems.Add(each.DemeritA == 0 ? "" : each.DemeritA.ToString());
                    itms.SubItems.Add(each.DemeritB == 0 ? "" : each.DemeritB.ToString());
                    itms.SubItems.Add(each.DemeritC == 0 ? "" : each.DemeritC.ToString());
                    itms.Tag = each;

                    listView.Items.Add(itms);
                }
            }
        }

        //學期判斷
        private int SortAutoSummary(AutoSummaryRecord x, AutoSummaryRecord y)
        {
            string SchoolYearSemester1 = x.SchoolYear.ToString().PadLeft(3, '0') + x.Semester.ToString().PadLeft(3, '0');
            string SchoolYearSemester2 = y.SchoolYear.ToString().PadLeft(3, '0') + y.Semester.ToString().PadLeft(3, '0');

            return SchoolYearSemester1.CompareTo(SchoolYearSemester2);
        }

        //開啟編輯畫面
        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                AutoSummaryRecord auto = (AutoSummaryRecord)listView.SelectedItems[0].Tag;

                DisciplineUnifytForm DemeritForm = new DisciplineUnifytForm(auto.RefStudentID, auto.SchoolYear, auto.Semester, UserPermission);
                DemeritForm.ShowDialog();
                if (!BGW.IsBusy)
                {
                    BGW.RunWorkerAsync();
                }
            }
        }

        //開啟編輯畫面
        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                AutoSummaryRecord auto = (AutoSummaryRecord)listView.SelectedItems[0].Tag;

                DisciplineUnifytForm DemeritForm = new DisciplineUnifytForm(auto.RefStudentID, auto.SchoolYear, auto.Semester, UserPermission);
                DemeritForm.ShowDialog();
                if (!BGW.IsBusy)
                {
                    BGW.RunWorkerAsync();
                }
            }
        }

        //新增
        private void buttonX1_Click_1(object sender, EventArgs e)
        {
            SelectSchoolYearSemester fm = new SelectSchoolYearSemester(this.PrimaryKey, UserPermission);
            fm.ShowDialog();

            if (!BGW.IsBusy)
            {
                BGW.RunWorkerAsync();
            }
        }
    }
}
