using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KGAU
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            bunifuTextBox2.UseSystemPasswordChar = true; //скрываем вводимые данные в поле пароль
            bunifuTextBox2.PasswordChar = '*';//символ вместо вводимых символов
        }

        private bool isMousePress = false; //переменная хранит нажатия кнопки мыши
        private Point _clickPoint; //точка мыши
        private Point _formStartPoint;//точка начала
        public static string path = @"Base\DB_SQLite.db"; //Путь к файлу БД
        int id;
        //кнопка Войти
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            try  //перехват ошибок
            {
                // строка подключения к БД
                using (var connection = new SqliteConnection(@"Data Source = " + path))
                {
                    connection.Open();
                    SqliteCommand cmd = new SqliteCommand("SELECT * FROM Users WHERE Login = '" + bunifuTextBox1.Text+"'", connection);
                    SqliteDataReader dr = cmd.ExecuteReader();
                    string name = "";//переменные для хранения имени  
                    int role = 0;                              //роли
                    bool flag = false;
                    while (dr.Read())                       //считываем по строчку резульат, пока есть строчки
                    {
                        if (dr[3].ToString().Equals(bunifuTextBox2.Text))
                        {
                            name = dr[1].ToString();//запоминаем имя
                            role = int.Parse(dr[4].ToString());//роль
                            id = int.Parse(dr[0].ToString());//id
                            flag = true;
                        }
                    }
                    if (flag)//если логин и пароль уникальны и совпали
                    {
                          switch (role) //исходя из роли пользователя
                          {//                 загружаем определенную форму
                              case 1:
                                  Main_admin fr = new Main_admin();
                                  fr.Show();
                                  this.Hide();
                                  break;
                             case 2:
                                  Main_users frk = new Main_users(id);
                                  frk.Show();
                                  this.Hide();
                                  break;
                            
                          }
                    }
                    else//если логин и пароль не совпали
                    {
                        MessageBox.Show("Логин или пароль неверны!");
                    }
                }
            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        private void bunifuGradientPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMousePress)
            {
                var cursorOffsetPoint = new Point( //считаем смещение курсора от старта
                    Cursor.Position.X - _clickPoint.X,
                    Cursor.Position.Y - _clickPoint.Y);

                Location = new Point( //смещаем форму от начальной позиции в соответствии со смещением курсора
                    _formStartPoint.X + cursorOffsetPoint.X,
                    _formStartPoint.Y + cursorOffsetPoint.Y);
            }
        }

        private void bunifuGradientPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            
                isMousePress = true; //запомнили что кнопка нажата
                _clickPoint = Cursor.Position; //запомнили позиции мышки
                _formStartPoint = Location;
        }

        private void bunifuGradientPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            isMousePress = false;//запоминаем что клавиша мыши отпущена
            _clickPoint = Point.Empty;
        }

        private void bunifuTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) //если нажата кнопка ENTER
            {
                bunifuButton1_Click(sender, e); //тоже самое что нажать кнопку ВОЙТИ
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();//Выход  
        }
    }
}
