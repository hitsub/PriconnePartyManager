using System;
using System.Windows;
using System.Windows.Media;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class TagViewModel : BindingBase
    {
        public ReactiveProperty<string> Text { get; private set; }
        public ReactiveProperty<SolidColorBrush> BackgroundColor { get; private set; }
        public ReactiveProperty<SolidColorBrush> TextColor { get; private set; }
        
        public ReactiveCommand OnAdd { get; } = new ReactiveCommand();
        public ReactiveCommand OnRemove { get; } = new ReactiveCommand();
        
        public Tag Tag { get; private set; }

        public TagViewModel(Tag tag)
        {
            SetTag(tag);
        }
        
        public TagViewModel(Tag tag, Window window, Action<Tag> onAdd = null)
        {
            SetTag(tag);

            OnAdd.Subscribe(() =>
            {
                onAdd?.Invoke(tag);
                //window.Close();
            });
        }

        public TagViewModel(Tag tag, Action<Tag> onRemove = null)
        {
            SetTag(tag);
            OnRemove.Subscribe(() => onRemove?.Invoke(tag));
        }

        public void SetTag(Tag tag)
        {
            Tag = tag;
            
            Text = new ReactiveProperty<string>(tag.Name);
            BackgroundColor = new ReactiveProperty<SolidColorBrush>(tag.BackgroundColorBrush);
            TextColor = new ReactiveProperty<SolidColorBrush>(tag.TextColorBrush);
            
            Text.AddTo(m_Disposables);
            BackgroundColor.AddTo(m_Disposables);
            TextColor.AddTo(m_Disposables);
        }
    }
}