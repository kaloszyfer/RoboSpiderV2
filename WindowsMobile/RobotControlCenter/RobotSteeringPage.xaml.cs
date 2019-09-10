using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using RobotControlCenter.Common;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace RobotControlCenter
{
    /// <summary>
    /// Strona sterowania robotem wyświetlana wewnątrz ramki znajdującej się na głównej stronie aplikacji.
    /// </summary>
    public sealed partial class RobotSteeringPage : Page
    {
        /// <summary>
        /// Rozkazy dla robota (skopiowane bezpośrednio z projektu na Arduino)
        /// </summary>
        enum RobotCommand
        {
            Stand = 0,        // robot stoi
            MoveFront,        // robot idzie do przodu
            MoveBack,         // robot idzie do tyłu
            MoveLeft,         // robot idzie w lewo
            MoveRight,        // robot idzie w prawo
            TurnLeft,         // robot skręca w lewo
            TurnRight,        // robot skręca w prawo
            GoToInitialPos    // robot wraca do pozycji początkowej
        };

        /// <summary>
        /// Konstruktor strony sterowania robotem.
        /// </summary>
        public RobotSteeringPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Obsługa zdarzenia kliknięcia w przycisk 'Strzałka skręcająca w lewo'. Obsługa polega na wysłaniu do robota
        /// rozkazu ruchu skręcającego w lewo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonTurnLeft_Click(object sender, RoutedEventArgs e)
        {
            await Bluetooth.SendByte((byte)RobotCommand.TurnLeft);
        }

        /// <summary>
        /// Obsługa zdarzenia kliknięcia w przycisk 'Strzałka do góry'. Obsługa polega na wysłaniu do robota
        /// rozkazu ruchu do przodu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonForward_Click(object sender, RoutedEventArgs e)
        {
            await Bluetooth.SendByte((byte)RobotCommand.MoveFront);
        }

        /// <summary>
        /// Obsługa zdarzenia kliknięcia w przycisk 'Strzałka skręcająca w prawo'. Obsługa polega na wysłaniu do robota
        /// rozkazu ruchu skręcającego w prawo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonTurnRight_Click(object sender, RoutedEventArgs e)
        {
            await Bluetooth.SendByte((byte)RobotCommand.TurnRight);
        }

        /// <summary>
        /// Obsługa zdarzenia kliknięcia w przycisk 'Strzałka w lewo'. Obsługa polega na wysłaniu do robota
        /// rozkazu ruchu w lewo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonGoLeft_Click(object sender, RoutedEventArgs e)
        {
            await Bluetooth.SendByte((byte)RobotCommand.MoveLeft);
        }

        /// <summary>
        /// Obsługa zdarzenia kliknięcia w przycisk 'Strzałka na dół'. Obsługa polega na wysłaniu do robota
        /// rozkazu ruchu do tyłu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonBackwards_Click(object sender, RoutedEventArgs e)
        {
            await Bluetooth.SendByte((byte)RobotCommand.MoveBack);
        }

        /// <summary>
        /// Obsługa zdarzenia kliknięcia w przycisk 'Strzałka w prawo'. Obsługa polega na wysłaniu do robota
        /// rozkazu ruchu w prawo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonGoRight_Click(object sender, RoutedEventArgs e)
        {
            await Bluetooth.SendByte((byte)RobotCommand.MoveRight);
        }

        /// <summary>
        /// Obsługa zdarzenia kliknięcia w przycisk wyłącznika. Obsługa polega na wyświetleniu użytkownikowi prośby o potwierdzenie
        /// wysłania komendy. W przypadku, gdy odpowiedź użytkownika jest pozytywna do robota zostaje wysłany rozkaz deaktywacji
        /// oraz wywoływane są następujące metody: kończąca połączenie Bluetooth i (poprzez globalną zmienną referencji
        /// do strony głównej) obsługująca zdarzenie wysłania komendy deaktywacji.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonGoInitialPos_Click(object sender, RoutedEventArgs e)
        {
            var result = await PopUp.ShowChoice(StringConsts.DeactivateRobotDialogText, StringConsts.DeactivateRobotDialogTitle, StringConsts.Yes, StringConsts.No);
            if (!result)
            {
                return;
            }
            await Bluetooth.SendByte((byte)RobotCommand.GoToInitialPos);
            Bluetooth.Disconnect();
            App.MainPage.HandleRobotDeactivation();
        }
    }
}
