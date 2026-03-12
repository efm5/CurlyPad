using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using CurlyPad.Properties;
using IWshRuntimeLibrary;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using static CurlyPad.Program.NativeMethods;

namespace CurlyPad {
   public partial class CurlyPadForm : Form {
      #region event support
      private static void TrimToBeginning() {
         textBox.Select(0, textBox.SelectionStart);
         textBox.Cut();
      }

      private static void TrimToEnd() {
         textBox.Select(textBox.SelectionStart, textBox.TextLength);
         textBox.Cut();
      }

      private static void LineEndingNN2RN() {
         if (string.IsNullOrEmpty(textBox.Text))
            return;
         string content = string.Empty;

         AllIfNothing();
         content = textBox.SelectedText;
         textBox.SelectedText = content.Replace("\n\n", "\r\n\r\n");
      }

      private static void LineEndingN2RN() {
         if (string.IsNullOrEmpty(textBox.Text))
            return;
         string content = string.Empty;

         AllIfNothing();
         content = textBox.SelectedText;
         textBox.SelectedText = content.Replace("\n", "\r\n");
      }

      private static void LineEndingNR2RN() {
         if (string.IsNullOrEmpty(textBox.Text))
            return;
         string content = string.Empty;

         AllIfNothing();
         content = textBox.SelectedText;
         textBox.SelectedText = content.Replace("\n\r", "\r\n");
      }

      private static void LineEndingRN2NN() {
         if (string.IsNullOrEmpty(textBox.Text))
            return;
         string content = string.Empty;

         AllIfNothing();
         content = textBox.SelectedText;
         textBox.SelectedText = content.Replace("\r\n", "\n\n");
      }

      private static void LineEndingRN2RNRN() {
         if (string.IsNullOrEmpty(textBox.Text))
            return;
         string content = string.Empty;

         AllIfNothing();
         content = textBox.SelectedText;
         textBox.SelectedText = content.Replace("\r\n", "\r\n\r\n");
      }

      private void WrapTextBox() {
         textBox.WordWrap = wordWrapTSMI.Checked;
         if (wordWrapTSMI.Checked) {
            textBox.ScrollBars = ScrollBars.Vertical;
         }
         else
            textBox.ScrollBars = ScrollBars.Both;
      }

      public static string SplitToLines(string pStringToSplit, int pMaximumLineLength) {
         return Regex.Replace(pStringToSplit, @"(.{1," + pMaximumLineLength + @"})(?:\s|$)", "$1\r\n");
      }

      private static void Help() {
         try {
            if (!System.IO.File.Exists(sHelpFolderLocation + @"Curly Pad Help.html")) {
               TimedMessage("“Curly Pad Help.html” could not be found", "Missing Help File", 0);
               return;
            }
            Process process = new Process {
               StartInfo = new ProcessStartInfo(sHelpFolderLocation + @"Curly Pad Help.html") {
                  UseShellExecute = true
               }
            };
            process.Start();
            Thread.Sleep(100);
         }
         catch (Exception pException) {
            TimedMessage("Help threw an exception" + Environment.NewLine + pException.ToString(),
               "ERROR", 0);
         }
      }

      public static void SetZoom(float pSize) {
         try {
            if (pSize < 8)
               pSize = 8;
            Settings.Default.Zoom = pSize;
            textBox.Font = new System.Drawing.Font(textBox.Font.Name, pSize, textBox.Font.Style);
         }
         catch (Exception pException) {
            TimedMessage("SetZoom(float pSize) threw an exception:" + Environment.NewLine + pException.ToString(),
               "EXCEPTION", 0);
         }
      }

      public static void SetZoom(string pInOut) {
         try {
            float currentSize = textBox.Font.SizeInPoints;
            if (pInOut == "+")
               currentSize += 1f;
            else {
               currentSize -= 1f;
               if (currentSize < 8f) {
                  currentSize = 8f;
               }
            }
            textBox.Font = new System.Drawing.Font(textBox.Font.Name, currentSize, textBox.Font.Style);
            Settings.Default.Zoom = currentSize;
            sZoomLevel = currentSize;
         }
         catch (Exception pException) {
            TimedMessage("SetZoom(string pInOut) threw an exception:" + Environment.NewLine + pException.ToString(),
               "EXCEPTION", 0);
         }
      }

      private void LoadFileHistory() {
         try {
            string[] files = System.IO.File.ReadAllLines(sRecentFileHistoryPath);
            List<string> original = files.ToList();
            int index = 0, originalEntries = files.Length;
            bool removed = false;
            foreach (string file in files) {
               if (System.IO.File.Exists(file)) {
                  ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem() {
                     Text = file,
                     ForeColor = Settings.Default.InterfaceFontColor,
                     BackColor = Settings.Default.InterfaceBackgroundColor
                  };
                  toolStripMenuItem.Click += new EventHandler(RecentFileTSMI_Click);
                  openRecentTSMI.DropDownItems.Add(toolStripMenuItem);
               }
               else {
                  original.RemoveAt(index--);
                  removed = true;
               }
               index++;
            }
            if (original.Count > Settings.Default.RecentFileHistoryLimit) {
               int redundant = original.Count - Settings.Default.RecentFileHistoryLimit;
               for (int i = 0; i < redundant; i++)
                  original.RemoveAt(0);
               removed = true;
            }
            if (removed) {
               System.IO.File.WriteAllLines(sRecentFileHistoryPath, original.ToArray());
            }
         }
         catch (Exception pException) {
            TimedMessage("LoadFileHistory threw an exception" + Environment.NewLine + pException.ToString(), "EXCEPTION", 0);
         }
      }

