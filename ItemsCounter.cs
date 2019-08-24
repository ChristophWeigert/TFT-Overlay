using Microsoft.VisualStudio.PlatformUI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TFT_Overlay
{
    public class ItemsCounter : UserControl
    {
        private ICommand _upCommand;
        private ICommand _downCommand;

        #region Methods...

        private void UpPoints() => this.Value += this.Step;

        private void DownPoints() => this.Value -= this.Step;

        #endregion

        #region Commands...
        public ICommand UpCommand => this._upCommand ?? (this._upCommand = new DelegateCommand(this.UpPoints));

        public ICommand DownCommand => this._downCommand ?? (this._downCommand = new DelegateCommand(this.DownPoints));

        #endregion

        #region Properties...

        #region ValueProperty
        public static DependencyProperty ValueProperty =
           DependencyProperty.Register(
               "Value",
               typeof(int),
               typeof(ItemsCounter),
               new PropertyMetadata(0));

        public int Value
        {
            get => (int)this.GetValue(ValueProperty);
            set
            {
                if (value < this.MinValue)
                    value = this.MinValue;
                if (value > this.MaxValue)
                    value = this.MaxValue;
                this.SetValue(ValueProperty, value);
            }
        }
        #endregion

        #region MinValueProperty
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue",
            typeof(int),
            typeof(ItemsCounter),
            new PropertyMetadata(0));

        public int MinValue
        {
            get => (int)this.GetValue(MinValueProperty);
            set
            {
                if (value > this.MaxValue)
                    this.MaxValue = value;
                this.SetValue(MinValueProperty, value);
            }
        }
        #endregion

        #region MaxValueProperty
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue",
            typeof(int),
            typeof(ItemsCounter),
            new PropertyMetadata(int.MaxValue));

        public int MaxValue
        {
            get => (int)this.GetValue(MaxValueProperty);
            set
            {
                if (value < this.MinValue)
                    value = this.MinValue;
                this.SetValue(MaxValueProperty, value);
            }
        }
        #endregion

        #region StepProperty
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step",
            typeof(int),
            typeof(ItemsCounter),
            new PropertyMetadata(1));

        public int Step
        {
            get => (int)this.GetValue(StepProperty);
            set => this.SetValue(StepProperty, value);
        }
        #endregion

        #endregion
    }
}
