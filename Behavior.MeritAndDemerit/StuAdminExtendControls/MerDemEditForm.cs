using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using SmartSchool.Common;
//using SmartSchool.Customization.Data;
//using SmartSchool.Customization.Data.StudentExtension;
using FISCA.DSAUtil;
using System.Xml;
using SmartSchool.Common;
using Framework;
using FISCA.LogAgent;
using K12.Data;
//using SmartSchool;

namespace JHSchool.Behavior.MeritAndDemerit
{
    public partial class MerDemEditForm : FISCA.Presentation.Controls.BaseForm
    {
        //private DateTime _startDate;
        //private DateTime _endDate;
        //List<string> MeritRowIdList = new List<string>();
        //List<string> DemeritRowIdList = new List<string>();
        //Dictionary<string, List<StudentRecord>> classList = new Dictionary<string, List<StudentRecord>>(); //班座條件使用
        //string SeachStud; //班座條件使用
        //bool TrueStudent = false; //班座條件使用
        //private List<string> students = new List<string>(); //待處理使用
        //private List<string> studentsList = new List<string>(); //待處理使用

        Dictionary<string, List<StRecord>> classList; //班級名稱,學生List
        List<K12.Data.ClassRecord> Classes; //全校班級
        List<StRecord> Students; //全校學生

        List<K12.Data.DisciplineRecord> DisciplineList = new List<K12.Data.DisciplineRecord>();

        Dictionary<string, StRecord> studDic;
        List<string> DelRowIdList = new List<string>();

        List<K12.Data.DisciplineRecord> DelRowRecordList = new List<K12.Data.DisciplineRecord>();

        int ConBoxIndex = 0;

        private string _startDate;
        private string _endDate;

        private string _txtReason;

        List<string> DicDisciplineStudList = new List<string>(); //列印清單

        private bool Waiting
        {
            set { picWaiting.Visible = value; }
        }

        private BackgroundWorker _loader;

        public MerDemEditForm()
        {
            InitializeComponent();
        }

        private void MerDemEditForm_Load(object sender, EventArgs e)
        {
            InitialBackgroundWorker(); //註冊BackgroundWorker事件

            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += delegate
            {
                ClassRef(); //更新班級資料
            };

            bg.RunWorkerCompleted += delegate
            {
                ControlEnabled = true;
                this.Text = "獎懲批次修改";

                InitialDate(); //初始化日期
                //btnRefresh_Click(null, null); //重新整理 

                cbRange.SelectedIndex = 4;
                comboBoxEx1.SelectedIndex = 0;

                btnRefresh.Pulse(5);
            };
            bg.RunWorkerAsync();
            ControlEnabled = false;
            this.Text = "資料讀取中";
        }

        /// <summary>
        /// 鎖定畫面
        /// </summary>
        private bool ControlEnabled
        {
            set
            {
                btnRefresh.Enabled = value;
                groupPanel3.Enabled = value;
                groupPanel2.Enabled = value;
                //groupPanel1.Enabled = value;
                dataGridViewX1.Enabled = value;
                btnModify.Enabled = value;
                btnExport.Enabled = value;
                btnAdd.Enabled = value;
                btnClear.Enabled = value;
            }
        }

        public void ClassRef()
        {
            #region 建立班級資訊

            classList = new Dictionary<string, List<StRecord>>();
            Classes = K12.Data.Class.SelectAll();
            Students = GetStudent();

            Students.Sort(new Comparison<StRecord>(SortComparerInStudent));

            foreach (StRecord eachstudent in Students)
            {
                if (!string.IsNullOrEmpty(eachstudent.RefClassID))
                {
                    if (!classList.ContainsKey(eachstudent.ClassName))
                        classList.Add(eachstudent.ClassName, new List<StRecord>());

                    classList[eachstudent.ClassName].Add(eachstudent);
                }
            }
            #endregion
        }

        private List<StRecord> GetStudent()
        {
            studDic = new Dictionary<string, StRecord>();
            List<StRecord> list = new List<StRecord>();
            DataTable dt = tool._Q.Select(@"select student.id as student_id,student.name as student_name,
student.student_number,student.gender,student.seat_no,
class.id as class_id,class.class_name
from student 
left join class on class.id=student.ref_class_id");
            foreach (DataRow row in dt.Rows)
            {
                StRecord st = new StRecord(row);
                list.Add(st);

                if (!studDic.ContainsKey(st.StudentID))
                {
                    studDic.Add(st.StudentID, st);
                }
            }

            list.Sort(new Comparison<StRecord>(SortComparerInStudent));
            return list;
        }

        private void InitialBackgroundWorker()
        {
            #region 註冊BackgroundWorker事件
            _loader = new BackgroundWorker();
            _loader.DoWork += new DoWorkEventHandler(_loader_DoWork);
            _loader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_loader_RunWorkerCompleted);
            #endregion
        }

