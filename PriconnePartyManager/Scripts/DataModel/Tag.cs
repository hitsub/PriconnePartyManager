using System.Text.Json.Serialization;
using System.Windows.Media;

namespace PriconnePartyManager.Scripts.DataModel
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public SolidColorBrush TextColorBrush => new SolidColorBrush(TextColor);

        [JsonIgnore]
        public SolidColorBrush BackgroundColorBrush => new SolidColorBrush(BackgroundColor);

        public Color TextColor { get; set; }

        public Color BackgroundColor { get; set; }
    }
}