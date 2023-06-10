using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Linq;


/* 
* Игра «Сокобан»                      
* Должна быть реализована игра “Сокобан”. Должны быть добавлены не
* менее пяти уровней и таблица рекордов. Уровни должны загружаться из
* файлов. Должен быть добавлен редактор уровней.
*/

namespace SokobanKR
{
    public partial class GameForm : Form
    {
        private static Label[] winPositions; // позиции для коробок
        private static Label[] boxes;
        private static Label labelplayer;
        private static Label labelWinPos;
        private static Label labelBox;

        private string[] readFileMaps; // вспомогательная переменная для считывания с файла    
        private string[] fileMaps; // файлы с картами
        private string[] temp; // вспомогающая переменная для считывания в файла
        private string recordFile; // переменная, хранящая путь до файла с рекордами
        private string[] readFile;// вспомогающая переменная для считывания в файла

        /// <summary>
        /// Карта представляет собой двумерный массив, заполненный цифрами, каждая из которых является определенным объектом
        /// </summary>
        private static int[,] maps;

        private const byte levelNumber = 5; // количество уровней
        private byte objNumber; // количество объектов (коробок)
        private static byte mapLength; // размерность карты
        private byte forInit; // итератор по первым числам файлов, хранящих карты
        private byte lvl = 1; // изначальный уровень
        private bool stopTimer = true; // флаг для остановки таймера
        private bool isWriterOpen = true; // проверка открытия потока
        private static Image fon; // внешний фон карты
        private static Rectangle rect; // прямоугольник для отрисовки на карте
        /// <summary>
        /// Графика для отрисовки на окне Form1
        /// </summary>
        private static Graphics graf;
        private static StreamWriter streamWriter; // поток для считывания и записи в файл
        private Image wall;
        private Image road;
        private Stopwatch stopWatch; // переменная для измерения времени выполнения программы (игры)

        // конструктор окна
        public GameForm()
        {
            InitializeComponent();

            rect = new Rectangle(0, 0, 50, 50);

            wall = new Bitmap(Properties.Resources.wall);
            road = new Bitmap(Properties.Resources.road);
            fon = new Bitmap(Properties.Resources.fon);

            // переменная для считывания с файла
            readFileMaps = new string[levelNumber];

            recordFile = "recordsFile.txt";
            streamWriter = new StreamWriter(recordFile, true);

            stopWatch = new Stopwatch();
            stopWatch.Start();


        }
        private void LoadMaps()
        {
            // файлы с картами хранятся отдельно
            try
            {
                fileMaps = new string[levelNumber]
                {
                    Directory.GetCurrentDirectory() + @"\levels\map1.txt",
                    Directory.GetCurrentDirectory() + @"\levels\map2.txt",
                    Directory.GetCurrentDirectory() + @"\levels\map3.txt",
                    Directory.GetCurrentDirectory() + @"\levels\map4.txt",
                    Directory.GetCurrentDirectory() + @"\levels\map5.txt"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Application.Exit();
            }

            // поочередно из каждого файла считываем все строки 
            readFileMaps = File.ReadAllLines(fileMaps[lvl - 1]);
            // графика для отрисовке на окне
            graf = CreateGraphics();

            // forInit выступает итератером по параметрам уровня (0 - число коробок, 1 - размер карты)
            forInit = 0;
            temp = new string[200];

            // первое число файла - для инициализации количества коробок
            objNumber = Convert.ToByte(readFileMaps[forInit++]);

            // второе число файла - для инициализации размеров карты (квадратные поля)
            mapLength = Convert.ToByte(readFileMaps[forInit++]);


            // после этого можем проинициализировать объекты коробок и их конечных позиций
            boxes = new Label[objNumber];
            winPositions = new Label[objNumber];

            // инициализируем карту двумерным массивом считанным размером
            maps = new int[mapLength, mapLength];

            // заполняем карту maps
            for (int xMap = 0; xMap < mapLength; xMap++)
            {   // во вспомогательный массив строк копируются подстроки строк файла карты, без разделителя
                temp = readFileMaps[xMap + forInit].Split(' ');
                for (int yMap = 0; yMap < mapLength; yMap++)
                    maps[xMap, yMap] = Convert.ToInt32(temp[yMap]);
            }
        }
       

        // по нажатию клавиш определяем состояние игры (победили или нет)
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            CheckMovePlayer(sender, e);
        }

