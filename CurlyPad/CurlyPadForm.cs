using CurlyPad.Properties;

namespace CurlyPad {
   public partial class CurlyPadForm : Form {
      public CurlyPadForm() {
         Opacity = 0;
         textBox = new MyTextBox {
            MaxLength = 9000000,
            Multiline = true,
            TabIndex = 0,
            AcceptsReturn = true,
            AcceptsTab = true,
            AllowDrop = true,
            Font = CreateNewFont(Settings.Default.TextFont),
            ContextMenuStrip = textBoxContextMenuStrip
         };
         textBox.TextChanged += TextBox_TextChanged;
         textBox.DragDrop += TextBox_DragDrop;
         textBox.DragEnter += TextBox_DragEnter;
         textBox.DragLeave += TextBox_DragLeave;
         textBox.KeyDown += TextBox_KeyDown;
         textBox.KeyUp += TextBox_KeyUp;
         textBox.Click += TextBox_Click;
         Controls.Add(textBox);
         TopMost = true;//efm5 Temporarily just to force it to come out on top
         InitializeComponent();
         sForm = this;
         searchTextBox.Enter += (pSender, pE) => {
            this.BeginInvoke(new Action(() => {
               searchTextBox.SelectAll();
            }));
         };
         replaceTextBox.Enter += (pSender, pE) => {
            this.BeginInvoke(new Action(() => {
               replaceTextBox.SelectAll();
            }));
         };
      }
   }
}
