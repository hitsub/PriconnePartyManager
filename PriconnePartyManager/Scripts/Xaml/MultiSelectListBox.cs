using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace PriconnePartyManager.Scripts.Xaml
{
    public class MultiSelectListBox : ListBox
    {
        public new static readonly DependencyProperty SelectedItemListProperty =
            DependencyProperty.Register(
                nameof(SelectedItemList), typeof(IList),
                typeof(MultiSelectListBox),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public new IList SelectedItemList
        {
            get { return (IList)this.GetValue(SelectedItemListProperty); }
            set { this.SetValue(SelectedItemListProperty, value); }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            this.SelectedItemList = base.SelectedItems;
        }
    }
}