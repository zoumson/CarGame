using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        /*
         * In total 4 levels to win the game
         * For each level player neeed to collect 10 coins 
         * The speed of the game increases as the level increases
         * Any enemy intercepted leads to the end of the game
         * At the end of the game player can choose to play agin or exit 
          */

        private readonly System.Media.SoundPlayer player1 = new System.Media.SoundPlayer();
        public Form1()
        {
            InitializeComponent();
            over.Visible = false;
            play.Visible = false;
            exit.Visible = false;
            winner.Visible = false;
            player1.SoundLocation = "mm.wav";
            player1.PlayLooping();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
                MoveLine(gameSpeed);
                MoveEnemy(gameSpeed);
                MoveCoin(gameSpeed);
                GameOver();
                CoinsCollection();
        }
        private readonly Random r = new Random(); // random number generator 
        private int totalCoins = 0,gameSpeed = 0; // Count collected coins and set the game speed
        private int minSpeed = 0;// Minimum speeed for each level 
        private readonly int maxSpeed = 21;// Maximum allowed speed 
        private readonly int speedOffset = 0;// offset increase the speed for each level 
        private readonly int maxLevel = 5; // max level to be winner  
        private readonly int nextLevelCoins = 10; //Coins Needed to pass to next level
        private readonly int nextLevelTimePause = 1; // Pause the game after reaching next level 
   

        void MoveEachLine(PictureBox line, int speed)// Move each line 
        {
            if (line.Top > this.Height)
            {
                line.Top = 0;
            }
            else line.Top += speed;
        }
        void MoveLine(int speed)// Move the entire lines
        {
            PictureBox[] allLines = { line1, line2, line3, line4};// 4 lines
            foreach (PictureBox line in allLines) MoveEachLine(line, speed);
        }
       
        void MoveEachEnenmy(PictureBox enemy, int speed, int lb, int ub)// Move each enemy
        {
            if (enemy.Top > this.Height)
            {
                enemy.Location = new Point(r.Next(lb, ub), 0);
            }
            else enemy.Top += speed;
        }
        void MoveEnemy(int speed)// Move the entire enemies
        {
            PictureBox[] allEnemies = { enemy1, enemy2, enemy3, enemy4 };// 4 enemies
            for(int i = 0; i < allEnemies.Length; i++)
            {
                if(i %2 == 0)MoveEachEnenmy(allEnemies[i], speed, 30, 180);// First half of the form at the left 
                else MoveEachEnenmy(allEnemies[i], speed, 220, 350); // Second half at the right side 
            }
        }
        void MoveEachCoin(PictureBox coin, int speed, int lb, int ub) // Move each coin
        {
            if (coin.Top > this.Height)
            {
                coin.Location = new Point(r.Next(lb, ub), 0);
            }
            else coin.Top += speed;
        }
        void MoveCoin(int speed) // Move the entire coins 
        {
            PictureBox[] allCoins = { coin1, coin2, coin3};// 3 coins 
            for (int i = 0; i < allCoins.Length; i++)
            {
                if (i % 2 == 0) MoveEachCoin(allCoins[i], speed, 30, 180);
                else MoveEachCoin(allCoins[i], speed, 220, 350);
            }
        }
       
        void GameOver()
        {
            if (car.Bounds.IntersectsWith(enemy1.Bounds) || car.Bounds.IntersectsWith(enemy2.Bounds) || car.Bounds.IntersectsWith(enemy3.Bounds) || car.Bounds.IntersectsWith(enemy4.Bounds))
            {
                timer1.Enabled = false;
                over.Visible = true;// GameOver 
                play.Visible = true;// Play Again
                exit.Visible = true;// Exit 
                player1.Stop();// Stop the music
            }
        }

        void CoinsCollection()
        {
            PictureBox[] allCoins = { coin1, coin2, coin3 };
            for (int i = 0; i < allCoins.Length; i++)
            {
                    if (car.Bounds.IntersectsWith(allCoins[i].Bounds))// One coin collected 
                    {
                        if (i % 2 == 0) allCoins[i].Location = new Point(r.Next(30, 180), 0);// Left half of the form 
                        else allCoins[i].Location = new Point(r.Next(220, 350), 0);// Right half of the form
                        totalCoins++;// Increase accumulated coins 
                        coinsCollected.Text = "Coins = " + totalCoins.ToString();// Refresh number of coins accumulated on screen
                        // Upgrade level 
                        if (totalCoins != 0 && totalCoins % nextLevelCoins == 0)// Max coins reached for a given level
                        {
                            int levelNow  = totalCoins / nextLevelCoins + 1;// Next level numbering 
                            // Increase game speed
                            if (levelNow == maxLevel)// winner 
                            {
                                timer1.Enabled = false;// Stop timer 
                                play.Visible = true;// Play Again
                                exit.Visible = true;// Exit 
                                winner.Visible = true;// You win 
                                break;// Don't process anything below 
                            }
                            minSpeed = levelNow + speedOffset;// Minimum speed for current level 
                            gameSpeed = minSpeed;// Start game speed with the new minimum speed level 
                            level.Text = "Level: "+levelNow.ToString();// Refresh level number on the screen 
                            Thread.Sleep(TimeSpan.FromSeconds(nextLevelTimePause));// Pause the screen to show level upgrade
                        }
                    }         
            }
        }


        private void play_Click(object sender, EventArgs e)
        {
            Application.Restart();// Restart the game
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);// Exit from the game
        }

        // Keyboard controls
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Left)
            {
                if (car.Left > 15) car.Left += -10;// car moving to the left
            }
            if(e.KeyCode == Keys.Right)
            {
                if (car.Right < 368) car.Left += 10;// car moving to the right
            }
            if (e.KeyCode == Keys.Up)
            {
                if(gameSpeed < maxSpeed) gameSpeed++;// speeding up the game to collect nore coins
            }
            if (e.KeyCode == Keys.Down)
            {
                if (gameSpeed > minSpeed) gameSpeed--;// speeding down to avoid enemies 
            }



        }
    }
}
