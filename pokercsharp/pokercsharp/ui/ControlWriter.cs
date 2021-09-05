
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace pokercsharp.log {
    public class ControlWriter : TextWriter {
        private TextBox textbox;
        public ControlWriter(TextBox textbox) {
            this.textbox = textbox;
        }

        public override void Write(char value) {
            textbox.Text += value;
            textbox.ScrollToEnd();
        }

        public override void Write(string value) {
            textbox.Text += value;
            textbox.ScrollToEnd();
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}
