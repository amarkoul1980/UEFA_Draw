using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;

namespace UEFA_Draw
{
    public struct Country
    {
        public Country(int id, string n, int p, int r)
        {
            IdNumber = id;
            Name = n;
            Pot = p;
            Ranking = r;
        }

        public int IdNumber { get; }
        public string Name { get; }
        public int Pot { get; }
        public int Ranking { get; }
    }

    public struct Constraint
    {
        public Constraint(int cf, string c1, int p1, int ca, string c2, int p2, bool r)
        {
            Country_For_ID = cf;
            Country_For = c1;
            Country_For_Pot = p1;
            Country_Against_ID = ca;
            Country_Against = c2;
            Country_Against_Pot = p2;
            Reason = r;
        }

        public int Country_For_ID { get; }
        public string Country_For { get; }
        public int Country_For_Pot { get; }
        public int Country_Against_ID { get; }
        public string Country_Against { get; }
        public int Country_Against_Pot { get; }
        public bool Reason { get; }
    }

    public struct Country_with_Index
    {
        public Country_with_Index(string n, int i)
        {
            Name = n;
            Index = i;
        }

        public string Name { get; set; }
        public int Index { get; set; }
    }

    public partial class Form1 : Form
    {
        List<Country> CountryList = new List<Country>();
        List<Constraint> ConstraintList = new List<Constraint>();
        List<ListBox> lstBox = new List<ListBox>();
        int pots, groups, small_groups, large_groups, small_group_teams, large_group_teams;

        public Form1()
        {
            if (!File.Exists("properties.ini"))
            {
                MessageBox.Show("properties.ini file was not found. The program will terminate!");
                Environment.Exit(0);
            }
            IniFile MyIni = new IniFile("properties.ini");

            pots = Convert.ToInt32(MyIni.Read("Pots"));
            groups = Convert.ToInt32(MyIni.Read("Groups"));
            small_groups = Convert.ToInt32(MyIni.Read("Small_Groups"));
            large_groups = Convert.ToInt32(MyIni.Read("Large_Groups"));
            small_group_teams = Convert.ToInt32(MyIni.Read("Small_Groups_Teams"));
            large_group_teams = Convert.ToInt32(MyIni.Read("Large_Groups_Teams"));

            for (int i = 1; i <= groups; i++)
            {
                Label l1 = new Label();
                l1.AutoSize = true;
                l1.Font = new Font(Label.DefaultFont, FontStyle.Bold);
                l1.Left = 700 - ((i % 2) * 300);
                if ((i % 2) == 1) l1.Top = 30 + 80 * (i - 1);
                else l1.Top = 30 + 80 * (i - 2);
                l1.Text = "Όμιλος " + i.ToString();
                Controls.Add(l1);
            }

            for (int i=0;i<groups;i++)
            {
                ListBox l1 = new ListBox();
                l1.Height = l1.ItemHeight * (large_group_teams + 1);
                l1.Left = 700 - ((i % 2) * 300);
                if ((i % 2) == 1) l1.Top = 80 * i;
                else l1.Top = 80 * (i + 1);
                Controls.Add(l1);
                lstBox.Add(l1);
            }

            InitializeComponent();
            load_data();
        }

        public void load_data()
        {
            OleDbConnection con = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;Data Source =|DataDirectory|\\Countries.mdb;");
            con.Open();
            string selectQuery = "SELECT ID, Country, Pot, Ranking FROM Countries ORDER BY Pot, Ranking;";
            OleDbCommand selectprojectcommand = new OleDbCommand(selectQuery, con);
            OleDbDataReader reader = selectprojectcommand.ExecuteReader();
            while (reader.Read())
            {
                CountryList.Add(new Country(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3)));
            }

