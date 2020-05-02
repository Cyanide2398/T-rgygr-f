using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Windows.Input;
using System.Diagnostics;

namespace Szoftverfejlesztes
{
    public partial class Form1 : Form
    {
        DB adatb = new DB();
        SQLiteCommand cmd;
        SQLiteDataAdapter sda;
        DataTable dt;
        List<Targy> targyak = new List<Targy>();
        List<Button> buttons = new List<Button>();
        List<String> yellows = new List<String>();
        Dictionary<int, DataTable> dataTables = new Dictionary<int, DataTable>();
        int szakid = Global.szakid;
        int iterator = 0;
        int green_it = 0;
        Dictionary<string, int> valaszthatok = new Dictionary<string, int>();
        Dictionary<KeyValuePair<string, int>, int> valasztottak = new Dictionary<KeyValuePair<string, int>, int>();
        bool warned = false;
        bool notified = false;


        Dictionary<int, Button> wasGreen = new Dictionary<int, Button>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.TextChanged += txtContents_TextChanged;
            textBox1.Multiline = true;
            textBox1.ScrollBars = ScrollBars.None;

            // Make the TextBox fit its initial text.
            AutoSizeTextBox(textBox1);

            sda = new SQLiteDataAdapter("select max(ajánlottfélév) from tantárgya where szakid = " + szakid + "", adatb.GetConnection());
            dt = new DataTable();
            sda.Fill(dt);
            int max = Int32.Parse(dt.Rows[0][0].ToString());
            int pointX = 30;
            int pointY = 40;
            for (int i = 1; i <= max; i++)
            {
                sda = new SQLiteDataAdapter("select count(*) from Tantárgy as t join tantárgya as tan on tan.tantárgykód = t.kód where tan.szakid = " + szakid + " and tan.kategória = 'Kötelező szakmai tárgyak' and tan.ajánlottfélév  = '" + i + "'", adatb.GetConnection());
                dt = new DataTable();
                sda.Fill(dt);
                int n = Int32.Parse(dt.Rows[0][0].ToString());

                cmd = new SQLiteCommand("select név from szak where id = 1");
                cmd.Connection = adatb.GetConnection();
                adatb.openConnection();
                textBox1.Text = (string)cmd.ExecuteScalar();
                adatb.closeConnection();

                sda = new SQLiteDataAdapter("select név from tantárgy join tantárgya as tan on tan.tantárgykód = tantárgy.kód where szakid = " + szakid + " and tan.ajánlottfélév  = '" + i + "'", adatb.GetConnection());
                dt = new DataTable();
                sda.Fill(dt);
                Label l = new Label();
                l.Text = "" + i + ". félév";
                l.Location = new Point(pointX, pointY - 30);
                panel1.Controls.Add(l);
                for (int j = 0; j < n; j++)
                {

                    Button a = new Button();
                    a.Text = dt.Rows[j][0].ToString();
                    a.Location = new Point(pointX, pointY);
                    a.AutoSize = true;
                    a.Click += new EventHandler(this.click_Buttons);
                    a.MouseHover += new EventHandler(this.ButtonName_MouseHover);
                    a.BackColor = System.Drawing.Color.White;
                    panel1.Controls.Add(a);
                    panel1.Show();

                    SQLiteCommand command = new SQLiteCommand("select kód from tantárgy as t join tantárgya as tan on tan.tantárgykód = t.kód where t.név ='" + a.Text + "' and tan.szakid = '" + szakid + "'");
                    command.Connection = adatb.GetConnection();
                    adatb.openConnection();
                    string kod = (string)command.ExecuteScalar();
                    a.Name = kod;
                    adatb.closeConnection();
                    command = new SQLiteCommand("select kredit from tantárgya where tantárgykód = '" + kod + "'and szakid = '" + szakid + "'");
                    command.Connection = adatb.GetConnection();
                    adatb.openConnection();
                    int kredit = Convert.ToInt32(command.ExecuteScalar());
                    adatb.closeConnection();
                    command = new SQLiteCommand("select kategória from tantárgya where tantárgykód = '" + kod + "'and szakid = '" + szakid + "'");
                    command.Connection = adatb.GetConnection();
                    adatb.openConnection();
                    string kat = (string)(command.ExecuteScalar());
                    adatb.closeConnection();

                    Targy t = new Targy(kod, a.Text, i, kredit, kat);
                    textprogressBar1.Maximum += kredit;
                    targyak.Add(t);
                    buttons.Add(a);

                    int elofeltetel_num = 0;
                    cmd = new SQLiteCommand("select count(előfeltételkód) from előfeltétele where ráépülőkód = '" + kod + "' and szakid = " + szakid + "");
                    cmd.Connection = adatb.GetConnection();
                    adatb.openConnection();
                    elofeltetel_num = Convert.ToInt32(cmd.ExecuteScalar());
                    adatb.closeConnection();

                    SQLiteDataAdapter sda1 = new SQLiteDataAdapter("select előfeltételkód from előfeltétele where ráépülőkód  = '" + kod + "' and szakid = " + szakid + "", adatb.GetConnection());
                    DataTable dt1 = new DataTable();
                    sda1.Fill(dt1);
                    for (int m = 0; m < elofeltetel_num; m++)
                    {
                        t.elofeltetelek.Add((string)dt1.Rows[m][0]);
                    }
                    pointY += 40;
                }
                pointY = 40;
                pointX += 400;
            }
            pointX = 30;
            pointY = 600;
            sda = new SQLiteDataAdapter("select distinct típus from tantárgya where szakid = " + szakid + " and ajánlottfélév = 0", adatb.GetConnection());
            dt = new DataTable();
            sda.Fill(dt);
            for(int i=0;i<dt.Rows.Count;i++)
            {
                sda = new SQLiteDataAdapter("select név from tantárgy as t join tantárgya as tan on t.kód = tan.tantárgykód where tan.szakid = " + szakid + " and tan.ajánlottfélév = 0 and tan.típus = '" + dt.Rows[i][0] + "'", adatb.GetConnection());
                DataTable dt1 = new DataTable();
                sda.Fill(dt1);
                Label l1 = new Label();
                l1.AutoSize = true;
                l1.Text = ""+ dt.Rows[i][0].ToString() +"";
                l1.Location = new Point(pointX, pointY - 30);
                panel1.Controls.Add(l1);
                for (int j = 0; j < dt1.Rows.Count; j++)
                {
                    Button a = new Button();
                    a.Text = dt1.Rows[j][0].ToString();
                    a.Location = new Point(pointX, pointY);
                    a.AutoSize = true;
                    a.Click += new EventHandler(this.click_Buttons);
                    a.MouseHover += new EventHandler(this.ButtonName_MouseHover);
                    a.BackColor = System.Drawing.Color.White;
                    panel1.Controls.Add(a);
                    panel1.Show();

                    SQLiteCommand command = new SQLiteCommand("select kód from tantárgy as t join tantárgya as tan on tan.tantárgykód = t.kód where t.név ='" + a.Text + "' and tan.szakid = '" + szakid + "'");
                    command.Connection = adatb.GetConnection();
                    adatb.openConnection();
                    string kod = (string)command.ExecuteScalar();
                    a.Name = kod;
                    adatb.closeConnection();
                    command = new SQLiteCommand("select kredit from tantárgya where tantárgykód = '" + kod + "'and szakid = '" + szakid + "'");
                    command.Connection = adatb.GetConnection();
                    adatb.openConnection();
                    int kredit = Convert.ToInt32(command.ExecuteScalar());
                    adatb.closeConnection();
                    command = new SQLiteCommand("select kategória from tantárgya where tantárgykód = '" + kod + "'and szakid = '" + szakid + "'");
                    command.Connection = adatb.GetConnection();
                    adatb.openConnection();
                    string kat = (string)(command.ExecuteScalar());
                    adatb.closeConnection();

                    Targy t = new Targy(kod, a.Text, i, kredit, kat);
                    targyak.Add(t);
                    buttons.Add(a);

                    int elofeltetel_num = 0;
                    cmd = new SQLiteCommand("select count(előfeltételkód) from előfeltétele where ráépülőkód = '" + kod + "' and szakid = " + szakid + "");
                    cmd.Connection = adatb.GetConnection();
                    adatb.openConnection();
                    elofeltetel_num = Convert.ToInt32(cmd.ExecuteScalar());
                    adatb.closeConnection();

                    SQLiteDataAdapter sda1 = new SQLiteDataAdapter("select előfeltételkód from előfeltétele where ráépülőkód  = '" + kod + "' and szakid = " + szakid + "", adatb.GetConnection());
                    DataTable dt2 = new DataTable();
                    sda1.Fill(dt2);
                    for (int m = 0; m < elofeltetel_num; m++)
                    {
                        t.elofeltetelek.Add((string)dt2.Rows[m][0]);
                    }

                    pointY += 40;

                }
                pointY = 600;
                pointX += 400;
            }
            SetAvailable();
            sda = new SQLiteDataAdapter("select név, sum(kredit) from kategóriája where szakid = "+ szakid +" group by név", adatb.GetConnection());
            dt = new DataTable();
            sda.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                valaszthatok.Add(dt.Rows[i][0].ToString(), Convert.ToInt32(dt.Rows[i][1]));
            }
            foreach(KeyValuePair<string, int> elem in valaszthatok)
            {
                textBox2.AppendText(elem.Key + ": 0/" + elem.Value);
                textprogressBar1.Maximum += elem.Value;
                textBox2.AppendText(Environment.NewLine);
                valasztottak.Add(elem, 0);
            }

        }

        void click_Buttons(object sender, EventArgs e)
        {
            if ((sender as Button).BackColor == System.Drawing.Color.White)
            {
                cmd = new SQLiteCommand("select count(előfeltételkód) from előfeltétele where ráépülőkód = '" + (sender as Button).Name + "'", adatb.GetConnection());
                adatb.openConnection();
                int felveh = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                adatb.closeConnection();
                sda = new SQLiteDataAdapter("select előfeltételkód from tantárgy as t join előfeltétele as e on t.kód = e.ráépülőkód where ráépülőkód = '" + (sender as Button).Name + "'", adatb.GetConnection());
                dt = new DataTable();
                sda.Fill(dt);
                MessageBox.Show("Ezt a tárgyat még nem veheted fel.");
                string msg = "Előfeltételek:";
                for (int i = 0; i < felveh; i++)
                {
                    Targy found = targyak.Find(x => x.kod == dt.Rows[i][0].ToString());
                    if (found != null)
                    {
                        msg += " '" + found.nev + "'";
                        if (found.teljesitett == true) { msg += " -Teljesítve\n"; }
                        else { msg += " -Nincs teljesítve\n"; }
                    }
                }
                msg += "" + targyak.Find(x => x.kod == (sender as Button).Name).elofeltetel_db + "";
                MessageBox.Show(msg);
            }
            else if ((sender as Button).BackColor == System.Drawing.Color.Yellow)
            {
                sda = new SQLiteDataAdapter("select ráépülőkód from előfeltétele where előfeltételkód = '" + (sender as Button).Name + "' and szakid =" + szakid + " and egyszerrefelveheto = 1", adatb.GetConnection());
                dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    (sender as Button).BackColor = System.Drawing.Color.Lime;
                    sda = new SQLiteDataAdapter("select előfeltételkód from előfeltétele where ráépülőkód = '" + (sender as Button).Name + "' and szakid =" + szakid + " and egyszerrefelveheto = 1", adatb.GetConnection());
                    dt = new DataTable();
                    sda.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Button b = buttons.Find(x => x.Name == dt.Rows[i][0].ToString());
                        if (b != null)
                        {
                            b.BackColor = System.Drawing.Color.Lime;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Button b = buttons.Find(x => x.Name == dt.Rows[i][0].ToString());
                        if (b != null)
                        {
                            Targy t = targyak.Find(x => x.kod == b.Name);
                            MessageBox.Show("Ez a tárgy egy másik tárggyal egyszerre felvehető. Felvételéhez vedd fel a(z) " + t.nev + " tárgyat!");
                        }
                    }
                }
            }
            else if ((sender as Button).BackColor == System.Drawing.Color.Lime)
            {
                
                sda = new SQLiteDataAdapter("select előfeltételkód from előfeltétele where ráépülőkód = '" + (sender as Button).Name + "' and szakid =" + szakid + " and egyszerrefelveheto = 1", adatb.GetConnection());
                dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    (sender as Button).BackColor = System.Drawing.Color.ForestGreen;
                    targyak.Find(x => x.kod == (sender as Button).Name).teljesitett = true;
                    Targy found = targyak.Find(x => x.nev == (sender as Button).Text);
                    if (textprogressBar1.Value + found.kredit < textprogressBar1.Maximum)
                    {
                        textprogressBar1.Value += found.kredit;
                    }
                    else if (textprogressBar1.Value + found.kredit == textprogressBar1.Maximum && notified == false)
                    {
                        textprogressBar1.Value += found.kredit;
                        MessageBox.Show("Gratulálunk, elérted a teljesítendő kreditek számát! Készen állsz a diplomára!");
                        notified = true;
                    }
                    else if (notified == false)
                    {
                        MessageBox.Show("Gratulálunk, elérted a teljesítendő kreditek számát! Készen állsz a diplomára!");
                        notified = true;
                    }
                    Dictionary<KeyValuePair<string, int>, int> temp = new Dictionary<KeyValuePair<string, int>, int>();
                    //MessageBox.Show(valasztottak.Count().ToString());
                    foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in valasztottak)
                    {
                        if (found.kategoria == elem.Key.Key)
                        {
                            var prev = elem.Value;
                            var prevkey = elem.Key;
                            prev += found.kredit;
                            if(prev > elem.Key.Value && warned == false)
                            {
                                MessageBox.Show("Túllépted a(z) " + elem.Key.Key + "kategóriából szükséges kreditek számát.\n További kreditek teljesítése ebből a kategóriából fizetési kötelezettségeket vonhat maga után");
                                warned = true;
                            }
                            temp.Add(prevkey, prev);
                        }
                        else
                        {
                            temp.Add(elem.Key, elem.Value);
                        }
                    }
                    valasztottak.Clear();
                    foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in temp)
                    {
                        valasztottak.Add(elem.Key, elem.Value);
                    }
                    temp.Clear();
                    textBox2.Text = "";
                    foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in valasztottak)
                    {
                        textBox2.Text += elem.Key.Key + ":" + elem.Value + "/" + elem.Key.Value;
                        textBox2.AppendText(Environment.NewLine);
                        textBox2.SelectionStart = 0;
                        textBox2.ScrollToCaret();
                    }
                    Highlight((sender as Button));
                    sda = new SQLiteDataAdapter("select ráépülőkód from előfeltétele where előfeltételkód= '" + (sender as Button).Name + "' and szakid =" + szakid + " and egyszerrefelveheto = 1", adatb.GetConnection());
                    dt = new DataTable();
                    sda.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Button b = buttons.Find(x => x.Name == dt.Rows[i][0].ToString());
                        if (b != null)
                        {
                            b.BackColor = System.Drawing.Color.ForestGreen;
                            found = targyak.Find(x => x.kod == dt.Rows[i][0].ToString());
                            found.teljesitett = true;
                            if (textprogressBar1.Value + found.kredit < textprogressBar1.Maximum)
                            {
                                textprogressBar1.Value += found.kredit;
                            }
                            else if (textprogressBar1.Value + found.kredit == textprogressBar1.Maximum && notified == false)
                            {
                                textprogressBar1.Value += found.kredit;
                                MessageBox.Show("Gratulálunk, elérted a teljesítendő kreditek számát! Készen állsz a diplomára!");
                                notified = true;
                            }
                            else if (notified == false)
                            {
                                MessageBox.Show("Gratulálunk, elérted a teljesítendő kreditek számát! Készen állsz a diplomára!");
                                notified = true;
                            }
                            foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in valasztottak)
                            {
                                if (found.kategoria == elem.Key.Key)
                                {
                                    var prev = elem.Value;
                                    var prevkey = elem.Key;
                                    prev += found.kredit;
                                    if (prev > elem.Key.Value && warned == false)
                                    {
                                        MessageBox.Show("Túllépted a(z) " + elem.Key.Key + "kategóriából szükséges kreditek számát.\n További kreditek teljesítése ebből a kategóriából fizetési kötelezettségeket vonhat maga után");
                                        warned = true;
                                    }
                                    temp.Add(prevkey, prev);
                                }
                                else
                                {
                                    temp.Add(elem.Key, elem.Value);
                                }
                            }
                            valasztottak.Clear();
                            foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in temp)
                            {
                                valasztottak.Add(elem.Key, elem.Value);
                            }
                            temp.Clear();
                            textBox2.Text = "";
                            foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in valasztottak)
                            {
                                textBox2.Text += elem.Key.Key + ":" + elem.Value + "/" + elem.Key.Value;
                                textBox2.AppendText(Environment.NewLine);
                                textBox2.SelectionStart = 0;
                                textBox2.ScrollToCaret();
                            }
                            Highlight(b);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Button b = buttons.Find(x => x.Name == dt.Rows[i][0].ToString());
                        if (b != null)
                        {
                            Targy t = targyak.Find(x => x.kod == b.Name);
                            MessageBox.Show("Ez a tárgy egy másik tárggyal egyszerre felvehető. Teljesítéséhez teljesítsd a(z) " + t.nev + " tárgyat!");
                        }
                    }
                }

            }
            else if ((sender as Button).BackColor == System.Drawing.Color.ForestGreen)
            {
                DialogResult dialog = MessageBox.Show("Biztosan ki szeretnéd venni ezt a tárgyat a teljesítettek közül?", "Teljesített tárgy törlése", MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                {
                    wasGreen.Add(green_it, (sender as Button));
                    green_it++;
                    (sender as Button).BackColor = System.Drawing.Color.Yellow;
                    targyak.Find(x => x.kod == (sender as Button).Name).teljesitett = false;


                    Targy found = targyak.Find(x => x.nev == (sender as Button).Text);


                    Dictionary<KeyValuePair<string, int>, int> temp = new Dictionary<KeyValuePair<string, int>, int>();
                    //MessageBox.Show(valasztottak.Count().ToString());
                    foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in valasztottak)
                    {
                        if (found.kategoria == elem.Key.Key)
                        {
                            var prev = elem.Value;
                            var prevkey = elem.Key;
                            prev -= found.kredit;
                            if (prev < elem.Key.Value && warned == true)
                            {
                                warned = false;
                            }
                            temp.Add(prevkey, prev);
                        }
                        else
                        {
                            temp.Add(elem.Key, elem.Value);
                        }
                    }
                    valasztottak.Clear();
                    foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in temp)
                    {
                        valasztottak.Add(elem.Key, elem.Value);
                    }
                    temp.Clear();
                    textBox2.Text = "";
                    foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in valasztottak)
                    {
                        textBox2.Text += elem.Key.Key + ":" + elem.Value + "/" + elem.Key.Value;
                        textBox2.AppendText(Environment.NewLine);
                        textBox2.SelectionStart = 0;
                        textBox2.ScrollToCaret();
                    }


                    cmd = new SQLiteCommand("select count(ráépülőkód) from előfeltétele where előfeltételkód='" + found.kod + "' and szakid = " + szakid + " and egyszerrefelveheto = 0");
                    cmd.Connection = adatb.GetConnection();
                    adatb.openConnection();
                    int len = Int32.Parse(cmd.ExecuteScalar().ToString());
                    adatb.closeConnection();
                    //MessageBox.Show(found.kod);
                    //MessageBox.Show(len.ToString());

                    deleteCompleted((sender as Button));
                    recountProgress();
                    dataTables.Clear();
                    wasGreen.Clear();
                    resetGreen();
                    iterator = 0;
                    green_it = 0;
                }
            }

        }


        private void AutoSizeTextBox(TextBox txt)
        {
            const int x_margin = 0;
            const int y_margin = 2;
            Size size = TextRenderer.MeasureText(txt.Text, txt.Font);
            txt.ClientSize =
                new Size(size.Width + x_margin, size.Height + y_margin);
        }
        private void txtContents_TextChanged(object sender, EventArgs e)
        {
            AutoSizeTextBox(sender as TextBox);
        }

        private void ButtonName_MouseHover(object sender, EventArgs e)
        {
            Targy found = targyak.Find(x => x.nev == (sender as Button).Text);
            string msg = "Targykód:" + found.kod + " ,Név:" + found.nev + " ,Ajánlott félév:" + found.ajanlottFelev + " ,Kredit:" + found.kredit + " ," + found.kategoria + ".";

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(sender as Button, msg);

        }

        private void Highlight(Button sender)
        {
            DataTable dt1 = new DataTable();
            if ((sender as Button).BackColor == System.Drawing.Color.ForestGreen)
            {
                Targy found = targyak.Find(x => x.kod == (sender as Button).Name);
                cmd = new SQLiteCommand("select count(ráépülőkód) from előfeltétele where előfeltételkód='" + found.kod + "' and szakid = " + szakid + " and egyszerrefelveheto = 0");
                cmd.Connection = adatb.GetConnection();
                adatb.openConnection();
                int len = Int32.Parse(cmd.ExecuteScalar().ToString());
                adatb.closeConnection();
                //MessageBox.Show(found.kod);
                //MessageBox.Show(len.ToString());


                sda = new SQLiteDataAdapter("select ráépülőkód from előfeltétele where előfeltételkód='" + found.kod + "' and szakid = " + szakid + " and egyszerrefelveheto = 0", adatb.GetConnection());
                dt1 = new DataTable();
                sda.Fill(dt1);

                if (len != 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        //MessageBox.Show((string)dt.Rows[i][0]);
                        if (targyak.Find(x => x.kod == (string)dt1.Rows[i][0]) != null)
                        {
                            Targy t = targyak.Find(x => x.kod == (string)dt1.Rows[i][0]);
                            //MessageBox.Show(t.elofeltetel_db.ToString() + "/" + t.elofeltetelek.Count());
                            if (t.elofeltetel_db + 1 <= t.elofeltetelek.Count())
                            {
                                targyak.Find(x => x.kod == (string)dt1.Rows[i][0]).elofeltetel_db++;
                                //MessageBox.Show(t.elofeltetel_db.ToString());

                            }
                            if (t.elofeltetel_db == t.elofeltetelek.Count() && buttons.Find(x => x.Name == (string)dt1.Rows[i][0]) != null)
                            {
                                buttons.Find(x => x.Name == (string)dt1.Rows[i][0]).BackColor = System.Drawing.Color.Yellow;

                            }
                        }
                    }
                }

            }
        }

        private void SetAvailable()
        {
            foreach(Button button in buttons)
            {
                button.BackColor = System.Drawing.Color.Yellow;
                Targy t = targyak.Find(x => x.kod == button.Name);
                t.teljesitett = false;
                t.elofeltetel_db = 0;
            }
            sda = new SQLiteDataAdapter("select ráépülőkód from előfeltétele where szakid = " + szakid + " and egyszerrefelveheto = 0", adatb.GetConnection());
            dt = new DataTable();
            sda.Fill(dt);
            for(int i =0;i<dt.Rows.Count;i++)
            {
                Button b = buttons.Find(x => x.Name == dt.Rows[i][0].ToString());
                if (b != null)
                {
                    b.BackColor = System.Drawing.Color.White;
                }
            }
        }

        private void resetGreen()
        {
            foreach (Targy t in targyak)
            {
                t.Green = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)//this is the reset
        {
            foreach (Button b in buttons)
            {
                b.BackColor = System.Drawing.Color.White;
            }
            foreach (Targy t in targyak)
            {
                t.elofeltetel_db = 0;
                t.teljesitett = false;
            }
            valasztottak.Clear();
            foreach (KeyValuePair<string, int> elem in valaszthatok)
            {
                valasztottak.Add(elem, 0);
            }
            textBox2.Text = "";
            foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in valasztottak)
            {
                textBox2.Text += elem.Key.Key + ":" + elem.Value + "/" + elem.Key.Value;
                textBox2.AppendText(Environment.NewLine);
                textBox2.SelectionStart = 0;
                textBox2.ScrollToCaret();
            }
            warned = false;
            SetAvailable();
            recountProgress();
        }

        private void deleteCompleted(Button sender)
        {
            Button found = sender;
            Targy tFound = targyak.Find(x => x.kod == found.Name);
            if (wasGreen.ContainsValue(found))
            {
                //MessageBox.Show(targyak.Find(x => x.kod == found.Name).nev + "found");
                tFound.Green = true;

            }

            cmd = new SQLiteCommand("select count(ráépülőkód) from előfeltétele where előfeltételkód='" + found.Name + "' and szakid = " + szakid + " and egyszerrefelveheto = 0");
            cmd.Connection = adatb.GetConnection();
            adatb.openConnection();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            adatb.closeConnection();
            //MessageBox.Show(found.kod);
            //MessageBox.Show(len.ToString());


            sda = new SQLiteDataAdapter("select ráépülőkód from előfeltétele where előfeltételkód='" + found.Name + "' and szakid = " + szakid + " and egyszerrefelveheto = 0", adatb.GetConnection());
            dt = new DataTable();
            sda.Fill(dt);

            dataTables.Add(iterator, dt);
            iterator++;
            DataTable currentdt = new DataTable();
            if (iterator == 1)
            {
                currentdt = dt;
            }
            else
            {
                currentdt = dt;            }
            //MessageBox.Show(count.ToString());

            if (currentdt.Rows.Count > 0)
            {

                for (int i = 0; i < currentdt.Rows.Count; i++)
                {
                    //MessageBox.Show(currentdt.Rows.Count.ToString());
                    Button actual = buttons.Find(x => x.Name == (string)currentdt.Rows[i][0]);
                    Targy t = targyak.Find(x => x.kod == (string)currentdt.Rows[i][0]);
                    if (actual != null)
                    {
                        //MessageBox.Show(t.nev);
                        if (actual.BackColor == System.Drawing.Color.ForestGreen && wasGreen.ContainsValue(actual) == false)
                        {
                            //MessageBox.Show(check.ToString());
                            wasGreen.Add(green_it, found);
                            green_it++;
                            t.Green = true;

                        }
                        actual.BackColor = System.Drawing.Color.White;
                        t.teljesitett = false;
                        Dictionary<KeyValuePair<string, int>, int> temp = new Dictionary<KeyValuePair<string, int>, int>();
                        //MessageBox.Show(valasztottak.Count().ToString());
                        foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in valasztottak)
                        {
                            if (t.kategoria == elem.Key.Key && t.Green ==true)
                            {
                                var prev = elem.Value;
                                var prevkey = elem.Key;
                                prev -= t.kredit;
                                if (prev < elem.Key.Value && warned == true)
                                {
                                    warned = false; ;
                                }
                                temp.Add(prevkey, prev);
                            }
                            else
                            {
                                temp.Add(elem.Key, elem.Value);
                            }
                        }
                        valasztottak.Clear();
                        foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in temp)
                        {
                            valasztottak.Add(elem.Key, elem.Value);
                        }
                        temp.Clear();
                        textBox2.Text = "";
                        foreach (KeyValuePair<KeyValuePair<string, int>, int> elem in valasztottak)
                        {
                            textBox2.Text += elem.Key.Key + ":" + elem.Value + "/" + elem.Key.Value;
                            textBox2.AppendText(Environment.NewLine);
                            textBox2.SelectionStart = 0;
                            textBox2.ScrollToCaret();
                        }

                        //MessageBox.Show(tFound.nev + ":" + tFound.Green.ToString());
                        if (t.elofeltetel_db - 1 >= 0 && tFound.Green == true)
                        {
                            t.elofeltetel_db -= 1;
                            //MessageBox.Show(t.nev + ":" + t.elofeltetel_db.ToString());
                        }
                        deleteCompleted(actual);
                        // MessageBox.Show(t.elofeltetel_db.ToString() + "/" + t.elofeltetelek.Count);
                    }
                }

            }

        }

        private void recountProgress()
        {
            textprogressBar1.Value = 0;
            foreach(Button b in buttons)
            {
                Targy t = targyak.Find(x => x.kod == b.Name);
                if (b.BackColor == System.Drawing.Color.ForestGreen)
                {
                    textprogressBar1.Value += t.kredit;
                }
            }
            
        }

        private void getYellowButtons()
        {
            yellows.Clear();
            foreach (Button btn in buttons) // button.Name = targy.kod alapjan
            {
                if (btn.BackColor == System.Drawing.Color.Yellow)
                { // Ha a gomb hattere sarga akkor hozza adjuk a listahoz
                    yellows.Add(btn.Name);
                }
            }
        }
        private void nextSemesterButton_Click(object sender, EventArgs e)
        {
            getYellowButtons(); // Lekeri a sarga gombok Name valtozoit(kodok)
            Debug.WriteLine("Original size: " + yellows.Count);
            Form3 nextSemester = new Form3(yellows); // Megnyitja a form3-at es at adja a yellows listat
            nextSemester.Show();
        }

    }
}
