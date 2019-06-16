using System;

namespace ComLineCDWithFinder
{
    [Flags]
    public enum Option : byte
    {
       IgnoreCase = 0b00000001,          
       CaseSensitive = 0b00000011,
       OutputSingle = 0b00011100,
       OutputCount = 0b00010100,
       OutputAll = 0b00001100,
       Help = 0b01111111,
       Undefined = 0b11111111
    }
}