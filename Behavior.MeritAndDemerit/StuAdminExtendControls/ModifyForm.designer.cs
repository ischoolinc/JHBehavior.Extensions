namespace JHSchool.Behavior.MeritAndDemerit
{
    partial class ModifyForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.txtNewReason = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.lblA = new DevComponents.DotNetBar.LabelX();
            this.lblB = new DevComponents.DotNetBar.LabelX();
            this.lblC = new DevComponents.DotNetBar.LabelX();
            this.txtA = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtB = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtC = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.lbSchoolYear = new DevComponents.DotNetBar.LabelX();
            this.lbSemester = new DevComponents.DotNetBar.LabelX();
            this.intSchoolYear = new DevComponents.Editors.IntegerInput();
            this.intSemester = new DevComponents.Editors.IntegerInput();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.cbRemark = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSchoolYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSemester)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.btnClose.Location = new System.Drawing.Point(220, 249);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "關閉";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtNewReason
            // 
            this.txtNewReason.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.txtNewReason.Border.Class = "TextBoxBorder";
            this.txtNewReason.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtNewReason.Location = new System.Drawing.Point(46, 124);
            this.txtNewReason.Multiline = true;
            this.txtNewReason.Name = "txtNewReason";
            this.txtNewReason.Size = new System.Drawing.Size(249, 114);
            this.txtNewReason.TabIndex = 9;
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.labelX1.Location = new System.Drawing.Point(6, 123);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(34, 21);
            this.labelX1.TabIndex = 8;
            this.labelX1.Text = "事由";
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.btnSave.Location = new System.Drawing.Point(139, 249);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblA
            // 
            this.lblA.AutoSize = true;
            this.lblA.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblA.BackgroundStyle.Class = "";
            this.lblA.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblA.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.lblA.Location = new System.Drawing.Point(7, 52);
            this.lblA.Name = "lblA";
            this.lblA.Size = new System.Drawing.Size(34, 21);
            this.lblA.TabIndex = 0;
            this.lblA.Text = "大功";
            // 
            // lblB
            // 
            this.lblB.AutoSize = true;
            this.lblB.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblB.BackgroundStyle.Class = "";
            this.lblB.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblB.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.lblB.Location = new System.Drawing.Point(107, 52);
            this.lblB.Name = "lblB";
            this.lblB.Size = new System.Drawing.Size(34, 21);
            this.lblB.TabIndex = 2;
            this.lblB.Text = "小功";
            // 
            // lblC
            // 
            this.lblC.AutoSize = true;
            this.lblC.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblC.BackgroundStyle.Class = "";
            this.lblC.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblC.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.lblC.Location = new System.Drawing.Point(207, 52);
            this.lblC.Name = "lblC";
            this.lblC.Size = new System.Drawing.Size(34, 21);
            this.lblC.TabIndex = 4;
            this.lblC.Text = "嘉獎";
            // 
            // txtA
            // 
            // 
            // 
            // 
            this.txtA.Border.Class = "TextBoxBorder";
            this.txtA.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtA.Location = new System.Drawing.Point(46, 51);
            this.txtA.Name = "txtA";
            this.txtA.Size = new System.Drawing.Size(50, 25);
            this.txtA.TabIndex = 1;
            // 
            // txtB
            // 
            // 
            // 
            // 
            this.txtB.Border.Class = "TextBoxBorder";
            this.txtB.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtB.Location = new System.Drawing.Point(146, 51);
            this.txtB.Name = "txtB";
            this.txtB.Size = new System.Drawing.Size(50, 25);
            this.txtB.TabIndex = 3;
            // 
            // txtC
            // 
            // 
            // 
            // 
            this.txtC.Border.Class = "TextBoxBorder";
            this.txtC.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtC.Location = new System.Drawing.Point(246, 51);
            this.txtC.Name = "txtC";
            this.txtC.Size = new System.Drawing.Size(50, 25);
            this.txtC.TabIndex = 5;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkRate = 0;
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // lbSchoolYear
            // 
            this.lbSchoolYear.AutoSize = true;
            this.lbSchoolYear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbSchoolYear.BackgroundStyle.Class = "";
            this.lbSchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbSchoolYear.Location = new System.Drawing.Point(7, 11);
            this.lbSchoolYear.Name = "lbSchoolYear";
            this.lbSchoolYear.Size = new System.Drawing.Size(47, 21);
            this.lbSchoolYear.TabIndex = 12;
            this.lbSchoolYear.Text = "學年度";
            // 
            // lbSemester
            // 
            this.lbSemester.AutoSize = true;
            this.lbSemester.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbSemester.BackgroundStyle.Class = "";
            this.lbSemester.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbSemester.Location = new System.Drawing.Point(175, 11);
            this.lbSemester.Name = "lbSemester";
            this.lbSemester.Size = new System.Drawing.Size(34, 21);
            this.lbSemester.TabIndex = 14;
            this.lbSemester.Text = "學期";
            // 
            // intSchoolYear
            // 
            this.intSchoolYear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.intSchoolYear.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intSchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intSchoolYear.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intSchoolYear.Location = new System.Drawing.Point(61, 9);
            this.intSchoolYear.MaxValue = 999;
            this.intSchoolYear.MinValue = 90;
            this.intSchoolYear.Name = "intSchoolYear";
            this.intSchoolYear.ShowUpDown = true;
            this.intSchoolYear.Size = new System.Drawing.Size(80, 25);
            this.intSchoolYear.TabIndex = 13;
            this.intSchoolYear.Value = 90;
            // 
            // intSemester
            // 
            this.intSemester.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.intSemester.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intSemester.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intSemester.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intSemester.Location = new System.Drawing.Point(216, 9);
            this.intSemester.MaxValue = 2;
            this.intSemester.MinValue = 1;
            this.intSemester.Name = "intSemester";
            this.intSemester.ShowUpDown = true;
            this.intSemester.Size = new System.Drawing.Size(80, 25);
            this.intSemester.TabIndex = 15;
            this.intSemester.Value = 1;
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(7, 90);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(34, 21);
            this.labelX2.TabIndex = 6;
            this.labelX2.Text = "備註";
            // 
            // cbRemark
            // 
            this.cbRemark.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbRemark.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbRemark.DisplayMember = "Text";
            this.cbRemark.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbRemark.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.cbRemark.FormattingEnabled = true;
            this.cbRemark.ItemHeight = 19;
            this.cbRemark.Location = new System.Drawing.Point(46, 90);
            this.cbRemark.Name = "cbRemark";
            this.cbRemark.Size = new System.Drawing.Size(249, 25);
            this.cbRemark.TabIndex = 7;
            // 
            // ModifyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 280);
            this.Controls.Add(this.cbRemark);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.intSemester);
            this.Controls.Add(this.intSchoolYear);
            this.Controls.Add(this.txtC);
            this.Controls.Add(this.txtB);
            this.Controls.Add(this.txtA);
            this.Controls.Add(this.txtNewReason);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lbSemester);
            this.Controls.Add(this.lbSchoolYear);
            this.Controls.Add(this.lblC);
            this.Controls.Add(this.lblB);
            this.Controls.Add(this.lblA);
            this.Controls.Add(this.labelX1);
            this.DoubleBuffered = true;
            this.Name = "ModifyForm";
            this.Text = "修改獎懲";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSchoolYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSemester)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.Controls.TextBoxX txtNewReason;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.LabelX lblA;
        private DevComponents.DotNetBar.LabelX lblB;
        private DevComponents.DotNetBar.LabelX lblC;
        private DevComponents.DotNetBar.Controls.TextBoxX txtA;
        private DevComponents.DotNetBar.Controls.TextBoxX txtB;
        private DevComponents.DotNetBar.Controls.TextBoxX txtC;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private DevComponents.Editors.IntegerInput intSemester;
        private DevComponents.Editors.IntegerInput intSchoolYear;
        private DevComponents.DotNetBar.LabelX lbSemester;
        private DevComponents.DotNetBar.LabelX lbSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbRemark;
    }
}