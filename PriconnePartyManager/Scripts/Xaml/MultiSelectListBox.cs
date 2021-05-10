using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace PriconnePartyManager.Scripts.Xaml
{
    public class MultiSelectListBox : ListBox
    {
        public new static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedItems), typeof(IList),
                typeof(MultiSelectListBox),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public new IList SelectedItems
        {
            get { return (IList)this.GetValue(SelectedItemsProperty); }
            set { this.SetValue(SelectedItemsProperty, value); }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            this.SelectedItems = base.SelectedItems;
        }
    }
}