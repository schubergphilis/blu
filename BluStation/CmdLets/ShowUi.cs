using System.Management.Automation;
using BluStation.Forms;

namespace BluStation.CmdLets
{
    /// <summary>
    /// BluStation Show-Runtime CmdLet
    /// </summary>
    [Cmdlet(VerbsCommon.Show, "UI")]
    public class ShowUi : PSCmdlet
    {
        /// <summary>
        /// RuntimeViewer is a Windows form: BluStaion.Forms.RuntimeViewer
        /// </summary>
        static readonly BluApp RuntimeViewer = new BluApp();
        /// <summary>
        /// Record processing shows the Win form as dialog
        /// This win form only returns DialogResult.Cancel when closing the form. That restul is redundant and is ignored.
        /// All usage is handeled in the win form as a Windows GUI application.
        /// </summary>
        protected override void ProcessRecord()
        {
            RuntimeViewer.ShowDialog();
        }
    }
}