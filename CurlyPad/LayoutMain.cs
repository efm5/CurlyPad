using CurlyPad.Properties;

namespace CurlyPad {
   public partial class CurlyPadForm : Form {
      private void LayoutMain() {
         if (!Settings.Default.ShowStatusBar)
            Controls.Remove(statusStrip);
         if (Controls.Contains(fontSizePanel))
            Controls.Remove(fontSizePanel);
         fontSizePanel.Hide();
         fontSizePanel.SendToBack();
         if (Controls.Contains(wheelingVelocityPanel))
            Controls.Remove(wheelingVelocityPanel);
         wheelingVelocityPanel.Hide();
         wheelingVelocityPanel.SendToBack();
         if (Controls.Contains(findPanel))
            Controls.Remove(findPanel);
         findPanel.Hide();
         findPanel.SendToBack();
         if (Controls.Contains(recentFilesHistoryPanel))
            Controls.Remove(recentFilesHistoryPanel);
         recentFilesHistoryPanel.Hide();
         recentFilesHistoryPanel.SendToBack();
         if (Controls.Contains(replacePanel))
            Controls.Remove(replacePanel);
         replacePanel.Hide();
         replacePanel.SendToBack();
         if (Controls.Contains(goPanel))
            Controls.Remove(goPanel);
         goPanel.Hide();
         goPanel.SendToBack();
         if (Controls.Contains(commentPreferencesPanel))
            Controls.Remove(commentPreferencesPanel);
         commentPreferencesPanel.Hide();
         commentPreferencesPanel.SendToBack();
         if (Controls.Contains(createThemePanel))
            Controls.Remove(createThemePanel);
         createThemePanel.Hide();
         createThemePanel.SendToBack();
         if (Controls.Contains(pickThemePanel))
            Controls.Remove(pickThemePanel);
         pickThemePanel.Hide();
         pickThemePanel.SendToBack();
         if (Controls.Contains(editThemePanel))
            Controls.Remove(editThemePanel);
         editThemePanel.Hide();
         editThemePanel.SendToBack();
         if (!showCommentMenuTSMI.Checked) {
            menuStrip.Items.Remove(commentTSMI);
            toolStrip.Items.Remove(commentAddDoubleTSB);
            toolStrip.Items.Remove(commentAddTripleTSB);
            toolStrip.Items.Remove(commentSummaryTSB);
            toolStrip.Items.Remove(cPlusPlusTSB);
            toolStrip.Items.Remove(commentRemoveTSB);
            toolStrip.Items.Remove(commentWrapTSB);
            toolStrip.Items.Remove(commentUpdateTSB);
            toolStrip.Items.Remove(commentOutTSB);
            toolStrip.Items.Remove(commentGetTSB);
            toolStrip.Items.Remove(commentReplaceTSB);
            toolStrip.Items.Remove(commentPreferencesTSB);
         }
         WrapTextBox();
         menuStrip.Dock = DockStyle.Top;
         //DEBUG efm5 2024 12 22 this does not look right
         //if (Settings.Default.ToolBarLocation == (int)ToolBarSize.Twenty_Four)
         //   toolStrip.ImageScalingSize = new Size(24, 24);
         //else if (Settings.Default.ToolBarLocation == (int)ToolBarSize.Thirty_Two)
         //   toolStrip.ImageScalingSize = new Size(32, 32);
         //else if (Settings.Default.ToolBarLocation == (int)ToolBarSize.Sixty_Four)
         //   toolStrip.ImageScalingSize = new Size(64, 64);
         //else if (Settings.Default.ToolBarLocation == (int)ToolBarSize.One_Hundred_Twenty_Eight)
         //   toolStrip.ImageScalingSize = new Size(128, 128);
         //else
         //   toolStrip.ImageScalingSize = new Size(16, 16);
         if (showToolBarTSMI.Checked) {
            HandleTools(toolBarShowToolsTSMI.Checked);
            HandleScrollers(toolBarShowScrollersTSMI.Checked);
         }
         else {
            toolStrip.Hide();
            toolStrip.SendToBack();
         }
         if (Settings.Default.ShowStatusBar) {
            Controls.Add(statusStrip);
            statusStrip.Dock = DockStyle.Bottom;
         }
         if (Settings.Default.FindScopeGlobal)
            findGlobalRadioButton.Checked = true;
         else
            findSelectionRadioButton.Checked = true;
         if (Settings.Default.ReplaceScopeGlobal)
            replaceGlobalRadioButton.Checked = true;
         else
            replaceSelectionRadioButton.Checked = true;
         ColorizeGui();
         MakePrefixes();
         UpdateStatusBar();
         CheckCurrentTheme();
         TopMost = alwaysOnTopTSMI.Checked;
         if (!string.IsNullOrEmpty(sArgument))
            LoadFile(sArgument);
         ResizeTextBox();
         HardFocus();
         if (Settings.Default.Fade)
            FadeIn();
         else
            Opacity = 1;
      }
   }
}