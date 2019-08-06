# RoboSpiderV2 - RoboSpider.ino
Plik edytowany w Visual Studio Code z rozszerzeniami: Arduino 0.2.27, C/C++ 0.24.1

Edycja przebiegała z następującymi plikami konfiguracyjnymi w folderze "<GŁ_KATALOG_PROJEKTU>/.vscode/":

arduino.json o zawartości:
{
    "sketch": "Arduino/RoboSpider.ino",
    "board": "arduino:avr:nano",
    "configuration": "cpu=atmega328"
}

oraz

c_cpp_properties.json o zawartości:
{
    "configurations": [
        {
            "name": "Linux",
            "includePath": [
                "/opt/arduino-1.8.9/tools/**",
                "/opt/arduino-1.8.9/hardware/arduino/avr/**",
                "/home/abozek/Arduino/libraries",
                "/opt/arduino-1.8.9/libraries"
            ],
            "forcedInclude": [
                "/opt/arduino-1.8.9/hardware/arduino/avr/cores/arduino/Arduino.h"
            ],
            "intelliSenseMode": "gcc-x64",
            "compilerPath": "/usr/bin/clang",
            "cStandard": "c11",
            "cppStandard": "c++17"
        }
    ],
    "version": 4
}
