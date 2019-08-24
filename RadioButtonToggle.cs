using System.Windows;
using System.Windows.Controls;

namespace TFT_Overlay
{
    public class OptionalRadioButton : RadioButton
    {
        protected override void OnClick()
        {
            bool? wasChecked = this.IsChecked;
            base.OnClick();
            if (this.IsOptional && wasChecked == true)
            {
                this.IsChecked = false;
            }
        }

        #region bool IsOptional dependency property

        public static DependencyProperty IsOptionalProperty =
            DependencyProperty.Register(
                "IsOptional",
                typeof(bool),
                typeof(OptionalRadioButton),
                new PropertyMetadata(true,
                    (obj, args) => { ((OptionalRadioButton) obj).OnIsOptionalChanged(args); }));

        public bool IsOptional
        {
            get => (bool) this.GetValue(IsOptionalProperty);
            set => this.SetValue(IsOptionalProperty, value);
        }

        private void OnIsOptionalChanged(DependencyPropertyChangedEventArgs args)
        {
            // TODO: Add event handler if needed
        }

        #endregion
    }
}