        private void InitialDate()
        {
            #region 初始化日期(預設區為隔七天)
            dateTimeInput1.Text = DateTime.Today.AddDays(-7).ToShortDateString();
            dateTimeInput2.Text = DateTime.Today.ToShortDateString();
            #endregion
        }

        private void cbRange_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            #region 範圍條件變更
            if (cbRange.SelectedIndex == 0) //班座
            {
                ChengClass(); //建立班級內容

                cbClass.Visible = true; //顯示班級控制
                lbClass.Text = "班級"; //更改班級lb
                lbClass.Visible = true; //顯示班級lb

                lbSeatNo.Visible = true; //顯示座號文字
                txtSeatNo.Visible = true; //顯示控制
                txtSeatNo.Text = "";

                txtClass.Visible = false;
                txtClass.Text = "";

                cbClass.SelectedIndex = 0;
            }
            else if (cbRange.SelectedIndex == 1) //學號
            {
                cbClass.Visible = false;
                cbClass.Items.Clear();

                txtClass.Visible = true;
                txtClass.Text = "";

                lbClass.Text = "學號";
                lbClass.Visible = true;

                lbSeatNo.Visible = false;
                txtSeatNo.Visible = false;
                txtSeatNo.Text = "";
            }
            else if (cbRange.SelectedIndex == 2) //班級
            {
                ChengClass(); //建立班級內容

                txtClass.Visible = false;
                txtClass.Text = "";

                cbClass.Visible = true; //顯示班級控制
                lbClass.Text = "班級"; //更改班級lb
                lbClass.Visible = true; //顯示班級lb

                lbSeatNo.Visible = false; //隱藏座號lb

                txtSeatNo.Visible = false; //隱藏座號控制
                txtSeatNo.Text = ""; //清空座號內容

                cbClass.SelectedIndex = 0;

            }
            else if (cbRange.SelectedIndex == 3) //年級
            {
                ChengGr();

                txtClass.Visible = false;
                txtClass.Text = "";

                cbClass.Visible = true; //顯示班級控制

                lbClass.Text = "年級"; //更改班級lb
                lbClass.Visible = true; //顯示班級lb

                lbSeatNo.Visible = false; //隱藏座號lb

                txtSeatNo.Visible = false; //隱藏座號控制
                txtSeatNo.Text = ""; //清空座號內容

                cbClass.SelectedIndex = 0;
            }
            else if (cbRange.SelectedIndex == 4) //全校
            {
                txtClass.Visible = false;
                txtClass.Text = "";

                cbClass.Visible = false; //隱藏班級控制
                cbClass.Items.Clear(); //清空班級內容

                lbClass.Visible = false; //隱藏班級lb


                lbSeatNo.Visible = false; //隱藏座號lb
                txtSeatNo.Visible = false; //隱藏座號控制
                txtSeatNo.Text = ""; //清空座號內容

                Students = GetStudent();

            }
            #endregion
        }

        private void ChengGr()
        {
            #region 年級資料處理

            cbClass.Items.Clear(); //清空下拉式選單
            cbClass.DisplayMember = "Key";
            Classes.Sort(new Comparison<K12.Data.ClassRecord>(SortComparer)); //排序

            Dictionary<string, List<K12.Data.ClassRecord>> dic = new Dictionary<string, List<K12.Data.ClassRecord>>();
            List<string> dicSort = new List<string>();

            foreach (K12.Data.ClassRecord DCA in Classes)
            {
                if (DCA.GradeYear == null)
                    continue;

                if (!dic.ContainsKey(DCA.GradeYear.ToString()))
                {
                    dic.Add(DCA.GradeYear.ToString(), new List<K12.Data.ClassRecord>());
                    dicSort.Add(DCA.GradeYear.ToString());
                }

                dic[DCA.GradeYear.ToString()].Add(DCA);
            }

            dicSort.Sort();

            foreach (string each in dicSort)
            {
                KeyValuePair<string, List<K12.Data.ClassRecord>> KKBOX = new KeyValuePair<string, List<K12.Data.ClassRecord>>(each, dic[each]);
                cbClass.Items.Add(KKBOX);
            }

            #endregion
        }

