namespace BluStation.Forms
{
    partial class BluApp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BluApp));
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabNodeObject = new System.Windows.Forms.TabPage();
            this.splitContainerNodeObject = new System.Windows.Forms.SplitContainer();
            this.jvNodeObject = new EPocalipse.Json.Viewer.JsonViewer();
            this.txtRunListProperties = new System.Windows.Forms.TextBox();
            this.lblRunListProperties = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.runlistTreeView = new System.Windows.Forms.TreeView();
            this.tabCompiler = new System.Windows.Forms.TabPage();
            this.splitContainerCompile = new System.Windows.Forms.SplitContainer();
            this.lblMode = new System.Windows.Forms.Label();
            this.cmbSprintMode = new System.Windows.Forms.ComboBox();
            this.btnCompile = new System.Windows.Forms.Button();
            this.lblRubyScript = new System.Windows.Forms.Label();
            this.txtRuby = new System.Windows.Forms.TextBox();
            this.tabCompileResults = new System.Windows.Forms.TabControl();
            this.tabSyntaxTree = new System.Windows.Forms.TabPage();
            this.tvAstNodes = new System.Windows.Forms.TreeView();
            this.tabErrors = new System.Windows.Forms.TabPage();
            this.txtErrDetails = new System.Windows.Forms.TextBox();
            this.lstErrors = new System.Windows.Forms.ListBox();
            this.tabCompiledJson = new System.Windows.Forms.TabPage();
            this.jvCompiledJson = new EPocalipse.Json.Viewer.JsonViewer();
            this.tabSprint = new System.Windows.Forms.TabPage();
            this.panelSprint = new System.Windows.Forms.Panel();
            this.txtSprintPs1 = new System.Windows.Forms.WebBrowser();
            this.tabResources = new System.Windows.Forms.TabPage();
            this.panelResources = new System.Windows.Forms.Panel();
            this.txtResourcesPsm1 = new System.Windows.Forms.WebBrowser();
            this.tabChefAPI = new System.Windows.Forms.TabPage();
            this.splitContainerChefAPI = new System.Windows.Forms.SplitContainer();
            this.lblPath = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.jvResponse = new EPocalipse.Json.Viewer.JsonViewer();
            this.lblResponse = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnPut = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnGet = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblSelect = new System.Windows.Forms.Label();
            this.txtSelect = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblValidator = new System.Windows.Forms.Label();
            this.lblValidationKey = new System.Windows.Forms.Label();
            this.lblOrganizationUri = new System.Windows.Forms.Label();
            this.lblOrganization = new System.Windows.Forms.Label();
            this.lblNodeName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblClientPem = new System.Windows.Forms.Label();
            this.txtValidator = new System.Windows.Forms.TextBox();
            this.txtValidationKey = new System.Windows.Forms.TextBox();
            this.txtOrganizationUri = new System.Windows.Forms.TextBox();
            this.txtOrganization = new System.Windows.Forms.TextBox();
            this.txtNodeName = new System.Windows.Forms.TextBox();
            this.txtClientRb = new System.Windows.Forms.TextBox();
            this.txtClientPem = new System.Windows.Forms.TextBox();
            this.txtClientPath = new System.Windows.Forms.TextBox();
            this.lblClientPath = new System.Windows.Forms.Label();
            this.lblClientName = new System.Windows.Forms.Label();
            this.txtClientName = new System.Windows.Forms.TextBox();
            this.imgIcons = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtError = new System.Windows.Forms.TextBox();
            this.tabMain.SuspendLayout();
            this.tabNodeObject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerNodeObject)).BeginInit();
            this.splitContainerNodeObject.Panel1.SuspendLayout();
            this.splitContainerNodeObject.Panel2.SuspendLayout();
            this.splitContainerNodeObject.SuspendLayout();
            this.tabCompiler.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCompile)).BeginInit();
            this.splitContainerCompile.Panel1.SuspendLayout();
            this.splitContainerCompile.Panel2.SuspendLayout();
            this.splitContainerCompile.SuspendLayout();
            this.tabCompileResults.SuspendLayout();
            this.tabSyntaxTree.SuspendLayout();
            this.tabErrors.SuspendLayout();
            this.tabCompiledJson.SuspendLayout();
            this.tabSprint.SuspendLayout();
            this.panelSprint.SuspendLayout();
            this.tabResources.SuspendLayout();
            this.panelResources.SuspendLayout();
            this.tabChefAPI.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerChefAPI)).BeginInit();
            this.splitContainerChefAPI.Panel1.SuspendLayout();
            this.splitContainerChefAPI.Panel2.SuspendLayout();
            this.splitContainerChefAPI.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabMain.Controls.Add(this.tabCompiler);
            this.tabMain.Controls.Add(this.tabChefAPI);
            this.tabMain.Controls.Add(this.tabNodeObject);
            this.tabMain.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabMain.ImageList = this.imgIcons;
            this.tabMain.Location = new System.Drawing.Point(3, 5);
            this.tabMain.Multiline = true;
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1196, 603);
            this.tabMain.TabIndex = 1;
            // 
            // tabNodeObject
            // 
            this.tabNodeObject.Controls.Add(this.splitContainerNodeObject);
            this.tabNodeObject.ImageKey = "node.ico";
            this.tabNodeObject.Location = new System.Drawing.Point(4, 26);
            this.tabNodeObject.Name = "tabNodeObject";
            this.tabNodeObject.Padding = new System.Windows.Forms.Padding(3);
            this.tabNodeObject.Size = new System.Drawing.Size(1188, 573);
            this.tabNodeObject.TabIndex = 0;
            this.tabNodeObject.Text = "Node Object";
            this.tabNodeObject.UseVisualStyleBackColor = true;
            // 
            // splitContainerNodeObject
            // 
            this.splitContainerNodeObject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerNodeObject.Location = new System.Drawing.Point(6, 6);
            this.splitContainerNodeObject.Name = "splitContainerNodeObject";
            // 
            // splitContainerNodeObject.Panel1
            // 
            this.splitContainerNodeObject.Panel1.Controls.Add(this.jvNodeObject);
            // 
            // splitContainerNodeObject.Panel2
            // 
            this.splitContainerNodeObject.Panel2.Controls.Add(this.txtRunListProperties);
            this.splitContainerNodeObject.Panel2.Controls.Add(this.lblRunListProperties);
            this.splitContainerNodeObject.Panel2.Controls.Add(this.label1);
            this.splitContainerNodeObject.Panel2.Controls.Add(this.runlistTreeView);
            this.splitContainerNodeObject.Panel2MinSize = 240;
            this.splitContainerNodeObject.Size = new System.Drawing.Size(1179, 564);
            this.splitContainerNodeObject.SplitterDistance = 935;
            this.splitContainerNodeObject.TabIndex = 2;
            // 
            // jvNodeObject
            // 
            this.jvNodeObject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.jvNodeObject.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jvNodeObject.Json = null;
            this.jvNodeObject.Location = new System.Drawing.Point(3, 3);
            this.jvNodeObject.Name = "jvNodeObject";
            this.jvNodeObject.Size = new System.Drawing.Size(929, 558);
            this.jvNodeObject.TabIndex = 2;
            // 
            // txtRunListProperties
            // 
            this.txtRunListProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRunListProperties.BackColor = System.Drawing.Color.LightGray;
            this.txtRunListProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRunListProperties.Location = new System.Drawing.Point(3, 434);
            this.txtRunListProperties.Multiline = true;
            this.txtRunListProperties.Name = "txtRunListProperties";
            this.txtRunListProperties.Size = new System.Drawing.Size(232, 127);
            this.txtRunListProperties.TabIndex = 5;
            // 
            // lblRunListProperties
            // 
            this.lblRunListProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRunListProperties.AutoSize = true;
            this.lblRunListProperties.Location = new System.Drawing.Point(7, 415);
            this.lblRunListProperties.Name = "lblRunListProperties";
            this.lblRunListProperties.Size = new System.Drawing.Size(84, 14);
            this.lblRunListProperties.TabIndex = 4;
            this.lblRunListProperties.Text = "Properties:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "Compiled RunList:";
            // 
            // runlistTreeView
            // 
            this.runlistTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.runlistTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.runlistTreeView.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runlistTreeView.Location = new System.Drawing.Point(3, 23);
            this.runlistTreeView.Name = "runlistTreeView";
            this.runlistTreeView.Size = new System.Drawing.Size(232, 374);
            this.runlistTreeView.TabIndex = 2;
            // 
            // tabCompiler
            // 
            this.tabCompiler.Controls.Add(this.splitContainerCompile);
            this.tabCompiler.ImageIndex = 1;
            this.tabCompiler.Location = new System.Drawing.Point(4, 26);
            this.tabCompiler.Name = "tabCompiler";
            this.tabCompiler.Size = new System.Drawing.Size(1188, 573);
            this.tabCompiler.TabIndex = 2;
            this.tabCompiler.Text = "Compiler";
            this.tabCompiler.UseVisualStyleBackColor = true;
            // 
            // splitContainerCompile
            // 
            this.splitContainerCompile.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainerCompile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerCompile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerCompile.ForeColor = System.Drawing.SystemColors.ControlText;
            this.splitContainerCompile.Location = new System.Drawing.Point(0, 0);
            this.splitContainerCompile.Name = "splitContainerCompile";
            // 
            // splitContainerCompile.Panel1
            // 
            this.splitContainerCompile.Panel1.Controls.Add(this.lblMode);
            this.splitContainerCompile.Panel1.Controls.Add(this.cmbSprintMode);
            this.splitContainerCompile.Panel1.Controls.Add(this.btnCompile);
            this.splitContainerCompile.Panel1.Controls.Add(this.lblRubyScript);
            this.splitContainerCompile.Panel1.Controls.Add(this.txtRuby);
            // 
            // splitContainerCompile.Panel2
            // 
            this.splitContainerCompile.Panel2.Controls.Add(this.tabCompileResults);
            this.splitContainerCompile.Size = new System.Drawing.Size(1188, 573);
            this.splitContainerCompile.SplitterDistance = 454;
            this.splitContainerCompile.TabIndex = 0;
            // 
            // lblMode
            // 
            this.lblMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(216, 8);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(42, 14);
            this.lblMode.TabIndex = 5;
            this.lblMode.Text = "Mode:";
            // 
            // cmbSprintMode
            // 
            this.cmbSprintMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSprintMode.FormattingEnabled = true;
            this.cmbSprintMode.Items.AddRange(new object[] {
            "Script",
            "LWRP",
            "StandAlone",
            "Dev"});
            this.cmbSprintMode.Location = new System.Drawing.Point(264, 5);
            this.cmbSprintMode.Name = "cmbSprintMode";
            this.cmbSprintMode.Size = new System.Drawing.Size(100, 22);
            this.cmbSprintMode.TabIndex = 4;
            // 
            // btnCompile
            // 
            this.btnCompile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCompile.Location = new System.Drawing.Point(370, 4);
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(74, 23);
            this.btnCompile.TabIndex = 3;
            this.btnCompile.Text = "Compile";
            this.btnCompile.UseVisualStyleBackColor = true;
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // lblRubyScript
            // 
            this.lblRubyScript.AutoSize = true;
            this.lblRubyScript.Location = new System.Drawing.Point(4, 9);
            this.lblRubyScript.Name = "lblRubyScript";
            this.lblRubyScript.Size = new System.Drawing.Size(42, 14);
            this.lblRubyScript.TabIndex = 1;
            this.lblRubyScript.Text = "Ruby:";
            // 
            // txtRuby
            // 
            this.txtRuby.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRuby.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.txtRuby.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRuby.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRuby.ForeColor = System.Drawing.Color.LightGray;
            this.txtRuby.HideSelection = false;
            this.txtRuby.Location = new System.Drawing.Point(4, 32);
            this.txtRuby.Multiline = true;
            this.txtRuby.Name = "txtRuby";
            this.txtRuby.Size = new System.Drawing.Size(442, 528);
            this.txtRuby.TabIndex = 0;
            this.txtRuby.Text = resources.GetString("txtRuby.Text");
            this.txtRuby.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRuby_KeyDown);
            // 
            // tabCompileResults
            // 
            this.tabCompileResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCompileResults.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabCompileResults.Controls.Add(this.tabSyntaxTree);
            this.tabCompileResults.Controls.Add(this.tabErrors);
            this.tabCompileResults.Controls.Add(this.tabCompiledJson);
            this.tabCompileResults.Controls.Add(this.tabSprint);
            this.tabCompileResults.Controls.Add(this.tabResources);
            this.tabCompileResults.Location = new System.Drawing.Point(3, 3);
            this.tabCompileResults.Name = "tabCompileResults";
            this.tabCompileResults.SelectedIndex = 0;
            this.tabCompileResults.Size = new System.Drawing.Size(729, 561);
            this.tabCompileResults.TabIndex = 0;
            // 
            // tabSyntaxTree
            // 
            this.tabSyntaxTree.Controls.Add(this.tvAstNodes);
            this.tabSyntaxTree.Location = new System.Drawing.Point(4, 26);
            this.tabSyntaxTree.Name = "tabSyntaxTree";
            this.tabSyntaxTree.Padding = new System.Windows.Forms.Padding(3);
            this.tabSyntaxTree.Size = new System.Drawing.Size(721, 531);
            this.tabSyntaxTree.TabIndex = 0;
            this.tabSyntaxTree.Text = "Syntax Tree";
            this.tabSyntaxTree.UseVisualStyleBackColor = true;
            // 
            // tvAstNodes
            // 
            this.tvAstNodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvAstNodes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvAstNodes.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvAstNodes.Location = new System.Drawing.Point(3, 3);
            this.tvAstNodes.Name = "tvAstNodes";
            this.tvAstNodes.Size = new System.Drawing.Size(711, 528);
            this.tvAstNodes.TabIndex = 0;
            this.tvAstNodes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvAstNodes_AfterSelect);
            // 
            // tabErrors
            // 
            this.tabErrors.Controls.Add(this.txtErrDetails);
            this.tabErrors.Controls.Add(this.lstErrors);
            this.tabErrors.Location = new System.Drawing.Point(4, 26);
            this.tabErrors.Name = "tabErrors";
            this.tabErrors.Padding = new System.Windows.Forms.Padding(3);
            this.tabErrors.Size = new System.Drawing.Size(106, 3);
            this.tabErrors.TabIndex = 1;
            this.tabErrors.Text = "Syntax Errors";
            this.tabErrors.UseVisualStyleBackColor = true;
            // 
            // txtErrDetails
            // 
            this.txtErrDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtErrDetails.BackColor = System.Drawing.Color.LightGray;
            this.txtErrDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtErrDetails.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtErrDetails.Location = new System.Drawing.Point(3, 7886);
            this.txtErrDetails.Multiline = true;
            this.txtErrDetails.Name = "txtErrDetails";
            this.txtErrDetails.Size = new System.Drawing.Size(9325, 51);
            this.txtErrDetails.TabIndex = 1;
            this.txtErrDetails.Text = "Error details";
            // 
            // lstErrors
            // 
            this.lstErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstErrors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstErrors.FormattingEnabled = true;
            this.lstErrors.ItemHeight = 14;
            this.lstErrors.Location = new System.Drawing.Point(3, 3);
            this.lstErrors.Name = "lstErrors";
            this.lstErrors.Size = new System.Drawing.Size(9325, 7688);
            this.lstErrors.TabIndex = 0;
            this.lstErrors.Click += new System.EventHandler(this.lstErrors_Click);
            // 
            // tabCompiledJson
            // 
            this.tabCompiledJson.Controls.Add(this.jvCompiledJson);
            this.tabCompiledJson.Location = new System.Drawing.Point(4, 26);
            this.tabCompiledJson.Name = "tabCompiledJson";
            this.tabCompiledJson.Size = new System.Drawing.Size(106, 3);
            this.tabCompiledJson.TabIndex = 3;
            this.tabCompiledJson.Text = "Compiled Json";
            this.tabCompiledJson.UseVisualStyleBackColor = true;
            // 
            // jvCompiledJson
            // 
            this.jvCompiledJson.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.jvCompiledJson.Json = null;
            this.jvCompiledJson.Location = new System.Drawing.Point(2, 3);
            this.jvCompiledJson.Name = "jvCompiledJson";
            this.jvCompiledJson.Size = new System.Drawing.Size(8714, 7404);
            this.jvCompiledJson.TabIndex = 0;
            // 
            // tabSprint
            // 
            this.tabSprint.Controls.Add(this.panelSprint);
            this.tabSprint.Location = new System.Drawing.Point(4, 26);
            this.tabSprint.Name = "tabSprint";
            this.tabSprint.Size = new System.Drawing.Size(106, 3);
            this.tabSprint.TabIndex = 2;
            this.tabSprint.Text = "Sprint";
            this.tabSprint.UseVisualStyleBackColor = true;
            // 
            // panelSprint
            // 
            this.panelSprint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSprint.Controls.Add(this.txtSprintPs1);
            this.panelSprint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSprint.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelSprint.Location = new System.Drawing.Point(0, 0);
            this.panelSprint.Name = "panelSprint";
            this.panelSprint.Size = new System.Drawing.Size(106, 3);
            this.panelSprint.TabIndex = 0;
            // 
            // txtSprintPs1
            // 
            this.txtSprintPs1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSprintPs1.Location = new System.Drawing.Point(0, 0);
            this.txtSprintPs1.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtSprintPs1.Name = "txtSprintPs1";
            this.txtSprintPs1.Size = new System.Drawing.Size(104, 20);
            this.txtSprintPs1.TabIndex = 1;
            // 
            // tabResources
            // 
            this.tabResources.Controls.Add(this.panelResources);
            this.tabResources.Location = new System.Drawing.Point(4, 26);
            this.tabResources.Name = "tabResources";
            this.tabResources.Size = new System.Drawing.Size(106, 3);
            this.tabResources.TabIndex = 4;
            this.tabResources.Text = "Resources";
            this.tabResources.UseVisualStyleBackColor = true;
            // 
            // panelResources
            // 
            this.panelResources.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelResources.Controls.Add(this.txtResourcesPsm1);
            this.panelResources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelResources.Location = new System.Drawing.Point(0, 0);
            this.panelResources.Name = "panelResources";
            this.panelResources.Size = new System.Drawing.Size(106, 3);
            this.panelResources.TabIndex = 0;
            // 
            // txtResourcesPsm1
            // 
            this.txtResourcesPsm1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResourcesPsm1.Location = new System.Drawing.Point(0, 0);
            this.txtResourcesPsm1.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtResourcesPsm1.Name = "txtResourcesPsm1";
            this.txtResourcesPsm1.Size = new System.Drawing.Size(104, 20);
            this.txtResourcesPsm1.TabIndex = 0;
            // 
            // tabChefAPI
            // 
            this.tabChefAPI.Controls.Add(this.splitContainerChefAPI);
            this.tabChefAPI.ImageIndex = 2;
            this.tabChefAPI.Location = new System.Drawing.Point(4, 26);
            this.tabChefAPI.Name = "tabChefAPI";
            this.tabChefAPI.Size = new System.Drawing.Size(1188, 573);
            this.tabChefAPI.TabIndex = 3;
            this.tabChefAPI.Text = "Chef API";
            this.tabChefAPI.UseVisualStyleBackColor = true;
            // 
            // splitContainerChefAPI
            // 
            this.splitContainerChefAPI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerChefAPI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerChefAPI.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerChefAPI.Location = new System.Drawing.Point(0, 0);
            this.splitContainerChefAPI.Name = "splitContainerChefAPI";
            // 
            // splitContainerChefAPI.Panel1
            // 
            this.splitContainerChefAPI.Panel1.Controls.Add(this.txtError);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.lblPath);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.txtPath);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.btnExport);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.jvResponse);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.lblResponse);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.btnDelete);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.btnPut);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.btnPost);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.btnGet);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.txtMessage);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.lblMessage);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.lblSelect);
            this.splitContainerChefAPI.Panel1.Controls.Add(this.txtSelect);
            // 
            // splitContainerChefAPI.Panel2
            // 
            this.splitContainerChefAPI.Panel2.Controls.Add(this.btnCancel);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.btnSave);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.lblValidator);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.lblValidationKey);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.lblOrganizationUri);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.lblOrganization);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.lblNodeName);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.label2);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.lblClientPem);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.txtValidator);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.txtValidationKey);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.txtOrganizationUri);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.txtOrganization);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.txtNodeName);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.txtClientRb);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.txtClientPem);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.txtClientPath);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.lblClientPath);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.lblClientName);
            this.splitContainerChefAPI.Panel2.Controls.Add(this.txtClientName);
            this.splitContainerChefAPI.Panel2MinSize = 620;
            this.splitContainerChefAPI.Size = new System.Drawing.Size(1188, 573);
            this.splitContainerChefAPI.SplitterDistance = 544;
            this.splitContainerChefAPI.TabIndex = 0;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(310, 19);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(42, 14);
            this.lblPath.TabIndex = 15;
            this.lblPath.Text = "Path:";
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPath.Location = new System.Drawing.Point(358, 14);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(170, 22);
            this.txtPath.TabIndex = 14;
            this.txtPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPath_KeyDown);
            // 
            // btnExport
            // 
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Location = new System.Drawing.Point(272, 175);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(60, 23);
            this.btnExport.TabIndex = 13;
            this.btnExport.Text = "EXPORT";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // jvResponse
            // 
            this.jvResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.jvResponse.Json = null;
            this.jvResponse.Location = new System.Drawing.Point(8, 232);
            this.jvResponse.Name = "jvResponse";
            this.jvResponse.Size = new System.Drawing.Size(520, 336);
            this.jvResponse.TabIndex = 12;
            // 
            // lblResponse
            // 
            this.lblResponse.AutoSize = true;
            this.lblResponse.BackColor = System.Drawing.Color.Transparent;
            this.lblResponse.Location = new System.Drawing.Point(8, 206);
            this.lblResponse.Name = "lblResponse";
            this.lblResponse.Size = new System.Drawing.Size(70, 14);
            this.lblResponse.TabIndex = 11;
            this.lblResponse.Text = "Response:";
            // 
            // btnDelete
            // 
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(206, 175);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(60, 23);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Text = "DELETE";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnPut
            // 
            this.btnPut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPut.Location = new System.Drawing.Point(140, 175);
            this.btnPut.Name = "btnPut";
            this.btnPut.Size = new System.Drawing.Size(60, 23);
            this.btnPut.TabIndex = 8;
            this.btnPut.Text = "PUT";
            this.btnPut.UseVisualStyleBackColor = true;
            this.btnPut.Click += new System.EventHandler(this.btnPut_Click);
            // 
            // btnPost
            // 
            this.btnPost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPost.Location = new System.Drawing.Point(74, 175);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(60, 23);
            this.btnPost.TabIndex = 7;
            this.btnPost.Text = "POST";
            this.btnPost.UseVisualStyleBackColor = true;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnGet
            // 
            this.btnGet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGet.Location = new System.Drawing.Point(8, 175);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(60, 23);
            this.btnGet.TabIndex = 6;
            this.btnGet.Text = "GET";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMessage.Location = new System.Drawing.Point(9, 59);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(520, 111);
            this.txtMessage.TabIndex = 5;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(6, 40);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(63, 14);
            this.lblMessage.TabIndex = 4;
            this.lblMessage.Text = "Message:";
            // 
            // lblSelect
            // 
            this.lblSelect.AutoSize = true;
            this.lblSelect.Location = new System.Drawing.Point(5, 19);
            this.lblSelect.Name = "lblSelect";
            this.lblSelect.Size = new System.Drawing.Size(56, 14);
            this.lblSelect.TabIndex = 1;
            this.lblSelect.Text = "Select:";
            // 
            // txtSelect
            // 
            this.txtSelect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSelect.Location = new System.Drawing.Point(67, 14);
            this.txtSelect.Name = "txtSelect";
            this.txtSelect.Size = new System.Drawing.Size(235, 22);
            this.txtSelect.TabIndex = 0;
            this.txtSelect.Text = "nodes";
            this.txtSelect.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSelect_KeyDown);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(445, 350);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(542, 350);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblValidator
            // 
            this.lblValidator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValidator.AutoSize = true;
            this.lblValidator.Location = new System.Drawing.Point(26, 350);
            this.lblValidator.Name = "lblValidator";
            this.lblValidator.Size = new System.Drawing.Size(77, 14);
            this.lblValidator.TabIndex = 18;
            this.lblValidator.Text = "Validator:";
            // 
            // lblValidationKey
            // 
            this.lblValidationKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValidationKey.AutoSize = true;
            this.lblValidationKey.Location = new System.Drawing.Point(26, 212);
            this.lblValidationKey.Name = "lblValidationKey";
            this.lblValidationKey.Size = new System.Drawing.Size(112, 14);
            this.lblValidationKey.TabIndex = 17;
            this.lblValidationKey.Text = "Validation Key:";
            // 
            // lblOrganizationUri
            // 
            this.lblOrganizationUri.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOrganizationUri.AutoSize = true;
            this.lblOrganizationUri.Location = new System.Drawing.Point(26, 184);
            this.lblOrganizationUri.Name = "lblOrganizationUri";
            this.lblOrganizationUri.Size = new System.Drawing.Size(126, 14);
            this.lblOrganizationUri.TabIndex = 16;
            this.lblOrganizationUri.Text = "Organization Uri:";
            // 
            // lblOrganization
            // 
            this.lblOrganization.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOrganization.AutoSize = true;
            this.lblOrganization.Location = new System.Drawing.Point(26, 156);
            this.lblOrganization.Name = "lblOrganization";
            this.lblOrganization.Size = new System.Drawing.Size(98, 14);
            this.lblOrganization.TabIndex = 15;
            this.lblOrganization.Text = "Organization:";
            // 
            // lblNodeName
            // 
            this.lblNodeName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNodeName.AutoSize = true;
            this.lblNodeName.Location = new System.Drawing.Point(26, 128);
            this.lblNodeName.Name = "lblNodeName";
            this.lblNodeName.Size = new System.Drawing.Size(77, 14);
            this.lblNodeName.TabIndex = 14;
            this.lblNodeName.Text = "Node Name:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 14);
            this.label2.TabIndex = 13;
            this.label2.Text = "Client.rb:";
            // 
            // lblClientPem
            // 
            this.lblClientPem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClientPem.AutoSize = true;
            this.lblClientPem.Location = new System.Drawing.Point(26, 72);
            this.lblClientPem.Name = "lblClientPem";
            this.lblClientPem.Size = new System.Drawing.Size(84, 14);
            this.lblClientPem.TabIndex = 12;
            this.lblClientPem.Text = "Client.pem:";
            // 
            // txtValidator
            // 
            this.txtValidator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValidator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtValidator.Location = new System.Drawing.Point(156, 348);
            this.txtValidator.Name = "txtValidator";
            this.txtValidator.Size = new System.Drawing.Size(148, 22);
            this.txtValidator.TabIndex = 11;
            // 
            // txtValidationKey
            // 
            this.txtValidationKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValidationKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtValidationKey.Location = new System.Drawing.Point(156, 210);
            this.txtValidationKey.Multiline = true;
            this.txtValidationKey.Name = "txtValidationKey";
            this.txtValidationKey.Size = new System.Drawing.Size(461, 132);
            this.txtValidationKey.TabIndex = 10;
            // 
            // txtOrganizationUri
            // 
            this.txtOrganizationUri.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOrganizationUri.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOrganizationUri.Location = new System.Drawing.Point(156, 182);
            this.txtOrganizationUri.Name = "txtOrganizationUri";
            this.txtOrganizationUri.Size = new System.Drawing.Size(461, 22);
            this.txtOrganizationUri.TabIndex = 9;
            // 
            // txtOrganization
            // 
            this.txtOrganization.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOrganization.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOrganization.Location = new System.Drawing.Point(156, 154);
            this.txtOrganization.Name = "txtOrganization";
            this.txtOrganization.Size = new System.Drawing.Size(293, 22);
            this.txtOrganization.TabIndex = 8;
            // 
            // txtNodeName
            // 
            this.txtNodeName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNodeName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNodeName.Location = new System.Drawing.Point(156, 126);
            this.txtNodeName.Name = "txtNodeName";
            this.txtNodeName.Size = new System.Drawing.Size(148, 22);
            this.txtNodeName.TabIndex = 7;
            // 
            // txtClientRb
            // 
            this.txtClientRb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClientRb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtClientRb.Location = new System.Drawing.Point(156, 98);
            this.txtClientRb.Name = "txtClientRb";
            this.txtClientRb.Size = new System.Drawing.Size(293, 22);
            this.txtClientRb.TabIndex = 6;
            // 
            // txtClientPem
            // 
            this.txtClientPem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClientPem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtClientPem.Location = new System.Drawing.Point(156, 70);
            this.txtClientPem.Name = "txtClientPem";
            this.txtClientPem.Size = new System.Drawing.Size(293, 22);
            this.txtClientPem.TabIndex = 5;
            // 
            // txtClientPath
            // 
            this.txtClientPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClientPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtClientPath.Location = new System.Drawing.Point(156, 42);
            this.txtClientPath.Name = "txtClientPath";
            this.txtClientPath.Size = new System.Drawing.Size(148, 22);
            this.txtClientPath.TabIndex = 4;
            // 
            // lblClientPath
            // 
            this.lblClientPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClientPath.AutoSize = true;
            this.lblClientPath.Location = new System.Drawing.Point(26, 44);
            this.lblClientPath.Name = "lblClientPath";
            this.lblClientPath.Size = new System.Drawing.Size(91, 14);
            this.lblClientPath.TabIndex = 3;
            this.lblClientPath.Text = "Client Path:";
            // 
            // lblClientName
            // 
            this.lblClientName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClientName.AutoSize = true;
            this.lblClientName.Location = new System.Drawing.Point(26, 16);
            this.lblClientName.Name = "lblClientName";
            this.lblClientName.Size = new System.Drawing.Size(91, 14);
            this.lblClientName.TabIndex = 2;
            this.lblClientName.Text = "Client Name:";
            // 
            // txtClientName
            // 
            this.txtClientName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClientName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtClientName.Location = new System.Drawing.Point(156, 14);
            this.txtClientName.Name = "txtClientName";
            this.txtClientName.Size = new System.Drawing.Size(148, 22);
            this.txtClientName.TabIndex = 1;
            // 
            // imgIcons
            // 
            this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
            this.imgIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imgIcons.Images.SetKeyName(0, "node.ico");
            this.imgIcons.Images.SetKeyName(1, "compiler.ico");
            this.imgIcons.Images.SetKeyName(2, "chefapi.ico");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 611);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1198, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel.Text = "Ready";
            // 
            // txtError
            // 
            this.txtError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtError.Enabled = false;
            this.txtError.Location = new System.Drawing.Point(339, 177);
            this.txtError.Multiline = true;
            this.txtError.Name = "txtError";
            this.txtError.Size = new System.Drawing.Size(189, 66);
            this.txtError.TabIndex = 16;
            // 
            // BluApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1198, 633);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BluApp";
            this.Text = "Blu Station";
            this.tabMain.ResumeLayout(false);
            this.tabNodeObject.ResumeLayout(false);
            this.splitContainerNodeObject.Panel1.ResumeLayout(false);
            this.splitContainerNodeObject.Panel2.ResumeLayout(false);
            this.splitContainerNodeObject.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerNodeObject)).EndInit();
            this.splitContainerNodeObject.ResumeLayout(false);
            this.tabCompiler.ResumeLayout(false);
            this.splitContainerCompile.Panel1.ResumeLayout(false);
            this.splitContainerCompile.Panel1.PerformLayout();
            this.splitContainerCompile.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCompile)).EndInit();
            this.splitContainerCompile.ResumeLayout(false);
            this.tabCompileResults.ResumeLayout(false);
            this.tabSyntaxTree.ResumeLayout(false);
            this.tabErrors.ResumeLayout(false);
            this.tabErrors.PerformLayout();
            this.tabCompiledJson.ResumeLayout(false);
            this.tabSprint.ResumeLayout(false);
            this.panelSprint.ResumeLayout(false);
            this.tabResources.ResumeLayout(false);
            this.panelResources.ResumeLayout(false);
            this.tabChefAPI.ResumeLayout(false);
            this.splitContainerChefAPI.Panel1.ResumeLayout(false);
            this.splitContainerChefAPI.Panel1.PerformLayout();
            this.splitContainerChefAPI.Panel2.ResumeLayout(false);
            this.splitContainerChefAPI.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerChefAPI)).EndInit();
            this.splitContainerChefAPI.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabNodeObject;
        private System.Windows.Forms.ImageList imgIcons;
        private System.Windows.Forms.TabPage tabCompiler;
        private System.Windows.Forms.SplitContainer splitContainerCompile;
        private System.Windows.Forms.Label lblRubyScript;
        private System.Windows.Forms.TextBox txtRuby;
        private System.Windows.Forms.TabControl tabCompileResults;
        private System.Windows.Forms.TabPage tabSyntaxTree;
        private System.Windows.Forms.TabPage tabErrors;
        private System.Windows.Forms.TabPage tabSprint;
        private System.Windows.Forms.ListBox lstErrors;
        private System.Windows.Forms.TreeView tvAstNodes;
        private System.Windows.Forms.TextBox txtErrDetails;
        private System.Windows.Forms.SplitContainer splitContainerNodeObject;
        private EPocalipse.Json.Viewer.JsonViewer jvNodeObject;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView runlistTreeView;
        private System.Windows.Forms.TabPage tabChefAPI;
        private System.Windows.Forms.SplitContainer splitContainerChefAPI;
        private System.Windows.Forms.Label lblSelect;
        private System.Windows.Forms.TextBox txtSelect;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnPut;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblClientName;
        private System.Windows.Forms.TextBox txtClientName;
        private System.Windows.Forms.Label lblValidator;
        private System.Windows.Forms.Label lblValidationKey;
        private System.Windows.Forms.Label lblOrganizationUri;
        private System.Windows.Forms.Label lblOrganization;
        private System.Windows.Forms.Label lblNodeName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblClientPem;
        private System.Windows.Forms.TextBox txtValidator;
        private System.Windows.Forms.TextBox txtValidationKey;
        private System.Windows.Forms.TextBox txtOrganizationUri;
        private System.Windows.Forms.TextBox txtOrganization;
        private System.Windows.Forms.TextBox txtNodeName;
        private System.Windows.Forms.TextBox txtClientRb;
        private System.Windows.Forms.TextBox txtClientPem;
        private System.Windows.Forms.TextBox txtClientPath;
        private System.Windows.Forms.Label lblClientPath;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblResponse;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.TextBox txtRunListProperties;
        private System.Windows.Forms.Label lblRunListProperties;
        private EPocalipse.Json.Viewer.JsonViewer jvResponse;
        private System.Windows.Forms.TabPage tabCompiledJson;
        private EPocalipse.Json.Viewer.JsonViewer jvCompiledJson;
        private System.Windows.Forms.Button btnCompile;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.ComboBox cmbSprintMode;
        private System.Windows.Forms.TabPage tabResources;
        private System.Windows.Forms.Panel panelSprint;
        private System.Windows.Forms.WebBrowser txtSprintPs1;
        private System.Windows.Forms.Panel panelResources;
        private System.Windows.Forms.WebBrowser txtResourcesPsm1;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.TextBox txtError;


    }
}