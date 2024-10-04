using System.IO;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MultiMedia_Project1
{
    internal class InputOnlineFile : InputFile
    {
        private FileStream stream;
        private string v;

        public InputOnlineFile(FileStream stream, string v)
        {
            this.stream = stream;
            this.v = v;
        }

        public override FileType FileType => throw new System.NotImplementedException();
    }
}