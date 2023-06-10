using System;
using System.Windows.Forms;

namespace SokobanKR
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameForm());//показывает какую форму открывать

            GameForm.Clear();
        }
    }
}
