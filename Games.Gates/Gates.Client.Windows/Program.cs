using System;
using Gates.ClientCore;

namespace Gates.Client.Windows
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            GameFactory.Create(new WindowsServicesModule());
        }
    }
#endif
}
