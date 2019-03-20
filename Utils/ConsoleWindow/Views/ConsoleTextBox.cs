using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace ConsoleWindow.Views
{
    internal class ConsoleTextBox : RichTextBox
    {
        public static readonly DependencyProperty CurrentDocumentProperty =
            DependencyProperty.Register(nameof(CurrentDocument),
                typeof(FlowDocument), typeof(ConsoleTextBox),
                new PropertyMetadata(null, CurrentDocumentPropertyChanged));

        public FlowDocument CurrentDocument
        {
            get => (FlowDocument)GetValue(CurrentDocumentProperty);
            set => SetValue(CurrentDocumentProperty, value);
        }

        //for perfomance
        private DispatcherOperation _runningScrollOperation;

        public ConsoleTextBox()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnloaded;

            if (Document is ConsoleDocument lastDocument)
            {
                lastDocument.DocumentChanged -= ConsoleDocumentOnDocumentChanged;
            }

            Loaded += OnLoaded;
        }

        private void OnNewDocument(FlowDocument newDocument)
        {
            if (Document is ConsoleDocument lastDocument)
            {
                lastDocument.DocumentChanged -= ConsoleDocumentOnDocumentChanged;
            }

            newDocument.PageWidth = 1000;

            if (newDocument is ConsoleDocument consoleDocument)
            {
                consoleDocument.DocumentChanged += ConsoleDocumentOnDocumentChanged;
            }

            Document = newDocument;

        }

        private void ConsoleDocumentOnDocumentChanged(object sender, EventArgs e)
        {
            if (_runningScrollOperation != null || !Selection.IsEmpty)
                return;

            _runningScrollOperation = Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                _runningScrollOperation = null;
                ScrollToEnd();
            }));
        }

        private static void CurrentDocumentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var consoleTextBox = (ConsoleTextBox) d;

            if (e.NewValue is FlowDocument newDocument)
            {
                consoleTextBox.OnNewDocument(newDocument);
            }
        }
    }
}
