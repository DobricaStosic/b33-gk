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
    public partial class frmSpisakTerena : Form
    { 
        OleDbConnection connnection;
        DateTime datum1, datum2;
        bool temp;
        public frmSpisakTerena()
        {
            InitializeComponent();
            string putanja = Environment.CurrentDirectory;
            string[] putanjaBaze = putanja.Split(new string[] { "bin" }, StringSplitOptions.None);
            AppDomain.CurrentDomain.SetData("DataDirectory", putanjaBaze[0]);

            connnection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=GolfKlub2.accdb");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSpisakTerena_Load(object sender, EventArgs e)
        {
            this.terenTableAdapter.Fill(this.golfKlub2DataSet.Teren);
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string datestring1 = comboBox2.Text + "/" + comboBox1.Text + "/" + maskedTextBox1.Text;
            string datestring2 = comboBox4.Text + "/" + comboBox3.Text + "/" + maskedTextBox2.Text;
            if (!(DateTime.TryParse(datestring1, out datum1)) || !(DateTime.TryParse(datestring2, out datum2)))
            {
                MessageBox.Show("Unesite validan datum", "Greska");
            }
            else
            {
                OleDbCommand command = new OleDbCommand("select distinct Teren.TerenID, Teren.Teren " +
                                                    "from Teren inner join Partija on Teren.TerenID = Partija.TerenID " + 
                                                    "where Partija.Datum between @datum1 and @datum2 " +
                                                    "order by Teren.TerenID", connnection);
                command.Parameters.AddWithValue("@datum1", datum1);
                command.Parameters.AddWithValue("@datum2", datum2);

                connnection.Open();
                OleDbDataReader reader = command.ExecuteReader();
                dataGridView1.Rows.Clear();
                while (reader.Read())
                {
                    int br = new int();
                    OleDbCommand command2 = new OleDbCommand("select Partija.PartijaID from Partija where Partija.TerenID = @id and Partija.Datum between @dat1 and @dat2", connnection);
                    command2.Parameters.AddWithValue("@id", reader[0]);
                    command2.Parameters.AddWithValue("@dat1", datum1);
                    command2.Parameters.AddWithValue("@dat2", datum2);
                    OleDbDataReader reader2 = command2.ExecuteReader();
                    while (reader2.Read())
                    {
                        br++;
                    }
                    reader2.Close();
                    command2.Dispose();
                    temp = false;
                    dataGridView1.Rows.Add(reader[0], reader[1], br);
                }
                reader.Close();
                command.Dispose();
                connnection.Close();
                temp = true;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                listView1.Items.Clear();
                OleDbCommand command = new OleDbCommand("select Partija.PartijaID, Partija.Datum " +
                                                    "from Partija inner join Teren on Partija.TerenID = Teren.TerenID " +
                                                    "where Teren.TerenID = @id and Partija.Datum between @datum1 and @datum2 " +
                                                    "order by Partija.Datum, Partija.PartijaID", connnection);

                command.Parameters.AddWithValue("@id", dataGridView1.SelectedRows[0].Cells[0].Value);
                command.Parameters.AddWithValue("@datum1", datum1);
                command.Parameters.AddWithValue("@datum2", datum2);

                if (connnection.State == ConnectionState.Closed)
                {
                    connnection.Open();
                }

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DateTime d;
                    DateTime.TryParse(reader[1].ToString(), out d);
                    string[] str = new string[] { reader[0].ToString(), d.ToShortDateString()};
                    ListViewItem item = new ListViewItem(str);
                    listView1.Items.Add(item);
                }
                reader.Close();
                command.Dispose();
                if (temp)
                {
                    connnection.Close();
                }
            }
        }
    }    
}
