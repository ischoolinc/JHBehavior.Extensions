using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using JHSchool.Data;
using FISCA.Presentation;
using Framework.Security;
using Framework;
using JHSchool.Affair;

namespace JHSchool.Behavior.MeritAndDemerit
{
    public class Program
    {
        [MainMethod()]
        static public void Main()
        {
            FISCA.Permission.FeatureAce UserPermission;
            //獎勵資料
            UserPermission = FISCA.Permission.UserAcl.Current[Permissions.獎勵資料項目];
            if (UserPermission.Editable || UserPermission.Viewable)
                Student.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<MeritItem>());

            //懲戒資料
            UserPermission = FISCA.Permission.UserAcl.Current[Permissions.懲戒資料項目];
            if (UserPermission.Editable || UserPermission.Viewable)
                Student.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<DemeritItem>());

            //獎懲學期統計
            UserPermission = FISCA.Permission.UserAcl.Current[Permissions.獎懲學期統計];
            if (UserPermission.Editable || UserPermission.Viewable)
                Student.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<DisciplineUnifytItem>());

            #region Merit

            RibbonBarItem rbItem = Student.Instance.RibbonBarItems["學務"];
            rbItem["獎勵"].Enable = false;
            rbItem["獎勵快速登錄"].Enable = false;

            rbItem["獎勵"].Image = Properties.Resources.achievement_64;
            rbItem["獎勵快速登錄"].Image = Properties.Resources.achievement_clock_64;

            rbItem["獎勵"].Click += delegate
            {
                MeritEditForm editor = new MeritEditForm(K12.Data.Student.SelectByIDs(Student.Instance.SelectedKeys));
                editor.ShowDialog();
            };

            rbItem["獎勵快速登錄"].Click += delegate
            {
                MutiMeritDemerit editor = new MutiMeritDemerit("獎勵");
                editor.ShowDialog();
            };

            //ListPaneContexMenu
            if (Permissions.Merit)
            {

                Student.Instance.ListPaneContexMenu["獎勵"].Enable = false;
                Student.Instance.ListPaneContexMenu["獎勵快速登錄"].Enable = false;

                Student.Instance.ListPaneContexMenu["獎勵"].Image = Properties.Resources.achievement_64;
                Student.Instance.ListPaneContexMenu["獎勵快速登錄"].Image = Properties.Resources.achievement_clock_64;

                Student.Instance.ListPaneContexMenu["獎勵"].Click += delegate
                {
                    if (Student.Instance.SelectedList.Count > 0)
                    {
                        MeritEditForm editor = new MeritEditForm(K12.Data.Student.SelectByIDs(Student.Instance.SelectedKeys));
                        editor.ShowDialog();
                    }
                };

                Student.Instance.ListPaneContexMenu["獎勵快速登錄"].Click += delegate
                {
                    if (Student.Instance.SelectedList.Count > 0)
                    {
                        MutiMeritDemerit editor = new MutiMeritDemerit("獎勵");
                        editor.ShowDialog();
                    }
                };
            }

            Student.Instance.SelectedListChanged += delegate
            {
                if (Permissions.Merit)
                {
                    rbItem["獎勵"].Enable = (Student.Instance.SelectedList.Count >= 1);
                    rbItem["獎勵快速登錄"].Enable = (Student.Instance.SelectedList.Count >= 1);

                    Student.Instance.ListPaneContexMenu["獎勵"].Enable = (Student.Instance.SelectedList.Count >= 1);
                    Student.Instance.ListPaneContexMenu["獎勵快速登錄"].Enable = (Student.Instance.SelectedList.Count >= 1);
                }
            };

            #endregion

            #region Demerit

            rbItem = Student.Instance.RibbonBarItems["學務"];
            rbItem["懲戒"].Enable = false;
            rbItem["懲戒快速登錄"].Enable = false;

            rbItem["懲戒"].Image = Properties.Resources.laws_64;
            rbItem["懲戒快速登錄"].Image = Properties.Resources.laws_clock_64;

            rbItem["懲戒"].Click += delegate
            {
                DemeritEditForm editForm = new DemeritEditForm(K12.Data.Student.SelectByIDs(Student.Instance.SelectedKeys));
                editForm.ShowDialog();
            };

            rbItem["懲戒快速登錄"].Click += delegate
            {
                MutiMeritDemerit editor = new MutiMeritDemerit("懲戒");
                editor.ShowDialog();
            };

            if (Permissions.Demerit)
            {
                Student.Instance.ListPaneContexMenu["懲戒"].Enable = false;
                Student.Instance.ListPaneContexMenu["懲戒快速登錄"].Enable = false;

                Student.Instance.ListPaneContexMenu["懲戒"].Image = Properties.Resources.laws_64;
                Student.Instance.ListPaneContexMenu["懲戒快速登錄"].Image = Properties.Resources.laws_clock_64;

                Student.Instance.ListPaneContexMenu["懲戒"].Click += delegate
                {
                    if (Student.Instance.SelectedList.Count > 0)
                    {
                        DemeritEditForm editForm = new DemeritEditForm(K12.Data.Student.SelectByIDs(Student.Instance.SelectedKeys));
                        editForm.ShowDialog();
                    }

                };

                Student.Instance.ListPaneContexMenu["懲戒快速登錄"].Click += delegate
                {
                    if (Student.Instance.SelectedList.Count > 0)
                    {
                        MutiMeritDemerit editor = new MutiMeritDemerit("懲戒");
                        editor.ShowDialog();
                    }
                };

                Student.Instance.SelectedListChanged += delegate
                {
                    if (Permissions.Demerit)
                    {
                        rbItem["懲戒"].Enable = (Student.Instance.SelectedList.Count >= 1);
                        rbItem["懲戒快速登錄"].Enable = (Student.Instance.SelectedList.Count >= 1);
                        Student.Instance.ListPaneContexMenu["懲戒"].Enable = (Student.Instance.SelectedList.Count >= 1);
                        Student.Instance.ListPaneContexMenu["懲戒快速登錄"].Enable = (Student.Instance.SelectedList.Count >= 1);
                    }
                };
            }

            #endregion

            RibbonBarItem batchEdit = StuAdmin.Instance.RibbonBarItems["批次作業/查詢"];
            batchEdit.Index = 1;
            batchEdit["獎懲批次修改"].Image = Properties.Resources.star_write_64;
            batchEdit["獎懲批次修改"].Enable = User.Acl["JHSchool.StuAdmin.Ribbon0050"].Executable;
            batchEdit["獎懲批次修改"].Click += delegate
            {
                MerDemEditForm DiscipTotal = new MerDemEditForm();
                DiscipTotal.ShowDialog();
            };

            Catalog ribbon = RoleAclSource.Instance["學生"]["功能按鈕"];
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0060", "獎勵"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0080", "懲戒"));

            ribbon = RoleAclSource.Instance["學務作業"];
            ribbon.Add(new RibbonFeature("JHSchool.StuAdmin.Ribbon0050", "獎懲批次修改"));

            Catalog detail = RoleAclSource.Instance["學生"]["資料項目"];
            detail.Add(new DetailItemFeature(typeof(DemeritItem)));
            detail.Add(new DetailItemFeature(typeof(MeritItem)));
            detail.Add(new DetailItemFeature(typeof(DisciplineUnifytItem))); //獎懲學期統計(NEW)

        }
    }
}
