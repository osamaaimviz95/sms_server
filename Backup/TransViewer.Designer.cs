using System;

namespace MessageViewer
{
    partial class TransViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransViewer));
            this.sbpStartedAtPanel = new System.Windows.Forms.StatusBarPanel();
            this.chkForceNightTime = new System.Windows.Forms.CheckBox();
            this.sbpRunningTimePanel = new System.Windows.Forms.StatusBarPanel();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Timer2 = new System.Windows.Forms.Timer(this.components);
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.sbpSocketsPanel = new System.Windows.Forms.StatusBarPanel();
            this.StatusBar1 = new System.Windows.Forms.StatusBar();
            this.sbpRequestsPanel = new System.Windows.Forms.StatusBarPanel();
            this.sbpPendingPanel = new System.Windows.Forms.StatusBarPanel();
            this.sbpSuccessPanel = new System.Windows.Forms.StatusBarPanel();
            this.sbFailPanel = new System.Windows.Forms.StatusBarPanel();
            this.sbOnQueuePanel = new System.Windows.Forms.StatusBarPanel();
            this.sbpThreadsPanel = new System.Windows.Forms.StatusBarPanel();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.grdTransList = new Janus.Windows.GridEX.GridEX();
            ((System.ComponentModel.ISupportInitialize)(this.sbpStartedAtPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpRunningTimePanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpSocketsPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpRequestsPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpPendingPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpSuccessPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbFailPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbOnQueuePanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpThreadsPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdTransList)).BeginInit();
            this.SuspendLayout();
            // 
            // sbpStartedAtPanel
            // 
            this.sbpStartedAtPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpStartedAtPanel.Name = "sbpStartedAtPanel";
            this.sbpStartedAtPanel.Text = "Started at";
            this.sbpStartedAtPanel.Width = 63;
            // 
            // chkForceNightTime
            // 
            this.chkForceNightTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkForceNightTime.Location = new System.Drawing.Point(581, 360);
            this.chkForceNightTime.Name = "chkForceNightTime";
            this.chkForceNightTime.Size = new System.Drawing.Size(120, 16);
            this.chkForceNightTime.TabIndex = 43;
            this.chkForceNightTime.Text = "Force Night Time";
            this.chkForceNightTime.CheckedChanged += new System.EventHandler(this.chkForceNightTime_CheckedChanged);
            // 
            // sbpRunningTimePanel
            // 
            this.sbpRunningTimePanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpRunningTimePanel.Name = "sbpRunningTimePanel";
            this.sbpRunningTimePanel.Text = "Running Time:";
            this.sbpRunningTimePanel.Width = 88;
            // 
            // Label5
            // 
            this.Label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Label5.BackColor = System.Drawing.Color.Orange;
            this.Label5.Location = new System.Drawing.Point(280, 360);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(56, 16);
            this.Label5.TabIndex = 42;
            this.Label5.Text = "Reversal";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label4
            // 
            this.Label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label4.BackColor = System.Drawing.Color.Navy;
            this.Label4.ForeColor = System.Drawing.Color.White;
            this.Label4.Location = new System.Drawing.Point(711, 360);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(96, 16);
            this.Label4.TabIndex = 41;
            this.Label4.Text = "Night Time Off";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label3
            // 
            this.Label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Label3.BackColor = System.Drawing.Color.Yellow;
            this.Label3.Location = new System.Drawing.Point(16, 360);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(56, 16);
            this.Label3.TabIndex = 40;
            this.Label3.Text = "Pending";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Timer2
            // 
            this.Timer2.Enabled = true;
            this.Timer2.Interval = 1000;
            this.Timer2.Tick += new System.EventHandler(this.Timer2_Tick);
            // 
            // Label2
            // 
            this.Label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Label2.BackColor = System.Drawing.Color.Red;
            this.Label2.Location = new System.Drawing.Point(192, 360);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(56, 16);
            this.Label2.TabIndex = 39;
            this.Label2.Text = "Fail";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label1
            // 
            this.Label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Label1.BackColor = System.Drawing.Color.Chartreuse;
            this.Label1.Location = new System.Drawing.Point(104, 360);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(56, 16);
            this.Label1.TabIndex = 38;
            this.Label1.Text = "Success";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sbpSocketsPanel
            // 
            this.sbpSocketsPanel.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.sbpSocketsPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpSocketsPanel.Name = "sbpSocketsPanel";
            this.sbpSocketsPanel.Text = "Sockets:";
            this.sbpSocketsPanel.Width = 58;
            // 
            // StatusBar1
            // 
            this.StatusBar1.Location = new System.Drawing.Point(0, 454);
            this.StatusBar1.Name = "StatusBar1";
            this.StatusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.sbpRequestsPanel,
            this.sbpPendingPanel,
            this.sbpSuccessPanel,
            this.sbFailPanel,
            this.sbOnQueuePanel,
            this.sbpThreadsPanel,
            this.sbpSocketsPanel,
            this.sbpStartedAtPanel,
            this.sbpRunningTimePanel});
            this.StatusBar1.ShowPanels = true;
            this.StatusBar1.Size = new System.Drawing.Size(832, 24);
            this.StatusBar1.TabIndex = 37;
            this.StatusBar1.Text = "StatusBar1";
            // 
            // sbpRequestsPanel
            // 
            this.sbpRequestsPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpRequestsPanel.Name = "sbpRequestsPanel";
            this.sbpRequestsPanel.Text = "Requests:";
            this.sbpRequestsPanel.Width = 65;
            // 
            // sbpPendingPanel
            // 
            this.sbpPendingPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpPendingPanel.Name = "sbpPendingPanel";
            this.sbpPendingPanel.Text = "Pending:";
            this.sbpPendingPanel.Width = 59;
            // 
            // sbpSuccessPanel
            // 
            this.sbpSuccessPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpSuccessPanel.Name = "sbpSuccessPanel";
            this.sbpSuccessPanel.Text = "Success:";
            this.sbpSuccessPanel.Width = 60;
            // 
            // sbFailPanel
            // 
            this.sbFailPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbFailPanel.Name = "sbFailPanel";
            this.sbFailPanel.Text = "Fail:";
            this.sbFailPanel.Width = 36;
            // 
            // sbOnQueuePanel
            // 
            this.sbOnQueuePanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbOnQueuePanel.Name = "sbOnQueuePanel";
            this.sbOnQueuePanel.Text = "On Queue:";
            this.sbOnQueuePanel.Width = 69;
            // 
            // sbpThreadsPanel
            // 
            this.sbpThreadsPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpThreadsPanel.Name = "sbpThreadsPanel";
            this.sbpThreadsPanel.Text = "Threads:";
            this.sbpThreadsPanel.Width = 59;
            // 
            // Timer1
            // 
            this.Timer1.Enabled = true;
            this.Timer1.Interval = 10;
            this.Timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // grdTransList
            // 
            this.grdTransList.AllowCardSizing = false;
            this.grdTransList.AllowColumnDrag = false;
            this.grdTransList.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.False;
            this.grdTransList.AlternatingRowFormatStyle.BackColor = System.Drawing.Color.Black;
            this.grdTransList.AlternatingRowFormatStyle.ForeColor = System.Drawing.Color.Chartreuse;
            this.grdTransList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdTransList.AutomaticSort = false;
            this.grdTransList.BackColor = System.Drawing.Color.Black;
            this.grdTransList.CellSelectionMode = Janus.Windows.GridEX.CellSelectionMode.SingleCell;
            this.grdTransList.ColumnHeaders = Janus.Windows.GridEX.InheritableBoolean.False;
            this.grdTransList.Cursor = System.Windows.Forms.Cursors.Default;
            this.grdTransList.EditorsControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.Regular;
            this.grdTransList.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold);
            this.grdTransList.GridLines = Janus.Windows.GridEX.GridLines.None;
            this.grdTransList.GroupByBoxVisible = false;
            this.grdTransList.InvalidValueAction = Janus.Windows.GridEX.InvalidValueAction.DiscardChanges;
            this.grdTransList.LayoutData = resources.GetString("grdTransList.LayoutData");
            this.grdTransList.Location = new System.Drawing.Point(16, 12);
            this.grdTransList.Name = "grdTransList";
            this.grdTransList.SelectedFormatStyle.BackColor = System.Drawing.Color.Black;
            this.grdTransList.Size = new System.Drawing.Size(792, 340);
            this.grdTransList.TabIndex = 36;
            // 
            // TransViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 478);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.StatusBar1);
            this.Controls.Add(this.grdTransList);
            this.Controls.Add(this.chkForceNightTime);
            this.Controls.Add(this.Label5);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Name = "TransViewer";
            this.Text = "Transactions Viewer C#";
            this.Load += new System.EventHandler(this.TransViewer_Load);
            this.Resize += new System.EventHandler(this.TransViewer_Resize);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TransViewer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.sbpStartedAtPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpRunningTimePanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpSocketsPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpRequestsPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpPendingPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpSuccessPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbFailPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbOnQueuePanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpThreadsPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdTransList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.StatusBarPanel sbpStartedAtPanel;
        private System.Windows.Forms.CheckBox chkForceNightTime;
        private System.Windows.Forms.StatusBarPanel sbpRunningTimePanel;
        private System.Windows.Forms.Label Label5;
        private System.Windows.Forms.Label Label4;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.Timer Timer2;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.StatusBarPanel sbpSocketsPanel;
        private System.Windows.Forms.StatusBar StatusBar1;
        private System.Windows.Forms.StatusBarPanel sbpRequestsPanel;
        private System.Windows.Forms.StatusBarPanel sbpPendingPanel;
        private System.Windows.Forms.StatusBarPanel sbpSuccessPanel;
        private System.Windows.Forms.StatusBarPanel sbFailPanel;
        private System.Windows.Forms.StatusBarPanel sbOnQueuePanel;
        private System.Windows.Forms.StatusBarPanel sbpThreadsPanel;
        private System.Windows.Forms.Timer Timer1;
        private Janus.Windows.GridEX.GridEX grdTransList;
    }
}

