using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace FlappyBird
{
    public partial class Form1 : Form
    {
        Graphics g;
        Rectangle[] buizenBoven;
        Rectangle[] buizenBeneden;
        Bitmap imgflapje;
        Bitmap achtergrondpanel;
        Bitmap imgbuisBoven;
        Bitmap imgbuisOnder;
        int score;
        Random rn;
        bool botsing;
        Rectangle rflappie;
        int yflappie;
        string gamestatus;
        int afstandHorizontaal;
        int afstandVerticaal;
        bool end;
        WindowsMediaPlayer player = new WindowsMediaPlayer();
        public Form1()
        {
            InitializeComponent();
            initialiseer();
        }

        private void initialiseer()
        {
            player.URL = "music.wav";
            gameItemsconfig();

        }
        private void gameItemsconfig()
        {
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null,
            panel1, new object[] { true });//flikkeren voorkomen

            botsing = false;
            rn = new Random();
            score = 0;
            gamestatus = "";
            afstandHorizontaal = 300;
            afstandVerticaal = 150;
            end = false;

            achtergrondpanel = new Bitmap("media/Achtergrond.jpg"); //achtergrond instellen
            
            //buizen
            imgbuisBoven = new Bitmap("media/pipe.png");
            imgbuisOnder = new Bitmap("media/pipe2.png");
            imgbuisBoven.MakeTransparent();
            imgbuisOnder.MakeTransparent();
            buizenBoven = new Rectangle[2];// 2 op 1 gamePanel
            buizenBeneden = new Rectangle[2];

            maakbuizen(600);//coordinaten van de rechthoeken

            //basisinstellingen vogeltje
                imgflapje = new Bitmap("media/starly.png");
            
            
            imgflapje.MakeTransparent();
            yflappie = panel1.Height / 2;
        }

        private void maakbuizen(int beginx)
        {
            int[] hoogtenieuwpaar = new int[] { 0, rn.Next(20, 80) };
            for (int i = 0; i < buizenBoven.Length; i++)//2 buizen
            {
                int ybegin = -60 + hoogtenieuwpaar[i];
                buizenBeneden[i] = new Rectangle(beginx + (afstandHorizontaal * i),
                ybegin, imgbuisOnder.Width, imgbuisOnder.Height);

                buizenBoven[i] = new Rectangle(beginx + (afstandHorizontaal * i),
                ybegin + afstandVerticaal + imgbuisOnder.Height, imgbuisBoven.Width, imgbuisBoven.Height);
                //rectangle(pos x, pos y, breedte, hoogte)
            }
            Console.WriteLine(buizenBeneden[0].Y + " " + buizenBeneden[1].Y);
        }

        private void TmrAchtergrond_Tick(object sender, EventArgs e)
        {
            toonBuizen();
            panel1.Invalidate();
        }

        private void toonBuizen()
        {
            yflappie += 5;//vogel zakt

            if (buizenBoven[0].X <= 0 - imgbuisBoven.Width-afstandHorizontaal)
            {
                score += 2;
                maakbuizen(panel1.Width - rn.Next(50, 120));
            }
            else
            {
                for (int i = 0; i < buizenBoven.Length; i++)
                {
                    buizenBoven[i].X -= 10;
                    buizenBeneden[i].X -= 10;
                }
            }

        }

        private void TmrVlieg_Tick(object sender, EventArgs e)
        {
            //botsen met de buizen detecteren = doorsnede van de rechthoeken
            for (int i = 0; i < buizenBeneden.Length; i++)
            {
                botsing = buizenBeneden[i].IntersectsWith(rflappie) ||
                    buizenBoven[i].IntersectsWith(rflappie) ||
                    (yflappie + 40) > panel1.Height;

                if (botsing) break; //stoppen van zodra botsing

            }
            if (botsing)
            {
                TmrVlieg.Stop();
                tmrAchtergrond.Stop();
                gamestatus = "Game Over";
                Loser frm = new Loser();
                frm.Show();
                player.controls.stop();
                this.Close();
                
            }
            else
            {
                yflappie += rn.Next(-2, 6); //op en neer fladderen
                panel1.Invalidate();
            }
        }

        private void Stijgen(object sender, EventArgs e)
        {
            
            if (score < 10)
            {
                imgflapje = new Bitmap("media/starly.png");
                imgflapje.MakeTransparent();
            }
            else if (score >=10 & score <20)
            {
                imgflapje = new Bitmap("media/staravia.png");
                imgflapje.MakeTransparent();
            }
            else
            {
                imgflapje = new Bitmap("media/staraptor.png");
                imgflapje.MakeTransparent();
            }
            if (end)
            {
                //klikken op paneel = stijgen
                yflappie -= 50;
                panel1.Invalidate();
            }
        }

        private void ValidatieToetsen(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.S) yflappie += 40;
            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Z) yflappie -= 40; 
            panel1.Invalidate();
        }

        private void SpelWeergeven(object sender, PaintEventArgs e)
        {
            g = e.Graphics;//achtergrond
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //afbeelding resize van een rechthoek1 op rechthoek2
            g.DrawImage(achtergrondpanel, new Rectangle(0, 0, 2500, 1500),
                0, 0, panel1.Width, panel1.Height, GraphicsUnit.Millimeter, null);

            //buizen
            for (int i = 0; i < buizenBeneden.Length; i++)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //afbeelding resize van een rechthoek1 op een rechthoek2
                //g.DrawRectangle(new Pen(Color.Red, 2), buizenBoven[i]); //test intersectie
                g.DrawImage(imgbuisBoven, buizenBoven[i],
                    0, 0, imgbuisBoven.Width, imgbuisBoven.Height, GraphicsUnit.Pixel, null);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
               //g.DrawRectangle(new Pen(Color.Red, 2), buizenBeneden[i]); //test intersectie
                g.DrawImage(imgbuisOnder, buizenBeneden[i],
                    0, 0, imgbuisOnder.Width, imgbuisBoven.Height, GraphicsUnit.Pixel, null);

            }
            //vogel
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            rflappie = new Rectangle(80, yflappie, 50, 50);
           // g.DrawRectangle(new Pen(Color.Yellow, 2), rflappie); //test intersectie
            g.DrawImage(imgflapje, rflappie, 0, 0, 110, 90, GraphicsUnit.Millimeter, null);
            Font fn = new Font("Callibri", 24, FontStyle.Bold);
            g.DrawString(gamestatus, fn, new SolidBrush(Color.Yellow), rflappie.X + 70, rflappie.Y);
            g.DrawString(score.ToString(), fn, new SolidBrush(Color.Yellow), 30, 50);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            TmrVlieg.Start();
            tmrAchtergrond.Start();
            end = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            player.controls.play();
        }
    }
}