      private void LoadFile(string pFullyQualifiedPath) {
         try {
            if (string.Equals(".lnk", Path.GetExtension(pFullyQualifiedPath))) {
               WshShell shell = new WshShell(); //Create a new WshShell Interface
               IWshShortcut link = (IWshShortcut)shell.CreateShortcut(pFullyQualifiedPath); //Link the interface to our shortcut
               pFullyQualifiedPath = link.TargetPath;
            }
            if (System.IO.File.Exists(pFullyQualifiedPath)) {
               int filterIndex = 2;//efm5 filterIndex represents a 1-starting list and we are skipping the first item (all files)
               foreach (string phrase in sAcceptableExtensions) {
                  string extension = Path.GetExtension(pFullyQualifiedPath).ToLower();
                  if (string.Equals(phrase, extension)) {
                     sDefaultExtension = extension.Replace(".", string.Empty);
                     sOpenedFilterIndex = filterIndex;
                     textBox.Text = System.IO.File.ReadAllText(pFullyQualifiedPath);
                     HandleWordwrap();
                     textBox.SelectionStart = 0;
                     textBox.SelectionLength = 0;
                     sCurrentFile = pFullyQualifiedPath;
                     textBox.Modified = false;
                     sImportedText = textBox.Text;
                     SetWindowTitle();
                     Directory.CreateDirectory(sMyDocumentsCurlyPadData);
                     using (FileStream fileStream = new FileStream(sRecentFileHistoryPath, FileMode.Append,
                        FileAccess.Write))
                     using (StreamWriter streamWriter = new StreamWriter(fileStream)) {
                        streamWriter.WriteLine(pFullyQualifiedPath);
                     }
                     string[] lines = System.IO.File.ReadAllLines(sRecentFileHistoryPath);
                     System.IO.File.WriteAllLines(sRecentFileHistoryPath, lines.Distinct().ToArray());
                     if (!MenuContains(openRecentTSMI, pFullyQualifiedPath)) {
                        ToolStripMenuItem tsmi = new ToolStripMenuItem() {
                           Text = pFullyQualifiedPath,
                           ForeColor = sCurrentTheme.mInterfaceFontColor,
                           BackColor = sCurrentTheme.mInterfaceBackgroundColor
                        };
                        tsmi.Click += RecentFileTSMI_Click;
                        openRecentTSMI.DropDownItems.Add(tsmi);
                     }
                     return;
                  }
                  filterIndex++;
               }
            }
            else
               TimedMessage(string.Format("LoadFile failed while trying to load a file: {0}{1}{0}" +
                        "The file does not appear to exist.", Environment.NewLine, sCurrentFile),
                    "File ERROR", 0);
         }
         catch (Exception pException) {
            TimedMessage("LoadFile threw an exception" + Environment.NewLine + pException.ToString(), "EXCEPTION", 0);
         }
      }

