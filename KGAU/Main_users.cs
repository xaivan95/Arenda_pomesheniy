using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KGAU
{
    public partial class Main_users : Form
    {
        List<Data> combo_pom = new List<Data>();
        DataTable pomes = new DataTable();
        List<DataRow> uslugi = new List<DataRow>();
        List<DataRow> oborud = new List<DataRow>();
        int _id;
        string title;
        string texts;
        public Main_users(int id)
        {
            InitializeComponent();
            _id = id;
            this.Text = "Окно пользователя - просмотр заявок";
            title = this.Text;
            texts = "Вы находитесь в окне просмотра Ваших заявок. Для получения иформации по заказу - нажмите на него. Для получения информаци о входящей в состав заказа услуги или оборудовании наведите на него курсор мыши";
        }

        private void Main_users_Load(object sender, EventArgs e)
        {
            Load_oboryd();
            Load_uslugi();
            Load_zakaz();
            Load_combobox();


            bunifuDataGridView3.Columns.Add("newColumnName", "ID");
            bunifuDataGridView3.Columns.Add("newColumnName1", "Название" );
            bunifuDataGridView3.Columns.Add("newColumnName2", "Описание");
            bunifuDataGridView3.Columns.Add("newColumnName3", "Стоимость");

            bunifuDataGridView3.Columns[0].Visible = false;
            bunifuDataGridView3.Columns[2].Visible = false;

            bunifuDataGridView3.Columns[0].ValueType = typeof(int);
            bunifuDataGridView3.Columns[1].ValueType = typeof(string);
            bunifuDataGridView3.Columns[2].ValueType = typeof(string);
            bunifuDataGridView3.Columns[3].ValueType = typeof(int);
            bunifuDataGridView3.AllowUserToAddRows = false;
            bunifuDataGridView3.RowHeadersVisible = false;
        }

        //*****
        //*****  ===== Верхнее меню =====
        //*****
        //*****
        private void Main_users_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();//закрывать приложение при закрытии форму
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Application.Exit();//закрывать приложение при закрытии форму
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Form1 fr = new Form1(); //возращаемся к форме авторизации
            fr.Show();
            this.Hide();//закрываем текущую форму
        }
        private void моиЗаявкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = ((ToolStripMenuItem)sender).Text;
            bunifuPages1.SetPage(name); //при нажатии на кнопку открывать соответствующую вкладку     
            Load_uslugi();
            Load_oboryd();
            switch (int.Parse(((ToolStripMenuItem)sender).Tag.ToString()))
            {
                case 1:
                    this.Text = "Окно пользователя - просмотр заявок";
                    title = this.Text;
                    texts = "Вы находитесь в окне просмотра Ваших заявок. Для получения иформации по заказу - нажмите на него. Для получения информаци о входящей в состав заказа услуги или оборудовании наведите на него курсор мыши";
                    break;
                case 2:
                    this.Text = "Окно пользователя - новая заявка";
                    title = this.Text;
                    texts = "Вы находитесь в окне добавления новой заявки. При добавлении новой заявки необходимо указать сведения об организации, название мероприятия, дату и время проведения мероприятия, контактный телефон, выбрать помещение и добавить необходимые услуги и оборудования";
                    break;
            }
        }

        //*****
        //*****  ===== END =====
        //*****
        //*****




        //*****
        //*****  ===== Загрузка информации =====
        //*****
        //*****
        private void Load_zakaz()
        {
            try  //перехват ошибок
            {
                // строка подключения к БД
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    SqliteCommand cmd = new SqliteCommand("SELECT Zakaz.ID, Zakaz.Corporation, USers.Name, Zakaz.Event_name, Zakaz.Time_start, Zakaz.Time_end,  Zakaz.Kolvo_person, Pomeshenie.Name, Zakaz.Summa, Zakaz.Status, Zakaz.Nomer From zakaz INNER JOIN  USers ON Users.ID = Zakaz.Name  INNER JOIN  Pomeshenie ON Pomeshenie.ID = Zakaz.Pomeshenie WHERE zakaz.Name = " +_id.ToString(), connection);
                    SqliteDataReader dr = cmd.ExecuteReader();
                    DataTable ds = new DataTable();
                    ds.Load(dr);                                                    //записываем данные с БД
                    bunifuDataGridView2.DataSource = ds;                        //выводим данные в форму
                    bunifuDataGridView2.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                    bunifuDataGridView2.Columns[0].Visible = false;               //убираем столбец с id
                    bunifuDataGridView2.Columns[1].HeaderText = "Название организации";  //название солбцов
                    bunifuDataGridView2.Columns[2].HeaderText = "Ф.И.О. заявителя";
                    bunifuDataGridView2.Columns[3].HeaderText = "Название мероприятия";
                    bunifuDataGridView2.Columns[4].HeaderText = "Время начала";
                    bunifuDataGridView2.Columns[5].HeaderText = "Время окончания";
                    bunifuDataGridView2.Columns[6].HeaderText = "Количество человек";
                    bunifuDataGridView2.Columns[7].HeaderText = "Пространство";
                    bunifuDataGridView2.Columns[8].HeaderText = "Сумма к оплате";
                    bunifuDataGridView2.Columns[9].HeaderText = "Состояние";
                    bunifuDataGridView2.Columns[10].HeaderText = "Контактный телефон";
                    bunifuDataGridView2.AllowUserToAddRows = false;
                    bunifuDataGridView2.RowHeadersVisible = false;
                }

            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        private void Load_combobox()
        {
            try  //перехват ошибок
            {
                // строка подключения к БД
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    SqliteCommand cmd = new SqliteCommand("Select * From Pomeshenie", connection);
                    SqliteDataReader dr = cmd.ExecuteReader();
                    pomes = new DataTable();
                    pomes.Load(dr);                                                    //записываем данные с БД
                    foreach (DataRow row in pomes.Rows) //заносим в список авторов
                    {
                        combo_pom.Add(new Data(int.Parse(row[0].ToString()), row[1].ToString()));
                    }
                    comboBox2.DataSource = combo_pom;
                    comboBox2.DisplayMember = "Name";
                    comboBox2.ValueMember = "id";
                }
                comboBox2.SelectedItem = comboBox2.Items[0];
                comboBox2_SelectedIndexChanged(new object(), new EventArgs());
            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        private void Load_uslugi()
        {
            try  //перехват ошибок
            {
                // строка подключения к БД
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    SqliteCommand cmd = new SqliteCommand("SELECT * FROM Uslugi", connection);
                    SqliteDataReader dr = cmd.ExecuteReader();
                    DataTable ds = new DataTable();
                    ds.Load(dr);//записываем данные с БД
                    bunifuDataGridView1.DataSource = ds;                        //выводим данные в форму
                    bunifuDataGridView1.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                    bunifuDataGridView1.Columns[0].Visible = false;               //убираем столбец с id
                    bunifuDataGridView1.Columns[1].HeaderText = "Название услуги";  //название солбцов
                    bunifuDataGridView1.Columns[3].HeaderText = "Стоимость";
                    bunifuDataGridView1.Columns[2].Visible = false;
                    bunifuDataGridView1.AllowUserToAddRows = false;
                    bunifuDataGridView1.RowHeadersVisible = false;
                }
                
                foreach (DataGridViewRow dr in bunifuDataGridView1.Rows)
                {                  
                    dr.Cells[1].ToolTipText = dr.Cells[2].Value.ToString();
                    dr.Cells[3].ToolTipText = dr.Cells[2].Value.ToString();
                }
                bunifuDataGridView1.ShowCellToolTips = true;
    
            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }
        private void Load_oboryd()
        {
            try  //перехват ошибок
            {
                // строка подключения к БД
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    SqliteCommand cmd = new SqliteCommand("SELECT * FROM Oboryd", connection);
                    SqliteDataReader dr = cmd.ExecuteReader();
                    DataTable ds = new DataTable();
                    ds.Load(dr);//записываем данные с БД
                    bunifuDataGridView5.DataSource = ds;                        //выводим данные в форму
                    bunifuDataGridView5.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                    bunifuDataGridView5.Columns[0].Visible = false;               //убираем столбец с id
                    bunifuDataGridView5.Columns[1].HeaderText = "Название оборудования";  //название солбцов
                    bunifuDataGridView5.Columns[3].HeaderText = "Стоимость";
                    bunifuDataGridView5.Columns[2].Visible = false;
                    bunifuDataGridView5.AllowUserToAddRows = false;
                    bunifuDataGridView5.RowHeadersVisible = false;
                }
                foreach (DataGridViewRow dr in bunifuDataGridView5.Rows)
                {
                    dr.Cells[1].ToolTipText = dr.Cells[2].Value.ToString();
                    dr.Cells[3].ToolTipText = dr.Cells[2].Value.ToString();
                }
            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        private void Load_dop_zakaz()
        {
            if (bunifuDataGridView2.Rows.Count > 0 && bunifuDataGridView2.SelectedRows.Count > 0)
                try  //перехват ошибок
                {
                    DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView2.SelectedCells; //получаем номер выделенной строчки
                    var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                    var dgvr = dgvc.OwningRow;
                    using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                    {
                        connection.Open();
                        SqliteCommand cmd = new SqliteCommand("SELECT Oboryd.Name, Oboryd.Price, Oboryd.Opisanie  FROM zakaz_oborud INNER JOIN Oboryd ON Oboryd.ID = zakaz_oborud.ID_oborud WHERE zakaz_oborud.ID_zakaz =" + dgvr.Cells[0].Value.ToString(), connection);
                        SqliteDataReader dr = cmd.ExecuteReader();
                        DataTable ds = new DataTable();
                        ds.Load(dr);                                                    //записываем данные с БД
                        bunifuDataGridView6.DataSource = ds;                        //выводим данные в форму
                        bunifuDataGridView6.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                        bunifuDataGridView6.Columns[0].HeaderText = "Название оборудования";  //название солбцов
                        bunifuDataGridView6.Columns[1].HeaderText = "Цена";
                        bunifuDataGridView6.Columns[2].Visible = false;
                        bunifuDataGridView6.AllowUserToAddRows = false;
                        bunifuDataGridView6.RowHeadersVisible = false;
                    }
                    foreach (DataGridViewRow dr in bunifuDataGridView6.Rows)
                    {
                        dr.Cells[0].ToolTipText = dr.Cells[2].Value.ToString();
                        dr.Cells[1].ToolTipText = dr.Cells[2].Value.ToString();
                    }
                    using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                    {
                        connection.Open();
                        SqliteCommand cmd = new SqliteCommand("SELECT Uslugi.Name, Uslugi.Price, Uslugi.Opisanie FROM zakaz_uslug INNER JOIN Uslugi ON Uslugi.ID = zakaz_uslug.ID_uslug WHERE zakaz_uslug.ID_zakaz =" + dgvr.Cells[0].Value.ToString(), connection);
                        SqliteDataReader dr = cmd.ExecuteReader();
                        DataTable ds = new DataTable();
                        ds.Load(dr);                                                    //записываем данные с БД
                        bunifuDataGridView7.DataSource = ds;                        //выводим данные в форму
                        bunifuDataGridView7.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                        bunifuDataGridView7.Columns[0].HeaderText = "Название услуги";  //название солбцов
                        bunifuDataGridView7.Columns[1].HeaderText = "Цена";
                        bunifuDataGridView7.Columns[2].Visible = false;
                        bunifuDataGridView7.AllowUserToAddRows = false;
                        bunifuDataGridView7.RowHeadersVisible = false;
                    }
                    foreach (DataGridViewRow dr in bunifuDataGridView7.Rows)
                    {
                        dr.Cells[0].ToolTipText = dr.Cells[2].Value.ToString();
                        dr.Cells[1].ToolTipText = dr.Cells[2].Value.ToString();
                    }
                    DateTime date1 = DateTime.Parse(dgvr.Cells[4].Value.ToString());
                    DateTime date2 = DateTime.Parse(dgvr.Cells[5].Value.ToString());
                    var prodolshit = (date2 - date1).TotalHours;
                    foreach (DataGridViewRow dataRow in bunifuDataGridView6.Rows)
                    {
                        dataRow.Cells[1].Value = int.Parse(dataRow.Cells[1].Value.ToString()) * prodolshit;
                    }
                    foreach (DataGridViewRow dataRow in bunifuDataGridView7.Rows)
                    {
                        dataRow.Cells[1].Value = int.Parse(dataRow.Cells[1].Value.ToString()) * prodolshit;
                    }
                }
                catch (Exception ex) //возникает при ошибках
                {
                    MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                    MessageBox.Show(ex.StackTrace.ToString());
                }
        }

        private void Rashet_summy()
        {
            double summ = 0;
            int n = 0;
            if (int.TryParse(comboBox2.SelectedValue.ToString(), out n))
            {
                var row = pomes.Select("ID = " + n.ToString()).ToList();
                var prodolshit = Math.Round((dateTimePicker2.Value - dateTimePicker1.Value).TotalHours);
                summ = int.Parse(row[0][5].ToString()) * prodolshit;
                bunifuDataGridView3.Rows.Clear();
                foreach (DataRow dataRow in oborud)
                    bunifuDataGridView3.Rows.Add(dataRow.ItemArray);
                foreach (DataRow dataRow in uslugi)
                    bunifuDataGridView3.Rows.Add(dataRow.ItemArray);
                foreach (DataGridViewRow dr in bunifuDataGridView3.Rows)
                {
                    summ += int.Parse(dr.Cells[3].Value.ToString()) * prodolshit;
                    dr.Cells[3].Value = int.Parse(dr.Cells[3].Value.ToString()) * prodolshit;
                }
                textBox18.Text = summ.ToString();
            }

        }
        //*****
        //*****  ===== END =====
        //*****
        //*****


        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            if (bunifuDataGridView5.SelectedRows.Count > 0)
            {
                var datarow = ((DataRowView)bunifuDataGridView5.SelectedRows[0].DataBoundItem).Row;
                bunifuDataGridView3.Rows.Add(datarow.ItemArray);
                oborud.Add(datarow);
                Rashet_summy();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = 0;
            if (int.TryParse(comboBox2.SelectedValue.ToString(), out n))
                if (n > 0)
                {
                    var dgvc = pomes.Select("ID = "+ n.ToString());
                    byte[] data = (byte[])dgvc[0][3];
                    MemoryStream ms = new MemoryStream(data);//считываем в потоке изображения и декодируем
                    Image returnImage = Image.FromStream(ms);
                    pictureBox1.BackgroundImage = returnImage;
                    textBox1.Text = dgvc[0][4].ToString();
                    label4.Text = "Тип помещения - "+ dgvc[0][2].ToString()+"     Стоимсоть - " + dgvc[0][5].ToString()+" руб/час";
                    Rashet_summy();
                }
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            if (bunifuDataGridView1.SelectedRows.Count > 0)
            {
                var datarow = ((DataRowView)bunifuDataGridView1.SelectedRows[0].DataBoundItem).Row;
                bunifuDataGridView3.Rows.Add(datarow.ItemArray);
                uslugi.Add(datarow);
                Rashet_summy();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Value = dateTimePicker1.Value;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("Введите корректную дату");
                dateTimePicker2.Value = dateTimePicker1.Value;
            }
            else
                Rashet_summy(); 
        }

        private void убратьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var point = bunifuDataGridView3.PointToClient(contextMenuStrip1.Bounds.Location);
                var info = bunifuDataGridView3.HitTest(point.X, point.Y);
                // Работаем с ячейкой
                var value = bunifuDataGridView3[info.ColumnIndex, info.RowIndex].OwningRow.Cells[1].Value.ToString();
                foreach (DataRow dataRow in oborud)
                    if (dataRow[1].Equals(value)) { oborud.Remove(dataRow); break; }
               foreach (DataRow dataRow in uslugi)
                    if (dataRow[1].Equals(value)) { uslugi.Remove(dataRow); break; }
                bunifuDataGridView3.Rows.RemoveAt(info.RowIndex);
                Rashet_summy();
            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            var point = bunifuDataGridView3.PointToClient(contextMenuStrip1.Bounds.Location);
            var info = bunifuDataGridView3.HitTest(point.X, point.Y);

            // Отменяем показ контекстного меню, если клик был не на ячейке
            if (info.RowIndex == -1 || info.ColumnIndex == -1)
            {
                e.Cancel = true;
            }
        }

        private void bunifuDataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            Load_dop_zakaz();
        }

        private void bunifuButton18_Click(object sender, EventArgs e)
        {
            try {
            using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
            {
                connection.Open();

                var cmd = new SqliteCommand("INSERT INTO Zakaz (Corporation, Name, Event_name, Time_start, Time_end, Kolvo_person, Pomeshenie, Summa, Status, Nomer ) VALUES  (@CO, @name, @ev, @ts, @te, @kp, @pom, @sum, @st, @nom)", connection);
                cmd.Parameters.Add(new SqliteParameter("@CO", textBox2.Text));
                cmd.Parameters.Add(new SqliteParameter("@name", _id));
                cmd.Parameters.Add(new SqliteParameter("@ev", textBox14.Text));
                cmd.Parameters.Add(new SqliteParameter("@ts", dateTimePicker1.Value.ToString()));
                cmd.Parameters.Add(new SqliteParameter("@te", dateTimePicker2.Value.ToString()));
                cmd.Parameters.Add(new SqliteParameter("@kp", textBox17.Text));
                cmd.Parameters.Add(new SqliteParameter("@pom", comboBox2.SelectedValue));
                cmd.Parameters.Add(new SqliteParameter("@sum", textBox18.Text));
                cmd.Parameters.Add(new SqliteParameter("@st", "Ожидание"));
                cmd.Parameters.Add(new SqliteParameter("@nom", maskedTextBox1.Text));
                cmd.ExecuteNonQuery();
                Load_zakaz();
            }
                DataGridViewRow rows = bunifuDataGridView2.Rows[bunifuDataGridView2.Rows.Count-1];
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    foreach (DataRow dr in oborud)
                    {
                        var cmd = new SqliteCommand("INSERT INTO zakaz_oborud (ID_zakaz, ID_oborud) VALUES (@zak, @obor)", connection);
                        cmd.Parameters.Add(new SqliteParameter("@zak",rows.Cells[0].Value));
                        cmd.Parameters.Add(new SqliteParameter("@obor", dr[0]));
                        cmd.ExecuteNonQuery();
                    }
                }
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    foreach (DataRow dr in uslugi)
                    {
                        var cmd = new SqliteCommand("INSERT INTO zakaz_uslug (ID_zakaz, ID_uslug) VALUES (@zak, @obor)", connection);
                        cmd.Parameters.Add(new SqliteParameter("@zak", rows.Cells[0].Value));
                        cmd.Parameters.Add(new SqliteParameter("@obor", dr[0]));
                        cmd.ExecuteNonQuery();
                    }
                }
                bunifuPages1.SetPage("Мои заявки"); //при нажатии на кнопку открывать соответствующую вкладку 
            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
         
        }

        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(texts, title);
        }
    }
}
