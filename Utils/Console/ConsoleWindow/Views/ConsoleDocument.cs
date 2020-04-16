using System;
using System.Windows.Documents;
using System.Windows.Media;

namespace ConsoleWindow.Views
{
    internal class ConsoleDocument : FlowDocument
    {
        public event EventHandler DocumentChanged;

        public ConsoleDocument()
        {
            FontSize = Constants.FontSize;
            FontFamily = Constants.FontFamily;
        }

        public void AppendLine(string text, Brush brush)
        {
            var p = new Paragraph(new Run(text))
            {
                Foreground = brush
            };

            Blocks.Add(p);

            DocumentChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
