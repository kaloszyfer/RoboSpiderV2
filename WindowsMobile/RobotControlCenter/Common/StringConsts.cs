using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotControlCenter.Common
{
    /// <summary>
    /// Klasa przechowująca stałe łańcuchy znaków używane wewnątrz aplikacji.
    /// </summary>
    static class StringConsts
    {
        public const string ApplicationTitle = "Centrum kontroli robota";
        public const string MainPageBtButtonContentConnect = "Połącz...";
        public const string MainPageBtButtonLabelBtCheck = "(trwa sprawdzanie dostępności bluetooth...)";
        public const string MainPageBtButtonLabelNoBt = "brak obsługi bluetooth";
        public const string MainPageBtButtonLabelConnect = "nawiąż połączenie z robotem";
        public const string MainPageBtButtonLabelConnecting = "(trwa nawiązywanie połączenia...)";
        public const string MainPageBtButtonLabelConnected = "nawiązano połączenie";
        public const string MainPageCtrlButtonContentSteer = "Steruj...";
        public const string MainPageCtrlButtonContentStop = "Przerwij...";
        public const string MainPageCtrlButtonLabelConnectFirst = "połącz się z robotem, aby umożliwić sterowanie";
        public const string MainPageCtrlButtonLabelSteer = "zacznij sterować robotem";
        public const string MainPageCtrlButtonLabelStop = "przerwij sterowanie robotem";

        public const string MainPageLabelStartInstruction = "Aby rozpocząć sterowanie robotem należy wykonać następujące czynności:\n - uruchomić robota,\n - użyć przycisku 'Połącz', aby się z nim połączyć,\n - uruchomić tryb sterowania przyciskiem 'Steruj'.";

        public const string ExitDialogTitle = "Wyłączenie aplikacji.";
        public const string ExitDialogText = "Czy na pewno chcesz wyłączyć aplikację?";
        public const string ExitDialogTextWithBt = "Jeśli robot już nie będzie używany, zaleca się, aby przed wyłączeniem aplikacji wprowadzić go w stan dezaktywacji. Kontynuować?";
        public const string Yes = "Tak";
        public const string No = "Nie";

        public const string DeactivateRobotDialogTitle = "Dezaktywacja robota.";
        public const string DeactivateRobotDialogText = "Czy na pewno chcesz dezaktywować robota?";

        public const string Error = "Błąd!";
        public const string BtNotSupportedError = "To urządzenie nie obsługuje bluetooth.";
        public const string BtNotActiveError = "Proszę uruchomić bluetooth.";
        public const string RobotNotPairedButCanPair = "Robot nie został jeszcze sparowany z urządzeniem. Za chwilę nastąpi próba sparowania.";
        public const string RobotNotPairedAndCannotPair = "Proszę najpierw sparować robota z urządzeniem.";
        public const string BtConnectionError = "Wystąpił błąd połączenia z robotem: ";
        public const string BtNoDeviceSetError = "Nie wybrano urządzenia docelowego (robota). Przerywam próbę nawiązania połączenia.";
        public const string BtDeviceConnectionError = "Nastąpił błąd połączenia z urządzeniem docelowym (robotem). Jeśli błąd będzie się powtarzał, proszę ponownie sparować urządzenia.";
        public const string BtSendDataError = "Wystąpił błąd podczas wysyłania komendy.";
    }
}
