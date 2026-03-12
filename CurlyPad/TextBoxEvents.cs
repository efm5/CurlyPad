namespace CurlyPad {
   public partial class CurlyPadForm : Form {
      private void TextBox_TextChanged(object? pSender, EventArgs pE) {
         if (sDoing) {
            sDoing = false;
            return;
         }
         if (!string.IsNullOrEmpty(textBox.Text)) {
            textBox.Modified = true;
            if (sUndoList.Count > 0) {
               UnReDoData unReDoData = sUndoList.Last();
               if ((unReDoData.mTextString != textBox.Text) && (unReDoData.sSelectionStart != textBox.SelectionStart))
                  sUndoList.Add(new UnReDoData(textBox.SelectionStart, textBox.Text));
            }
            else
               sUndoList.Add(new UnReDoData(textBox.SelectionStart, textBox.Text));
            //CheckDoItems();
         }
         else
            textBox.Modified = false;
         SetWindowTitle();
         UpdateStatusBar();
      }

      private void TextBox_KeyUp(object? pSender, KeyEventArgs pE) {
         sIsCntrolKeyDown = pE.Control;
         if (pE.KeyCode == Keys.F1) {
            Help();
            pE.Handled = true;
         }
         else
            UpdateStatusBar();
      }

      private void TextBox_KeyDown(object? pSender, KeyEventArgs pE) {
         sIsCntrolKeyDown = pE.Control;
      }

      private void MouseWheeling(object? pSender, MouseEventArgs pE) {
         if (pE.Delta > 0) {//Forward
            if (sIsCntrolKeyDown) {
               SetZoom("+");
               sZoomLevel++;
               ((HandledMouseEventArgs)pE).Handled = true;
            }
         }
         if (pE.Delta < 0) {//Backward
            if (sIsCntrolKeyDown) {
               SetZoom("-");
               sZoomLevel--;
               ((HandledMouseEventArgs)pE).Handled = true;
            }
         }
      }

      private void TextBox_DragDrop(object? pSender, DragEventArgs pE) {
         string[] files = (string[])pE.Data.GetData(DataFormats.FileDrop);

         if (files != null && files.Length != 0) {
            sCurrentFile = files[0];
            if ((textBox.Modified == true) && (textBox.Text.Length > 0)) {
               DialogResult dialogResult = MessageBox.Show("It looks like there is text which has not been saved." +
                   Environment.NewLine + "Would you like to save a file before loading a new one?",
                   "Save Before Closing?", MessageBoxButtons.YesNoCancel);
               if (dialogResult == DialogResult.Yes)
                  SaveAs();
               else if (dialogResult == DialogResult.Cancel)
                  return;
               //efm5 DialogResult.No just falls through
            }
            LoadFile(sCurrentFile);
         }
      }

      private void TextBox_DragEnter(object? pSender, DragEventArgs pE) {
         if (pE.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            pE.Effect = DragDropEffects.All;
      }

      private void TextBox_DragLeave(object? pSender, EventArgs pE) {//DEBUG efm5 2024 03 9 is this necessary
      }

      private void TextBox_DragOver(object? pSender, DragEventArgs pE) {
         if (pE.Data.GetDataPresent(DataFormats.FileDrop))
            pE.Effect = DragDropEffects.Copy;
         else
            pE.Effect = DragDropEffects.None;
      }

      private void TextBox_Click(object? pSender, EventArgs pE) {
         UpdateStatusBar();
      }
   }
}