            selectQuery = "SELECT Constraints.Country_for, C1.Country, C1.Pot, Constraints.Country_against, C2.Country, C2.Pot, Constraints.Prohibited FROM ([Constraints] INNER JOIN Countries AS C1 ON Constraints.Country_for = C1.ID) INNER JOIN Countries AS C2 ON Constraints.Country_against = C2.ID;";
            selectprojectcommand = new OleDbCommand(selectQuery, con);
            reader = selectprojectcommand.ExecuteReader();
            while (reader.Read())
            {
                ConstraintList.Add(new Constraint(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetString(4), reader.GetInt32(5), reader.GetBoolean(6)));
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Params paramWindow = new Params();
            paramWindow.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Countries countriesWindow = new Countries();

            string banned="", hosts="";

            foreach(Country c in CountryList)
            {
                if (c.Pot == -1) banned = banned + c.Name + "\n";
                if (c.Pot == 0) hosts = hosts + c.Name + "\n";
            }
            countriesWindow.set_labels_banned(banned);
            countriesWindow.set_labels_hosts(hosts);

            for (int i=1;i<=pots;i++)
            {
                Label l1 = new Label();
                l1.AutoSize = true;
                l1.Left = 600 - ((i % 2) * 520);
                if ((i % 2) == 1) l1.Top = 100 + 70 * (i - 1);
                else l1.Top = 100 + 70 * (i - 2);
                l1.Text = "Pot " + i.ToString() + ":";
                countriesWindow.Controls.Add(l1);

                Label l2 = new Label();
                l2.AutoSize = true;
                l2.Left = 720 - ((i % 2) * 520);
                if ((i % 2) == 1) l2.Top = 100 + 70 * (i - 1);
                else l2.Top = 100 + 70 * (i - 2);
                foreach (Country c in CountryList)
                {
                    if (c.Pot == i) l2.Text = l2.Text + c.Name + "\n";
                }
                countriesWindow.Controls.Add(l2);
            }

            countriesWindow.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Constraints constraintsWindow = new Constraints();

            int met = 0;
            foreach (Constraint c in ConstraintList)
            {
                Label l1 = new Label();
                l1.AutoSize = true;
                l1.Left = 100;
                l1.Top = 70 + met * 20;
                l1.Text = c.Country_For;
                constraintsWindow.Controls.Add(l1);

                Label l2 = new Label();
                l2.AutoSize = true;
                l2.Left = 300;
                l2.Top = 70 + met * 20;
                l2.Text = c.Country_Against;
                constraintsWindow.Controls.Add(l2);

                Label l3 = new Label();
                l3.AutoSize = true;
                l3.Left = 500;
                l3.Top = 70 + met * 20;
                if (c.Reason) l3.Text = "Αντιπαλότητα";
                else l3.Text = "Απόσταση";
                constraintsWindow.Controls.Add(l3);

                met++;
            }

            constraintsWindow.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            labelCountry.Text = "";
            labelCountry.Update();
            foreach (ListBox lsb in lstBox)
            {
                lsb.Items.Clear();
            }
            for (int i=1;i<=pots;i++)
            {
                int met = 0;
                List<int> numbers = new List<int>();
                List<string> countries = new List<string>();
                List<Country_with_Index> cwI = new List<Country_with_Index>();

                foreach (Country c in CountryList)
                {
                    if (c.Pot == i)
                    {
                        met++;
                        //countries.Add(c.Name);
                        cwI.Add(new Country_with_Index(c.Name,0));

                        int country_id = c.IdNumber;
                        foreach (Constraint con in ConstraintList)
                        {
                            int country_for_id = con.Country_For_ID;
                            int country_against_id = con.Country_Against_ID;
                            int country_for_pot = con.Country_For_Pot;
                            int country_against_pot = con.Country_Against_Pot;

                            if (((country_for_id==country_id) && (country_against_pot < c.Pot)) || ((country_against_id == country_id) && (country_for_pot < c.Pot)))
                            {
                                Country_with_Index temp = cwI[cwI.Count - 1];
                                temp.Index++;
                                cwI[cwI.Count - 1] = temp;
                            }
                        }
                    }
                }

                bool sorting_switch = true;
                while (sorting_switch==true)
                {
                    sorting_switch = false;

                    for (int q=0;q<cwI.Count-1;q++)
                    {
                        if (cwI[q].Index<cwI[q+1].Index)
                        {
                            Country_with_Index temp = cwI[q];
                            cwI[q] = cwI[q + 1];
                            cwI[q + 1] = temp;
                            sorting_switch = true;
                        }
                    }
                }

                //countries.Clear();
                for (int q = 0; q < cwI.Count; q++)
                {
                    countries.Add(cwI[q].Name);
                }

                var bucket = Enumerable.Range(1, met).ToList();
                var random = new Random();

                for (int j = 0; j < met; j++)
                {
                    var index = -1;
                    var number = -1;
                    if (cwI[j].Index>0) index = j;
                    else index = random.Next(bucket.Count);
                    number = bucket[index];
                    bucket.RemoveAt(index);

                    if (checkBox1.Checked == true)
                    {
                        labelCountry.Text = cwI[number - 1].Name;
                        labelCountry.Update();
                        System.Threading.Thread.Sleep(100);
                    }

                    if (i == 1)
                    {
                        lstBox[j].Items.Add(cwI[number - 1].Name);
                        lstBox[j].Update();
                        if (checkBox1.Checked == true) System.Threading.Thread.Sleep(100);
                    }
                    else
                    {
                        List<int> potential_groups = new List<int>();
                        for (int q = 0; q < met; q++)
                        {
                            bool sw = false;

                            if (lstBox[q].Items.Count < i)
                            {
                                for (int r = 0; r < lstBox[q].Items.Count; r++)
                                {
                                    string str = lstBox[q].Items[r].ToString();

                                    foreach (Constraint c in ConstraintList)
                                    {
                                        if (((c.Country_For == cwI[number - 1].Name) && (c.Country_Against == str)) || ((c.Country_For == str) && (c.Country_Against == cwI[number - 1].Name)))
                                        {
                                            sw = true;
                                        }
                                    }
                                }
                                if (sw == false)
                                {
                                    potential_groups.Add(q);
                                }
                            }


                        }

                        if (checkBox1.Checked)
                        {
                            labelPots.Text = "";
                            labelPots.Update();
                            foreach (int pg in potential_groups)
                            {
                                labelPots.Text = labelPots.Text + "Όμιλος " + (pg + 1) + "\n";
                            }
                            labelPots.Update();
                        }

                        var random2 = new Random();
                        var index2 = random2.Next(potential_groups.Count);

                        lstBox[potential_groups[index2]].Items.Add(cwI[number - 1].Name);
                        lstBox[potential_groups[index2]].Update();

                        if (checkBox1.Checked == true) System.Threading.Thread.Sleep(100);
                    }
                }
            }
            button4.Enabled = true;
            checkBox1.Enabled = true;
            labelCountry.Text = "";
            labelPots.Text = "";
        }
    }
}
