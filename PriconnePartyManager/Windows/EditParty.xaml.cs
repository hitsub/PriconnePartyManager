using System.Windows;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.Mvvm.ViewModel;

namespace PriconnePartyManager.Windows
{
	/// <summary>
	/// EditParty.xaml の相互作用ロジック
	/// </summary>
	public partial class EditParty : Window
	{
		public EditParty()
		{
			InitializeComponent();
			DataContext = new EditPartyViewModel(Database.I.Units);
		}
	}
}
