using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodeGen
{
    public partial class Form1 : Form
    {
        bool[] isIgnored = new bool[100];
        private int index = 1, topIndex = 1;
        int[] rytOps = new int[100];
        string[] que = new string[100];
        string[][] ops = new string[100][];
        static string colorCor = "#090", colorWrng = "#D22";

        public Form1()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            lindex.Text = index.ToString();
            ops[index] = new string[4];
            bPrev.Enabled = false;
            restoreDefCol();
        }
        private void cbEnbCol_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnbCol.Checked)
            {
                tbCorr.Enabled = true;
                tbWrng.Enabled = true;
            }
            else
            {
                tbCorr.Enabled = false;
                tbWrng.Enabled = false;
            }
        }

        void restoreDefCol() { tbCorr.Text = colorCor; tbWrng.Text = colorWrng; }
        int checkRadio()
        {
            if (rO1.Checked)
                return 1;
            else if (rO2.Checked)
                return 2;
            else if (rO3.Checked)
                return 3;
            else if (rO4.Checked)
                return 4;
            else return 0;
        }
        void update()
        {
            que[index] = tbQ.Text;
            ops[index][0] = tbO1.Text;
            ops[index][1] = tbO2.Text;
            ops[index][2] = tbO3.Text;
            ops[index][3] = tbO4.Text;
            rytOps[index] = checkRadio();
            isIgnored[index] = cbIg.Checked;
        }
        void load()
        {
            tbQ.Text = que[index];
            tbO1.Text = ops[index][0];
            tbO2.Text = ops[index][1];
            tbO3.Text = ops[index][2];
            tbO4.Text = ops[index][3];
            switch (rytOps[index])
            {
                case 1: rO1.Checked = true; break;
                case 2: rO2.Checked = true; break;
                case 3: rO3.Checked = true; break;
                case 4: rO4.Checked = true; break;
            }
            cbIg.Checked = isIgnored[index];
        }
        void clear()
        {
            tbQ.Text = "";
            tbO1.Text = "";
            tbO2.Text = "";
            tbO3.Text = "";
            tbO4.Text = "";
            rO1.Checked = false;
            rO2.Checked = false;
            rO3.Checked = false;
            rO4.Checked = false;
            cbIg.Checked = false;
        }
        bool askIgnoreMsg()
        {
            DialogResult result = MessageBox.Show("Please mark the correct answer\nOr click OK to ignore this question", "Fill required fields", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                cbIg.Checked = true;
                return true;
            }
            else
                return false;
        }

        private void bPrev_Click(object sender, EventArgs e)
        {
            if (!cbIg.Checked && checkRadio() == 0)
                if (!askIgnoreMsg())
                    return;
            
            update();
            clear();
            index--;
            lindex.Text = index.ToString();
            load();
            if (bPrev.Enabled == false) bPrev.Enabled = true;
            if (index == 1) bPrev.Enabled = false;
        }
        private void bNxt_Click(object sender, EventArgs e)
        {
            if (!cbIg.Checked && checkRadio() == 0)
                if (!askIgnoreMsg())
                    return;

            update();
            clear();
            index++;
            lindex.Text = index.ToString();
            if (index > topIndex)
            {
                topIndex++;
                ops[index] = new string[4];
            }
            else load();
            if (bPrev.Enabled == false) bPrev.Enabled = true;
            if (index == 100) bNxt.Enabled = false;
        }


        private void bGen_Click(object sender, EventArgs e)
        {
            if (!cbIg.Checked && checkRadio() == 0)
                if (!askIgnoreMsg())
                    return;

            update();

            string code = "<!-- CodeGen aplha build -->\n\n";

            for (int i = 1; i <= topIndex; i++)
                if (!isIgnored[i])
                    code += gen(i.ToString(), que[i], ops[i], rytOps[i]);

            string[] script = { "<script>\n\tfunction corr(objL) { $(objL).text('Correct'); ",
            "}\n\tfunction wrong(objL, rytAns){ $(objL).text('The answer is: ' + rytAns); ",
            "}\n</script>\n\n" };

            code += script[0];

            if (cbEnbCol.Checked)
                code += "$(objL).css('color', '" + tbCorr.Text + "'); ";
            if (cbAnimDelay.Checked)
                code += "$(objL).hide(); $(objL).fadeIn(170); ";

            code += script[1];

            if (cbEnbCol.Checked)
                code += "$(objL).css('color', '" + tbWrng.Text + "'); ";
            if(cbAnimDelay.Checked)
                code += "$(objL).hide(); $(objL).fadeIn(500); ";

            code += script[2];

            Clipboard.SetText(code);
            MessageBox.Show("HTML code has been generated and copied to clipboard, press Ctrl + V to paste it anywhere", "Code copied to clipboard", MessageBoxButtons.OK);
        }


        private string gen(string x, string q, string[] ops, int rytOpsNum)
        {

            string buff = q + "<br/>\n";

            for (int i = 1; i <= 4; i++)
            {
                buff += "<input type='radio' name='" + x + "' onclick=";

                if (i == rytOpsNum)
                    buff += "\"corr('#res" + x + "');\"/>";
                else
                    buff += "\"wrong('#res" + x + "', '" + ops[rytOpsNum - 1] + "');\"/>";

                buff += ops[i - 1] + "\n";
            }

            return buff += "</br><label id='res" + x + "'></label><br/><br/>\n\n";
        }

        private void bDefCol_Click(object sender, EventArgs e)
        {
            restoreDefCol();
        }
    }
}
