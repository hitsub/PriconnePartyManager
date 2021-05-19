using System.Windows;
using PriconnePartyManager.Scripts.Mvvm.ViewModel;
using PriconnePartyManager.Scripts.Sql;

namespace PriconnePartyManager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SqlConnector m_SqlConnector;

		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = new MainWindowViewModel();
		}
	}
}
