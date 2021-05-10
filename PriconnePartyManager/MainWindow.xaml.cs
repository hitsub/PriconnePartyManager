using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PriconnePartyManager.Scripts;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Mvvm.ViewModel;
using PriconnePartyManager.Scripts.Sql;
using PriconnePartyManager.Scripts.Sql.Model;

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
