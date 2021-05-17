using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class ImportPartyViewModel : BindingBase
    {
        public ReactiveProperty<string> Json { get; } = new ReactiveProperty<string>(string.Empty);
        
        public ReactiveCommand Submit { get; } = new ReactiveCommand();
        
        public ReactiveCommand Cancel { get; } = new ReactiveCommand();

        public ImportPartyViewModel()
        {
            Submit.Subscribe(x => OnSubmit((Window) x));
            Cancel.Subscribe(x => OnCancel((Window) x));
        }

        private void OnSubmit(Window x)
        {
            if (string.IsNullOrEmpty(Json.Value))
            {
                MessageBox.Show("入力してください");
                return;
            }
            
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };

            var instance = JsonSerializer.Deserialize<UserParty>(Json.Value, options);
            Database.I.AddParty(instance);
            x.Close();
        }

        private void OnCancel(Window x)
        {
            x.Close();
        }
    }
}