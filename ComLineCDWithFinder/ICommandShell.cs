using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLineCDWithFinder
{
    interface ICommandShell
    {
        void Write(string str);

        string Read();

        void PutCommandToLine(string command);
    }
}
