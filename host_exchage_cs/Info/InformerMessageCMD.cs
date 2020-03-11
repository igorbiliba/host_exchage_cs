using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace host_exchage_cs.Info
{
    class InformerMessageCMD
    {
        public TextBox textBox;
        public InformerMessageWindow type;

        public InformerMessageCMD(InformerMessageWindow type, TextBox textBox)
        {
            this.type = type;
            this.textBox = textBox;
        }

        public void Process(string[] messages)
        {
            textBox.Invoke((MethodInvoker)delegate {
                foreach (var message in messages)
                {
                    if (message == "clear")
                    {
                        clear();
                        continue;
                    }

                    set(message);
                }
            });
        }

        void set(string line)
        {
            textBox.AppendText(line);
            textBox.AppendText(Environment.NewLine);
        }

        void clear()
        {
            textBox.Clear();
        }
    }
}