        // движение игрока
        private void MovePlayer(KeyEventArgs e)
        { // событие нажатие на клавиши
            switch (e.KeyCode.ToString())
            {
                case "Up": // движение вверх
                           // проверка на наличие стены и сдвиг коробки
                    if (!CheckWall("Up", labelplayer) &&
                         Box.UpBoxMove(labelplayer, boxes))
                        labelplayer.Top -= labelplayer.Height;
                    break;

                case "Down": // движение вниз
                             // проверка на наличие стены и сдвиг коробки
                    if (!CheckWall("Down", labelplayer) &&
                         Box.DownBoxMove(labelplayer, boxes))
                        labelplayer.Top += labelplayer.Height;

                    break;

                case "Left": // движение влево
                             // проверка на наличие стены и сдвиг коробки
                    if (!CheckWall("Left", labelplayer) &&
                         Box.LeftBoxMove(labelplayer, boxes))
                        labelplayer.Left -= labelplayer.Width;
                    break;

                case "Right": // движение вправо
                              // проверка на наличие стены и сдвиг коробки
                    if (!CheckWall("Right", labelplayer) &&
                         Box.RightBoxMove(labelplayer, boxes))
                        labelplayer.Left += labelplayer.Width;
                    break;

                default:
                    break;
            }
        }
        public bool IsStopTimer()
        {
            return stopTimer;
        }

