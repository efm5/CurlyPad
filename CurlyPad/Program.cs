using static CurlyPad.CurlyPadForm;
using static CurlyPad.Program.NativeMethods;

namespace CurlyPad {
   internal static partial class Program {
      [STAThread]
      static void Main(string[] pArguments) {
         SetProcessDPIAware();
         Application.SetHighDpiMode(HighDpiMode.SystemAware);
         ApplicationConfiguration.Initialize();
         //if (arguments.Count() > 0)
         //   foreach (string phrase in arguments)
         //      MessageBox.Show(phrase, "TITLE", MessageBoxButtons.OK, MessageBoxIcon.Error);
         if (pArguments.Length == 1)
            sArgument = pArguments[0];
         Application.Run(new CurlyPadForm());
      }
   }
}