using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LR1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		public void OnClick(object sender, RoutedEventArgs e)
		{
			TextRange textRange = new TextRange(	   
			input.Document.ContentStart,
			input.Document.ContentEnd);
			string text = textRange.Text;

			byte[] inputData = Encoding.UTF8.GetBytes(text);
			SWIFFTHash swifftHash = new SWIFFTHash();
			byte[] hash = swifftHash.ComputeHash(inputData);
			string result = BitConverter.ToString(hash).Replace("-", string.Empty);

			output.Document.Blocks.Clear();
			output.Document.Blocks.Add(new Paragraph(new Run(result)));
		}
	}
}
