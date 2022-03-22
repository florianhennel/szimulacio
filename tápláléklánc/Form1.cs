using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tápláléklánc
{

    public partial class Form1 : Form
    { 

        public Form1()
        {
            InitializeComponent();

        }
        class Vektor
        {
            public double X { get; set; }
            public double Y { get; set; }

            public Vektor(double x, double y) => (X, Y) = (x, y);
            public Vektor(Point P) => (X, Y) = ((double)P.X, (double)P.Y);

            public static Vektor operator +(Vektor a, Vektor b) => new Vektor(a.X + b.X, a.Y + b.Y);
            public static Vektor operator -(Vektor a, Vektor b) => new Vektor(a.X - b.X, a.Y - b.Y);
            public static double operator *(Vektor a, Vektor b) => a.X * b.X + a.Y * b.Y;
            public static Vektor operator *(double lambda, Vektor b) => new Vektor(lambda * b.X, lambda * b.Y);
            public static Vektor operator *(Vektor b, double lambda) => lambda * b;
            public double távolságnégyzete(Vektor b) => (this - b).HosszNégyzet();
            public static Vektor operator /(Vektor b, double lambda) => (1 / lambda) * b;
            public double HosszNégyzet() => X * X + Y * Y;
            public double Hossz() => Math.Sqrt(HosszNégyzet());



        }
        class Allat
        {
            public static List<Allat> lista = new List<Allat>();
            public string nev;
            public Color szin;
            Label nevcimke;
            Label mcimke;
            Label helycimke;
            Label vcimke;


            public double m;
            public Vektor hely;
            public Vektor v; // speed=sebesség velocity=sebességvektor, egy időegység alatt hova mozdul el


            public Brush toll;

            public Allat(string nev, Color szin, double m, Vektor hely, Vektor v, Panel panel)
            {
                this.nev = nev;
                this.m = m;
                this.hely = hely;
                this.v = v;

                int letolas = Allat.lista.Count * 100;

                this.nevcimke = new Label();
                this.nevcimke.Text = this.nev;
                this.nevcimke.Location = new Point(0, 10 + letolas);
                this.nevcimke.ForeColor = szin;
                this.nevcimke.AutoSize = true;
                panel.Controls.Add(nevcimke);

                toll = new SolidBrush(szin);

                this.mcimke = new Label();
                this.mcimke.Location = new Point(80, 10 + letolas);
                panel.Controls.Add(mcimke);

                this.helycimke = new Label();
                this.helycimke.Location = new Point(10, 35 + letolas);
                panel.Controls.Add(helycimke);

                this.vcimke = new Label();
                this.vcimke.Location = new Point(10, 60 + letolas);
                panel.Controls.Add(vcimke);

                MonitorFrissít();
                Allat.lista.Add(this);



            }
            private void Léptet() => hely += v;
            private void MonitorFrissít()
            {
                helycimke.Text = $"hely = ({hely.X};{hely.Y})";
                vcimke.Text = $"v = ({v.X};{v.Y})";
                mcimke.Text = $"r= {m}, {rajta_van_e_a_pályán(this)}";
            }

            public static void Rajzold_le_mind_ide(PictureBox palya)
            {
                Bitmap kep = new Bitmap(palya.Width, palya.Height);
                Graphics rajzolókészlet = Graphics.FromImage(kep);

                foreach (Allat item in Allat.lista)
                    item.Rajzoldle(rajzolókészlet);

                palya.Image = kep;
                palya.Refresh();
            }

            void Rajzoldle(Graphics rajzolókészlet)
            {
                double r = Math.Sqrt(m*15);
                int xp = (int)Math.Round(hely.X - r);
                int yp = (int)Math.Round(hely.Y - r);
                int R = (int)Math.Round(r);

                rajzolókészlet.FillEllipse(this.toll, new Rectangle(xp, yp, 2 * R, 2 * R));
            }
            public static bool rajta_van_e_a_pályán(Allat a)
            {
                bool rajta = true;
                if (a.hely.X-(Math.Round(Math.Sqrt(a.m*15)))<0 || a.hely.X+ (Math.Round(Math.Sqrt(a.m*15))) > 1000)
                {
                    rajta = false;
                }
                if (a.hely.Y - (Math.Round(Math.Sqrt(a.m*15))) < 0 || a.hely.Y + (Math.Round(Math.Sqrt(a.m*15))) > 550)
                {
                    rajta = false;
                }
                return rajta;
            }
            public static bool találkozás(Allat a, Allat b) 
            {
                bool találkoznak = false;   
                if (Math.Sqrt(Math.Pow((a.hely.X - b.hely.X), 2) + Math.Pow((a.hely.Y - b.hely.Y), 2)) < 50)
                {
                    találkoznak = true;
                }
                
                return találkoznak;
            }
            public static void Összes_léptetése(Allat a)
            {
                /*for (int i = 0; i < lista.Count; i++)
                {
                    if (rajta_van_e_a_pályán(lista[i]))
                    {
                        for (int j = 0; j < lista.Count; j++)
                        {
                            if (i != j)
                            {
                                if (találkozás(lista[i], lista[j]))
                                {
                                    Vektor régi2 = new Vektor(lista[j].v.X,lista[j].v.Y);
                                    Vektor régi1 = new Vektor(lista[i].v.X,lista[i].v.Y);
                                    lista[i].v.X = (lista[j].v.X - lista[i].hely.X) / 100;
                                    lista[i].v.Y = (lista[j].v.Y - lista[i].hely.Y) / 100;
                                    lista[j].v.X = -lista[i].v.X;
                                    lista[j].v.Y = -lista[i].v.Y;
                                    
                                    if (lista[i].m > lista[j].m)
                                    {
                                        lista[i].m+=lista[j].m;
                                        lista[i].v = régi1;
                                        lista.RemoveAt(j);
                                        break;

                                    }
                                    else if (lista[i].m < lista[j].m)
                                    {
                                        lista[j].m+=lista[i].m;
                                        lista[j].v = régi2;
                                        lista.RemoveAt(i);
                                        break;

                                    }
                                    else if (lista[i].m == lista[j].m)
                                    {
                                        Random r = new Random();
                                        if (r.Next(1, 2) == 1)
                                        {
                                            lista[i].m+=lista[j].m;
                                            lista[i].v = régi1;
                                            lista.RemoveAt(j);
                                            break;
                                        }
                                        else
                                        {
                                            lista[j].m+=lista[i].m;
                                            lista[j].v = régi2;
                                            lista.RemoveAt(i);
                                            break;
                                        }
                                    }
                                }
                                else
                                    lista[i].Léptet();break;
                            }
                            else
                            {
                                lista[i].Léptet();break;
                            }

                        }
                    }
                    else
                    {
                          if (lista[i].hely.X - (Math.Round(Math.Sqrt(lista[i].m * 15))) < 0)
                          {
                                lista[i].v.X = -(lista[i].v.X); 
                                lista[i].Léptet();
                          }
                          else if (lista[i].hely.X + (Math.Round(Math.Sqrt(lista[i].m * 15))) > 1000)
                          {
                                lista[i].v.X = -(lista[i].v.X); 
                                lista[i].Léptet();
                          }
                          else if (lista[i].hely.Y - (Math.Round(Math.Sqrt(lista[i].m * 15))) < 0)
                          {
                                lista[i].v.Y = -(lista[i].v.Y); 
                                lista[i].Léptet();
                          }
                          else if (lista[i].hely.Y + (Math.Round(Math.Sqrt(lista[i].m * 15))) > 550)
                          {
                                lista[i].v.Y = -(lista[i].v.Y); 
                                lista[i].Léptet();
                          }     
                    }
                    continue;
                }*/
                if (rajta_van_e_a_pályán(a))
                {
                    for (int j = 0; j < lista.Count; j++)
                    {
                        if (a != lista[j])
                        {
                            if (találkozás(a, lista[j]))
                            {
                                Vektor régi2 = new Vektor(a.v.X, lista[j].v.Y);
                                Vektor régi1 = new Vektor(a.v.X, a.v.Y);
                                a.v.X = (lista[j].v.X - a.hely.X) / 100;
                                a.v.Y = (lista[j].v.Y - a.hely.Y) / 100;
                                lista[j].v.X = -a.v.X;
                                lista[j].v.Y = -a.v.Y;

                                if (a.m > lista[j].m)
                                {
                                    a.m += lista[j].m;
                                    a.v = régi1;
                                    lista.RemoveAt(j);
                                    break;

                                }
                                else if (a.m < lista[j].m)
                                {
                                    lista[j].m += a.m;
                                    lista[j].v = régi2;
                                    lista.Remove(a);
                                    break;

                                }
                                else if (a.m == lista[j].m)
                                {
                                    Random r = new Random();
                                    if (r.Next(1, 2) == 1)
                                    {
                                        a.m += lista[j].m;
                                        a.v = régi1;
                                        lista.RemoveAt(j);
                                        break;
                                    }
                                    else
                                    {
                                        lista[j].m += a.m;
                                        lista[j].v = régi2;
                                        lista.Remove(a);
                                        break;
                                    }
                                }
                            }
                            else
                                a.Léptet(); break;
                        }
                        else
                        {
                            a.Léptet(); break;
                        }

                    }
                }
                else
                {
                    if (a.hely.X - (Math.Round(Math.Sqrt(a.m * 15))) < 0)
                    {
                        a.v.X = -(a.v.X);
                        a.Léptet();
                    }
                    else if (a.hely.X + (Math.Round(Math.Sqrt(a.m * 15))) > 1000)
                    {
                        a.v.X = -(a.v.X);
                        a.Léptet();
                    }
                    else if (a.hely.Y - (Math.Round(Math.Sqrt(a.m * 15))) < 0)
                    {
                        a.v.Y = -(a.v.Y);
                        a.Léptet();
                    }
                    else if (a.hely.Y + (Math.Round(Math.Sqrt(a.m * 15))) > 550)
                    {
                        a.v.Y = -(a.v.Y);
                        a.Léptet();
                    }
                }
            }
            internal static void OsszesMonitor()
            {
                foreach (Allat item in Allat.lista)
                    item.MonitorFrissít();
            }
        }

        private void button1_Click(object sender, EventArgs e) => Metronomkapcsolas();
        bool fut = false;
        private void Metronomkapcsolas()
        {
            if (fut)
            {
                metronom.Stop();
                button1.Text = "START";

            }
            else
            {
                metronom.Start();
                button1.Text = "STOP";
                Allat.OsszesMonitor();
            }
            fut = !fut;
        }

        private void metronom_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Allat.lista.Count; i++)
            {
                Allat.Összes_léptetése(Allat.lista[i]);
            }
            //Allat.Összes_léptetése(); // megváltoztatja a pozíciókat
            Allat.Rajzold_le_mind_ide(palya); // Kiírja az eredményt
            Allat.OsszesMonitor();
        }
        private void palya_MouseDown(object sender, MouseEventArgs i)
        {
            Random r = new Random();
            new Allat(textBox1.Text, Color.Green, (double)numericUpDown1.Value, new Vektor(i.Location.X, i.Location.Y), new Vektor(r.Next(-3,3), r.Next(-3,3)), monitorpanel);
            Allat.Rajzold_le_mind_ide(palya); // Kiírja az eredményt
            metronom.Stop();
        }

        private void palya_MouseUp(object sender, MouseEventArgs i)
        {
            Allat.Rajzold_le_mind_ide(palya); // Kiírja az eredményt
            textBox1.Clear();
            if (fut)
                metronom.Start();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Allat.lista.Clear();
            monitorpanel.Controls.Clear();
            Allat.Rajzold_le_mind_ide(palya);
            fut = false;
            metronom.Stop(); button1.Text = "START";
            /*new Allat(textBox1.Text, Color.Green, (double)numericUpDown1.Value, new Vektor(500, 225), new Vektor(-1,1), monitorpanel);
            Allat.Rajzold_le_mind_ide(palya);*/
        }
    }
}
