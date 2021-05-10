using System;
using System.Windows.Markup;

namespace PriconnePartyManager.Scripts.Xaml
{
    public class EnumListExtension : MarkupExtension
    {
        private Type enumType_;

        public EnumListExtension(Type type)
        {
            enumType_ = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return System.Enum.GetValues(enumType_);
        }
    }
}