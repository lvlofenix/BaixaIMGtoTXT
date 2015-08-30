using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {    
        //variaveis globais
        int imgcar = 0;
        int imgbai = 1;
        int imgfail = 0;
        Thread tt;
        Thread tg;
        int qualthread = 0;
        string[] imgbroken;
        Boolean controleStop = false;
        Sobre sobre = new Sobre();
        Form2 erro;
        Ajuda ajuda = new Ajuda();
        
        //iniciando o form (construtor)
        public Form1()
        {
            InitializeComponent();
        }

        [STAThread]
        public void carregaThreads(int qual)
        {
            try
            {
                if (qual == 1)
                {
                    tg = new Thread(new ThreadStart(procuraImagens));
                    tg.ApartmentState = ApartmentState.STA;
                    tg.Start();
                }
                else
                {
                    tt = new Thread(new ThreadStart(baixaImagens));
                    tt.ApartmentState = ApartmentState.STA;
                    tt.Start();
                }
            }
            catch
            {
                MessageBox.Show("deu bosta");
            }
        }       

        //funções 

        /* procuraImagens.
         * Função para localizar o arquivo txt. 
         */
        public void procuraImagens()
        {
            imgcar = 0;
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                controleStop = false;
                label2.Invoke((MethodInvoker)delegate { label2.Text = "Imagens Carregadas: " + imgcar; });
                listBox1.Invoke((MethodInvoker)delegate { listBox1.Items.Clear(); });
                bt_baixar.Invoke((MethodInvoker)delegate { bt_baixar.Enabled = false; });
                openFileDialog1.Title = "Selecionar TXT";
                alimentaLista(openFileDialog1.FileName);
                imgbai = 1;
            }
            else
            {
                br_carregar.Invoke((MethodInvoker)delegate { br_carregar.Enabled = true; });
            }
        }

        /* alimendaLista - string text.
         * função para ler e carregar o arquivo TXT 
         */
        public void alimentaLista(string text)
        {
            try
            {
                string line;
                imgcar = 0;
                System.IO.StreamReader file = new System.IO.StreamReader(text);
                while ((line = file.ReadLine()) != null & controleStop == false)
                {
                        if (line == "")
                        {
                        }
                        else
                        {
                            line = line.Replace(" ", "").Replace(",", "").Replace(" ", "");
                            if (line.Substring(line.Length - 3, 3) == "gif" || line.Substring(line.Length - 3, 3) == "png" || line.Substring(line.Length - 3, 3) == "jpg" || line.Substring(line.Length - 3, 3) == "bmp")
                            {
                                listBox1.Invoke((MethodInvoker)delegate { listBox1.Items.Add(line); });
                                bt_baixar.Invoke((MethodInvoker)delegate { bt_baixar.Enabled = true; });
                                imgcar++;
                            }

                        }
                        label2.Invoke((MethodInvoker)delegate { label2.Text = "Imagens Carregadas: " + imgcar; });
                    }
                file.Close();
                br_carregar.Invoke((MethodInvoker)delegate { br_carregar.Enabled = true; });
            }
            catch(Exception ea)
            {
                MessageBox.Show(ea + "");
            }
        }
        
        /* baixaImagens
         * função inicia o download das imagens
         */
        public void baixaImagens()
        {
                if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
                {
                    bt_cancel.Invoke((MethodInvoker)delegate { bt_cancel.Show(); });
                    string tipoImg = "";
                    controleStop = false;
                    imgbai = 0;
                    int pegai = 0;
                    progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = 0; });
                    progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Maximum = imgcar; });
                    imgbroken = new String[imgcar+1];
                    progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Step = 1; });
                    int numlist = Convert.ToInt32(listBox1.Items.Count.ToString());
                    for (int i = 0; i <= numlist - 1; i++)
                    {
                        pegai = i;
                        try
                        {
                            if (controleStop == false)
                            {
                                pictureBox1.ImageLocation = listBox1.Items[i] + "";
                                string fileURL = @"" + listBox1.Items[i];
                                pictureBox1.ImageLocation = listBox1.Items[i]+"";
                                DirectoryInfo di = new DirectoryInfo(folderBrowserDialog2.SelectedPath + "/");
                                WebClient client = new WebClient();
                                tipoImg = listBox1.Items[i] + "";
                                tipoImg = tipoImg.Substring(tipoImg.Length - 3, 3);
                                client.DownloadFile(fileURL, di + "" + i + "." + tipoImg);
                                progressBar1.Invoke((MethodInvoker)delegate { progressBar1.PerformStep(); });
                                imgbai++;
                                label1.Invoke((MethodInvoker)delegate { label1.Text = "Imagens Baixadas: " + imgbai; });
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show(e + "");
                            progressBar1.Invoke((MethodInvoker)delegate { progressBar1.PerformStep(); });
                            imgfail++;
                            imgbroken[imgfail] = listBox1.Items[pegai]+"";
                            label3.Invoke((MethodInvoker)delegate { label3.Text = "Itens que falharam: " + imgfail; });

                        }
                    }
                    bt_cancel.Invoke((MethodInvoker)delegate { bt_cancel.Visible = false; });
                    MessageBox.Show("Imagens Baixadas com Sucesso: " + imgbai, "ACABOU!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
        }

        /* verificaVisu
         * função habilidade ou desativar a pre visualização.
         */
        public void verificaVisu()
        {
            if (!checkBox1.Checked)
            {
                pictureBox1.ImageLocation = null;
                Refresh();
            }
            else
            {
                Refresh();
                pictureBox1.ImageLocation = "/loading.gif";
                Refresh();
                pictureBox1.ImageLocation = listBox1.SelectedItem + "";
                Refresh();
            }
        }

        /* cancelaDown
         * função para realizar o cancelamento de downloads em processo.
         */
        public void cancelaDown()
        {
            try
            {
                controleStop = true;
                bt_cancel.Hide();
                br_carregar.Invoke((MethodInvoker)delegate { br_carregar.Enabled = true; }); 
            }
            catch(Exception e)
            {
                MessageBox.Show("Algo deu errado!!\n"+e);
            }
        }

        //botões
        //carrega a função de baixar imagens
        private void button2_Click(object sender, EventArgs e) { qualthread = 2; carregaThreads(0); }
        //procura imagens
        private void button1_Click(object sender, EventArgs e) { qualthread = 1; br_carregar.Enabled = false; carregaThreads(1); }
        //carrega a pre visulização ao clikcar no link
        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e){verificaVisu();}
        //carreção a função de cancelar downloads em andamento.
        private void button3_Click_2(object sender, EventArgs e) { 
            cancelaDown(); 
            br_carregar.Invoke((MethodInvoker)delegate { br_carregar.Enabled = true; }); 
        }
        //carrega o form com os links que deram erro.
        private void label3_Click(object sender, EventArgs e)
        {
            if (label3.Text == "Itens que falharam: 0")
            {
                MessageBox.Show("Não Houve Falhas!");
            }
            else
            {
                erro = new Form2(imgbroken);
                erro.Show();
            }
        }
        //carrega a função de ativar/desativar a pre visualização.
        private void checkBox1_CheckedChanged(object sender, EventArgs e){verificaVisu();}
        //abre o sobre
        private void label7_Click(object sender, EventArgs e) { sobre.Show(); ajuda.Hide(); }
        //abre o ajuda
        private void label5_Click(object sender, EventArgs e) { sobre.Hide(); ajuda.Show(); }
    }
}