        // отрисовка карты
        public void DrawMap()
        {
            int draw;

            for (int yMap = 0; yMap < mapLength; yMap++)
            {
                for (int xMap = 0; xMap < mapLength; xMap++)
                {
                    draw = maps[yMap, xMap];

                    rect.X = xMap * 50;
                    rect.Y = yMap * 50 + 20;

                    switch (draw)
                    {
                        case 0: // под местом появления игрока будет дорога
                            graf.DrawImage(road, rect);
                            break;

                        case 1:
                            graf.DrawImage(road, rect);
                            break;

                        case 2:
                            graf.DrawImage(wall, rect);
                            break;

                        case 3: // также под коробками будет дорога
                            graf.DrawImage(road, rect);
                            break;

                        case 4: // и под цонечными позициями
                            graf.DrawImage(road, rect);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        // проверка наличие стен
        public static bool CheckWall(string move, Label obj)
        {
            int objX = obj.Location.X;
            int objY = obj.Location.Y;
            int len = 50; // все одного размера (шаг, коробка, игрок, стенка)

            switch (move)
            {
                // деление на len - приведение пикселов к клеточному размеру карты
                case "Up":
                    return (maps[(objY - len) / len, (objX / len)] == 2);

                case "Down":
                    return (maps[(objY + len) / len, (objX / len)] == 2);

                case "Left":
                    return (maps[(objY / len), (objX - len) / len] == 2);

                case "Right":
                    return (maps[(objY / len), (objX + len) / len] == 2);

                default:
                    break;
            }

            return true;
        }
        public void CheckMovePlayer(object sender, KeyEventArgs e)
        {
            if (!Box.Checkwin(boxes, winPositions))
            {
                MovePlayer(e);
                Sound.MoveSoundPlay();
            }
            else
            {
                isWriterOpen = true;
                int waitTime = 1;

                Sound.VictorySoundPlay();
                Thread.Sleep(waitTime * 1000);

                if (lvl == 5 && stopTimer) // при прохождении 5 уровня
                {
                    stopTimer = !stopTimer; // останавливаем таймер

                    string timerText = SetTimer(((stopWatch.ElapsedMilliseconds / 1000) - waitTime * 4).ToString());

                    streamWriter.WriteLine(timerText); // записываем в файл

                    timerText += " секунд"; // выводим в окно результат

                    SetTimer(timerText);

                    SortRecordFile(recordFile); // сортировка рекордов в файле 
                }
                ChangeLevel(sender, e);// смена уровней
            }
        }
        // отображение рекордов 
        public string GetRecordText()
        {
            if (!isWriterOpen)
                return "";
            streamWriter.Close();
            readFile = File.ReadAllLines(recordFile);

            int size = readFile.Length < 10 ? readFile.Length : 10;

            string records = size > 0 ? "" : "(пусто)";

            for (int i = 0; i < size; i++)
                records += (i + 1) + ".   " + readFile[i] + '\n';

            return records;
        }
        // создание объектов, взаимодействующих друг с другом
        public void CreateObjects()
        {
            CreatePlayer();
            CreateBoxes();
            CreateWinPositions();
        }

        private void CreatePlayer()
        {
            for (int yMap = 0; yMap < mapLength; yMap++)
                for (int xMap = 0; xMap < mapLength; xMap++)
                    if (maps[yMap, xMap] == 0)
                    {
                        labelplayer = new Label
                        {
                            Image = new Bitmap(Properties.Resources.adam),
                            Location = new Point(xMap * 50, yMap * 50 + 20),
                            Size = new Size(50, 50)
                        };

                        Controls.Add(labelplayer);
                        return;
                    }
        }
        private void CreateBoxes()
        {
            int numer = 0;

            for (int yMap = 0; yMap < mapLength; yMap++)
                for (int xMap = 0; xMap < mapLength; xMap++)
                    if (maps[yMap, xMap] == 3)
                    {
                        labelBox = new Label
                        {
                            Image = new Bitmap(Properties.Resources.box),
                            Location = new Point(xMap * 50, yMap * 50 + 20),
                            Size = new Size(50, 50)
                        };

                        boxes[numer++] = labelBox;
                        Controls.Add(labelBox);
                    }
        }
        private void CreateWinPositions()
        {
            int numer = 0;

            for (int yMap = 0; yMap < mapLength; yMap++)
                for (int xMap = 0; xMap < mapLength; xMap++)
                    if (maps[yMap, xMap] == 4)
                    {
                        labelWinPos = new Label
                        {
                            Image = new Bitmap(Properties.Resources.winPos),
                            Location = new Point(xMap * 50, yMap * 50 + 20),
                            Size = new Size(50, 50)
                        };

                        winPositions[numer++] = labelWinPos;
                        Controls.Add(labelWinPos);
                    }
        }

        // уничтожение объектов, взаимодействующих друг с другом
        public void DisposeObjects()
        {
            DisposePlayer();
            DisposeBoxes();
            DisposeWinPositions();
        }

        private void DisposePlayer()
        {
            for (int yMap = 0; yMap < mapLength; yMap++)
                for (int xMap = 0; xMap < mapLength; xMap++)
                    if (maps[yMap, xMap] == 0)
                        Controls.Remove(labelplayer);
        }
        private void DisposeBoxes()
        {
            for (int yMap = 0; yMap < mapLength; yMap++)
                for (int xMap = 0; xMap < mapLength; xMap++)
                    if (maps[yMap, xMap] == 3)
                        if (labelBox.Created)
                            Controls.Remove(labelBox);
        }
        private void DisposeWinPositions()
        {
            for (int yMap = 0; yMap < mapLength; yMap++)
                for (int xMap = 0; xMap < mapLength; xMap++)
                    if (maps[yMap, xMap] == 4)
                        if (labelWinPos.Created)
                            Controls.Remove(labelWinPos);
        }

        // упорядочивание списка рекодров
        private void SortRecordFile(string recordFile)
        {
            streamWriter.Close();

            readFile = File.ReadAllLines(recordFile); // считать все рекорды

            int lastRecord = Convert.ToInt32(File.ReadLines(recordFile).Last()); // запомнить последний, в случае переполнения ожидаемого числа рекордов

            int size = readFile.Length < 10 ? readFile.Length : 10; // высчитать длину

            int[] array = new int[size];

            for (int i = 0; i < size; i++)
                array[i] = Convert.ToInt32(readFile[i]); // перевести все в двумерный массив

            if (readFile.Length > size && array.Max() > lastRecord) // при необходимости сменить последний рекорд
                array[size - 1] = lastRecord;

            Array.Sort(array); // отсортировать массив

            for (int i = 0; i < size; i++) // в текстовом виде копируем отсортированный массив
                readFile[i] = array[i].ToString(); // для перезаписи файла

            File.WriteAllLines(recordFile, readFile); // перезаписываем файл с отсортированными рекордами

        }

        // переход
        private void ChangeLevel(object sender, KeyEventArgs e)
        {
            while (stopWatch.IsRunning)
            {
                Sound.VictorySoundStop();
                if (lvl < 5)
                    lvl++;
                else
                    break;

                switch (lvl)
                {
                    case 2:
                        SetTimer("");
                        ReLoad(Level("2"));
                        break;

                    case 3:
                        SetTimer("");
                        ReLoad(Level("3"));
                        break;

                    case 4:
                        SetTimer("");
                        ReLoad(Level("4"));
                        break;

                    case 5:
                        SetTimer("");
                        ReLoad(Level("5"));
                        stopWatch.Stop();
                        break;

                    default:
                        break;
                }
                break;
            }
        }
        // задать некоторые параметры перед сменой уровня

        public void UpdateLvL(string figure)
        {
            lvl = Level(figure);
            ReLoad(Level(figure));
        }
        // очистить все объекты и отрисовать карту
        private void ReLoad(byte lvl)
        {
            graf.DrawImage(fon, 0, 0);
            Dis();
            LoadMaps();
            DisposeObjects();
            CreateObjects();
            DrawMap();
        }
        public byte Level(string level)
        { 
            return Convert.ToByte(level);
        }
        public string GetLevel()
        {
            return lvl.ToString();

        }
        // освобождение элементов, требующих замену для смены уровня
        private static void Dis()
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i].Dispose();
                winPositions[i].Dispose();
            }

            boxes = null;
            winPositions = null;
            maps = null;
        }
        // освобождение всех ресурсов, отображенных графикой и закрытие потока
        public static void Clear()
        {
            graf.Dispose();
            streamWriter.Close();
        }
        // загрузка формы
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadMaps();
            CreateObjects();
        }

