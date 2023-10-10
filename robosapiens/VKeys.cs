using System.Collections.Generic;

namespace RoboSAPiens
{
    // https://help.sap.com/viewer/b47d018c3b9b45e897faf66a6c0885a8/770.01/en-US/71d8c95e9c7947ffa197523a232d8143.html
    public class VKeys
    {
        private static readonly Dictionary<string, int> dict = new()
        {
            {"Enter", 0},
            {"F1", 1},
            {"F2", 2},
            {"F3", 3},
            {"F4", 4},
            {"F5", 5},
            {"F6", 6},
            {"F7", 7},
            {"F8", 8},
            {"F9", 9},
            {"F10", 10},
            {"Ctrl+S", 11},
            {"F12", 12},
            {"Shift+F1", 13},
            {"Shift+F2", 14},
            {"Shift+F3", 15},
            {"Shift+F4", 16},
            {"Shift+F5", 17},
            {"Shift+F6", 18},
            {"Shift+F7", 19},
            {"Shift+F8", 20},
            {"Shift+F9", 21},
            {"Shift+Ctrl+0", 22},
            {"Shift+F11", 23},
            {"Shift+F12", 24},
            {"Ctrl+F1", 25},
            {"Ctrl+F2", 26},
            {"Ctrl+F3", 27},
            {"Ctrl+F4", 28},
            {"Ctrl+F5", 29},
            {"Ctrl+F6", 30},
            {"Ctrl+F7", 31},
            {"Ctrl+F8", 32},
            {"Ctrl+F9", 33},
            {"Ctrl+F10", 34},
            {"Ctrl+F11", 35},
            {"Ctrl+F12", 36},
            {"Ctrl+Shift+F1", 37},
            {"Ctrl+Shift+F2", 38},
            {"Ctrl+Shift+F3", 39},
            {"Ctrl+Shift+F4", 40},
            {"Ctrl+Shift+F5", 41},
            {"Ctrl+Shift+F6", 42},
            {"Ctrl+Shift+F7", 43},
            {"Ctrl+Shift+F8", 44},
            {"Ctrl+Shift+F9", 45},
            {"Ctrl+Shift+F10", 46},
            {"Ctrl+Shift+F11", 47},
            {"Ctrl+Shift+F12", 48},
            {"Ctrl+E", 70},
            {"Ctrl+F", 71},
            {"Ctrl+/", 72},
            {"Ctrl+\\", 73},
            {"Ctrl+N", 74},
            {"Ctrl+O", 75},
            {"Ctrl+X", 76},
            {"Ctrl+C", 77},
            {"Ctrl+V", 78},
            {"Ctrl+Z", 79},
            {"Ctrl+PageUp", 80},
            {"PageUp", 81},
            {"PageDown", 82},
            {"Ctrl+PageDown", 83},
            {"Ctrl+G", 84},
            {"Ctrl+R", 85},
            {"Ctrl+P", 86}
        };

        public static int? getKeyCombination(string keyCombination) 
        {
            return dict.GetValueOrDefault(keyCombination, -1) switch 
            {
                -1 => null,
                int vkey => vkey
            };
        }
    }
}
