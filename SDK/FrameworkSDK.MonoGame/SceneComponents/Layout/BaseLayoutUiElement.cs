using System;
using FrameworkSDK.MonoGame.SceneComponents.Layout.Attributes;

namespace FrameworkSDK.MonoGame.SceneComponents.Layout
{
    public abstract class BaseLayoutUiElement : BaseLayoutUiContainer, ILayoutUiElement
    {
        public float? X { get; set; }
        public float? Y { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }

        public float? WidthPercentage
        {
            get => _widthPercentage;
            set
            {
                if (value != null && value < 0)
                    throw new ArgumentOutOfRangeException($"{nameof(WidthPercentage)} < 0");

                _widthPercentage = value;
            }
        }
        private float? _widthPercentage;

        public float? HeightPercentage
        {
            get => _heightPercentage;
            set
            {
                if (value != null && value < 0)
                    throw new ArgumentOutOfRangeException($"{nameof(HeightPercentage)} < 0");

                _heightPercentage = value;
            }
        }
        private float? _heightPercentage;

        public float? XPercentage
        {
            get => _xPercentage;
            set
            {
                if (value != null && value < 0)
                    throw new ArgumentOutOfRangeException($"{nameof(XPercentage)} < 0");

                _xPercentage = value;
            }
        }
        private float? _xPercentage;

        public float? YPercentage
        {
            get => _yPercentage;
            set
            {
                if (value != null && value < 0)
                    throw new ArgumentOutOfRangeException($"{nameof(YPercentage)} < 0");

                _yPercentage = value;
            }
        }
        private float? _yPercentage;

        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.None;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.None;
        
        public float MarginX { get; set; }
        public float MarginY { get; set; }
        
        public bool IsVisible { get; set; } = true;
        
        public Padding Padding { get; set; } = new Padding();

        public override bool CheckVisible()
        {
            return IsVisible && (Parent == null || Parent.CheckVisible());
        }
    }
}