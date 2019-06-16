using System;

namespace ComLineCDWithFinder.Infrastructure
{
    [Flags]
    public enum Option : byte
    {
        //need for check if two flags are incompatible, using & if 0 => all fine 
       IgnoreCase = 0b00000001,          
       CaseSensitive = 0b00000011,

       OutputSingle = 0b00011100,
       OutputCount = 0b00010100,
       OutputAll = 0b00001100,

       Help = 0b01111111,

       Undefined = 0b11111111
    }
}