        // отрисовка на форме
        private void GamePaint(object sender, PaintEventArgs e)
        {
            DrawMap();
        }

        // переход на 1 уровень
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (IsStopTimer())
                return;
            Sound.VictorySoundStop();
            UpdateLvL("1");
        }

        // переход на 2 уровень
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (IsStopTimer())
                return;
            Sound.VictorySoundStop();
            UpdateLvL("2");
        }
        
        // переход на 3 уровень
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (IsStopTimer())
                return;
            Sound.VictorySoundStop();
            UpdateLvL("3");
        }
        
        // переход на 4 уровень
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (IsStopTimer())
                return;
            Sound.VictorySoundStop();
            UpdateLvL("4");
        }   
        
        // переход на 5 уровень
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (IsStopTimer())
              return;
            Sound.VictorySoundStop();
            UpdateLvL("5");
        }
        
        // настройки звука передвижения
        private void вклToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sound.MoveSoundPlayON();
            Sound.VictorySoundPlayON();
        }
        private void выклToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sound.MoveSoundPlayOFF();
            Sound.VictorySoundPlayOFF();
        }
        
        public string SetTimer(string time)
        {
            return (timer.Text = time);
        }
        
        //переход на форму с рекордами
        private void рекордыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Records recordText = new Records(GetRecordText());
            recordText.Show();

        }
        
        //переход на форму со справкой
        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Description desc = new Description();
            desc.Show();
        }
        
        //перезапуск текущего уровня
        private void перезапускToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sound.VictorySoundStop();
            UpdateLvL(GetLevel());
        }
    }
}
