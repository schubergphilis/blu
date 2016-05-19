using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Irony.Compiler;
using Microsoft.Win32;
using BluApi.Chef.ChefAPI;
using BluApi.Common;
using BluStation.BluSprint;
using EPocalipse.Json.Viewer;
using ColorCode;
using ReturnType = BluApi.Common.Function;


namespace BluStation.Forms
{
    public partial class BluApp : Form
    {
        static BluApp()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => Assembly.Load(AssemblyResolver.ResolveAssembly(args));
        }

        public BluApp()
        {
            InitializeComponent();
            InitializeBluApp();

        }

        // ══════════════════════════
        // Form component events
        // ══════════════════════════

        private void txtRuby_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control || (e.KeyCode != Keys.A)) return;
            if (sender != null)
                ((TextBox)sender).SelectAll();
            e.Handled = true;
        }

        private void txtSelect_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                btnGet_Click(this, new EventArgs());
            }
        }

        private void txtPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnGet_Click(this, new EventArgs());
            }
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            Compile();
        }

        private void lstErrors_Click(object sender, EventArgs e)
        {
            txtErrDetails.Text = "";
            SyntaxError se = lstErrors.SelectedItem as SyntaxError;
            if (se == null) return;
            ShowLocation(se.Location, 1);
            txtErrDetails.Text = se.Message + "\r\n (L:C = " + se.Location + ", parser state: " + se.State + ")";
        }

        private void tvAstNodes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowNode(tvAstNodes.SelectedNode);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoadConfig();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            txtError.Text = String.Empty;
            try
            {
                string path = "/";
                if (txtSelect.Text != "") path = txtPath.Text;

                ReturnType rt = ChefEndpoint.Get(txtSelect.Text, path);
                if (rt.Result == 0)
                {
                    jvResponse.Json = rt.Data;
                }
                else
                {
                    txtError.Text = rt.Message;
                }
            }
            catch (Exception ex)
            {
                txtError.Text = "Unable to GET data: \r\n" + ex.Message +
                                  "\r\nCan not find the requested endpoint on the Chef server.";
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            txtError.Text = String.Empty;
            try
            {
                ChefRequest cr = new ChefRequest();
                string nodeName = ChefConfig.NodeName;
                string json = cr.Delete(nodeName, txtSelect.Text);
                jvResponse.Json = json;
                jvResponse.Text = json;
                jvResponse.ShowTab(Tabs.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to DELETE resource: \r\n" + ex.Message + "\r\nCan not find the requested endpoint on the Chef server.",
                    "Chef API Path Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void btnPost_Click(object sender, EventArgs e)
        {
            txtError.Text = String.Empty;
            try
            {
                ChefRequest cr = new ChefRequest();
                string nodeName = ChefConfig.NodeName;
                string json = cr.Post(nodeName, txtSelect.Text, txtMessage.Text);
                jvResponse.Json = json;
                jvResponse.Text = json;
                jvResponse.ShowTab(Tabs.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to POST: \r\n" + ex.Message + "\r\nCan not find the requested endpoint on the Chef server.",
                    "Chef API Path Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnPut_Click(object sender, EventArgs e)
        {
            txtError.Text = String.Empty;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            txtError.Text = String.Empty;
            ExportSoftwareRepository();
        }

        // ══════════════════════════
        // Form methods
        // ══════════════════════════

        private void InitializeBluApp()
        {
            /*
            //setting form to 95% of screen resolution:
            const int _const = 95;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            Size = new Size((screenWidth * _const) / 100, (screenHeight * _const) / 100);
            StartPosition = FormStartPosition.CenterParent;
            */

            // Maximize form
            WindowState = FormWindowState.Maximized;

            // Set selected sprint mode
            cmbSprintMode.Text = "Script";

            // Load node_object from json file
            string json = String.Empty;
            /*
            
            if (File.Exists(@"c:\chef\blu\runtime\node_object.json"))
            {
                json = File.ReadAllText(@"c:\chef\blu\runtime\node_object.json");
                jvNodeObject.Json = json;
                runlistTreeView.Nodes.Add(BuildRegistryTree());
            }
            */
            LoadConfig();
       
        }

        /// <summary>
        /// Compiles Ruby script to Abstract Syntax Tree
        /// </summary>
        private void Compile()
        {
            // Clear view
            lstErrors.Items.Clear();
            tvAstNodes.Nodes.Clear();
            txtSprintPs1.DocumentText = String.Empty;
            txtResourcesPsm1.DocumentText = String.Empty;

            Sprint sprint = new Sprint { Mode = cmbSprintMode.Text };

            if (cmbSprintMode.Text == "Script")
            {
                sprint.RubyStack = txtRuby.Text;
                sprint.Compile();
            }
            else
            {
                sprint.Build();
                txtRuby.Text = sprint.RubyStack;
            }

            const string style = "<STYLE type=\"text/css\">body, div { background-color: #252526 !important; color: #CCC !important; font-size:16px;} </STYLE>";

            if (sprint.SprintPs1Content != null)
            {
                string sprintPs1ContentHighlight =
                    style
                + ReStyle(new CodeColorizer().Colorize(sprint.SprintPs1Content,
                    Languages.PowerShell));
                Application.DoEvents();
                txtSprintPs1.DocumentText = sprintPs1ContentHighlight;
            }


            if (sprint.ResourcesPs1Content != null)
            {
                string resourcesPs1ContentHighlight =
                    style +
                    ReStyle(new CodeColorizer().Colorize(sprint.ResourcesPs1Content,
                    Languages.PowerShell));
                Application.DoEvents();
                txtResourcesPsm1.DocumentText = resourcesPs1ContentHighlight;
            }

            if (sprint.SyntaxErrors.Count > 0)
            {
                foreach (SyntaxError err in sprint.SyntaxErrors)
                {
                    lstErrors.Items.Add(err);
                }
            }
            ShowAstNodes(sprint.AstRootNode);
            tvAstNodes.ExpandAll();
            jvCompiledJson.Json = sprint.CompiledJson;
        }

        private string ReStyle(string input)
        {
            string output = input
                .Replace("color:Green;", "color:#758975;")
                .Replace("color:#A31515;", "color:#8C7166;")
                .Replace("color:OrangeRed;", "color:#778A99;")
                .Replace("color:Blue;", "color:#C6C09B;");
            return output;
        }



        /// <summary>
        /// Adds child nodes to Tree view
        /// </summary>
        /// <param name="parent">Parent tree view node</param>
        /// <param name="name">Child name</param>
        /// <returns></returns>
        private static TreeNode AddChildNode(TreeNode parent, string name)
        {
            var child = new TreeNode(name);
            parent.Nodes.Add(child);
            return child;
        }

        /// <summary>
        /// Adds registry subtree to Tree view
        /// </summary>
        /// <param name="treeRoot">Treeview root node</param>
        /// <param name="registryKeyToAdd">Registry key to show</param>
        private static void AddSubTree(TreeNode treeRoot, RegistryKey registryKeyToAdd)
        {
            var child = AddChildNode(treeRoot, Path.GetFileNameWithoutExtension(registryKeyToAdd.Name));
            foreach (var subKeyName in registryKeyToAdd.GetSubKeyNames())
            {
                try
                {
                    AddSubTree(child, registryKeyToAdd.OpenSubKey(subKeyName));
                }
                catch (Exception ex)
                {
                    AddChildNode(child, string.Format("{0} - Access denied : " + ex.Message, subKeyName));
                }
            }
        }

        /// <summary>
        /// Build Treenode root from registry key
        /// </summary>
        /// <returns>Treeview root node</returns>
        private static TreeNode BuildRegistryTree()
        {
            var root = new TreeNode("Runtime");
            AddSubTree(root, Registry.LocalMachine.OpenSubKey("SOFTWARE\\Blu\\Runtime"));
            return root;
        }

        /// <summary>
        /// Clears Treeview items, then adds AST tree to Treeview
        /// </summary>
        /// <param name="rootNode">Abstract Syntax Tree root node</param>
        private void ShowAstNodes(AstNode rootNode)
        {
            tvAstNodes.Nodes.Clear();
            AddAstNode(null, rootNode);
        }

        /// <summary>
        /// Recursively adds AST nodes to Treeview
        /// </summary>
        /// <param name="parent">Parent Treenode</param>
        /// <param name="node">AST node</param>
        private void AddAstNode(TreeNode parent, AstNode node)
        {
            if (node == null) return;
            var token1 = node as Token;
            if (token1 != null)
            {
                Token token = token1;
                if (token.Terminal.Category != TokenCategory.Content) return;
            }
            string txt = node.ToString();
            TreeNode tn = (parent == null ?
              tvAstNodes.Nodes.Add(txt) : parent.Nodes.Add(txt));
            tn.Tag = node;
            GenericNode genNode = node as GenericNode;
            if (genNode == null) return;
            foreach (AstNode child in genNode.ChildNodes)
                AddAstNode(tn, child);
        }

        /// <summary>
        /// Show the specific position in Textbox (used for syntax errors)
        /// </summary>
        /// <param name="location">Location of the string in the Textbox</param>
        /// <param name="length">Length of the string</param>
        private void ShowLocation(SourceLocation location, int length)
        {
            if (location.Position < 0) return;
            txtRuby.Select(location.Position, length);
            txtRuby.ScrollToCaret();
        }

        /// <summary>
        /// Shows location of the selected AST node inside the text
        /// </summary>
        /// <param name="node">Selected node</param>
        private void ShowNode(TreeNode node)
        {
            if (node == null) return;
            AstNode ast = node.Tag as AstNode;
            if (ast == null) return;
            ShowLocation(ast.Location, 1);
        }

        /// <summary>
        /// Loads configuration from HKLM\Software\Blu\Config
        /// Updates settings tab with this data
        /// </summary>
        private void LoadConfig()
        {
            ChefConfigurator chefConfigurator = new ChefConfigurator();
            chefConfigurator.LoadConfig();
            txtClientName.Text = ChefConfig.ClientName;
            txtClientPath.Text = ChefConfig.ClientPath;
            txtClientPem.Text = ChefConfig.ClientPem;
            txtNodeName.Text = ChefConfig.NodeName;
            txtOrganization.Text = ChefConfig.Organization;
            txtOrganizationUri.Text = ChefConfig.OrganizationUri.ToString();
            txtValidationKey.Text = ChefConfig.ValidationKey;
            txtValidator.Text = ChefConfig.Validator;
        }

        /// <summary>
        /// Saves settings tab to ChefConfig
        /// Updates HKLM\Software\Blu\Config
        /// </summary>
        private void SaveConfig()
        {
            ChefConfigurator chefConfigurator = new ChefConfigurator();
            ChefConfig.ClientName = txtClientName.Text;
            ChefConfig.ClientPath = txtClientPath.Text;
            ChefConfig.ClientPem = txtClientPem.Text;
            ChefConfig.NodeName = txtNodeName.Text;
            ChefConfig.Organization = txtOrganization.Text;
            ChefConfig.OrganizationUri = new Uri(txtOrganizationUri.Text);
            ChefConfig.ValidationKey = txtValidationKey.Text;
            ChefConfig.Validator = txtValidator.Text;
            chefConfigurator.SaveConfig();
            MessageBox.Show("Configuration is saved to HKLM\\Software\\Blu\\Config", "Save Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportSoftwareRepository()
        {
            string folder = String.Empty;
            Dictionary<string, dynamic> nodeList = new Dictionary<string, dynamic>();

            try
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    folder = dialog.SelectedPath;
                    nodeList = (Dictionary<string, dynamic>)ChefEndpoint.Get("nodes", "/").Object;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to Export data: \r\n" + ex.Message + "\r\nCan not find the requested endpoint on the Chef server.",
                    "Chef API Path Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            foreach (var node in nodeList)
            {
                ExportSingleNodeRepository(node.Key, folder);
            }
            MessageBox.Show("Chef endpoint is exported succcsfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void ExportSingleNodeRepository(string node, string folder)
        {
            ReturnType rt = new ReturnType();
            string installedPackages = "";
            try
            {
                rt = ChefEndpoint.Get("nodes/" + node, "default/app_inventory/installed_packages");
                if (rt.Result == 0)
                {
                    installedPackages = rt.Data;
                }
            }
            catch (Exception ex)
            {
                txtError.Text = "Error getting Chef endpoint: " + ex.Message;
            }

            if (!string.IsNullOrEmpty(installedPackages))
            {
                try
                {
                    File.WriteAllText(folder + "\\" + node + ".json", installedPackages);
                }
                catch (Exception ex)
                {
                    txtError.Text = "Error exporting json: " + ex.Message;
                }    
            }
        }
    }
}