        private void ChengClass()
        {
            #region 班級資料處理

            cbClass.Items.Clear(); //清空下拉式選單
            cbClass.DisplayMember = "Key";
            Classes.Sort(new Comparison<K12.Data.ClassRecord>(SortComparer)); //排序

            foreach (K12.Data.ClassRecord DCA in Classes)
            {
                if (DCA.Name == string.Empty)
                    continue;
                KeyValuePair<string, K12.Data.ClassRecord> KKBOX = new KeyValuePair<string, K12.Data.ClassRecord>(DCA.Name, DCA);
                cbClass.Items.Add(KKBOX);
            }
            #endregion
        }

        private int SortComparer(K12.Data.ClassRecord x, K12.Data.ClassRecord y)
        {
            string xx = x.Name;
            string yy = y.Name;
            return xx.CompareTo(yy);
        }

        private int SortComparerInStudent(StRecord x, StRecord y)
        {
            #region 排序學生
            if (x.RefClassID == "" || y.RefClassID == "")
            {
                string xx = "" + x.StudentNumber;
                string yy = "" + y.StudentNumber;
                return xx.CompareTo(yy);
            }
            else
            {
                string xx = "" + x.ClassName + x.SeatNo;
                string yy = "" + y.ClassName + y.SeatNo;
                return xx.CompareTo(yy);
            }
            #endregion
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            #region 更新資料

            _startDate = dateTimeInput1.Text;
            _endDate = dateTimeInput2.Text;
            _txtReason = txtReason.Text.Trim();
            btnRefresh.Enabled = false; //關掉開關
            DelRowRecordList.Clear();

            if (cbRange.SelectedIndex == 0)
            {
                #region 班級座號
                KeyValuePair<string, K12.Data.ClassRecord> item = (KeyValuePair<string, K12.Data.ClassRecord>)cbClass.SelectedItem;
                K12.Data.ClassRecord classR = item.Value;
                StRecord SeleteStud = null;

                if (classList.ContainsKey(classR.Name))
                {
                    List<StRecord> stulist = classList[classR.Name];

                    foreach (StRecord stud in stulist)
                    {
                        if (stud.SeatNo == null)
                        {
                            continue;
                        }

                        if ("" + stud.SeatNo == txtSeatNo.Text)
                        {
                            SeleteStud = stud;
                            break;
                        }
                    }
                }

                if (SeleteStud == null)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("查無此座號,請重新輸入");
                    btnRefresh.Enabled = true;
                    Waiting = false;
                    txtSeatNo.SelectAll();
                    return;
                }
                else
                {
                    List<StRecord> NowList = new List<StRecord>();
                    NowList.Add(SeleteStud);
                    _loader.RunWorkerAsync(NowList);
                }
                #endregion
            }
            else if (cbRange.SelectedIndex == 1)
            {
                #region 學號
                StRecord SeleteStud = null;

                foreach (StRecord stud in Students)
                {
                    if (stud.StudentNumber.ToLower() == txtClass.Text.Trim().ToLower())
                    {
                        SeleteStud = stud;
                        break;
                    }
                }

                if (SeleteStud == null)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("查無此學號,請重新輸入");
                    btnRefresh.Enabled = true;
                    Waiting = false;
                    txtClass.SelectAll();
                }
                else
                {
                    List<StRecord> NowList = new List<StRecord>();
                    NowList.Add(SeleteStud);
                    _loader.RunWorkerAsync(NowList);
                }
                #endregion
            }
            else if (cbRange.SelectedIndex == 2)
            {
                #region 班級
                KeyValuePair<string, K12.Data.ClassRecord> item = (KeyValuePair<string, K12.Data.ClassRecord>)cbClass.SelectedItem;

                if (item.Value.Students.Count == 0)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("本班級並無學生");
                    btnRefresh.Enabled = true;
                    Waiting = false;
                    return;
                }

                if (classList.ContainsKey(item.Value.Name))
                {
                    List<StRecord> stud = classList[item.Value.Name];
                    _loader.RunWorkerAsync(stud);
                }
                #endregion
            }
            else if (cbRange.SelectedIndex == 3)
            {
                #region 年級
                KeyValuePair<string, List<K12.Data.ClassRecord>> item = (KeyValuePair<string, List<K12.Data.ClassRecord>>)cbClass.SelectedItem;
                List<K12.Data.ClassRecord> classRList = item.Value;
                List<StRecord> stud = new List<StRecord>();
                //
                //ClassRecord classR = item.Value;
                //StRecord SeleteStud = null;

                foreach (K12.Data.ClassRecord cr in classRList)
                {
                    if (classList.ContainsKey(cr.Name))
                    {
                        stud.AddRange(classList[cr.Name]);
                    }
                }

                _loader.RunWorkerAsync(stud);
                #endregion
            }
            else if (cbRange.SelectedIndex == 4) //全校
            {
                _loader.RunWorkerAsync(Students);
            }
            #endregion

            //查詢不Log
            //ApplicationLog.Log("獎懲批次修改", "查詢", "獎懲批次修改，已使用查詢功能。");
        }

        private void _loader_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 開始執行背景作業

            List<StRecord> studList = (List<StRecord>)e.Argument;

            List<string> list = new List<string>();

            foreach (StRecord each in studList)
            {
                if (!list.Contains(each.StudentID))
                {
                    list.Add(each.StudentID);
                }
            }

            DisciplineList.Clear();

            try
            {
                if (ConBoxIndex == 0) //缺曠日期
                {
                    foreach (DisciplineRecord each in Discipline.SelectByOccurDate(list, dateTimeInput1.Value, dateTimeInput2.Value))
                    {
                        if (each.Reason.ToLower().Contains(_txtReason.ToLower()) || each.Remark.ToLower().Contains(_txtReason.ToLower()))
                        {
                            DisciplineList.Add(each);
                        }
                    }
                    //DisciplineList = JHDiscipline.SelectByOccurDate(list, dateTimeInput1.Value, dateTimeInput2.Value);
                }
                else if (ConBoxIndex == 1) //登錄日期
                {
                    foreach (DisciplineRecord each in Discipline.SelectByRegisterDate(list, dateTimeInput1.Value, dateTimeInput2.Value))
                    {
                        if (each.Reason.ToLower().Contains(_txtReason.ToLower()) || each.Remark.ToLower().Contains(_txtReason.ToLower()))
                        {
                            DisciplineList.Add(each);
                        }
                    }
                    //DisciplineList = JHDiscipline.SelectByRegisterDate(list, dateTimeInput1.Value, dateTimeInput2.Value);
                }

            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("取得獎懲失敗。" + ex);
                btnRefresh.Enabled = true;
                Waiting = false; //旋轉停止
            }

            DisciplineList.Sort(SortByClassAndSeatNo); //排序
            
            #endregion
        }
        
        private void _loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region 背景作業完成
            btnRefresh.Enabled = true; //按鈕打開
            Waiting = false; //旋轉停止

            if (e.Error == null)
            {
                if (DisciplineList.Count != 0)
                {
                    FillDataGridView();
                }
                else
                {
                    dataGridViewX1.Rows.Clear();
                    FISCA.Presentation.Controls.MsgBox.Show("查無獎懲資料!");
                    return;
                }
            }
            #endregion
        }

        private bool CheckMerit(DisciplineRecord JH, List<string> li)
        {
            #region 處理資料是否顯示
            List<string> st = new List<string>();
            st.Clear();

            if (JH.MeritFlag == "1")
            {
                if (JH.MeritA > 0)
                {
                    st.Add("大功");
                }
                if (JH.MeritB > 0)
                {
                    st.Add("小功");
                }
                if (JH.MeritC > 0)
                {
                    st.Add("嘉獎");
                }
            }
            else if (JH.MeritFlag == "0")
            {
                if (JH.DemeritA > 0)
                {
                    st.Add("大過");
                }
                if (JH.DemeritB > 0)
                {
                    st.Add("小過");
                }
                if (JH.DemeritC > 0)
                {
                    st.Add("警告");
                }
            }

            foreach (string each in st)
            {
                if (li.Contains(each))
                {
                    return false; //傳出false就是包含內容
                }
            }
            return true;
            #endregion
        }

        private void FillDataGridView()
        {
            #region 更新畫面資料
            dataGridViewX1.Rows.Clear();
            dataGridViewX1.SuspendLayout();

            List<string> jone = new List<string>();
            foreach (CheckBox each in groupPanel1.Controls)
            {
                if (each.Checked)
                {
                    jone.Add(each.Text);
                }
            }

            List<string> StudentCount = new List<string>();

            foreach (DisciplineRecord eachDis in DisciplineList)
            {

                //取得學生
                if (studDic.ContainsKey(eachDis.RefStudentID))
                {
                    StRecord SR = studDic[eachDis.RefStudentID];


                    string discipline = GetDisciplineString(eachDis);

                    if (discipline == "")
                        continue;

                    if (eachDis.MeritFlag == "0") //是懲戒
                    {
                        if (eachDis.Cleared == "是") //已消過就離開
                        {
                            continue;
                        }
                    }

                    if (CheckMerit(eachDis, jone)) //如果不是CheckBox所勾選內容
                        continue;

                    if (!StudentCount.Contains(eachDis.RefStudentID))
                    {
                        StudentCount.Add(eachDis.RefStudentID); //學生人數統計
                    }

                    DataGridViewRow _row = new DataGridViewRow();
                    _row.CreateCells(dataGridViewX1);

                    _row.Cells[0].Value = eachDis.ID; //獎懲編號
                    _row.Cells[1].Value = eachDis.RefStudentID; //系統編號
                    _row.Cells[2].Value = eachDis.OccurDate.ToShortDateString(); //獎懲日期
                    if (SR.ClassName != "")
                    {
                        _row.Cells[3].Value = SR.ClassName; //班級
                    }
                    _row.Cells[4].Value = SR.SeatNo; //座號
                    _row.Cells[5].Value = SR.StudentNumber; //學號
                    _row.Cells[6].Value = SR.StudentName; //姓名
                    _row.Cells[7].Value = SR.Gender; //性別
                    _row.Cells[8].Value = discipline; //獎懲次數
                    _row.Cells[9].Value = eachDis.Reason; //事由
                    _row.Cells[10].Value = eachDis.Remark; //備註 2019/12/24 新增
                    _row.Cells[11].Value = eachDis.SchoolYear; //學年度
                    _row.Cells[12].Value = eachDis.Semester; //學期
                    if (eachDis.RegisterDate.HasValue)
                    {
                        _row.Cells[13].Value = eachDis.RegisterDate.Value.ToShortDateString();
                    }
                    _row.Cells[14].Value = eachDis.MeritFlag; //獎懲區分

                    _row.Tag = eachDis;

                    dataGridViewX1.Rows.Add(_row);
                }

                txtHelpStudentCount.Text = "學生人數：" + StudentCount.Count;

                dataGridViewX1.ResumeLayout();

                if (dataGridViewX1.Rows.Count > 0)
                    dataGridViewX1.Rows[0].Selected = false;
                #endregion
            }
        }

        private string GetDisciplineString(DisciplineRecord JHDRecord)
        {
            #region 獎懲判斷
            string result = "";
            if (JHDRecord.MeritFlag == "1")
            {
                if (JHDRecord.MeritA > 0)
                {
                    result += string.Format("大功:{0}", JHDRecord.MeritA);
                }

                if (JHDRecord.MeritB > 0)
                {
                    result += string.Format("小功:{0}", JHDRecord.MeritB);
                }

                if (JHDRecord.MeritC > 0)
                {
                    result += string.Format("嘉獎:{0}", JHDRecord.MeritC);
                }
            }
            else if (JHDRecord.MeritFlag == "0")
            {
                if (JHDRecord.Cleared == "是")
                    return "";

                if (JHDRecord.DemeritA > 0)
                {
                    result += string.Format("大過:{0}", JHDRecord.DemeritA);
                }

                if (JHDRecord.DemeritB > 0)
                {
                    result += string.Format("小過:{0}", JHDRecord.DemeritB);
                }

                if (JHDRecord.DemeritC > 0)
                {
                    result += string.Format("警告:{0}", JHDRecord.DemeritC);
                }
            }
            else if (JHDRecord.MeritFlag == "2")
            {
                result = ""; //暫時無留校察看
                //result = "留校察看";
            }

            return result;
            
            #endregion
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            #region 修改獎懲
            if (dataGridViewX1.SelectedRows.Count <= 0) return;

            #region 檢查選取獎懲的類型是否一致 (MeritFlag一致)

            DataGridViewRow firstRow = dataGridViewX1.SelectedRows[dataGridViewX1.SelectedRows.Count - 1];
            string firstFlag = "" + firstRow.Cells[colMeritFlag.Index].Value;
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                if (row == firstRow) continue;
                if ("" + row.Cells[colMeritFlag.Index].Value != firstFlag)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("請選取相同獎懲類別之獎懲記錄。", "錯誤");
                    return;
                }
            }

            #endregion

            #region 檢查選取的獎懲

            List<K12.Data.DisciplineRecord> helper = new List<K12.Data.DisciplineRecord>();
            helper.Clear();
            foreach (DataGridViewRow each in dataGridViewX1.SelectedRows)
            {
                helper.Add((K12.Data.DisciplineRecord)each.Tag);
            }

            bool warning = false;

            string firstReason = "" + firstRow.Cells[colReason.Index].Value;
            string firstDiscipline = GetDisciplineString(helper[0]);
            List<string> listText = new List<string>();

            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                listText.Add("" + row.Cells[colID.Index].Value);
                if (row == firstRow) continue;
                if ("" + row.Cells[colReason.Index].Value != firstReason || "" + row.Cells[colDisciplineCount.Index].Value != firstDiscipline)
                    warning = true;
            }

            if (warning)
                FISCA.Presentation.Controls.MsgBox.Show("您所選擇的獎懲記錄中包含不同獎懲次數或事由，\n系統將會以[最後選取]為預設內容。", "警告");

            #endregion

            #region 檢查選取之學年度學期是否一致

            string checkSchoolYear = "";
            string checkSemester = "";
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                if (row.IsNewRow)
                    continue;

                if (checkSchoolYear == "" && checkSemester == "")
                {
                    checkSchoolYear = "" + row.Cells[colSchoolYear.Index].Value;
                    checkSemester = "" + row.Cells[colSemester.Index].Value;
                    continue;
                }
                if (checkSchoolYear != "" + row.Cells[colSchoolYear.Index].Value
                    || checkSemester != "" + row.Cells[colSemester.Index].Value)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("您所選擇的獎懲記錄,學年度學期並不一致\n無法進行批次修改!!", "錯誤");
                    return;
                }
            }
            #endregion

            ModifyForm form = new ModifyForm(helper);
            if (form.ShowDialog() == DialogResult.OK)
            {
                btnRefresh_Click(null, null); //更新畫面資料
            }
            #endregion
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            #region 匯出
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            DataGridViewExport export = new DataGridViewExport(dataGridViewX1);
            export.Save(saveFileDialog1.FileName);
            ApplicationLog.Log("獎懲批次修改", "匯出", "已將獎懲查詢內容匯出。");

            if (new CompleteForm().ShowDialog() == DialogResult.Yes)
                System.Diagnostics.Process.Start(saveFileDialog1.FileName);
            #endregion
        }

        private void dataGridViewX1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnModify_Click(null, null);
        }

        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {
            #region 更新右鍵刪除功能所選擇的清單List
            DelRowIdList.Clear();
            DelRowRecordList.Clear();

            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                if (row.Tag != null)
                {
                    DelRowIdList.Add("" + row.Cells[0].Value);
                    DelRowRecordList.Add((K12.Data.DisciplineRecord)row.Tag);
                }
            }
            #endregion
        }

        private void comboBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConBoxIndex = comboBoxEx1.SelectedIndex;
        }

        #region 待處理功能
        private void btnAdd_Click(object sender, EventArgs e)
        {
            List<string> TemporalStud = GetSelectedStudentList();

            Student.Instance.AddToTemporal(TemporalStud);

            FISCA.Presentation.Controls.MsgBox.Show("新增 " + TemporalStud.Count + " 名學生於待處理");
            labelX3.Text = "待處理共 " + Student.Instance.TemporaList.Count + " 名學生";
            labelX3.Visible = true;
            btnClear.Visible = true;
        }

        private List<string> GetSelectedStudentList()
        {
            List<string> _temporallist = new List<string>();
            foreach (DataGridViewRow var in dataGridViewX1.SelectedRows)
            {
                if (!_temporallist.Contains("" + var.Cells[colStudentID.Index].Value))
                {
                    _temporallist.Add("" + var.Cells[colStudentID.Index].Value);
                }
            }
            return _temporallist;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Student.Instance.RemoveFromTemporal(Student.Instance.TemporalKeys);
            FISCA.Presentation.Controls.MsgBox.Show("已清除 待處理 所有學生");
            labelX3.Visible = false;
            btnClear.Visible = false;
        }
        #endregion

        private void 修改獎懲ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnModify_Click(null, null);
        }

        private void 加入待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAdd_Click(null, null);
        }

        private void 刪除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region 右鍵刪除功能
            DialogResult DR = new DialogResult();
            DR = FISCA.Presentation.Controls.MsgBox.Show("是否刪除選擇之獎懲內容?\n共" + DelRowIdList.Count + " 筆", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
            if (DR == DialogResult.Yes)
            {
                StringBuilder sbDEL = new StringBuilder();
                sbDEL.AppendLine("已進行批次刪除獎懲資料");
                sbDEL.AppendLine("刪除資料如下：");
                foreach (K12.Data.DisciplineRecord each in DelRowRecordList)
                {
                    if (each.MeritFlag == "1")
                    {
                        sbDEL.Append("獎勵日期「" + each.OccurDate.ToShortDateString() + "」");
                        sbDEL.Append("學生「" + each.Student.Name + "」");
                        sbDEL.Append("班級「" + (each.Student.Class != null ? each.Student.Class.Name : "") + "」");
                        sbDEL.AppendLine("座號「" + (each.Student.SeatNo.HasValue ? each.Student.SeatNo.Value.ToString() : "") + "」");
                    }
                    else if (each.MeritFlag == "0")
                    {
                        sbDEL.Append("懲戒日期「" + each.OccurDate.ToShortDateString() + "」");
                        sbDEL.Append("學生「" + each.Student.Name + "」");
                        sbDEL.Append("班級「" + (each.Student.Class != null ? each.Student.Class.Name : "") + "」");
                        sbDEL.AppendLine("座號「" + (each.Student.SeatNo.HasValue ? each.Student.SeatNo.Value.ToString() : "") + "」");
                    }
                    else if (each.MeritFlag == "2")
                    {
                        sbDEL.Append("留查日期「" + each.OccurDate.ToShortDateString() + "」");
                        sbDEL.Append("學生「" + each.Student.Name + "」");
                        sbDEL.Append("班級「" + (each.Student.Class != null ? each.Student.Class.Name : "") + "」");
                        sbDEL.AppendLine("座號「" + (each.Student.SeatNo.HasValue ? each.Student.SeatNo.Value.ToString() : "") + "」");
                    }
                }
                try
                {
                    K12.Data.Discipline.Delete(DelRowIdList);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("刪除發生錯誤");
                    return;
                }

                ApplicationLog.Log("獎懲批次修改", "刪除", sbDEL.ToString() + "\n已將「" + DelRowIdList.Count + "」筆獎懲資料刪除。");
                FISCA.Presentation.Controls.MsgBox.Show("刪除成功!!");
                btnRefresh_Click(null, null);

                //if (!_loader.IsBusy) //重置相關資料?
                //{
                //    Merit.Instance.SyncAllBackground();
                //    Demerit.Instance.SyncAllBackground();
                //}
            }
            #endregion
        }

        //排序
        private int SortByClassAndSeatNo(DisciplineRecord attendX, DisciplineRecord attendy)
        {
            K12.Data.StudentRecord x = attendX.Student;
            K12.Data.StudentRecord y = attendy.Student;
            string 班級名稱1 = (x.Class == null ? "" : x.Class.Name) + "::";
            string 座號1 = (x.SeatNo.HasValue ? x.SeatNo.Value.ToString().PadLeft(2, '0') : "") + "::";
            string 班級名稱2 = (y.Class == null ? "" : y.Class.Name) + "::";
            string 座號2 = (y.SeatNo.HasValue ? y.SeatNo.Value.ToString().PadLeft(2, '0') : "") + "::";
            string 日期1 = attendX.OccurDate.ToShortDateString();
            string 日期2 = attendy.OccurDate.ToShortDateString();
            班級名稱1 += 座號1;
            班級名稱1 += 日期1;

            班級名稱2 += 座號2;
            班級名稱2 += 日期2;

            return 班級名稱1.CompareTo(班級名稱2);
        }

        private void 修改事由資料toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            #region 檢查選取獎懲的類型是否一致 (MeritFlag一致)

            DataGridViewRow firstRow = dataGridViewX1.SelectedRows[dataGridViewX1.SelectedRows.Count - 1];
            string firstFlag = "" + firstRow.Cells[colMeritFlag.Index].Value;
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                if (row == firstRow) continue;
                if ("" + row.Cells[colMeritFlag.Index].Value != firstFlag)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("請選取相同獎懲類別之獎懲記錄。", "錯誤");
                    return;
                }
            }

            #endregion

            #region 獎懲事由
            if (dataGridViewX1.SelectedRows.Count <= 0) return;

            List<K12.Data.DisciplineRecord> helper = new List<K12.Data.DisciplineRecord>();
            foreach (DataGridViewRow each in dataGridViewX1.SelectedRows)
            {
                helper.Add((K12.Data.DisciplineRecord)each.Tag);
            }

            ChangeTextForm ctf = new ChangeTextForm(helper);
            DialogResult dr = ctf.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                btnRefresh_Click(null, null); //更新畫面資料
            }
            #endregion
        }

        private void cbAllSelset_CheckedChanged(object sender, EventArgs e)
        {
            #region 全選
            cbDemeritAA.Checked = cbAllSelset.Checked;
            cbDemeritBB.Checked = cbAllSelset.Checked;
            cbDemeritCC.Checked = cbAllSelset.Checked;
            #endregion
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            #region 全選
            cbMeritAA.Checked = checkBox1.Checked;
            cbMeritBB.Checked = checkBox1.Checked;
            cbMeritCC.Checked = checkBox1.Checked;
            #endregion
        }

        private void 修改學年度學期ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<K12.Data.DisciplineRecord> list = new List<K12.Data.DisciplineRecord>();
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                K12.Data.DisciplineRecord jhd = (K12.Data.DisciplineRecord)row.Tag;
                list.Add(jhd);
            }

            SetValueSchoolYearSemester ssy = new SetValueSchoolYearSemester();
            DialogResult dr = ssy.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("已進行批次修改獎懲資料之學年度/學期：");
                sb.AppendLine(string.Format("以下所選取資料已指定為學年度「{0}」學期「{1}」", ssy._schoolYear.ToString(), ssy._semester.ToString()));

                foreach (K12.Data.DisciplineRecord each in list)
                {
                    if (each.MeritFlag == "0")
                    {
                        sb.Append("懲戒資料：");
                        sb.Append("學生「" + each.Student.Name + "」");
                        sb.Append("懲戒日期「" + each.OccurDate.ToShortDateString() + "」");
                        sb.Append("原學年「" + each.SchoolYear.ToString() + "」");
                        sb.AppendLine("原學期「" + each.Semester.ToString() + "」");
                    }
                    else if (each.MeritFlag == "1")
                    {
                        sb.Append("獎勵資料：");
                        sb.Append("學生「" + each.Student.Name + "」");
                        sb.Append("獎勵日期「" + each.OccurDate.ToShortDateString() + "」");
                        sb.Append("原學年「" + each.SchoolYear.ToString() + "」");
                        sb.AppendLine("原學期「" + each.Semester.ToString() + "」");
                    }

                    each.SchoolYear = ssy._schoolYear;
                    each.Semester = ssy._semester;
                }
                K12.Data.Discipline.Update(list);

                ApplicationLog.Log("獎懲批次修改", "更新", sb.ToString());
                FISCA.Presentation.Controls.MsgBox.Show("儲存成功!");
                btnRefresh_Click(null, null);
            }
            else
            {
                FISCA.Presentation.Controls.MsgBox.Show("未修改!");
            }
        }

        private void 批次增加前置詞ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<K12.Data.DisciplineRecord> list = new List<K12.Data.DisciplineRecord>();
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                K12.Data.DisciplineRecord jhd = (K12.Data.DisciplineRecord)row.Tag;
                list.Add(jhd);
            }

            ChangeResonBatch ssy = new ChangeResonBatch(list);
            DialogResult dr = ssy.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                btnRefresh_Click(null, null); //更新畫面資料
            }


        }

        private void 修改備註資料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region 批次修改獎懲備註
            if (dataGridViewX1.SelectedRows.Count <= 0) return;

            List<DisciplineRecord> helper = new List<DisciplineRecord>();
            foreach (DataGridViewRow each in dataGridViewX1.SelectedRows)
            {
                helper.Add((DisciplineRecord)each.Tag);
            }

            ChangeRemarkForm ctf = new ChangeRemarkForm(helper);
            DialogResult dr = ctf.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                btnRefresh_Click(null, null); //更新畫面資料
            }
            #endregion
        }
    }

    class StRecord
    {
        public StRecord(DataRow row)
        {
            StudentID = "" + row["student_id"];
            StudentName = "" + row["student_name"];
            StudentNumber = "" + row["student_number"];

            if ("" + row["gender"] == "0")
                Gender = "女";
            else if ("" + row["gender"] == "1")
                Gender = "男";
            else
                Gender = "";

            SeatNo = "" + row["seat_no"];
            ClassName = "" + row["class_name"];
            RefClassID = "" + row["class_id"];
        }

        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public string Gender { get; set; }
        public string SeatNo { get; set; }
        public string ClassName { get; set; }
        public string RefClassID { get; set; }


    }
}
