using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        string[] lines;   
        public Form2(string[] eae)
        {
            InitializeComponent();
            lines = new String[eae.Length];
            try
            {               
                listBox2.Items.Add("LINKS QUEBRADOS");
                for (int i = 0; i <= eae.Length-1; i++)
                {
                    listBox2.Items.Add(eae[i] + "");
                    lines[i] = listBox2.Items[i] + "";
                }
                Refresh();
            }
            catch(Exception e) { MessageBox.Show("Algo deu Errado!!: \n"+e); }
        }

        //fechar janela
        private void bt_baixar_Click(object sender, EventArgs e){this.Close();}

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    string caminho = folderBrowserDialog1.SelectedPath + "/falhas.txt";
                    System.IO.File.WriteAllLines(caminho, lines);
                    MessageBox.Show("Arquivo com os links quebrados salvo com sucesso!!","FALHAS",MessageBoxButtons.OK,MessageBoxIcon.Information);

                }
            }
            catch (Exception ea){MessageBox.Show("Algo deu errado!!: \n"+ea);}
        }
    }
}