      private static bool MenuContains(ToolStripMenuItem pToolStripMenuItem, string pText) {
         foreach (ToolStripMenuItem toolStripMenuItem in pToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
            if (string.Equals(toolStripMenuItem.Text, pText, StringComparison.OrdinalIgnoreCase))
               return true;
         return false;
      }

      private bool OpenFile() {
         try {
            openFileDialog.Title = sAppName + " - Open File";
            openFileDialog.DefaultExt = sDefaultExtension;
            openFileDialog.Filter = sExtensionsFilter;
            openFileDialog.FilterIndex = sOpenedFilterIndex;
            openFileDialog.FileName = string.Empty;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
               LoadFile(openFileDialog.FileName);
            return true;
         }
         catch (Exception pException) {
            TimedMessage("OpenFile threw an exception" + Environment.NewLine + pException.ToString(), "EXCEPTION", 0);
         }
         return false;
      }

      private bool SaveFile(string pFullyQualifiedPath, TextEncoding pEncoding = TextEncoding.Unicode) {
         Encoding encoding = Encoding.Unicode;

         if (string.IsNullOrEmpty(pFullyQualifiedPath)) {
            textBox.Modified = !SaveAs();
            return false;
         }
         switch (pEncoding) {
            case TextEncoding.ASCII:
               encoding = Encoding.ASCII;
               break;
            //case TextEncoding.UTF7://efm5 UTF7 is depreciated
            //   encoding = Encoding.UTF7;
            //   break;
            case TextEncoding.UTF8:
               encoding = Encoding.UTF8;
               break;
            case TextEncoding.UTF16BE:
               encoding = Encoding.BigEndianUnicode;
               break;
            case TextEncoding.UTF32:
               encoding = Encoding.UTF32;
               break;
               //case TextEncoding.UTF64://efm5 UTF64 Is defined but probably not currently used
               //   encoding = Encoding.UTF64;
               //   break;
         }
         try {
            if (string.Equals(Path.GetExtension(pFullyQualifiedPath), ".vbs", StringComparison.OrdinalIgnoreCase))
               encoding = Encoding.ASCII;
            using (StreamWriter writer = new StreamWriter(pFullyQualifiedPath, false, encoding)) {
               try {
                  writer.Write(textBox.Text);
                  sImportedText = textBox.Text;
                  SetWindowTitle();
                  return true;
               }
               catch (Exception pException) {
                  TimedMessage("SaveFile (interior) failed while trying to save a file:" + Environment.NewLine +
                  sCurrentFile + Environment.NewLine + pException.ToString(), "EXCEPTION", 0);
               }
            }
         }
         catch (Exception pException) {
            TimedMessage("SaveFile (exterior) failed while trying to save a file:" + Environment.NewLine +
            sCurrentFile + Environment.NewLine + pException.ToString(), "EXCEPTION", 0);
         }
         return false;
      }

      private bool SaveAs() {
         try {
            if (CommonFileDialog.IsPlatformSupported) {
               sTextEncoding = TextEncoding.Unicode;
               CommonSaveFileDialog saveDialog = new CommonSaveFileDialog();
               if (string.IsNullOrEmpty(sCurrentFile)) {
                  saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                  saveDialog.DefaultFileName = string.Empty;
                  saveDialog.DefaultExtension = sDefaultExtension;
               }
               else {
                  saveDialog.InitialDirectory = Path.GetDirectoryName(sCurrentFile);
                  saveDialog.DefaultFileName = Path.GetFileName(sCurrentFile);
                  saveDialog.DefaultExtension = Path.GetExtension(sCurrentFile);
               }
               saveDialog.Title = "Save File";
               foreach (string filter in sFilterExtensionList) {
                  string[] pair = filter.Split('|');
                  saveDialog.Filters.Add(new CommonFileDialogFilter(pair[0], pair[1]));
               }

               CommonFileDialogRadioButtonList list = new CommonFileDialogRadioButtonList("radioButtonOptions");
               list.SelectedIndexChanged += SaveAsDialogEncoding_SelectedIndexChanged;

               CommonFileDialogRadioButtonListItem ascii = new CommonFileDialogRadioButtonListItem("ASCII"),
                  utf8 = new CommonFileDialogRadioButtonListItem("UTF8"),
                  unicode = new CommonFileDialogRadioButtonListItem("Unicode");

               list.Items.Add(ascii);
               list.Items.Add(utf8);
               list.Items.Add(unicode);
               switch ((TextEncoding)Settings.Default.TextEncoding) {
                  case TextEncoding.ASCII:
                     list.SelectedIndex = 0;
                     break;
                  case TextEncoding.UTF8:
                     list.SelectedIndex = 1;
                     break;
                  //case TextEncoding.Unicode:
                  default:
                     list.SelectedIndex = 2;
                     break;
               }
               saveDialog.Controls.Add(list);
               if ((saveDialog.ShowDialog() == CommonFileDialogResult.Ok) && !string.IsNullOrEmpty(saveDialog.FileName)) {
                  sCurrentFile = saveDialog.FileName;
                  int selectedType = saveDialog.SelectedFileTypeIndex;
                  string extension = Path.GetExtension(saveDialog.FileName), wantedExtension = String.Empty;
                  if (saveDialog.SelectedFileTypeIndex > 1)//efm5 SelectedFileTypeIndex is a 1-based index
                     wantedExtension = "." + sFilterExtensionList[saveDialog.SelectedFileTypeIndex - 1].Split('.').Last();
                  else
                     wantedExtension = ".txt";
                  sCurrentFile = Path.ChangeExtension(sCurrentFile, wantedExtension);
                  textBox.Modified = !SaveFile(sCurrentFile, sTextEncoding);
                  sImportedText = textBox.Text;
                  SetWindowTitle();
                  return true;
               }
            }
            else
               TimedMessage("SaveAs CommonFileDialog.IsPlatformSupported failed.", "Save File ERROR", 3500);
         }
         catch (Exception pException) {
            TimedMessage("SaveAs threw an exception" + Environment.NewLine + pException.ToString(), "EXCEPTION", 0);
         }
         return false;
      }

      private void Save() {
         if (textBox.Text.Length == 0) {
            DialogResult dialogResult = MessageBox.Show("The text box is empty. If you save now the resulting file will also be empty." +
               Environment.NewLine + "Do you really want to save an empty file?",
                "Save Empty File?", MessageBoxButtons.YesNoCancel);
            if ((dialogResult == DialogResult.No) || (dialogResult == DialogResult.Cancel))
               return;
         }
         if (string.IsNullOrEmpty(sCurrentFile))
            textBox.Modified = !SaveAs();
         else
            textBox.Modified = !SaveFile(sCurrentFile);
      }

      private void Print(bool pPrintSelection = false) {
         try {
            bool goodToGo = true;
            if (sCurrentTheme.mTextBoxBackgroundColor != Color.White) {
               if (MessageBox.Show(string.Format("The current background color is not white; " +
                  "printing will be {0} (the text color) on a white background.", sCurrentTheme.mTextBoxFontColor) +
                  Environment.NewLine + Environment.NewLine +
                  "Do you wish to continue?",
                  "Decision Time",
                  MessageBoxButtons.YesNo) == DialogResult.No)
                  goodToGo = false;
            }
            if (goodToGo) {
               if ((textBox.SelectionLength > 0) && pPrintSelection)
                  sTextToPrint = textBox.SelectedText;
               else
                  sTextToPrint = textBox.Text;
               printDialog.Document = printDocument;
               if (printDialog.ShowDialog() == DialogResult.OK)
                  printDocument.Print();
            }
         }
         catch (Exception pException) {
            TimedMessage("Print threw an exception." + Environment.NewLine + pException.ToString(), "ERROR", 0);
         }
      }

      private static void New() {
         if (string.IsNullOrEmpty(textBox.Text))
            return;
         DialogResult dialogResult = MessageBox.Show("The text box is not empty." +
            Environment.NewLine + "Do you really want to empty it?",
             "Empty TextBox?", MessageBoxButtons.YesNoCancel);
         if (dialogResult == DialogResult.Yes)
            textBox.Text = string.Empty;
      }
      #endregion

      private void MakePrefixes() {
         FlatButton(findWhatPrefixButton);
         FlatButton(searchSearchPrefixButton);
         FlatButton(searchReplacePrefixButton);
         FlatButton(goToPrefixButton);
      }

      private void UpdateStatusBar() {
         int newLines = 0, wordCount, lineCount, characterCount = 0;
         string contents = textBox.Text;
         string[] words = new string[] { };

         if (!string.IsNullOrEmpty(textBox.Text)) {
            foreach (char ch in textBox.Text)
               if ((ch != '\n') && (ch != '\r'))
                  characterCount++;
            foreach (char ch in contents)
               if (ch == '\n')
                  newLines++;
            contents = contents.Replace("\r", string.Empty);
            contents = contents.Replace('\n', ' ');
            contents = contents.Trim();
            words = contents.Split(' ');
         }
         wordCount = words.Length;
         foreach (string phrase in words) {
            if (string.IsNullOrEmpty(phrase))
               wordCount--;
         }
         lineCount = textBox.Lines.Length;
         if (lineCount == 0)
            lineCount = 1;
         int characterPosition = textBox.SelectionStart,
            currentLine = textBox.GetLineFromCharIndex(textBox.SelectionStart) + 1,
            linePercent = (int)Math.Ceiling(((float)currentLine / (float)lineCount) * 100f),
            characterPercent = 0;
         if (characterCount != 0)
            characterPercent = (int)Math.Ceiling(((float)characterPosition / (float)textBox.Text.Length) * 100f);
         charactersStatusLabel.Text = string.Format("Characters: {0}", characterCount);
         wordsStatusLabel.Text = string.Format("  Words: {0}", wordCount);
         string linesParagraph = "Lines";
         if (wordWrapTSMI.Checked)
            linesParagraph = "Paragraphs";
         linesStatusLabel.Text = string.Format("  {0}: {1}", linesParagraph, lineCount);
         positionStatusLabel.Text = string.Format("Position: Line #{0}; = {1}%; Character #{2} = overall {3}%",
            currentLine, linePercent, characterPosition, characterPercent);
      }

      private void SetWindowTitle() {
         string title = string.Empty;

         if (!string.IsNullOrEmpty(sCurrentFile))
            title = Path.GetFileNameWithoutExtension(sCurrentFile) + " - ";
         else
            title = "untitled – ";

         if ((textBox.Modified == true) && (textBox.Text.Length > 0) &&
            !string.Equals(textBox.Text, sImportedText, StringComparison.Ordinal))
            title = "* " + title;
         Text = title + sWindowTitle;
      }

      public static void TimedMessage(string pMessage, string pTitle = "", int pDuration = 4500) {
         IntPtr handle = IntPtr.Zero;

         if (sForm != null)
            handle = sForm.Handle;
         _ = MessageBoxTimeout(handle, pMessage, pTitle, TIMED_MESSAGEBOX_FLAGS, 0, pDuration);
      }

      private static void HandleWordwrap() {
         if (Settings.Default.WordWrap) {
            textBox.WordWrap = true;
            textBox.ScrollBars = ScrollBars.Vertical;
         }
         else {
            textBox.WordWrap = false;
            textBox.ScrollBars = ScrollBars.Both;
         }
      }

      private void RestoreMain() {
         if (sNeedsSizing)
            RestoreWindow();
         ControlBox = true;
         menuStrip.Enabled = true;
         textBox.Enabled = true;
         textBox.Focus();
      }

      private void DisableMain() {
         ControlBox = false;
         menuStrip.Enabled = false;
         textBox.Enabled = false;
      }

      private void FitToDialog() {
         int taller = menuStrip.Height + statusStrip.Height, wider = SystemInformation.VerticalScrollBarWidth;
         const float high = 1.3f, wide = 1.1f;

         sNeedsSizing = false;
         switch (sEscapeFrom) {
            case EscapeFrom.Find:
               if (Width < ((int)(findPanel.Width * wide) + wider)) {
                  Width = (int)(findPanel.Width * wide) + wider;
                  sNeedsSizing = true;
               }
               if (Height < ((int)(findPanel.Height * wide) + taller)) {
                  Height = (int)(findPanel.Height * high) + taller;
                  sNeedsSizing = true;
               }
               break;
            case EscapeFrom.Replace:
               if (Width < ((int)(replacePanel.Width * wide) + wider)) {
                  Width = (int)(replacePanel.Width * wide) + wider;
                  sNeedsSizing = true;
               }
               if (Height < ((int)(replacePanel.Height * wide) + taller)) {
                  Height = (int)(replacePanel.Height * high) + taller;
                  sNeedsSizing = true;
               }
               break;
            //case EscapeFrom.Go:
            default:
               if (Width < ((int)(goPanel.Width * wide) + wider)) {
                  Width = (int)(goPanel.Width * wide) + wider;
                  sNeedsSizing = true;
               }
               if (Height < ((int)(goPanel.Height * wide) + taller)) {
                  Height = (int)(goPanel.Height * high) + taller;
                  sNeedsSizing = true;
               }
               break;
         }
      }

      private void HardFocus() {
         TopMost = true;
         Show();
         BringToFront();
         Activate();
         textBox.Show();
         textBox.BringToFront();
         textBox.Enabled = true;
         _ = textBox.Focus();
         textBox.Refresh();
         ActiveControl = textBox;
         TopMost = Settings.Default.AlwaysOnTop;
      }

      private bool AllIndexesOf(string pInput, string pSearchingFor) {
         int index = 0;
         bool success = false;

         sFindIndexes.Clear();
         if (string.IsNullOrEmpty(pInput) || string.IsNullOrEmpty(pSearchingFor)) {
            TimedMessage("pInput or pSearchingFor is null or empty.", "FAILURE", 3000);
            return false;
         }
         StringComparison comparisonMode =
            findMatchCaseCheckBox.Checked ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
         while ((index = pInput.IndexOf(pSearchingFor, index, comparisonMode)) != -1) {
            sFindIndexes.Add(index++);
            success = true;
         }
         return success;
      }

      private void RememberWindow() {
         sOriginalSize = Size;
         sOriginalLocation = Location;
         if (textBox.ScrollBars != ScrollBars.None) {
            sTextBoxScrollbars = textBox.ScrollBars;
            textBox.ScrollBars = ScrollBars.None;
         }
      }

      private void RestoreWindow() {
         Size = sOriginalSize;
         Location = sOriginalLocation;
         textBox.ScrollBars = sTextBoxScrollbars;
      }

      private static void ActivateVisualStudio() {
         try {
            Process process = null;
            IntPtr vsHandle = IntPtr.Zero;
            Process[] processList = Process.GetProcesses();

            foreach (Process theprocess in processList) {
               if (string.Equals(theprocess.ProcessName, "devenv", StringComparison.OrdinalIgnoreCase)) {
                  process = theprocess;
                  break;
               }
            }
            if (process != null) {
               vsHandle = process.MainWindowHandle;
               if (!SetForegroundWindow(vsHandle))
                  TimedMessage("SetForegroundWindow failed", "ERROR", 3000);
               Thread.Sleep(50);
               _ = SetActiveWindow(vsHandle);
               Thread.Sleep(20);//efm5 Possibly unnecessary
            }
            else
               TimedMessage("GetProcessesByName failed", "ERROR", 3000);
         }
         catch (Exception pException) {
            TimedMessage("Exception caught; the error message is:" + Environment.NewLine + pException.ToString(),
               "EXCEPTION CAUGHT", 0);
         }
      }

      private void RespondToIconSizing(ToolStripMenuItem pToolStripMenuItem) {
         if (int.TryParse(pToolStripMenuItem.Tag as string, out int oInteger)) {
            RespondToIconSize(oInteger);
            ResizeTextBox();
         }
         else
            TimedMessage("RespondToIconSizing – could not parse the pToolStripMenuItem.Tag", "TryParse ERROR");
      }

      private void RespondToIconSize(int pSize) {
         sToolbarScale = pSize;
         toolBarSize16TSMI.Click -= ToolBarSize16TSMI_Click;
         toolBarSize24TSMI.Click -= ToolBarSize24TSMI_Click;
         toolBarSize32TSMI.Click -= ToolBarSize32TSMI_Click;
         toolBarSize64TSMI.Click -= ToolBarSize64TSMI_Click;
         toolBarSize128TSMI.Click -= ToolBarSize128TSMI_Click;
         toolBarSize16TSMI.Checked = false;
         toolBarSize24TSMI.Checked = false;
         toolBarSize32TSMI.Checked = false;
         toolBarSize64TSMI.Checked = false;
         toolBarSize128TSMI.Checked = false;
         switch (pSize) {
            case 16:
            default:
               toolBarSize16TSMI.Checked = true;
               break;
            case 24:
               toolBarSize24TSMI.Checked = true;
               break;
            case 32:
               toolBarSize32TSMI.Checked = true;
               break;
            case 64:
               toolBarSize64TSMI.Checked = true;
               break;
            case 128:
               toolBarSize128TSMI.Checked = true;
               break;
         }
         toolBarSize16TSMI.Click += ToolBarSize16TSMI_Click;
         toolBarSize24TSMI.Click += ToolBarSize24TSMI_Click;
         toolBarSize32TSMI.Click += ToolBarSize32TSMI_Click;
         toolBarSize64TSMI.Click += ToolBarSize64TSMI_Click;
         toolBarSize128TSMI.Click += ToolBarSize128TSMI_Click;
         foreach (ToolStripButton tsmi in toolStrip.Items.OfType<ToolStripButton>())
            tsmi.Size = new Size(pSize, pSize);
         toolStrip.ImageScalingSize = new Size(pSize, pSize);
         toolStrip.Refresh();
         if (Settings.Default.Fade)
            FadeOut();
         if (WindowState == FormWindowState.Maximized) {
            WindowState = FormWindowState.Normal;
            Width++;
            Width--;
            WindowState = FormWindowState.Maximized;
         }
         else {
            Width++;
            Width--;
         }
         ResizeTextBox();
         if (Settings.Default.Fade)
            FadeIn(Settings.Default.Opacity);
      }

      private void ResizeTextBox() {
         int top = menuStrip.Bottom, heightSpacer = SystemInformation.HorizontalScrollBarHeight + 15,
            widthSpacer = 0, leftSpacer = 0;

         if (Controls.Contains(toolStrip)) {
            switch (toolStrip.Dock) {
               case DockStyle.Top:
               default:
                  top = toolStrip.Bottom;
                  heightSpacer += toolStrip.Height;
                  break;
               case DockStyle.Bottom:
                  heightSpacer += toolStrip.Height;
                  break;
               case DockStyle.Left:
                  widthSpacer += toolStrip.Width;
                  leftSpacer = widthSpacer;
                  break;
               case DockStyle.Right:
                  widthSpacer += toolStrip.Width;
                  break;
            }

         }
         if (Controls.Contains(statusStrip))
            heightSpacer += statusStrip.Height;
         textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
         textBox.Location = new Point(leftSpacer, top);
         textBox.Size = new Size(ClientSize.Width - widthSpacer, ClientSize.Height - heightSpacer);
         textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
      }

      private void GetExternalFont(FontUsage pFontUsage) {
         string fontName = textBox.Font.Name, monoProportional = "Proportional";
         float size = textBox.Font.SizeInPoints;

         sFontUsage = pFontUsage;
         if (pFontUsage == FontUsage.Interface) {
            fontName = menuStrip.Font.Name;
            size = menuStrip.Font.SizeInPoints;
         }
         else if (pFontUsage == FontUsage.TextBoxMono)
            monoProportional = "Monospace";
         try {
            if (System.IO.File.Exists(sAppFolder + "ExternalFontPicker.exe")) {
               Process process = new Process {
                  StartInfo = new ProcessStartInfo(sAppFolder + "ExternalFontPicker.exe") {
                     UseShellExecute = true,
                     Arguments = fontName.Replace(" ", "*") + "~" +
                        string.Format("{0}", (int)size) + "`" + monoProportional
                  }
               };
               process.Start();
               Thread.Sleep(100);
            }
            else
               TimedMessage("“ExternalFontPicker.exe” Could not be found on disc.",
                  "FILE ERROR", 5000);
         }
         catch (Exception pException) {
            TimedMessage(
               "GetExternalFont threw an exception" + Environment.NewLine + pException.ToString(),
               "ERROR", 0);
         }
      }

      private void GetExternalColor(ColorUsage pColorUsage) {
         string argument = string.Empty;
         Color current = Color.White;

         sColorUsage = pColorUsage;
         if (pColorUsage == ColorUsage.Text)
            current = textBox.ForeColor;
         if (pColorUsage == ColorUsage.TextBackground)
            current = textBox.BackColor;
         else if (pColorUsage == ColorUsage.InterfaceText)
            current = menuStrip.ForeColor;
         else if (pColorUsage == ColorUsage.InterfaceBackground)
            current = menuStrip.BackColor;
         else if (pColorUsage == ColorUsage.StatusBarBackground)
            current = statusStrip.BackColor;
         else if (pColorUsage == ColorUsage.DialogBackground)
            current = goPanel.BackColor;
         if (!IsKnownColor(current))
            current = Color.White;

         argument = current.Name.Replace(" ", string.Empty) + "`" +
            menuStrip.Font.Name.Replace(" ", string.Empty) + "`" +
            string.Format("{0}", (int)menuStrip.Font.SizeInPoints);

         try {
            if (System.IO.File.Exists(sAppFolder + "ExternalColorPicker.exe")) {
               Process process = new Process {
                  StartInfo = new ProcessStartInfo(sAppFolder + "ExternalColorPicker.exe") {
                     UseShellExecute = true,
                     Arguments = argument
                  }
               };
               process.Start();
               Thread.Sleep(100);
            }
            else
               TimedMessage("“ExternalColorPicker.exe” Could not be found on disc.",
                  "FILE ERROR", 5000);
         }
         catch (Exception pException) {
            TimedMessage(
               "GetExternalColor threw an exception" + Environment.NewLine + pException.ToString(),
               "ERROR", 0);
         }
      }

      private static void AllIfNothing() {
         if ((Settings.Default.AllIfNothing) && string.IsNullOrEmpty(textBox.SelectedText))
            textBox.SelectAll();
      }

      private static void PaintButton(ToolStripButton pButton) {
         pButton.Font = CreateNewFont(sCurrentTheme.mInterfaceFont);
         pButton.ForeColor = sCurrentTheme.mInterfaceFontColor;
         pButton.BackColor = sCurrentTheme.mInterfaceBackgroundColor;
      }

      private void HandleTools(bool pAdd) {
         toolBarTopTSMI.Checked = false;
         toolBarBottomTSMI.Checked = false;
         toolBarLeftTSMI.Checked = false;
         toolBarRightTSMI.Checked = false;
         if (pAdd) {
            switch ((ToolBarLocation)Settings.Default.ToolBarLocation) {
               default:
                  //case ToolBarLocation.Top:
                  toolStrip.Dock = DockStyle.Top;
                  toolBarTopTSMI.Checked = true;
                  break;
               case ToolBarLocation.Bottom:
                  toolStrip.Dock = DockStyle.Bottom;
                  toolBarBottomTSMI.Checked = true;
                  break;
               case ToolBarLocation.Left:
                  toolStrip.Dock = DockStyle.Left;
                  toolBarLeftTSMI.Checked = true;
                  break;
               case ToolBarLocation.Right:
                  toolStrip.Dock = DockStyle.Right;
                  toolBarRightTSMI.Checked = true;
                  break;
            }
            toolBarSize16TSMI.Checked = false;
            toolBarSize24TSMI.Checked = false;
            toolBarSize32TSMI.Checked = false;
            toolBarSize64TSMI.Checked = false;
            toolBarSize128TSMI.Checked = false;
            switch ((ToolBarSize)Settings.Default.ToolBarSize) {
               default:
                  //case ToolBarSize.Sixteen:
                  toolBarSize16TSMI.Checked = true;
                  break;
               case ToolBarSize.Twenty_Four:
                  toolBarSize24TSMI.Checked = true;
                  break;
               case ToolBarSize.Thirty_Two:
                  toolBarSize32TSMI.Checked = true;
                  break;
               case ToolBarSize.Sixty_Four:
                  toolBarSize64TSMI.Checked = true;
                  break;
               case ToolBarSize.One_Hundred_Twenty_Eight:
                  toolBarSize128TSMI.Checked = true;
                  break;
            }
            if (!toolStrip.Items.Contains(newToolStripButton))
               toolStrip.Items.Add(newToolStripButton);
            if (!toolStrip.Items.Contains(openToolStripButton))
               toolStrip.Items.Add(openToolStripButton);
            if (!toolStrip.Items.Contains(saveToolStripButton))
               toolStrip.Items.Add(saveToolStripButton);
            if (!toolStrip.Items.Contains(saveAsTSB))
               toolStrip.Items.Add(saveAsTSB);
            if (!toolStrip.Items.Contains(printToolStripButton))
               toolStrip.Items.Add(printToolStripButton);
            if (!toolStrip.Items.Contains(cutToolStripButton))
               toolStrip.Items.Add(cutToolStripButton);
            if (!toolStrip.Items.Contains(copyToolStripButton))
               toolStrip.Items.Add(copyToolStripButton);
            if (!toolStrip.Items.Contains(pasteToolStripButton))
               toolStrip.Items.Add(pasteToolStripButton);
            if (!toolStrip.Items.Contains(commentAddDoubleTSB))
               toolStrip.Items.Add(commentAddDoubleTSB);
            if (!toolStrip.Items.Contains(commentAddTripleTSB))
               toolStrip.Items.Add(commentAddTripleTSB);
            if (!toolStrip.Items.Contains(commentSummaryTSB))
               toolStrip.Items.Add(commentSummaryTSB);
            if (!toolStrip.Items.Contains(cPlusPlusTSB))
               toolStrip.Items.Add(cPlusPlusTSB);
            if (!toolStrip.Items.Contains(commentRemoveTSB))
               toolStrip.Items.Add(commentRemoveTSB);
            if (!toolStrip.Items.Contains(commentWrapTSB))
               toolStrip.Items.Add(commentWrapTSB);
            if (!toolStrip.Items.Contains(commentUpdateTSB))
               toolStrip.Items.Add(commentUpdateTSB);
            if (!toolStrip.Items.Contains(commentOutTSB))
               toolStrip.Items.Add(commentOutTSB);
            if (!toolStrip.Items.Contains(commentGetTSB))
               toolStrip.Items.Add(commentGetTSB);
            if (!toolStrip.Items.Contains(commentReplaceTSB))
               toolStrip.Items.Add(commentReplaceTSB);
            if (!toolStrip.Items.Contains(commentReverseEqualityTSB))
               toolStrip.Items.Add(commentReverseEqualityTSB);
            if (!toolStrip.Items.Contains(commentPreferencesTSB))
               toolStrip.Items.Add(commentPreferencesTSB);
            if (!toolStrip.Items.Contains(helpToolStripButton))
               toolStrip.Items.Add(helpToolStripButton);
            foreach (ToolStripButton toolStripButton in toolStrip.Items.OfType<ToolStripButton>()) {
               if (toolStripButton.DisplayStyle == ToolStripItemDisplayStyle.Image)
                  toolStripButton.Size = new Size(Settings.Default.ToolBarSize, Settings.Default.ToolBarSize);
            }
            toolStrip.ImageScalingSize = new Size(Settings.Default.ToolBarSize, Settings.Default.ToolBarSize);
         }
         else {
            if (toolStrip.Items.Contains(newToolStripButton))
               toolStrip.Items.Remove(newToolStripButton);
            if (toolStrip.Items.Contains(openToolStripButton))
               toolStrip.Items.Remove(openToolStripButton);
            if (toolStrip.Items.Contains(saveToolStripButton))
               toolStrip.Items.Remove(saveToolStripButton);
            if (toolStrip.Items.Contains(saveAsTSB))
               toolStrip.Items.Remove(saveAsTSB);
            if (toolStrip.Items.Contains(printToolStripButton))
               toolStrip.Items.Remove(printToolStripButton);
            if (toolStrip.Items.Contains(cutToolStripButton))
               toolStrip.Items.Remove(cutToolStripButton);
            if (toolStrip.Items.Contains(copyToolStripButton))
               toolStrip.Items.Remove(copyToolStripButton);
            if (toolStrip.Items.Contains(pasteToolStripButton))
               toolStrip.Items.Remove(pasteToolStripButton);
            if (toolStrip.Items.Contains(commentAddDoubleTSB))
               toolStrip.Items.Remove(commentAddDoubleTSB);
            if (toolStrip.Items.Contains(commentAddTripleTSB))
               toolStrip.Items.Remove(commentAddTripleTSB);
            if (toolStrip.Items.Contains(commentSummaryTSB))
               toolStrip.Items.Remove(commentSummaryTSB);
            if (toolStrip.Items.Contains(cPlusPlusTSB))
               toolStrip.Items.Remove(cPlusPlusTSB);
            if (toolStrip.Items.Contains(commentRemoveTSB))
               toolStrip.Items.Remove(commentRemoveTSB);
            if (toolStrip.Items.Contains(commentWrapTSB))
               toolStrip.Items.Remove(commentWrapTSB);
            if (toolStrip.Items.Contains(commentUpdateTSB))
               toolStrip.Items.Remove(commentUpdateTSB);
            if (toolStrip.Items.Contains(commentOutTSB))
               toolStrip.Items.Remove(commentOutTSB);
            if (toolStrip.Items.Contains(commentGetTSB))
               toolStrip.Items.Remove(commentGetTSB);
            if (toolStrip.Items.Contains(commentReplaceTSB))
               toolStrip.Items.Remove(commentReplaceTSB);
            if (toolStrip.Items.Contains(commentReverseEqualityTSB))
               toolStrip.Items.Remove(commentReverseEqualityTSB);
            if (toolStrip.Items.Contains(commentPreferencesTSB))
               toolStrip.Items.Remove(commentPreferencesTSB);
            if (toolStrip.Items.Contains(helpToolStripButton))
               toolStrip.Items.Remove(helpToolStripButton);
         }
      }

      private void HandleScrollers(bool pAdd) {
         if (pAdd) {
            if (!toolStrip.Items.Contains(toolTopButton))
               toolStrip.Items.Add(toolTopButton);
            if (!toolStrip.Items.Contains(toolLeftEdgeButton))
               toolStrip.Items.Add(toolLeftEdgeButton);
            if (!toolStrip.Items.Contains(toolRightEdgeButton))
               toolStrip.Items.Add(toolRightEdgeButton);
            if (!toolStrip.Items.Contains(toolBottomButton))
               toolStrip.Items.Add(toolBottomButton);
            if (!toolStrip.Items.Contains(toolScrollLeftButton))
               toolStrip.Items.Add(toolScrollLeftButton);
            if (!toolStrip.Items.Contains(toolScrollUpButton))
               toolStrip.Items.Add(toolScrollUpButton);
            if (!toolStrip.Items.Contains(toolScrollDownButton))
               toolStrip.Items.Add(toolScrollDownButton);
            if (!toolStrip.Items.Contains(toolScrollRightButton))
               toolStrip.Items.Add(toolScrollRightButton);
            if (!toolStrip.Items.Contains(toolPageLeftButton))
               toolStrip.Items.Add(toolPageLeftButton);
            if (!toolStrip.Items.Contains(toolPageUpButton))
               toolStrip.Items.Add(toolPageUpButton);
            if (!toolStrip.Items.Contains(toolPageDownButton))
               toolStrip.Items.Add(toolPageDownButton);
            if (!toolStrip.Items.Contains(toolPageRightButton))
               toolStrip.Items.Add(toolPageRightButton);
            foreach (ToolStripButton toolStripButton in toolStrip.Items.OfType<ToolStripButton>()) {
               if (toolStripButton.DisplayStyle == ToolStripItemDisplayStyle.Text)
                  PaintButton(toolStripButton);
            }
         }
         else {
            if (toolStrip.Items.Contains(toolTopButton))
               toolStrip.Items.Remove(toolTopButton);
            if (toolStrip.Items.Contains(toolLeftEdgeButton))
               toolStrip.Items.Remove(toolLeftEdgeButton);
            if (toolStrip.Items.Contains(toolRightEdgeButton))
               toolStrip.Items.Remove(toolRightEdgeButton);
            if (toolStrip.Items.Contains(toolBottomButton))
               toolStrip.Items.Remove(toolBottomButton);
            if (toolStrip.Items.Contains(toolScrollLeftButton))
               toolStrip.Items.Remove(toolScrollLeftButton);
            if (toolStrip.Items.Contains(toolScrollUpButton))
               toolStrip.Items.Remove(toolScrollUpButton);
            if (toolStrip.Items.Contains(toolScrollDownButton))
               toolStrip.Items.Remove(toolScrollDownButton);
            if (toolStrip.Items.Contains(toolScrollRightButton))
               toolStrip.Items.Remove(toolScrollRightButton);
            if (toolStrip.Items.Contains(toolPageLeftButton))
               toolStrip.Items.Remove(toolPageLeftButton);
            if (toolStrip.Items.Contains(toolPageUpButton))
               toolStrip.Items.Remove(toolPageUpButton);
            if (toolStrip.Items.Contains(toolPageDownButton))
               toolStrip.Items.Remove(toolPageDownButton);
            if (toolStrip.Items.Contains(toolPageRightButton))
               toolStrip.Items.Remove(toolPageRightButton);
         }
      }
   }
}