using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using System.Diagnostics;
using Windows.Devices.Radios;
using Windows.Storage.Streams;
using System.IO;

namespace RobotControlCenter.Common
{
    /// <summary>
    /// Klasa obsługująca Bluetooth.
    /// </summary>
    public static class Bluetooth
    {
        private static StreamSocket _socket = null;
        private static RfcommDeviceService _service = null;
        private static DeviceInformation _device = null;

        private static bool _foundDevice = false;
        private static bool _isConnected = false;

        /// <summary>
        /// Zwraca czy aktualnie nawiązane jest połączenie.
        /// </summary>
        public static bool IsConnected { get => _isConnected; set => _isConnected = value; }

        /// <summary>
        /// Ustawia docelowe urządzenie z którym będzie przeprowadzana komunikacja.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns></returns>
        public static async Task SetTargetDevice(DeviceInformation targetDevice)
        {
            if (_device != null)
            {
                if (_device.Id.Contains(targetDevice.Id))   // jeśli ustawiono już urządzenie i ma ten sam identyfikator
                {
                    return;                                 // przerywam
                }
            }
            var devices = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
            _foundDevice = false;
            //_device = devices.Where(d => d.Id.Contains(targetDevice.Id)).FirstOrDefault(); _foundDevice = true;
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices.ElementAt(i).Id.Contains(targetDevice.Id))  // jeśli element listy urządzeń protokołu rfcomm zawiera id wybranego urządzenia
                {
                    _device = devices.ElementAt(i);                     // przypisuję element do zmiennej
                    _foundDevice = true;                                // ustawiam flagę znalezienia urządzenia
                    break;                                              // i przerywam pętlę
                }
            }
            //_device = targetDevice;
        }

        /// <summary>
        /// Próbuje nawiązać połączenie z wcześniej wskazanym urządzeniem. (Zobacz: <seealso cref="SetTargetDevice(DeviceInformation)"/>)
        /// </summary>
        /// <returns></returns>
        public static async Task Connect()
        {
            if (!_foundDevice)                                      // jeśli znacznik, mówi, że nie znaleziono urządzenia
            {
                PopUp.Show(StringConsts.BtDeviceConnectionError, StringConsts.Error);
                return;                                             // wyświetlam komunikat i przerywam
            }
            if (_device == null)                                    // jeśli nie ustawiono urządzenia -> przerywam (raczej do tego nie dojdzie, ale na wszelki wypadek...)
            {
                PopUp.Show(StringConsts.BtNoDeviceSetError, StringConsts.Error);
                return;
            }
            try
            {
                if (_service != null)
                {
                    _service.Dispose();
                }
                _service = await RfcommDeviceService.FromIdAsync(_device.Id);
                if (_socket != null)
                {
                    //await _socket.CancelIOAsync();
                    _socket.Dispose();
                }
                _socket = new StreamSocket();
                await _socket.ConnectAsync(
                      _service.ConnectionHostName,
                      _service.ConnectionServiceName,
                      SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                _isConnected = true;
            }
            catch (Exception ex)
            {
                PopUp.Show(StringConsts.BtConnectionError + ex.ToString(), StringConsts.Error);
                Disconnect();
            }
        }

        /// <summary>
        /// Rozłącza połączenie Bluetooth oraz zwalnia zasoby.
        /// </summary>
        public static /*async*/ void Disconnect()
        {
            try
            {
                if (_socket != null)
                {
                    //await _socket.CancelIOAsync();
                    _socket.Dispose();
                    _socket = null;
                }
                if (_service != null)
                {
                    _service.Dispose();
                    _service = null;
                }
                _isConnected = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Bluetooth disconnecting problem: " + ex.ToString());
            }
        }

        /// <summary>
        /// Sprawdza czy urządzenie, na którym działa aplikacja obsługuje Bluetooth.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsSupported()
        {
            var radios = await Radio.GetRadiosAsync();
            var result = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth) != null;
            return result;
        }

        /// <summary>
        /// Sprawdza czy Bluetooth jest uruchomiony.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsEnabled()
        {
            var radios = await Radio.GetRadiosAsync();
            var btRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth);
            var result = (btRadio != null) && (btRadio.State == RadioState.On);
            return result;
        }

        /// <summary>
        /// Wysyła podany bajt do wcześniej wskazanego urządzenia.
        /// (Zobacz także: <seealso cref="SetTargetDevice(DeviceInformation)"/>, <seealso cref="Connect()"/>, <seealso cref="IsConnected"/>)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static async Task SendByte(byte b)
        {
            try
            {
                //var writer = new DataWriter(_socket.OutputStream);
                //writer.WriteByte(b);
                //await writer.StoreAsync();
                Stream outputStream = _socket.OutputStream.AsStreamForWrite();
                outputStream.WriteByte(b);
                await outputStream.FlushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Bluetooth sending byte \"" + b + "\" problem: " + ex.ToString());
                Disconnect();
                App.MainPage.HandleBluetoothError();
            }
        }
    }
}
