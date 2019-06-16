namespace ComLineCDWithFinder.Infrastructure
{
    interface ICommandShell
    {
        void Write(string str);

        string Read();

        void PutCommandToLine(string command);
    }
}
