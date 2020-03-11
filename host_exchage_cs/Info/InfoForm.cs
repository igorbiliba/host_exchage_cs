using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace host_exchage_cs.Info
{
    public partial class InfoForm : Form
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        List<InformerMessageCMD> windows;
        public InfoForm()
        {
            InitializeComponent();
            MaximizeBox = false;

            windows = new List<InformerMessageCMD>() {
                new InformerMessageCMD(InformerMessageWindow.balances,       this.balancesTextBox),
                new InformerMessageCMD(InformerMessageWindow.btcAddresses,   this.btcAddressesTextBox),
                new InformerMessageCMD(InformerMessageWindow.clientCntUseds, this.clientCntUsedsTextBox),
                new InformerMessageCMD(InformerMessageWindow.clientsInRun,   this.clientsInRunTextBox),
                new InformerMessageCMD(InformerMessageWindow.emailChecker,   this.emailCheckerTextBox),
            };

            this.balancesTextBox.ScrollBars       = ScrollBars.Vertical;
            this.btcAddressesTextBox.ScrollBars   = ScrollBars.Vertical;
            this.clientCntUsedsTextBox.ScrollBars = ScrollBars.Vertical;
            this.clientsInRunTextBox.ScrollBars   = ScrollBars.Vertical;
            this.emailCheckerTextBox.ScrollBars   = ScrollBars.Vertical;
        }

        public void OnMessage(InformerMessage message)
        {
            windows
                .FindLast(el => el.type == message.window)
                .Process(message.messages);
        }
    }
}
