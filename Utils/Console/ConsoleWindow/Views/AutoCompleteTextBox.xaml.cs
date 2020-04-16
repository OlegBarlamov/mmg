using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ConsoleWindow.Views
{
	public partial class AutoCompleteTextBox
	{
		public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register(nameof(Items),
				typeof(object), typeof(AutoCompleteTextBox),
				new PropertyMetadata(null));

		public object Items
		{
			get => GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register(nameof(Text),
				typeof(string), typeof(AutoCompleteTextBox),
				new PropertyMetadata(null, TextPropertyPropertyChanged));

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public static readonly DependencyProperty WatermarkProperty =
			DependencyProperty.Register(nameof(Watermark),
				typeof(string), typeof(AutoCompleteTextBox),
				new PropertyMetadata(null));

		public string Watermark
		{
			get => (string)GetValue(WatermarkProperty);
			set => SetValue(WatermarkProperty, value);
		}

		public static readonly DependencyProperty ItemTemplateProperty =
			DependencyProperty.Register(nameof(ItemTemplate),
				typeof(DataTemplate), typeof(AutoCompleteTextBox),
				new PropertyMetadata(null));

		public DataTemplate ItemTemplate
		{
			get => (DataTemplate)GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		public static readonly DependencyProperty ItemToTextConverterProperty =
			DependencyProperty.Register(nameof(ItemToTextConverter),
				typeof(IValueConverter), typeof(AutoCompleteTextBox),
				new PropertyMetadata(null));

		public IValueConverter ItemToTextConverter
		{
			get => (IValueConverter)GetValue(ItemToTextConverterProperty);
			set => SetValue(ItemToTextConverterProperty, value);
		}

		private Window _ownedWindow;

		public AutoCompleteTextBox()
		{
			InitializeComponent();

			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnLoaded;

			_ownedWindow = Window.GetWindow(this);
			if (_ownedWindow != null)
			{
				_ownedWindow.LocationChanged += WindowOnLocationChanged;
				Unloaded += OnUnloaded;
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			Unloaded -= OnUnloaded;

			if (_ownedWindow != null)
				_ownedWindow.LocationChanged -= WindowOnLocationChanged;

			Loaded += OnLoaded;
		}

		private void WindowOnLocationChanged(object sender, EventArgs e)
		{
			var offset = Popup.HorizontalOffset;
			Popup.HorizontalOffset = offset + 1;
			Popup.HorizontalOffset = offset;
		}

		private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			Text = TextBox.Text;

			var textBoxText = Text;
			if (string.IsNullOrWhiteSpace(textBoxText))
			{
				SetSuggestions(Array.Empty<object>());
				ClosePopup();
				return;
			}

			var filteredItems = FilterItemsByText(Items as IEnumerable<object>, textBoxText);
			SetSuggestions(filteredItems);

			if (filteredItems.Count > 0)
				ShowPopup();
			else
				ClosePopup();
		}

		private IReadOnlyList<object> FilterItemsByText(IEnumerable<object> items, string text)
		{
			if (items == null)
				return Array.Empty<object>();

			text = text.ToLowerInvariant();

			var itemsWithText = items.Select(i => new Tuple<object, string>(i, ConvertItem(i)?.ToLowerInvariant()));
			var suggestedItemsWithText = itemsWithText.Where(tuple => IsSuggest(tuple.Item2, text)).ToList();

			suggestedItemsWithText.Sort((e1, e2) => string.CompareOrdinal(e1.Item2, e2.Item2));
			return suggestedItemsWithText.Select(tuple => tuple.Item1).ToList();
		}

		private string ConvertItem(object item)
		{
			if (ItemToTextConverter == null)
				return item?.ToString();

			return (string)ItemToTextConverter.Convert(item, typeof(string), null, CultureInfo.CurrentCulture);
		}

		private bool IsSuggest(string itemText, string enteredText)
		{
			if (string.IsNullOrWhiteSpace(itemText))
				return false;

			return itemText.IndexOf(enteredText, StringComparison.Ordinal) == 0 && itemText != enteredText;
		}

		private void SetSuggestions(IReadOnlyList<object> items)
		{
			Suggestions.Items.Clear();
			foreach (var item in items)
			{
				Suggestions.Items.Add(item);
			}
			if (items.Count > 0)
				Suggestions.ScrollIntoView(items.Last());
		}

		private void ClosePopup()
		{
			Popup.IsOpen = false;
		}

		private void ShowPopup()
		{
			Popup.IsOpen = true;
		}

		private static void TextPropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var self = (AutoCompleteTextBox)d;
			var newText = e.NewValue as string ?? string.Empty;
			if (self.TextBox.Text != newText)
			{
				self.TextBox.Text = newText;
				self.TextBox.CaretIndex = newText.Length;
			}
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up && Suggestions.Items.Count > 0 && !(e.OriginalSource is ListBoxItem) && Popup.IsOpen)
			{
				var listBoxItem = Suggestions.ItemContainerGenerator.ContainerFromItem(Suggestions.SelectedItem) as ListBoxItem
								  ?? Suggestions.ItemContainerGenerator.ContainerFromItem(Suggestions.Items[Suggestions.Items.Count - 1]) as ListBoxItem;

				listBoxItem?.Focus();
				Suggestions.SelectedItem = listBoxItem?.Content;
				e.Handled = true;
			}

			if (e.Key == Key.Escape && Popup.IsOpen)
			{
				ClosePopup();
			}
		}

		private void Suggestions_OnSelectionChanged(object selectedItem)
		{
			if (selectedItem == null)
				return;

			var text = ConvertItem(selectedItem);
			Text = text;

			TextBox.Focus();
		}

		private void Suggestions_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DependencyObject obj = Suggestions.ContainerFromElement((Visual)e.OriginalSource);
				if (obj != null)
				{
					if (obj is FrameworkElement element)
					{
						if (element is ListBoxItem item)
							Suggestions_OnSelectionChanged(item.Content);
					}
				}
			}
		}

		private void Suggestions_OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && Suggestions.SelectedItem != null)
			{
				Suggestions_OnSelectionChanged(Suggestions.SelectedItem);
				e.Handled = true;
			}
			else if (e.Key == Key.Up)
			{
				var nextSelected = Suggestions.SelectedIndex - 1;
				if (nextSelected >= 0)
					Suggestions.SelectedIndex = nextSelected;
			}
			else if (e.Key == Key.Down)
			{
				var nextSelected = Suggestions.SelectedIndex + 1;
				if (nextSelected < Suggestions.Items.Count)
					Suggestions.SelectedIndex = nextSelected;
			}
		}
	}
}
