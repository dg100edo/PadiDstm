using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PADI_DSTM;
using padi_dstm_exceptions;

namespace TestingInterface
{
    public partial class InterfaceDesign : Form
    {

        private Dictionary<int, PadInt> vars;


        public InterfaceDesign()
        {
            InitializeComponent();
            vars = new Dictionary<int, PadInt>();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void status_button_click(object sender, EventArgs e)
        {
            PadiDstm.Status();
            richTextBox1.AppendText("Status "+ "\r\n");

        }

        private void url_textbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void rectangleShape3_Click(object sender, EventArgs e)
        {

        }

        private void rectangleShape1_Click(object sender, EventArgs e)
        {

        }

        private void create_button_click(object sender, EventArgs e)
        {
            string id_string = ObjectIdBox.Text;
            int id = Int32.Parse(id_string);
            PadInt padInt = PadiDstm.CreatePadInt(id);
            if (padInt == null)
            {
                MessageBox.Show("Padint with id: " + id + " already exists.");
            }
            else
            {
                vars[id] = padInt;
                richTextBox1.AppendText("Padint with id: " + id + " was created." + "\r\n");
            }
        }

        private void init_button_click(object sender, EventArgs e)
        {
            PadiDstm.Init();
            richTextBox1.AppendText("Init" + "\r\n");
        }

        private void tx_init_button_click(object sender, EventArgs e)
        {
            PadiDstm.TxBegin();
            richTextBox1.AppendText("txBegin" + "\r\n");
        }

        private void tx_commit_button_click(object sender, EventArgs e)
        {
            if (PadiDstm.TxCommit())
            {
                richTextBox1.AppendText("Transaction commited." + "\r\n");
            }
            else
            {
                richTextBox1.AppendText("Transaction aborted by system." + "\r\n");
            }

        }

        private void tx_abort_button_click(object sender, EventArgs e)
        {
            PadiDstm.TxAbort();
            richTextBox1.AppendText("txAbort" + "\r\n");

        }

        private void url_label_click(object sender, EventArgs e)
        {

        }

        private void rectangleShape2_Click(object sender, EventArgs e)
        {

        }

        private void fail_button_click(object sender, EventArgs e)
        {
 
            string url = urlBox.Text;
            PadiDstm.Fail(url);
            richTextBox1.AppendText("Fail" + "\r\n");

        }

        private void freeze_button_click(object sender, EventArgs e)
        {
           
            string url = urlBox.Text;
            PadiDstm.Freeze(url);
            richTextBox1.AppendText("Freeze" + "\r\n");
        }

        private void recover_button_click(object sender, EventArgs e)
        {

            string url = urlBox.Text;
            PadiDstm.Recover(url);
            richTextBox1.AppendText("Recover" + "\r\n");
        }

        private void acess_button_click(object sender, EventArgs e)
        {
            string id_string = ObjectIdBox.Text;
            int id = Int32.Parse(id_string);
            PadInt padInt = PadiDstm.AccessPadInt(id);
            if (padInt == null)
            {
                MessageBox.Show("Padint with id: " + id + " have not been not created yet.");
            }
            else
            {
                vars[id] = padInt;
                richTextBox1.AppendText("Padint with id: " + id + " was acessed." + "\r\n");
            }
        }

        private void read_button_click(object sender, EventArgs e)
        {
            string id_string = ObjectIdBox.Text;
            int id = Int32.Parse(id_string);
            try
            {
                PadInt padInt = vars[id];
                richTextBox1.AppendText("PadInt " + id + ": " + padInt.Read() + "\r\n");
            }
            catch(TxException exc){
                MessageBox.Show(exc.Message);
            } catch (KeyNotFoundException) {
                MessageBox.Show("Error: PadInt: " + id + " was not created or initialized");
                
                //DEBUG PURPOSES
               // richTextBox1.AppendText("### DEBUG: \r\n" + ex.StackTrace + "\r\n");
            }
            
        }

        private void write_button_click(object sender, EventArgs e)
        {

            string id_string = ObjectIdBox.Text;
            int id = Int32.Parse(id_string);

            string value_string = ValueBox.Text;
            int value = Int32.Parse(value_string);
          
            try
            {
                PadInt padInt = vars[id];
                padInt.Write(value);
                richTextBox1.AppendText("PadInt " + id + ": " + " written value: " + value + "\r\n");
            }
            catch (TxException exc)
            {
                MessageBox.Show(exc.Message);
            } catch (KeyNotFoundException)
            {
                MessageBox.Show("Error: PadInt: " + id + "was not created or initialized");
            }
        }

        private void InterfaceDesign_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void rectangleShape4_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
