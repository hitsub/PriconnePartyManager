using System;
using System.Linq;
using System.Windows;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Extensions;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class AddTagWindowViewModel : BindingBase
    {
        public ReactiveCollection<TagViewModel> Tags { get; } = new ReactiveCollection<TagViewModel>();
        
        public ReactiveCommand Close { get; } = new ReactiveCommand();

        public AddTagWindowViewModel(Action<Tag> onAddTag, Window window)
        {
            Tags.Clear();
            Tags.AddRange(Database.I.Tags.Select(x => new TagViewModel(x, window, onAddTag)));

            Close.Subscribe(window.Close);
        }
    }
}