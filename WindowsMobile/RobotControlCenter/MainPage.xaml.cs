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

using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;

using Windows.UI.Core;

using RobotControlCenter.Common;


//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace RobotControlCenter
{
    /// <summary>
    /// Główna strona aplikacji
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Konstruktor głównej strony aplikacji
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            App.MainPage = this;        // przypisanie referencji do tej strony globalnej zmiennej aplikacji (umożliwia to dostęp do strony z innych miejsc aplikacji)

            labelTitle.Text = StringConsts.ApplicationTitle;                            // przypisanie obiektom odpowiednich łańcuchów znaków
            labelStartInstruction.Text = StringConsts.MainPageLabelStartInstruction;

            buttonConnect.Content = StringConsts.MainPageBtButtonContentConnect;
            labelButtonConnect.Text = StringConsts.MainPageBtButtonLabelBtCheck;

            buttonControl.Content = StringConsts.MainPageCtrlButtonContentSteer;
            labelButtonControl.Text = StringConsts.MainPageCtrlButtonLabelConnectFirst;

            frame.Navigate(typeof(RobotSteeringPage));                                  // ramka -> nawigacja do strony sterującej robotem (ramka początkowo niewidoczna)

            _checkBluetoothAvailability();                                              // sprawdzenie czy urządzenie posiada moduł BT i zależnie od wyniku odpowiednie ustawienie UI
        }

        /// <summary>
        /// Sprawdza czy urządzenie obsługuje Bluetooth. Zależnie od wyniku sprawdzenia odpowiednio ustawia elementy UI.
        /// </summary>
        private async void _checkBluetoothAvailability()
        {
            await System.Threading.Tasks.Task.Delay(1000);

            var isBtSupported = await Bluetooth.IsSupported();
            if (!isBtSupported) {
                _setLabelText(labelButtonConnect, StringConsts.MainPageBtButtonLabelNoBt);
                PopUp.Show(StringConsts.BtNotSupportedError, StringConsts.Error);
            }
            else
            {
                _setButtonIsEnabled(buttonConnect, true);
                _setImageOpacity(imageConnect, 1);
                _animateBluetoothIcon(false);
                _setLabelText(labelButtonConnect, StringConsts.MainPageBtButtonLabelConnect);
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia w przycisk 'Połącz'. Obsługa polega na sprawdzeniu załączenia modułu Bluetooth.
        /// W przypadku, gdy wynik sprawdzenia jest pozytywny wyświetlane jest menu wyboru urządzenia o nazwie "RobotPajak".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (labelStartInstruction.Visibility == Visibility.Visible) {
                labelStartInstruction.Visibility = Visibility.Collapsed;
            }
            var isBtActive = await Bluetooth.IsEnabled();
            if (isBtActive)
            {
                var picker = new DevicePicker();
                picker.Filter.SupportedDeviceSelectors.Add(BluetoothDevice.GetDeviceSelectorFromDeviceName("RobotPajak"));
                picker.Show(new Rect(10, 10, 300, 300));
                picker.DeviceSelected += (pckr, args) => { _selectedDeviceHandler(pckr, args); };
            }
            else
            {
                PopUp.Show(StringConsts.BtNotActiveError, StringConsts.Error);
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie wyboru urządzenia. Obsługa polega na sprawdzeniu czy wybrane urządzenie jest sparowane.
        /// W przypadku, gdy wynik sprawdzenia jest pozytywny wywoływana jest metoda próbująca nawiązać połączenie z wybranym urządzeniem.
        /// W przeciwnym razie, jeśli jest taka możliwość wywoływana jest metoda parująca urządzenia.
        /// </summary>
        /// <param name="pckr"></param>
        /// <param name="args"></param>
        private void _selectedDeviceHandler(DevicePicker pckr, DeviceSelectedEventArgs args)
        {
            pckr.Hide();
            if (args.SelectedDevice.Pairing.IsPaired)
            {
                _connectToDevice(args.SelectedDevice);
            }
            else
            {
                if (args.SelectedDevice.Pairing.CanPair)
                {
                    PopUp.Show(StringConsts.RobotNotPairedButCanPair, StringConsts.Error);
                    _pairDevices(args.SelectedDevice);
                }
                else
                {
                    PopUp.Show(StringConsts.RobotNotPairedAndCannotPair, StringConsts.Error);
                }
            }
        }

        /// <summary>
        /// Paruje z wskazanym urządzeniem. Jeżeli wynik parowania jest pozytywny wywoływana jest metoda próbująca nawiązać połączenie z wybranym urządzeniem.
        /// </summary>
        /// <param name="device"></param>
        private async void _pairDevices(DeviceInformation device)
        {
            var result = await device.Pairing.PairAsync();
            if ((result.Status == DevicePairingResultStatus.Paired) || (result.Status == DevicePairingResultStatus.AlreadyPaired))
            {
                _connectToDevice(device);
            }
        }

        /// <summary>
        /// Próbuje nawiązać połączenie z wskazanym urządzeniem. Zależnie od wyniku elementy UI są odpowiednio ustawiane.
        /// </summary>
        /// <param name="device"></param>
        private async void _connectToDevice(DeviceInformation device)
        {
            _setButtonIsEnabled(buttonConnect, false);
            _animateBluetoothIcon(true);
            _setLabelText(labelButtonConnect, StringConsts.MainPageBtButtonLabelConnecting);
            await Bluetooth.SetTargetDevice(device);
            await Bluetooth.Connect();
            if (Bluetooth.IsConnected)
            {
                _setLabelText(labelButtonConnect, StringConsts.MainPageBtButtonLabelConnected);
                _animateBluetoothIcon(true);
                _setButtonIsEnabled(buttonControl, true);
                _setLabelText(labelButtonControl, StringConsts.MainPageCtrlButtonLabelSteer);
                _setImageOpacity(imageControl, 1);
            }
            else
            {
                _setButtonIsEnabled(buttonConnect, true);
                _animateBluetoothIcon(false);
                _setLabelText(labelButtonConnect, StringConsts.MainPageBtButtonLabelConnect);
            }
        }

        /// <summary>
        /// Ustawia stan załączenia wskazanego przycisku.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="isEnabled"></param>
        private async void _setButtonIsEnabled(Button button, bool isEnabled)
        {   // zmiana ustawień elementów UI, która nie odbywa się bezpośrednio poprzez interakcję z UI musi znaleźć się wewnątrz poniższej operacji:
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                button.IsEnabled = isEnabled;
            }
            );
        }

        /// <summary>
        /// Ustawia tekst wskazanego obiektu typu TextBlock.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="text"></param>
        private async void _setLabelText(TextBlock label, string text)
        {   // zmiana ustawień elementów UI, która nie odbywa się bezpośrednio poprzez interakcję z UI musi znaleźć się wewnątrz poniższej operacji:
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                label.Text = text;
            }
            );
        }

        /// <summary>
        /// Ustawia wartość wypełnienia wskazanego obrazu.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="value"></param>
        private async void _setImageOpacity(Image image, double value)
        {   // zmiana ustawień elementów UI, która nie odbywa się bezpośrednio poprzez interakcję z UI musi znaleźć się wewnątrz poniższej operacji:
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                image.Opacity = value;
            }
            );
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia w przycisk 'Steruj'. Obsługa polega na sprawdzeniu czy ramka wyświetlająca elementy wysyłające
        /// komendy do robota jest schowana. W przypadku, gdy wynik sprawdzenia jest pozytywny ramka jest wyświetlana.
        /// W przeciwnym razie ramka jest chowana. Ustawiane są także odpowiednie elementy UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonControl_Click(object sender, RoutedEventArgs e)
        {
            if (frame.Visibility == Visibility.Collapsed)
            {
                frame.Visibility = Visibility.Visible;
                buttonControl.Content = StringConsts.MainPageCtrlButtonContentStop;
                labelButtonControl.Text = StringConsts.MainPageCtrlButtonLabelStop;
                _animateSteeringWheel(true);
            }
            else
            {
                frame.Visibility = Visibility.Collapsed;
                buttonControl.Content = StringConsts.MainPageCtrlButtonContentSteer;
                labelButtonControl.Text = StringConsts.MainPageCtrlButtonLabelSteer;
                _animateSteeringWheel(false);
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia w przycisk 'Wyjdź'. Obsługa polega na sprawdzeniu czy istnieje połączenie z urządzeniem Bluetooth.
        /// Zależnie od wyniku sprawdzenia wyświetlane jest odpowiednie pytanie. W przypadku, gdy wynik sprawdzenia jest pozytywny i użytkownik
        /// potwierdził zamknięcie aplikacji wywoływana jest metoda rozłączająca połączenie Bluetooth.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            bool btConnected = Bluetooth.IsConnected;
            bool result = await PopUp.ShowChoice(btConnected ? StringConsts.ExitDialogTextWithBt : StringConsts.ExitDialogText, StringConsts.ExitDialogTitle, StringConsts.Yes, StringConsts.No);
            if (!result)
            {
                return;
            }
            if (btConnected)
            {
                Bluetooth.Disconnect();
            }
            Application.Current.Exit();
        }

        /// <summary>
        /// Zależnie od podanego parametru uruchamia lub zatrzymuje animację ikonki Bluetooth.
        /// </summary>
        /// <param name="play"></param>
        private async void _animateBluetoothIcon(bool play)
        {   // zmiana ustawień elementów UI, która nie odbywa się bezpośrednio poprzez interakcję z UI musi znaleźć się wewnątrz poniższej operacji:
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (play)
                {
                    btIconAnimation.Begin();
                }
                else
                {
                    btIconAnimation.Stop();
                }
            }
            );
        }

        /// <summary>
        /// Zależnie od podanego parametru uruchamia lub zatrzymuje animację ikonki kierownicy.
        /// </summary>
        /// <param name="play"></param>
        private async void _animateSteeringWheel(bool play)
        {   // zmiana ustawień elementów UI, która nie odbywa się bezpośrednio poprzez interakcję z UI musi znaleźć się wewnątrz poniższej operacji:
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (play)
                {
                    SteerAnimatedRotateTransform.Angle = -(double)SteerIconAnimationParameters.To;  // ustawienie kąta odchylenia kierownicy na kąt przeciwny do kąta jaki ikonka osiąga na koniec animacji
                    SteerIconAnimation.Begin();
                }
                else
                {
                    SteerIconAnimation.Stop();
                    SteerAnimatedRotateTransform.Angle = 0;                                         // wyzerowanie kąta
                }
            }
            );
        }

        /// <summary>
        /// Obsługuje błąd połączenia Bluetooth. Obsługa polega na wyświetleniu informacji o błędzie oraz zresetowaniu elementów UI do stanu początkowego
        /// (metoda wywoływana w miejscach, gdzie może wystąpić błąd związany z połączeniem Bluetooth).
        /// Uwaga: metoda ta jest publiczna!
        /// </summary>
        public void HandleBluetoothError()
        {
            PopUp.Show(StringConsts.BtSendDataError, StringConsts.Error);
            _resetBluetoothUIElements();
        }

        /// <summary>
        /// Resetuje elementy UI do stanu początkowego oraz wywołuje metodę sprawdzającą stan modułu Bluetooth.
        /// </summary>
        private async void _resetBluetoothUIElements()
        {   // zmiana ustawień elementów UI, która nie odbywa się bezpośrednio poprzez interakcję z UI musi znaleźć się wewnątrz poniższej operacji:
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                frame.Visibility = Visibility.Collapsed;

                buttonConnect.IsEnabled = false;
                labelButtonConnect.Text = StringConsts.MainPageBtButtonLabelBtCheck;
                imageConnect.Opacity = 0.2;

                buttonControl.IsEnabled = false;
                buttonControl.Content = StringConsts.MainPageCtrlButtonContentSteer;
                labelButtonControl.Text = StringConsts.MainPageCtrlButtonLabelConnectFirst;
                imageControl.Opacity = 0.2;
            }
            );
            _animateBluetoothIcon(false);
            _animateSteeringWheel(false);
            _checkBluetoothAvailability();
        }

        /// <summary>
        /// Obsługuje zdarzenie wysłania komendy deaktywującej robota. Obsługa polega na zresetowaniu elementów UI do stanu początkowego.
        /// </summary>
        public void HandleRobotDeactivation()
        {
            _resetBluetoothUIElements();
        }
    }
}
