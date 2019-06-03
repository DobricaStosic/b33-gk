using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace KecojevicB33
{
    public partial class frmIgraci : Form
    {
        OleDbConnection connection;

        bool sifra = false;
        public frmIgraci()
        {
            InitializeComponent();
            string path = Environment.CurrentDirectory;
            string[] pathB = path.Split(new string[] { "bin" }, StringSplitOptions.None);
            AppDomain.CurrentDomain.SetData("DataDirectory", pathB[0]);
            connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=GolfKlub2.accdb");
        }
        public void isprazni()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            maskedTextBox1.Text = "";
            comboBox2.SelectedIndex = 0;
        }
        public string numberStrip(string broj)
        {
            broj = broj.Replace("-", "");
            broj = broj.Replace("(", "");
            broj = broj.Replace(")", "");
            broj = broj.Replace(" ", "");
            return broj;
        }
        public bool validacija()
        {
            string text = textBox4.Text;
            bool mejl = true;
            if (text.IndexOf('@') == -1)
            {
                mejl = false;
            }
            else if (text.IndexOf('.', text.IndexOf('@')) == -1)
            {
                mejl = false;
            }
            string broj = numberStrip(maskedTextBox1.Text);          

            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("Polja su prazna!", "Greska");
                return false;
            }
            else if (!mejl)
            {
                MessageBox.Show("Mejl mora biti u formatu abc@def.com!", "Greska");
                return false;
            }
            else if (broj.Count() != 9 && broj.Count() != 10)
            {
                MessageBox.Show("Telefon mora biti u formatu (___)___-____ sa 9 ili 10 cifara!", "Greska");
                return false;
            }
            else{
                return true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmIgraci_Load(object sender, EventArgs e)
        {
            OleDbCommand command = new OleDbCommand("select IgracID from Igrac", connection);
            connection.Open();
            OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                comboBox1.Items.Add(reader[0]);
            }
            command = new OleDbCommand("select Grad from Grad order by Grad", connection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox2.Items.Add(reader[0]);
            }
            comboBox2.SelectedIndex = 0;
            reader.Close();
            command.Dispose();
            connection.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            OleDbCommand command = new OleDbCommand("select Igrac.Ime, Igrac.Prezime, Igrac.Adresa, Grad.Grad, Igrac.Email, Igrac.Telefon " +
                                                "from Igrac inner join Grad on Igrac.GradID = Grad.GradID " +
                                                "where IgracID = @id", connection);
            command.Parameters.AddWithValue("@id", comboBox1.SelectedItem.ToString());
            connection.Open();
            OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                sifra = true;
                textBox1.Text = reader[0].ToString();
                textBox2.Text = reader[1].ToString();
                textBox3.Text = reader[2].ToString();
                textBox4.Text = reader[4].ToString();
                maskedTextBox1.Text = reader[5].ToString();

                int selected = new int();
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (comboBox2.Items[i].ToString() == reader[3].ToString())
                    {
                        selected = i;
                    }
                }
                comboBox2.SelectedIndex = selected;
            }
            reader.Close();
            command.Dispose();
            connection.Close();
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            sifra = false;
            OleDbCommand command = new OleDbCommand("select Igrac.Ime, Igrac.Prezime, Igrac.Adresa, Grad.Grad, Igrac.Email, Igrac.Telefon " +
                                            "from Igrac inner join Grad on Igrac.GradID = Grad.GradID " +
                                            "where IgracID = @id", connection);
            command.Parameters.AddWithValue("@id", comboBox1.Text);
            connection.Open();
            int id;
            if (int.TryParse(comboBox1.Text, out id))
            {
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    sifra = true;
                    textBox1.Text = reader[0].ToString();
                    textBox2.Text = reader[1].ToString();
                    textBox3.Text = reader[2].ToString();
                    textBox4.Text = reader[4].ToString();
                    maskedTextBox1.Text = reader[5].ToString();

                    int selected = new int();
                    for (int i = 0; i < comboBox2.Items.Count; i++)
                    {
                        if (comboBox2.Items[i].ToString() == reader[3].ToString())
                        {
                            selected = i;
                        }
                    }
                    comboBox2.SelectedIndex = selected;
                }
                if (!sifra)
                {
                    isprazni();
                }
                reader.Close();
            }
            else{
                isprazni();
            }
            command.Dispose();
            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sifra)
            {
                MessageBox.Show("Sifra postoji!", "Greska");
            }
            else
            {
                int id;
                if (int.TryParse(comboBox1.Text, out id))
                {
                    if(validacija())
                    {
                        string mejl = textBox4.Text;
                        string broj = numberStrip(maskedTextBox1.Text); 
                        OleDbCommand commandI = new OleDbCommand("insert into Igrac(IgracID, Ime, Prezime, Adresa, GradID, Email, Telefon) values(@id, @ime, @prezime, @adresa, @gradid, @email, @telefon)", connection);
                        OleDbCommand commandS = new OleDbCommand("select Grad.GradID from Grad where Grad = @grad", connection);
                        commandS.Parameters.AddWithValue("@grad", comboBox2.Text);
                        connection.Open();
                        OleDbDataReader reader = commandS.ExecuteReader();
                        int gradid = new int();
                        while (reader.Read())
                        {
                            int.TryParse(reader[0].ToString(), out gradid);
                        }
                        commandI.Parameters.AddWithValue("@id", id);
                        commandI.Parameters.AddWithValue("@ime", textBox1.Text);
                        commandI.Parameters.AddWithValue("@prezime", textBox2.Text);
                        commandI.Parameters.AddWithValue("@adresa", textBox3.Text);
                        commandI.Parameters.AddWithValue("@gradid", gradid);
                        commandI.Parameters.AddWithValue("@email", mejl);
                        commandI.Parameters.AddWithValue("@telefon", broj);

                        OleDbDataAdapter adapter = new OleDbDataAdapter();
                        adapter.InsertCommand = commandI;
                        adapter.InsertCommand.ExecuteNonQuery();
                        MessageBox.Show("Igrac upisan");
                        isprazni();
                        sifra = true;

                        commandI = new OleDbCommand("select IgracID from Igrac", connection);
                        reader = commandI.ExecuteReader();

                        comboBox1.Items.Clear();

                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader[0]);
                        }

                        reader.Close();
                        commandS.Dispose();
                        commandI.Dispose();
                        adapter.Dispose();
                        connection.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Unesite validnu sifru", "Greska");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!sifra)
            {
                MessageBox.Show("Sifra ne postoji", "Greska");
            }
            else
            {
                int id;
                if (int.TryParse(comboBox1.Text, out id))
                {
                    OleDbCommand command = new OleDbCommand("delete from Igrac where IgracID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                    MessageBox.Show("Igrac obrisan");
                    sifra = false;
                    command = new OleDbCommand("select IgracID from Igrac", connection);
                    OleDbDataReader reader = command.ExecuteReader();
                    comboBox1.Items.Clear();
                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader[0]);
                    }
                    isprazni();
                    connection.Close();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!sifra)
            {
                MessageBox.Show("Sifra ne postoji!", "Greska");
            }
            else
            {
                int id;
                if (int.TryParse(comboBox1.Text, out id))
                {
                    if(validacija())
                    {
                        string mejl = textBox4.Text;
                        string broj = numberStrip(maskedTextBox1.Text); 
                        OleDbCommand commandU = new OleDbCommand("update Igrac set Ime=@ime, Prezime=@prezime, Adresa=@adresa, GradID=@gradid, Email=@email, Telefon=@telefon where IgracID = @id", connection);
                        OleDbCommand commandS = new OleDbCommand("select Grad.GradID from Grad where Grad = @grad", connection);
                        commandS.Parameters.AddWithValue("@grad", comboBox2.Text);
                        connection.Open();
                        OleDbDataReader reader = commandS.ExecuteReader();
                        int gradid = new int();
                        while (reader.Read())
                        {
                            int.TryParse(reader[0].ToString(), out gradid);
                        }
                        commandU.Parameters.AddWithValue("@ime", textBox1.Text);
                        commandU.Parameters.AddWithValue("@prezime", textBox2.Text);
                        commandU.Parameters.AddWithValue("@adresa", textBox3.Text);
                        commandU.Parameters.AddWithValue("@gradid", gradid);
                        commandU.Parameters.AddWithValue("@email", mejl);
                        commandU.Parameters.AddWithValue("@telefon", broj);
                        commandU.Parameters.AddWithValue("@id", id);

                        OleDbDataAdapter adapter = new OleDbDataAdapter();
                        adapter.UpdateCommand = commandU;
                        adapter.UpdateCommand.ExecuteNonQuery();
                        MessageBox.Show("Podaci su uspesno azurirani.", id.ToString() + ".-" + textBox1.Text + " " + textBox2.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        sifra = true;

                        commandU = new OleDbCommand("select IgracID from Igrac", connection);
                        reader = commandU.ExecuteReader();
                        comboBox1.Items.Clear();
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader[0]);
                        }

                        reader.Close();
                        commandS.Dispose();
                        commandU.Dispose();
                        adapter.Dispose();
                        connection.Close();
                    }
                }
            }
        }
    }
}
