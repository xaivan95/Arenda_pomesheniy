﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace KGAU
{
    public partial class Main_admin : Form
    {
        private int _id;
        private string FileLocation;
        private int maxImageSize = 5097152;
        private List<Data> role;
        private List<Data> pomesh;
        List<Data> combo_pom = new List<Data>();
        DataTable pomes = new DataTable();
        string title;
        string texts;
        public Main_admin()
        {
            InitializeComponent();
            saveFileDialog1.Filter = "Word document|*.doc";//формат выходных данных
            saveFileDialog1.Title = "Save the Word Document";
        }


        private void Main_admin_Load(object sender, EventArgs e)
        {
            Load_zakaz();
            Load_pomeshenie();
            Load_uslugi();
            Load_oboryd();
            Load_users();
            Load_combobox();
            role = new List<Data>();
            role.Add(new Data(1, "Администратор"));
            role.Add(new Data(2, "Пользователь"));
            comboBox4.DataSource = role;
            comboBox4.DisplayMember = "Name";
            comboBox4.ValueMember = "id";
            this.Text = "Окно администратора - просмотр заявок";
            title = this.Text;
            texts = "Вы находитесь в окне просмотра заявок. Для получения иформации по заказу - нажмите на него. Для получения информаци о входящей в состав заказа услуги или оборудовании наведите на него курсор мыши. Выбранный заказ можно редактировать, удалить или изменить статус заказа.";
        }
        //****
        //****
        //****Верхнее меню - обработчики нажатия ******
        //****
        //****
        private void заявкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = ((ToolStripMenuItem)sender).Text;
            bunifuPages1.SetPage(name); //при нажатии на кнопку открывать соответствующую вкладку
            switch (int.Parse(((ToolStripMenuItem)sender).Tag.ToString()))
            {
                case 1:
                    this.Text = "Окно администратора - просмотр заявок";
                    title = this.Text;
                    texts = "Вы находитесь в окне просмотра заявок. Для получения иформации по заказу - нажмите на него. Для получения информаци о входящей в состав заказа услуги или оборудовании наведите на него курсор мыши. Выбранный заказ можно редактировать, удалить или изменить статус заказа.";
                    break;
                case 2:
                    this.Text = "Окно администратора - пространства";
                    title = this.Text;
                    texts = "Вы находитесь в окне добавления/редактирования предоставляемых пространств";
                    break;
                case 3:
                    this.Text = "Окно администратора - услуги";
                    title = this.Text;
                    texts = "Вы находитесь в окне добавления/редактирования предоставляемых услуг";
                    break;
                case 4:
                    this.Text = "Окно администратора - оборудование";
                    title = this.Text;
                    texts = "Вы находитесь в окне добавления/редактирования предоставляемого оборудования";
                    break;
                case 5:
                    this.Text = "Окно администратора - пользователи";
                    title = this.Text;
                    texts = "Вы находитесь в окне добавления/редактирвоания пользователей. При добавлении новго пользователя необходимо заполнить все поля.";
                    break;
                default:
                    this.Text = "Окно администратора - редактирование заявки";
                    title = this.Text;
                    texts = "Вы находитесь в окне редактирования заявки. При редактировании заявки необходимо указать новые сведения об организации, название мероприятия, дату и время проведения мероприятия, контактный телефон, изменить выбранное помещение и убрать выбранные услуги и оборудования";
                    break;
            }
        }

        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(texts, title);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();//закрывать приложение при закрытии форму
        }

        private void Main_admin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();//закрывать приложение при закрытии форму
        }

        private void сменитьПользователяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 fr = new Form1(); //возращаемся к форме авторизации
            fr.Show();
            this.Hide();//закрываем текущую форму
        }
        public static string desktopPath = Directory.GetCurrentDirectory();//путь к exe программы
        Object _missingObj = System.Reflection.Missing.Value;
        Object trueObj = true;
        Object falseObj = false;
        Word._Application application;
        Word._Document document;
        private void сохранитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult;
                    switch (this.Text)
                    {
                        //сведения по заявкам
                        case "Окно администратора - просмотр заявок":
                        dialogResult = MessageBox.Show("Сохранить сведения о поступивших заявках?", "Заявки", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            application = new Word.Application();
                            // создаем путь к файлу
                            Object templatePathObj = desktopPath + "/Base/zayvki.dotx";
                            document = application.Documents.Add(ref templatePathObj, ref _missingObj, ref _missingObj, ref _missingObj);

                            Word.Find find = application.Selection.Find;
                            find.Text = "{Date}";
                            find.Replacement.Text = DateTime.Now.ToLongDateString();
                            Object wrap = Word.WdFindWrap.wdFindContinue;
                            Object replace = Word.WdReplace.wdReplaceAll;
                            find.Execute(FindText: Type.Missing,
                                MatchCase: false,
                                MatchWholeWord: false,
                                MatchWildcards: false,
                                MatchSoundsLike: _missingObj,
                                MatchAllWordForms: false,
                                Forward: true,
                                Wrap: wrap,
                                Format: false,
                                ReplaceWith: _missingObj, Replace: replace);
                            Word.Table _table = document.Tables[1];

                            using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                            {
                                connection.Open();
                                SqliteCommand cmd = new SqliteCommand("SELECT Zakaz.ID, Zakaz.Corporation, USers.Name, Zakaz.Event_name, Zakaz.Time_start, Zakaz.Time_end,  Zakaz.Kolvo_person, Pomeshenie.Name, Zakaz.Summa, Zakaz.Status, Zakaz.Nomer From zakaz INNER JOIN  USers ON Users.ID = Zakaz.Name INNER JOIN  Pomeshenie ON Pomeshenie.ID = Zakaz.Pomeshenie ", connection);
                                SqliteDataReader dr = cmd.ExecuteReader();
                                int i = 2;
                                while (dr.Read())
                                {
                                    _table.Rows.Add(ref _missingObj);
                                    var _currentRange = _table.Cell(i, 1).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = (i - 1).ToString();
                                    _currentRange = _table.Cell(i, 2).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[1].ToString();
                                    _currentRange = _table.Cell(i, 3).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[2].ToString();
                                    _currentRange = _table.Cell(i, 4).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[3].ToString();

                                    _currentRange = _table.Cell(i, 5).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[4].ToString();

                                    _currentRange = _table.Cell(i, 6).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[5].ToString();

                                    _currentRange = _table.Cell(i, 7).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[6].ToString();

                                    _currentRange = _table.Cell(i, 8).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[7].ToString();

                                    _currentRange = _table.Cell(i, 9).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[8].ToString();

                                    _currentRange = _table.Cell(i, 10).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[9].ToString();

                                    _currentRange = _table.Cell(i, 11).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[10].ToString();

                                    i++;
                                }
                            }
                        }
                        break;


                    case "Окно администратора - пространства":
                        dialogResult = MessageBox.Show("Сохранить сведения о имеющихся помещениях?", "Пространства", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            application = new Word.Application();
                            // создаем путь к файлу
                            Object templatePathObj = desktopPath + "/Base/pomes.dotx";
                            document = application.Documents.Add(ref templatePathObj, ref _missingObj, ref _missingObj, ref _missingObj);

                            Word.Find find = application.Selection.Find;
                            find.Text = "{Date}";
                            find.Replacement.Text = DateTime.Now.ToLongDateString();
                            Object wrap = Word.WdFindWrap.wdFindContinue;
                            Object replace = Word.WdReplace.wdReplaceAll;
                            find.Execute(FindText: Type.Missing,
                                MatchCase: false,
                                MatchWholeWord: false,
                                MatchWildcards: false,
                                MatchSoundsLike: _missingObj,
                                MatchAllWordForms: false,
                                Forward: true,
                                Wrap: wrap,
                                Format: false,
                                ReplaceWith: _missingObj, Replace: replace);
                            Word.Table _table = document.Tables[1];

                            using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                            {
                                connection.Open();
                                SqliteCommand cmd = new SqliteCommand("SELECT * FROM pomeshenie", connection);
                                SqliteDataReader dr = cmd.ExecuteReader();
                                int i = 2;
                                while (dr.Read())
                                {
                                    _table.Rows.Add(ref _missingObj);
                                    var _currentRange = _table.Cell(i, 1).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = (i - 1).ToString();
                                    _currentRange = _table.Cell(i, 2).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[1].ToString();
                                    _currentRange = _table.Cell(i, 3).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[2].ToString();
                                    _currentRange = _table.Cell(i, 4).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[4].ToString();

                                    _currentRange = _table.Cell(i, 5).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[5].ToString();


                                    byte[] data = (byte[])dr[3];
                                    MemoryStream ms = new MemoryStream(data);//считываем в потоке изображения и декодируем
                                    Image returnImage = Image.FromStream(ms);
                                    Clipboard.SetImage(ScaleImageMain(returnImage));
                                    _currentRange = _table.Cell(i, 6).Range;
                                    _currentRange.Paste();

                                    i++;
                                }
                            }
                        }
                        break;


                    case "Окно администратора - услуги":
                        dialogResult = MessageBox.Show("Сохранить сведения о оказываемых услугах?", "Услуги", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            application = new Word.Application();
                            // создаем путь к файлу
                            Object templatePathObj = desktopPath + "/Base/uslugi.dotx";
                            document = application.Documents.Add(ref templatePathObj, ref _missingObj, ref _missingObj, ref _missingObj);

                            Word.Find find = application.Selection.Find;
                            find.Text = "{Date}";
                            find.Replacement.Text = DateTime.Now.ToLongDateString();
                            Object wrap = Word.WdFindWrap.wdFindContinue;
                            Object replace = Word.WdReplace.wdReplaceAll;
                            find.Execute(FindText: Type.Missing,
                                MatchCase: false,
                                MatchWholeWord: false,
                                MatchWildcards: false,
                                MatchSoundsLike: _missingObj,
                                MatchAllWordForms: false,
                                Forward: true,
                                Wrap: wrap,
                                Format: false,
                                ReplaceWith: _missingObj, Replace: replace);
                            Word.Table _table = document.Tables[1];

                            using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                            {
                                connection.Open();
                                SqliteCommand cmd = new SqliteCommand("SELECT * FROM Uslugi", connection);
                                SqliteDataReader dr = cmd.ExecuteReader();
                                int i = 2;
                                while (dr.Read())
                                {
                                    _table.Rows.Add(ref _missingObj);
                                    var _currentRange = _table.Cell(i, 1).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = (i - 1).ToString();
                                    _currentRange = _table.Cell(i, 2).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[1].ToString();
                                    _currentRange = _table.Cell(i, 3).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[2].ToString();
                                    _currentRange = _table.Cell(i, 4).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[3].ToString();

                                    i++;
                                }
                            }
                        }
                        break;



                    case "Окно администратора - оборудование":
                        dialogResult = MessageBox.Show("Сохранить сведения о предоставляемом оборудовании?", "Оборудование", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            application = new Word.Application();
                            // создаем путь к файлу
                            Object templatePathObj = desktopPath + "/Base/Oboryd.dotx";
                            document = application.Documents.Add(ref templatePathObj, ref _missingObj, ref _missingObj, ref _missingObj);

                            Word.Find find = application.Selection.Find;
                            find.Text = "{Date}";
                            find.Replacement.Text = DateTime.Now.ToLongDateString();
                            Object wrap = Word.WdFindWrap.wdFindContinue;
                            Object replace = Word.WdReplace.wdReplaceAll;
                            find.Execute(FindText: Type.Missing,
                                MatchCase: false,
                                MatchWholeWord: false,
                                MatchWildcards: false,
                                MatchSoundsLike: _missingObj,
                                MatchAllWordForms: false,
                                Forward: true,
                                Wrap: wrap,
                                Format: false,
                                ReplaceWith: _missingObj, Replace: replace);
                            Word.Table _table = document.Tables[1];

                            using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                            {
                                connection.Open();
                                SqliteCommand cmd = new SqliteCommand("SELECT * FROM Oboryd", connection);
                                SqliteDataReader dr = cmd.ExecuteReader();
                                int i = 2;
                                while (dr.Read())
                                {
                                    _table.Rows.Add(ref _missingObj);
                                    var _currentRange = _table.Cell(i, 1).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = (i - 1).ToString();
                                    _currentRange = _table.Cell(i, 2).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[1].ToString();
                                    _currentRange = _table.Cell(i, 3).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[2].ToString();
                                    _currentRange = _table.Cell(i, 4).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[3].ToString();

                                    i++;
                                }
                            }
                        }
                        break;

                    case "Окно администратора - пользователи":
                        dialogResult = MessageBox.Show("Сохранить сведения о польователях?", "Пользователи", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            application = new Word.Application();
                            // создаем путь к файлу
                            Object templatePathObj = desktopPath + "/Base/Users.dotx";
                            document = application.Documents.Add(ref templatePathObj, ref _missingObj, ref _missingObj, ref _missingObj);

                            Word.Find find = application.Selection.Find;
                            find.Text = "{Date}";
                            find.Replacement.Text = DateTime.Now.ToLongDateString();
                            Object wrap = Word.WdFindWrap.wdFindContinue;
                            Object replace = Word.WdReplace.wdReplaceAll;
                            find.Execute(FindText: Type.Missing,
                                MatchCase: false,
                                MatchWholeWord: false,
                                MatchWildcards: false,
                                MatchSoundsLike: _missingObj,
                                MatchAllWordForms: false,
                                Forward: true,
                                Wrap: wrap,
                                Format: false,
                                ReplaceWith: _missingObj, Replace: replace);
                            Word.Table _table = document.Tables[1];

                            using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                            {
                                connection.Open();
                                SqliteCommand cmd = new SqliteCommand("SELECT Users.ID, Users.Name, Users.Login, Users.Password, Role.Name FROM USers INNER JOIN Role ON Role.Id = Users.Role", connection);
                                SqliteDataReader dr = cmd.ExecuteReader();
                                int i = 2;
                                while (dr.Read())
                                {
                                    _table.Rows.Add(ref _missingObj);
                                    var _currentRange = _table.Cell(i, 1).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = (i - 1).ToString();
                                    _currentRange = _table.Cell(i, 2).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[1].ToString();
                                    _currentRange = _table.Cell(i, 3).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[2].ToString();
                                    _currentRange = _table.Cell(i, 4).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[3].ToString();
                                    _currentRange = _table.Cell(i, 5).Range;
                                    _currentRange.Bold = 0;
                                    _currentRange.Text = dr[4].ToString();
                                    i++;
                                }
                            }
                        }
                        break;

                }
                if (!this.Text.Equals("Окно администратора - редактирование заявки"))
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                        return;
                    // получаем выбранный файл
                    Object pathToSaveObj = saveFileDialog1.FileName;
                    document.SaveAs(ref pathToSaveObj, Word.WdSaveFormat.wdFormatDocument, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj);
                    application.Visible = true;
                }
                    
                
            }
            catch (Exception ex) //возникает при ошибках
            {
                document.Close(ref falseObj, ref _missingObj, ref _missingObj);
                application.Quit(ref _missingObj, ref _missingObj, ref _missingObj);
                document = null;
                application = null;
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
                    foreach (DataRow row in pomes.Rows) //заносим в список
                    {
                        combo_pom.Add(new Data(int.Parse(row[0].ToString()), row[1].ToString()));
                    }
                    comboBox2.DataSource = combo_pom;
                    comboBox2.DisplayMember = "Name";
                    comboBox2.ValueMember = "id";
                }

            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        private void Load_zakaz()
        {
            try  //перехват ошибок
            {
                // строка подключения к БД
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    SqliteCommand cmd = new SqliteCommand("SELECT Zakaz.ID, Zakaz.Corporation, USers.Name, Zakaz.Event_name, Zakaz.Time_start, Zakaz.Time_end,  Zakaz.Kolvo_person, Pomeshenie.Name, Zakaz.Summa, Zakaz.Status, Zakaz.Nomer From zakaz INNER JOIN  USers ON Users.ID = Zakaz.Name INNER JOIN  Pomeshenie ON Pomeshenie.ID = Zakaz.Pomeshenie ", connection);
                    SqliteDataReader dr = cmd.ExecuteReader();
                    DataTable ds = new DataTable();
                    ds.Load(dr);                                                    //записываем данные с БД
                    bunifuDataGridView1.DataSource = ds;                        //выводим данные в форму
                    bunifuDataGridView1.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                    bunifuDataGridView1.Columns[0].Visible = false;               //убираем столбец с id
                    bunifuDataGridView1.Columns[1].HeaderText = "Название организации";  //название солбцов
                    bunifuDataGridView1.Columns[2].HeaderText = "Ф.И.О. заявителя";
                    bunifuDataGridView1.Columns[3].HeaderText = "Название мероприятия";
                    bunifuDataGridView1.Columns[4].HeaderText = "Время начала";
                    bunifuDataGridView1.Columns[5].HeaderText = "Время окончания";
                    bunifuDataGridView1.Columns[6].HeaderText = "Количество человек";
                    bunifuDataGridView1.Columns[7].HeaderText = "Пространство";
                    bunifuDataGridView1.Columns[8].HeaderText = "Сумма к оплате";
                    bunifuDataGridView1.Columns[9].HeaderText = "Состояние";
                    bunifuDataGridView1.Columns[10].HeaderText = "Контактный телефон";
                    bunifuDataGridView1.AllowUserToAddRows = false;
                    bunifuDataGridView1.RowHeadersVisible = false;
                }
                Load_dop_zakaz();
            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }
        private void Load_dop_zakaz()
        {
            if (bunifuDataGridView1.Rows.Count > 0 && bunifuDataGridView1.SelectedRows.Count > 0)
                try  //перехват ошибок
                {
                    DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView1.SelectedCells; //получаем номер выделенной строчки
                    var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                    var dgvr = dgvc.OwningRow;
                    using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                    {
                        connection.Open();
                        SqliteCommand cmd = new SqliteCommand("SELECT Oboryd.Name, Oboryd.Price FROM zakaz_oborud INNER JOIN Oboryd ON Oboryd.ID = zakaz_oborud.ID_oborud WHERE zakaz_oborud.ID_zakaz =" + dgvr.Cells[0].Value.ToString(), connection);
                        SqliteDataReader dr = cmd.ExecuteReader();
                        DataTable ds = new DataTable();
                        ds.Load(dr);                                                    //записываем данные с БД
                        bunifuDataGridView6.DataSource = ds;                        //выводим данные в форму
                        bunifuDataGridView6.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                        bunifuDataGridView6.Columns[0].HeaderText = "Название оборудования";  //название солбцов
                        bunifuDataGridView6.Columns[1].HeaderText = "Цена";
                        bunifuDataGridView6.AllowUserToAddRows = false;
                        bunifuDataGridView6.RowHeadersVisible = false;
                    }
                    using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                    {
                        connection.Open();
                        SqliteCommand cmd = new SqliteCommand("SELECT Uslugi.Name, Uslugi.Price FROM zakaz_uslug INNER JOIN Uslugi ON Uslugi.ID = zakaz_uslug.ID_uslug WHERE zakaz_uslug.ID_zakaz =" + dgvr.Cells[0].Value.ToString(), connection);
                        SqliteDataReader dr = cmd.ExecuteReader();
                        DataTable ds = new DataTable();
                        ds.Load(dr);                                                    //записываем данные с БД
                        bunifuDataGridView7.DataSource = ds;                        //выводим данные в форму
                        bunifuDataGridView7.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                        bunifuDataGridView7.Columns[0].HeaderText = "Название услуги";  //название солбцов
                        bunifuDataGridView7.Columns[1].HeaderText = "Цена";
                        bunifuDataGridView7.AllowUserToAddRows = false;
                        bunifuDataGridView7.RowHeadersVisible = false;
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
        private void Load_pomeshenie()
        {
            openFileDialog1.Filter = "Файлы изображений (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";//фильтр изображений
            try  //перехват ошибок
            {
                // строка подключения к БД
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    SqliteCommand cmd = new SqliteCommand("SELECT * FROM Pomeshenie", connection);
                    SqliteDataReader dr = cmd.ExecuteReader();
                    DataTable ds = new DataTable();
                    ds.Load(dr);                                                    //записываем данные с БД
                    bunifuDataGridView2.DataSource = ds;                        //выводим данные в форму
                    bunifuDataGridView2.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                    bunifuDataGridView2.Columns[0].Visible = false;               //убираем столбец с id
                    bunifuDataGridView2.Columns[1].HeaderText = "Название помещения";  //название солбцов
                    bunifuDataGridView2.Columns[2].HeaderText = "Тип помещения";
                    bunifuDataGridView2.Columns[5].HeaderText = "Стоимость";
                    bunifuDataGridView2.Columns[3].Visible = false;
                    bunifuDataGridView2.Columns[4].Visible = false;
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
                    bunifuDataGridView3.DataSource = ds;                        //выводим данные в форму
                    bunifuDataGridView3.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                    bunifuDataGridView3.Columns[0].Visible = false;               //убираем столбец с id
                    bunifuDataGridView3.Columns[1].HeaderText = "Название услуги";  //название солбцов
                    bunifuDataGridView3.Columns[3].HeaderText = "Стоимость";
                    bunifuDataGridView3.Columns[2].Visible = false;
                    bunifuDataGridView3.AllowUserToAddRows = false;
                    bunifuDataGridView3.RowHeadersVisible = false;
                }

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
                    bunifuDataGridView4.DataSource = ds;                        //выводим данные в форму
                    bunifuDataGridView4.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                    bunifuDataGridView4.Columns[0].Visible = false;               //убираем столбец с id
                    bunifuDataGridView4.Columns[1].HeaderText = "Название оборудования";  //название солбцов
                    bunifuDataGridView4.Columns[3].HeaderText = "Стоимость";
                    bunifuDataGridView4.Columns[2].Visible = false;
                    bunifuDataGridView4.AllowUserToAddRows = false;
                    bunifuDataGridView4.RowHeadersVisible = false;
                }

            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        private void Load_users()
        {
            try  //перехват ошибок
            {
                // строка подключения к БД
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    SqliteCommand cmd = new SqliteCommand("SELECT Users.ID, Users.Name, Users.Login, Users.Password, Role.Name FROM USers INNER JOIN Role ON Role.Id = Users.Role", connection);
                    SqliteDataReader dr = cmd.ExecuteReader();
                    DataTable ds = new DataTable();
                    ds.Load(dr);//записываем данные с БД
                    bunifuDataGridView5.DataSource = ds;                        //выводим данные в форму
                    bunifuDataGridView5.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                    bunifuDataGridView5.Columns[0].Visible = false;               //убираем столбец с id
                    bunifuDataGridView5.Columns[1].HeaderText = "ФИО пользователя";  //название солбцов
                    bunifuDataGridView5.Columns[2].HeaderText = "Логин";
                    bunifuDataGridView5.Columns[3].HeaderText = "Пароль";  //название солбцов
                    bunifuDataGridView5.Columns[4].HeaderText = "Права доступа";
                    bunifuDataGridView5.AllowUserToAddRows = false;
                    bunifuDataGridView5.RowHeadersVisible = false;
                }

            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        private void Load_dop_zakaz_edit(int id)
        {
            try
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    SqliteCommand cmd = new SqliteCommand("SELECT zakaz_oborud.ID,  Oboryd.Name, Oboryd.Price FROM zakaz_oborud INNER JOIN Oboryd ON Oboryd.ID = zakaz_oborud.ID_oborud WHERE zakaz_oborud.ID_zakaz =" + id.ToString(), connection);
                    SqliteDataReader dr = cmd.ExecuteReader();
                    DataTable ds = new DataTable();
                    ds.Load(dr);                                                    //записываем данные с БД
                    bunifuDataGridView8.DataSource = ds;                        //выводим данные в форму
                    bunifuDataGridView8.RowHeadersVisible = false;                        //скрываем столбец с номерами строк
                    bunifuDataGridView8.Columns[0].Visible = false;
                    bunifuDataGridView8.Columns[1].HeaderText = "Название оборудования";  //название солбцов
                    bunifuDataGridView8.Columns[2].HeaderText = "Цена";
                    bunifuDataGridView8.AllowUserToAddRows = false;
                    bunifuDataGridView8.RowHeadersVisible = false;
                }
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    SqliteCommand cmd = new SqliteCommand("SELECT zakaz_uslug.ID,  Uslugi.Name, Uslugi.Price FROM zakaz_uslug INNER JOIN Uslugi ON Uslugi.ID = zakaz_uslug.ID_uslug WHERE zakaz_uslug.ID_zakaz =" + id.ToString(), connection);
                    SqliteDataReader dr = cmd.ExecuteReader();
                    DataTable ds = new DataTable();
                    ds.Load(dr);                                                    //записываем данные с БД
                    bunifuDataGridView9.DataSource = ds;                        //выводим данные в форму
                    bunifuDataGridView9.RowHeadersVisible = false;
                    bunifuDataGridView9.Columns[0].Visible = false;                            //скрываем столбец с номерами строк
                    bunifuDataGridView9.Columns[1].HeaderText = "Название услуги";  //название солбцов
                    bunifuDataGridView9.Columns[2].HeaderText = "Цена";
                    bunifuDataGridView9.AllowUserToAddRows = false;
                    bunifuDataGridView9.RowHeadersVisible = false;
                }
            }
            catch (Exception ex) //возникает при ошибках
            {
                MessageBox.Show(ex.Message.ToString()); //выводим полученную ошибку
                MessageBox.Show(ex.StackTrace.ToString());
            }
}
        //*****
        //*****  ===== END =====
        //*****
        //*****


        //*****
        //*****  ===== Форма с заявками =====
        //*****
        //*****
        //кнопка удалить заказ
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            if (bunifuDataGridView1.SelectedRows.Count > 0)
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView1.SelectedCells; //получаем номер выделенной строчки
                    var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                    var dgvr = dgvc.OwningRow;
                    connection.Open();
                    var query = "DELETE FROM zakaz WHERE ID = " + dgvr.Cells[0].Value;
                    var cmd = new SqliteCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    Load_zakaz();
                }
            }
        }
        //кнопка обновить статус заказа
        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            if (bunifuDataGridView1.SelectedRows.Count > 0)
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView1.SelectedCells; //получаем номер выделенной строчки
                    var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                    var dgvr = dgvc.OwningRow;
                    connection.Open();
                    var query = "UPDATE zakaz SET Status = 'Готово' WHERE ID = " + dgvr.Cells[0].Value;
                    var cmd = new SqliteCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    Load_zakaz();
                }
            }
        }
        //доступность кнопки Выполнено
        private void bunifuDataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (bunifuDataGridView1.SelectedRows.Count > 0)
            {
                    DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView1.SelectedCells; //получаем номер выделенной строчки
                    var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                    var dgvr = dgvc.OwningRow;
                        if (dgvr.Cells[9].Value.ToString().Equals("Готово"))
                                bunifuButton3.Enabled = false;
                        else
                                bunifuButton3.Enabled = true;
                Load_dop_zakaz();
            }
        }

        //Кнопка редактирования
        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            if (bunifuDataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView1.SelectedCells; //получаем номер выделенной строчки
                var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                var dgvr = dgvc.OwningRow;
                _id = int.Parse(dgvr.Cells[0].Value.ToString());
                textBox11.Text = dgvr.Cells[1].Value.ToString();
                textBox14.Text = dgvr.Cells[3].Value.ToString();
               dateTimePicker1.Value = DateTime.Parse(dgvr.Cells[4].Value.ToString());
                dateTimePicker2.Value = DateTime.Parse(dgvr.Cells[5].Value.ToString());
                textBox17.Text = dgvr.Cells[6].Value.ToString();
                comboBox2.Text = dgvr.Cells[7].Value.ToString();                 //ToDo!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                textBox18.Text = dgvr.Cells[8].Value.ToString();
                textBox19.Text = dgvr.Cells[9].Value.ToString();
                maskedTextBox1.Text = dgvr.Cells[10].Value.ToString();
                Load_dop_zakaz_edit(_id);
               
                Rashet_summy();
                bunifuPages1.SetPage("Редактировать"); //при нажатии на кнопку открывать соответствующую вкладку
            }
        }
        //поиск
        private void bunifuTextBox1_TextChange(object sender, EventArgs e)
        {
            //поиск по заявкам
            bool flag = false; //состояние поиска
            bunifuDataGridView1.CurrentCell = null; //снимаем выделения строк с таблицы
            string s = bunifuTextBox1.Text.ToLower();//делаем вводимый текст маленькими буквами
            if (bunifuTextBox1.Text.Equals("")) //если ничего не введено
            {
                foreach (DataGridViewRow row in bunifuDataGridView1.Rows)
                {
                    row.Visible = true;//делаем все строчки видимыми
                }
            }
            else //если что-то ввели
            {
                foreach (DataGridViewRow row in bunifuDataGridView1.Rows)
                {
                    flag = false;//состояние поиска - не найдено
                    if (row.Cells[1].Value.ToString().ToLower().Contains(s)) flag = true;//поиск по компании
                    if (row.Cells[2].Value.ToString().ToLower().Contains(s)) flag = true;//поиск по ФИО
                    if (row.Cells[3].Value.ToString().ToLower().Contains(s)) flag = true;//поиск по мероприятию           
                    if (flag) row.Visible = true;//если нашли совпадение строчка видна
                    else row.Visible = false;//иначе скрываем
                }
            }
        }


        //*****
        //*****  ===== END =====
        //*****
        //*****


        //*****
        //*****  ===== Форма с редактированием =====
        //*****
        //*****
        //отмена
        private void bunifuButton19_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("Заявки"); //при нажатии на кнопку открывать соответствующую вкладку 
            Clear_form();
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
        private void bunifuButton20_Click(object sender, EventArgs e)
        {
            if (bunifuDataGridView9.SelectedRows.Count > 0)
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView9.SelectedCells; //получаем номер выделенной строчки
                    var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                    var dgvr = dgvc.OwningRow;
                    connection.Open();
                    var query = "DELETE FROM zakaz_uslug WHERE ID = " + dgvr.Cells[0].Value;
                    var cmd = new SqliteCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    Load_dop_zakaz_edit(_id);
                    Rashet_summy();
                }
            }
        } 

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            if (bunifuDataGridView8.SelectedRows.Count > 0)
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView8.SelectedCells; //получаем номер выделенной строчки
                    var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                    var dgvr = dgvc.OwningRow;
                    connection.Open();
                    var query = "DELETE FROM zakaz_oborud WHERE ID = " + dgvr.Cells[0].Value;
                    var cmd = new SqliteCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    Load_dop_zakaz_edit(_id);
                    Rashet_summy();
                }
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Rashet_summy();
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
                foreach (DataGridViewRow dr in bunifuDataGridView9.Rows)
                {
                    foreach (DataGridViewRow dd in bunifuDataGridView3.Rows)
                        if (dr.Cells[1].Value.Equals(dd.Cells[1].Value)) dr.Cells[2].Value = dd.Cells[3].Value;
                    summ += int.Parse(dr.Cells[2].Value.ToString()) * prodolshit;
                    dr.Cells[2].Value = int.Parse(dr.Cells[2].Value.ToString()) * prodolshit;
                }
                foreach (DataGridViewRow dr in bunifuDataGridView8.Rows)
                {
                    foreach (DataGridViewRow dd in bunifuDataGridView4.Rows)
                        if (dr.Cells[1].Value.Equals(dd.Cells[1].Value)) dr.Cells[2].Value = dd.Cells[3].Value;
                    summ += int.Parse(dr.Cells[2].Value.ToString()) * prodolshit;
                    dr.Cells[2].Value = int.Parse(dr.Cells[2].Value.ToString()) * prodolshit;
                }
                textBox18.Text = summ.ToString();
            }

        }
        //изменить
        private void bunifuButton18_Click(object sender, EventArgs e)
        {
            using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
            {
                connection.Open();

                var cmd = new SqliteCommand("UPDATE Zakaz SET Corporation = @CO, Event_name = @ev, Time_start = @ts, Time_end = @te, Kolvo_person = @kp, Pomeshenie = @pom, Summa = @sum, Status = @st, Nomer = @nom  WHERE ID = " + _id.ToString(), connection);                          
               cmd.Parameters.Add(new SqliteParameter("@CO", textBox11.Text));
                cmd.Parameters.Add(new SqliteParameter("@ev", textBox14.Text));
                cmd.Parameters.Add(new SqliteParameter("@ts", dateTimePicker1.Value.ToString()));
                cmd.Parameters.Add(new SqliteParameter("@te", dateTimePicker2.Value.ToString()));
                cmd.Parameters.Add(new SqliteParameter("@kp", textBox17.Text));
                cmd.Parameters.Add(new SqliteParameter("@pom", comboBox2.SelectedValue));
                cmd.Parameters.Add(new SqliteParameter("@sum", textBox18.Text));
                cmd.Parameters.Add(new SqliteParameter("@st", textBox19.Text));
                cmd.Parameters.Add(new SqliteParameter("@nom", maskedTextBox1.Text));
                cmd.ExecuteNonQuery();
                Load_zakaz();
                Clear_form();
            }
                bunifuPages1.SetPage("Заявки"); //при нажатии на кнопку открывать соответствующую вкладку 
        }


        //*****
        //*****  ===== END =====
        //*****
        //*****




        //*****
        //*****  ===== Форма с пространствами =====
        //*****
        //*****

        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            if (bunifuButton4.Text.Equals("Добавить"))
            {
                bunifuButton4.Text = "Сохранить";
                bunifuPanel1.Visible = true;
                bunifuButton5.Enabled = false;
                bunifuButton6.Text = "Отмена";

            }
            else
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    var cmd = new SqliteCommand("INSERT INTO Pomeshenie (Name, Type, Foto, Opisanie, Price) VALUES (@name, @type, @foto, @opisanie, @price)", connection);
                    cmd.Parameters.Add(new SqliteParameter("@name", textBox1.Text));
                    cmd.Parameters.Add(new SqliteParameter("@type", comboBox1.Items[comboBox1.SelectedIndex].ToString()));
                    cmd.Parameters.Add(new SqliteParameter("@opisanie", textBox3.Text));
                    FileLocation = textBox2.Text;
                    byte[] data = null;
                    if (File.Exists(FileLocation))
                    {
                        using (FileStream stream = File.Open(FileLocation, FileMode.Open))
                        {
                            BinaryReader br = new BinaryReader(stream);
                            data = br.ReadBytes(maxImageSize);
                       }
                    }
                    cmd.Parameters.Add(new SqliteParameter("@foto", data));
                    cmd.Parameters.Add(new SqliteParameter("@price", int.Parse(textBox4.Text)));
                    cmd.ExecuteNonQuery();
                    Load_pomeshenie();
                }
                bunifuButton4.Text = "Добавить";
                bunifuPanel1.Visible = false;
                bunifuButton5.Enabled = true;
                bunifuButton6.Enabled = true;
                bunifuButton6.Text = "Удалить";
                Clear_form();
            }
        }

        private void bunifuDataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (bunifuDataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView2.SelectedCells; //получаем номер выделенной строчки
                var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                var dgvr = dgvc.OwningRow;
                bunifuTextBox3.Text = dgvr.Cells[4].Value.ToString();
                byte[] data = (byte[])dgvr.Cells[3].Value;
                MemoryStream ms = new MemoryStream(data);//считываем в потоке изображения и декодируем
                Image returnImage = Image.FromStream(ms);
                pictureBox1.BackgroundImage = returnImage;
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            textBox2.Text = filename;//путь к выбранному файлу
        }

        private void bunifuButton6_Click(object sender, EventArgs e)
        {
            if (bunifuButton6.Text.Equals("Удалить"))
            {
                if (bunifuDataGridView2.SelectedRows.Count > 0)
                {
                    using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                    {
                        DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView2.SelectedCells; //получаем номер выделенной строчки
                        var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                        var dgvr = dgvc.OwningRow;
                        connection.Open();
                        var query = "DELETE FROM Pomeshenie WHERE ID = " + dgvr.Cells[0].Value;
                        var cmd = new SqliteCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        Load_pomeshenie();
                    }
                }
            }
            else
            {
                if (!bunifuButton5.Enabled)
                {
                    bunifuButton4.Text = "Добавить";
                    bunifuPanel1.Visible = false;
                    bunifuButton5.Enabled = true;
                    bunifuButton6.Enabled = true;
                    bunifuButton6.Text = "Удалить";
                    Clear_form(); 
                }
                else
                {
                    bunifuButton5.Text = "Редактировать";
                    bunifuPanel1.Visible = false;
                    bunifuButton4.Enabled = true;
                    bunifuButton6.Enabled = true;
                    bunifuButton6.Text = "Удалить";
                    Clear_form();
                }
            }

        }

        private void bunifuButton5_Click(object sender, EventArgs e)
        {
            if (bunifuButton5.Text.Equals("Редактировать"))
            {
                if (bunifuDataGridView2.SelectedRows.Count > 0)
                {
                    DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView2.SelectedCells; //получаем номер выделенной строчки
                    var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                    var dgvr = dgvc.OwningRow;
                    textBox1.Text = dgvr.Cells[1].Value.ToString();
                    textBox3.Text = dgvr.Cells[4].Value.ToString();
                    textBox4.Text = dgvr.Cells[5].Value.ToString();
                    comboBox2.Text = dgvr.Cells[2].Value.ToString();
                    _id = int.Parse(dgvr.Cells[0].Value.ToString());
                    bunifuButton5.Text = "Сохранить";
                    bunifuPanel1.Visible = true;
                    bunifuButton4.Enabled = false;
                    bunifuButton6.Text = "Отмена";
                }
            }
            else
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    var cmd = new SqliteCommand("UPDATE Pomeshenie SET Name = @name, Type = @type, Foto =  @foto, Opisanie = @opisanie, Price = @price WHERE ID = "+_id.ToString(), connection);
                    cmd.Parameters.Add(new SqliteParameter("@name", textBox1.Text));
                    cmd.Parameters.Add(new SqliteParameter("@type", comboBox1.Items[comboBox1.SelectedIndex].ToString()));
                    cmd.Parameters.Add(new SqliteParameter("@opisanie", textBox3.Text));
                    FileLocation = textBox2.Text;
                    byte[] data = null;
                    if (File.Exists(FileLocation))
                    {
                        using (FileStream stream = File.Open(FileLocation, FileMode.Open))
                        {
                            BinaryReader br = new BinaryReader(stream);
                            data = br.ReadBytes(maxImageSize);
                        }
                    }
                    cmd.Parameters.Add(new SqliteParameter("@foto", data));
                    cmd.Parameters.Add(new SqliteParameter("@price", int.Parse(textBox4.Text)));
                    cmd.ExecuteNonQuery();
                    Load_pomeshenie();
                }
                bunifuButton5.Text = "Редактировать";
                bunifuPanel1.Visible = false;
                bunifuButton4.Enabled = true;
                bunifuButton6.Text = "Удалить";
                Clear_form();
            }
        }

        private void bunifuTextBox2_TextChange(object sender, EventArgs e)
        {
            //поиск по пространствам
            bool flag = false; //состояние поиска
            bunifuDataGridView2.CurrentCell = null; //снимаем выделения строк с таблицы
            string s = bunifuTextBox2.Text.ToLower();//делаем вводимый текст маленькими буквами
            if (bunifuTextBox2.Text.Equals("")) //если ничего не введено
            {
                foreach (DataGridViewRow row in bunifuDataGridView2.Rows)
                {
                    row.Visible = true;//делаем все строчки видимыми
                }
            }
            else //если что-то ввели
            {
                foreach (DataGridViewRow row in bunifuDataGridView2.Rows)
                {
                    flag = false;//состояние поиска - не найдено
                    if (row.Cells[1].Value.ToString().ToLower().Contains(s)) flag = true;//поиск по компании
                    if (row.Cells[2].Value.ToString().ToLower().Contains(s)) flag = true;//поиск по ФИО
                    if (row.Cells[3].Value.ToString().ToLower().Contains(s)) flag = true;//поиск по мероприятию           
                    if (flag) row.Visible = true;//если нашли совпадение строчка видна
                    else row.Visible = false;//иначе скрываем
                }
            }
        }




        //*****
        //*****  ===== END =====
        //*****
        //*****







        //*****
        //*****  ===== Форма с услугами =====
        //*****
        //*****
        private void bunifuButton7_Click(object sender, EventArgs e)
        {
            if (bunifuButton7.Text.Equals("Добавить"))
            {
                bunifuButton7.Text = "Сохранить";
                bunifuPanel2.Visible = true;
                bunifuButton8.Enabled = false;
                bunifuButton9.Text = "Отмена";

            }
            else
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    var cmd = new SqliteCommand("INSERT INTO Uslugi (Name, Opisanie, Price) VALUES (@name, @opisanie, @price)", connection);
                    cmd.Parameters.Add(new SqliteParameter("@name", textBox8.Text));
                    cmd.Parameters.Add(new SqliteParameter("@opisanie", textBox6.Text));
                    cmd.Parameters.Add(new SqliteParameter("@price", int.Parse(textBox5.Text)));
                    cmd.ExecuteNonQuery();
                    Load_uslugi();
                }
                bunifuButton7.Text = "Добавить";
                bunifuPanel2.Visible = false;
                bunifuButton8.Enabled = true;
                bunifuButton9.Text = "Удалить";
                Clear_form();
            }
        }
        //кнопка удаления
        private void bunifuButton9_Click(object sender, EventArgs e)
        {
            if (bunifuButton9.Text.Equals("Удалить"))
            {
                if (bunifuDataGridView3.SelectedRows.Count > 0)
                {
                    using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                    {
                        DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView3.SelectedCells; //получаем номер выделенной строчки
                        var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                        var dgvr = dgvc.OwningRow;
                        connection.Open();
                        var query = "DELETE FROM Uslugi WHERE ID = " + dgvr.Cells[0].Value;
                        var cmd = new SqliteCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        Load_uslugi();
                    }
                }
            }
            else
            {
                if (!bunifuButton8.Enabled)
                {
                    bunifuButton7.Text = "Добавить";
                    bunifuPanel2.Visible = false;
                    bunifuButton8.Enabled = true;
                    bunifuButton9.Text = "Удалить";
                    Clear_form();
                }
                else
                {
                    bunifuButton8.Text = "Редактировать";
                    bunifuPanel2.Visible = false;
                    bunifuButton7.Enabled = true;
                    bunifuButton9.Text = "Удалить";
                    Clear_form();
                }
            }

        }
        //кнопка редактирования
        private void bunifuButton8_Click(object sender, EventArgs e)
        {
            if (bunifuButton8.Text.Equals("Редактировать"))
            {
                if (bunifuDataGridView3.SelectedRows.Count > 0)
                {
                    DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView3.SelectedCells; //получаем номер выделенной строчки
                    var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                    var dgvr = dgvc.OwningRow;
                    textBox8.Text = dgvr.Cells[1].Value.ToString();
                    textBox6.Text = dgvr.Cells[2].Value.ToString();
                    textBox5.Text = dgvr.Cells[3].Value.ToString();
                    _id = int.Parse(dgvr.Cells[0].Value.ToString());
                    bunifuButton8.Text = "Сохранить";
                    bunifuPanel2.Visible = true;
                    bunifuButton7.Enabled = false;
                    bunifuButton9.Text = "Отмена";
                   
                }

            }
            else
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    var cmd = new SqliteCommand("UPDATE Uslugi SET Name = @name, Opisanie = @opisanie, Price = @price WHERE ID = " + _id.ToString(), connection);
                    cmd.Parameters.Add(new SqliteParameter("@name", textBox8.Text));
                    cmd.Parameters.Add(new SqliteParameter("@opisanie", textBox6.Text));
                    cmd.Parameters.Add(new SqliteParameter("@price", int.Parse(textBox5.Text)));
                    cmd.ExecuteNonQuery();
                    Load_uslugi();
                }
                bunifuButton8.Text = "Редактировать";
                bunifuPanel2.Visible = false;
                bunifuButton7.Enabled = true;
                bunifuButton9.Text = "Удалить";
                Clear_form();
            }
        }
        //поиск
        private void bunifuTextBox7_TextChange(object sender, EventArgs e)
        {
            //поиск по пространствам
            bool flag = false; //состояние поиска
            bunifuDataGridView3.CurrentCell = null; //снимаем выделения строк с таблицы
            string s = bunifuTextBox7.Text.ToLower();//делаем вводимый текст маленькими буквами
            if (bunifuTextBox7.Text.Equals("")) //если ничего не введено
            {
                foreach (DataGridViewRow row in bunifuDataGridView3.Rows)
                {
                    row.Visible = true;//делаем все строчки видимыми
                }
            }
            else //если что-то ввели
            {
                foreach (DataGridViewRow row in bunifuDataGridView3.Rows)
                {
                    flag = false;//состояние поиска - не найдено
                    if (row.Cells[1].Value.ToString().ToLower().Contains(s)) flag = true;//поиск по компании          
                    if (flag) row.Visible = true;//если нашли совпадение строчка видна
                    else row.Visible = false;//иначе скрываем
                }
            }
        }

        private void bunifuDataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            if (bunifuDataGridView3.SelectedRows.Count > 0)
            {
                DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView3.SelectedCells; //получаем номер выделенной строчки
                var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                var dgvr = dgvc.OwningRow;
                bunifuTextBox4.Text = dgvr.Cells[2].Value.ToString();
            }
        }



        //*****
        //*****  ===== END =====
        //*****
        //*****


        //*****
        //*****  =====  Форма с оборудованием  =====
        //*****
        //*****
        //Добавить оборудование
        private void bunifuButton10_Click(object sender, EventArgs e)
        {
            if (bunifuButton10.Text.Equals("Добавить"))
            {
                bunifuButton10.Text = "Сохранить";
                bunifuPanel3.Visible = true;
                bunifuButton11.Enabled = false;
                bunifuButton12.Text = "Отмена";

            }
            else
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    var cmd = new SqliteCommand("INSERT INTO Oboryd (Name, Opisanie, Price) VALUES (@name, @opisanie, @price)", connection);
                    cmd.Parameters.Add(new SqliteParameter("@name", textBox10.Text));
                    cmd.Parameters.Add(new SqliteParameter("@opisanie", textBox9.Text));
                    cmd.Parameters.Add(new SqliteParameter("@price", int.Parse(textBox7.Text)));
                    cmd.ExecuteNonQuery();
                    Load_oboryd();
                }
                bunifuButton10.Text = "Добавить";
                bunifuPanel3.Visible = false;
                bunifuButton11.Enabled = true;
                bunifuButton12.Text = "Удалить";
                Clear_form();
            }
        }
        //Выводим описание
        private void bunifuDataGridView4_SelectionChanged(object sender, EventArgs e)
        {
            if (bunifuDataGridView4.SelectedRows.Count > 0)
            {
                DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView4.SelectedCells; //получаем номер выделенной строчки
                var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                var dgvr = dgvc.OwningRow;
                bunifuTextBox8.Text = dgvr.Cells[2].Value.ToString();
            }
        }

        private void bunifuTextBox5_TextChange(object sender, EventArgs e)
        {
            //поиск по оборудованию
            bool flag = false; //состояние поиска
            bunifuDataGridView4.CurrentCell = null; //снимаем выделения строк с таблицы
            string s = bunifuTextBox5.Text.ToLower();//делаем вводимый текст маленькими буквами
            if (bunifuTextBox5.Text.Equals("")) //если ничего не введено
            {
                foreach (DataGridViewRow row in bunifuDataGridView4.Rows)
                {
                    row.Visible = true;//делаем все строчки видимыми
                }
            }
            else //если что-то ввели
            {
                foreach (DataGridViewRow row in bunifuDataGridView4.Rows)
                {
                    flag = false;//состояние поиска - не найдено
                    if (row.Cells[1].Value.ToString().ToLower().Contains(s)) flag = true;//поиск по компании          
                    if (flag) row.Visible = true;//если нашли совпадение строчка видна
                    else row.Visible = false;//иначе скрываем
                }
            }
        }
        //Кнопка редактирования
        private void bunifuButton11_Click(object sender, EventArgs e)
        {
            
            if (bunifuButton11.Text.Equals("Редактировать"))
            {
                if (bunifuDataGridView4.SelectedRows.Count > 0)
                {
                    DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView4.SelectedCells; //получаем номер выделенной строчки
                    var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                    var dgvr = dgvc.OwningRow;
                    textBox10.Text = dgvr.Cells[1].Value.ToString();
                    textBox9.Text = dgvr.Cells[2].Value.ToString();
                    textBox7.Text = dgvr.Cells[3].Value.ToString();
                    _id = int.Parse(dgvr.Cells[0].Value.ToString());
                    bunifuButton11.Text = "Сохранить";
                    bunifuPanel3.Visible = true;
                    bunifuButton10.Enabled = false;
                    bunifuButton12.Text = "Отмена";
                }

            }
            else
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    var cmd = new SqliteCommand("UPDATE Oboryd SET Name = @name, Opisanie = @opisanie, Price = @price WHERE ID = " + _id.ToString(), connection);
                    cmd.Parameters.Add(new SqliteParameter("@name", textBox10.Text));
                    cmd.Parameters.Add(new SqliteParameter("@opisanie", textBox9.Text));
                    cmd.Parameters.Add(new SqliteParameter("@price", int.Parse(textBox7.Text)));
                    cmd.ExecuteNonQuery();
                    Load_oboryd();
                }
                bunifuButton11.Text = "Редактировать";
                bunifuPanel3.Visible = false;
                bunifuButton10.Enabled = true;
                bunifuButton12.Text = "Удалить";
                Clear_form();
            }
        }
        //Кнопка удаления
        private void bunifuButton12_Click(object sender, EventArgs e)
        {
            if (bunifuButton12.Text.Equals("Удалить"))
            {
                if (bunifuDataGridView4.SelectedRows.Count > 0)
                {
                    using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                    {
                        DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView4.SelectedCells; //получаем номер выделенной строчки
                        var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                        var dgvr = dgvc.OwningRow;
                        connection.Open();
                        var query = "DELETE FROM Oboryd WHERE ID = " + dgvr.Cells[0].Value;
                        var cmd = new SqliteCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        Load_oboryd();
                    }
                }
            }
            else
            {
                if (!bunifuButton11.Enabled)
                {
                    bunifuButton10.Text = "Добавить";
                    bunifuPanel3.Visible = false;
                    bunifuButton11.Enabled = true;
                    bunifuButton12.Text = "Удалить";
                    Clear_form();
                }
                else
                {
                    bunifuButton11.Text = "Редактировать";
                    bunifuPanel3.Visible = false;
                    bunifuButton10.Enabled = true;
                    bunifuButton12.Text = "Удалить";
                    Clear_form();
                }
            }
        }
        //*****
        //*****  ===== END =====
        //*****
        //*****






        //*****
        //*****  ===== Форма с пользователями  =====
        //*****
        //*****
        //Кнопка добавить
        private void bunifuButton13_Click(object sender, EventArgs e)
        {
            if (bunifuButton13.Text.Equals("Добавить"))
            {
                bunifuButton13.Text = "Сохранить";
                bunifuPanel4.Visible = true;
                bunifuButton14.Enabled = false;
                bunifuButton15.Text = "Отмена";

            }
            else
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    var cmd = new SqliteCommand("INSERT INTO Users (Name, Login, Password, Role) VALUES (@name, @login, @pass, @role)", connection);
                    cmd.Parameters.Add(new SqliteParameter("@name", textBox16.Text));
                    cmd.Parameters.Add(new SqliteParameter("@login", textBox15.Text));
                    cmd.Parameters.Add(new SqliteParameter("@pass", textBox13.Text));
                    cmd.Parameters.Add(new SqliteParameter("@role", comboBox4.SelectedValue));
                    cmd.ExecuteNonQuery();
                    Load_users();
                }
                bunifuButton13.Text = "Добавить";
                bunifuPanel4.Visible = false;
                bunifuButton14.Enabled = true;
                bunifuButton15.Text = "Удалить";
                Clear_form();
            }
        }
        //Кнопка редактировать
        private void bunifuButton14_Click(object sender, EventArgs e)
        {
            if (bunifuButton14.Text.Equals("Редактировать"))
            {
                if (bunifuDataGridView5.SelectedRows.Count > 0)
                {
                    DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView5.SelectedCells; //получаем номер выделенной строчки
                    var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                    var dgvr = dgvc.OwningRow;
                    _id = int.Parse(dgvr.Cells[0].Value.ToString());
                    textBox16.Text = dgvr.Cells[1].Value.ToString();
                    textBox15.Text = dgvr.Cells[2].Value.ToString();
                    textBox13.Text = dgvr.Cells[3].Value.ToString();
                    var i = comboBox4.FindString(dgvr.Cells[4].Value.ToString());
                    comboBox4.SelectedIndex = i;                
                    bunifuButton14.Text = "Сохранить";
                    bunifuPanel4.Visible = true;
                    bunifuButton13.Enabled = false;
                    bunifuButton15.Text = "Отмена";
                }

            }
            else
            {
                using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                {
                    connection.Open();
                    var cmd = new SqliteCommand("UPDATE Users SET Name = @name, Login = @login, Password = @pass, Role = @role WHERE ID = " + _id.ToString(), connection);
                    cmd.Parameters.Add(new SqliteParameter("@name", textBox16.Text));
                    cmd.Parameters.Add(new SqliteParameter("@login", textBox15.Text));
                    cmd.Parameters.Add(new SqliteParameter("@pass", textBox13.Text));
                    cmd.Parameters.Add(new SqliteParameter("@role", comboBox4.SelectedValue));
                    cmd.ExecuteNonQuery();
                    Load_users();
                }
                bunifuButton14.Text = "Редактировать";
                bunifuPanel4.Visible = false;
                bunifuButton13.Enabled = true;
                bunifuButton15.Text = "Удалить";
                Clear_form();
            }
        }
        //Кнопка удалить
        private void bunifuButton15_Click(object sender, EventArgs e)
        {
            if (bunifuButton15.Text.Equals("Удалить"))
            {
                if (bunifuDataGridView5.SelectedRows.Count > 0)
                {
                    using (var connection = new SqliteConnection(@"Data Source = " + Form1.path))
                    {
                        DataGridViewSelectedCellCollection DGVCell = bunifuDataGridView5.SelectedCells; //получаем номер выделенной строчки
                        var dgvc = DGVCell[1];//запоминаем данные из выделенной строки
                        var dgvr = dgvc.OwningRow;
                        connection.Open();
                        var query = "DELETE FROM Users WHERE ID = " + dgvr.Cells[0].Value;
                        var cmd = new SqliteCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        Load_users();
                    }
                }
            }
            else
            {
                if (!bunifuButton14.Enabled)
                {
                    bunifuButton13.Text = "Добавить";
                    bunifuPanel4.Visible = false;
                    bunifuButton14.Enabled = true;
                    bunifuButton15.Text = "Удалить";
                    Clear_form();
                }
                else
                {
                    bunifuButton14.Text = "Редактировать";
                    bunifuPanel4.Visible = false;
                    bunifuButton13.Enabled = true;
                    bunifuButton15.Text = "Удалить";
                    Clear_form();
                }
            }
        }
        //поиск по пользователям
        private void bunifuTextBox6_TextChange(object sender, EventArgs e)
        {
            //поиск по оборудованию
            bool flag = false; //состояние поиска
            bunifuDataGridView5.CurrentCell = null; //снимаем выделения строк с таблицы
            string s = bunifuTextBox6.Text.ToLower();//делаем вводимый текст маленькими буквами
            if (bunifuTextBox6.Text.Equals("")) //если ничего не введено
            {
                foreach (DataGridViewRow row in bunifuDataGridView5.Rows)
                {
                    row.Visible = true;//делаем все строчки видимыми
                }
            }
            else //если что-то ввели
            {
                foreach (DataGridViewRow row in bunifuDataGridView5.Rows)
                {
                    flag = false;//состояние поиска - не найдено
                    if (row.Cells[1].Value.ToString().ToLower().Contains(s)) flag = true;//поиск по ФИО     
                    if (row.Cells[2].Value.ToString().ToLower().Contains(s)) flag = true;//поиск по логину 
                    if (flag) row.Visible = true;//если нашли совпадение строчка видна
                    else row.Visible = false;//иначе скрываем
                }
            }
        }


        //*****
        //*****  ===== END =====
        //*****
        //*****



        //*****
        //*****  ===== Очистка форм =====
        //*****
        //*****

        private void Clear_form()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox8.Text = "";
            textBox6.Text = "";
            textBox5.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            textBox7.Text = "";
            textBox16.Text = "";
            textBox15.Text = "";
            textBox13.Text = "";
            textBox11.Text = "";
            textBox14.Text = "";
            textBox17.Text = "";
            textBox18.Text = "";
            textBox19.Text = "";
          maskedTextBox1.Text = "";
        }



        static Image ScaleImageMain(Image img)
        {
            int x1 = 400;
            int y1 = 400;
            int x2 = 3;
            int y2 = 3;
            if (img.Width > img.Height)
            {
                x1 = 400;
                y1 = (int)Math.Round((double)img.Height / (img.Width / 400));
                y2 = (int)Math.Round((double)((400 - y1) / 2));

            }
            else
            {
                if (img.Width < img.Height)
                {
                    y1 = 400;
                    x1 = (int)Math.Round((double)img.Width / (img.Height / 400));
                    x2 = (int)Math.Round((double)((400 - x1) / 2));
                }
            }
            img = ScaleImage(img, x1, y1);
            Image dest = new Bitmap(408, 408);
            Graphics gr = Graphics.FromImage(dest);
            // Здесь рисуем рамку.
            Pen blackPen = new Pen(Color.Black, 3);
            float x = 0.0F;
            float y = 0.0F;
            float width = 408.0F;
            float height = 408.0F;
            gr.DrawRectangle(blackPen, x, y, width, height);

            gr.DrawImage(img, x2, y2, img.Width, img.Height);

            return dest;
        }

        static Image ScaleImage(Image source, int width, int height)
        {

            Image dest = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(dest))
            {
                gr.FillRectangle(Brushes.White, 0, 0, width, height);  // Очищаем экран
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                float srcwidth = source.Width;
                float srcheight = source.Height;
                float dstwidth = width;
                float dstheight = height;

                if (srcwidth <= dstwidth && srcheight <= dstheight)  // Исходное изображение меньше целевого
                {
                    int left = (width - source.Width) / 2;
                    int top = (height - source.Height) / 2;
                    gr.DrawImage(source, left, top, source.Width, source.Height);
                }
                else if (srcwidth / srcheight > dstwidth / dstheight)  // Пропорции исходного изображения более широкие
                {
                    float cy = srcheight / srcwidth * dstwidth;
                    float top = ((float)dstheight - cy) / 2.0f;
                    if (top < 1.0f) top = 0;
                    gr.DrawImage(source, 0, top, dstwidth, cy);
                }
                else  // Пропорции исходного изображения более узкие
                {
                    float cx = srcwidth / srcheight * dstheight;
                    float left = ((float)dstwidth - cx) / 2.0f;
                    if (left < 1.0f) left = 0;
                    gr.DrawImage(source, left, 0, cx, dstheight);
                }

                return dest;
            }
        }
    }

    public class Data //класс для списка, содержащий имя и ID
    {
        public string Name { set; get; }
        public int id { set; get; }
        public Data(int id, string Name)
        {
            this.Name = Name;
            this.id = id;
        }
    }

}
