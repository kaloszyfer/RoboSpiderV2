using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace RobotControlCenter.Common
{
    /// <summary>
    /// Klasa odpowiedzialna za wyświetlania okien dialogowych.
    /// </summary>
    public static class PopUp
    {
        /// <summary>
        /// Wyświetla okno o podanym tytule zawierające podany tekst.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        public static async void Show(string text, string title)
        {
            try
            {
                var msg = new MessageDialog(text, title);
                await msg.ShowAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PopUp (content: \"" + text + "\", title: \"" + title + "\") Error:" + ex.ToString());
            }
        }

        /// <summary>
        /// Wyświetla okno o podanym tytule zawierające podany tekst oraz przyciski potwierdzenia i odrzucenia o podanych treściach.
        /// Zwracany wynik to 'true' lub 'false' zależnie od tego czy użytkownik kliknął w przycisk potwierdzenia czy odrzucenia.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <param name="optionTrue"></param>
        /// <param name="optionFalse"></param>
        /// <returns></returns>
        public static async Task<bool> ShowChoice(string text, string title, string optionTrue, string optionFalse)
        {
            try
            {
                var msg = new MessageDialog(text, title);
                msg.Commands.Clear();
                msg.Commands.Add(new UICommand { Label = optionTrue, Id = 0 });
                msg.Commands.Add(new UICommand { Label = optionFalse, Id = 1 });
                var result = await msg.ShowAsync();
                if ((int)result.Id == 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PopUp (content: \"" + text + "\", title: \"" + title + "\") Error:" + ex.ToString());
                return false;
            }
        }
    }
}
