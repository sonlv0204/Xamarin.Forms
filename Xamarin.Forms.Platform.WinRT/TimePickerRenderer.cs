﻿using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Internals;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public class TimePickerRenderer : ViewRenderer<TimePicker, Windows.UI.Xaml.Controls.TimePicker>
	{
		Brush _defaultBrush;
		bool _fontApplied;
		FontFamily _defaultFontFamily;

		protected override void Dispose(bool disposing)
		{
			if (disposing && Control != null)
			{
				Control.TimeChanged -= OnControlTimeChanged;
				Control.Loaded -= ControlOnLoaded;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var picker = new Windows.UI.Xaml.Controls.TimePicker();
					SetNativeControl(picker);

					Control.TimeChanged += OnControlTimeChanged;
					Control.Loaded += ControlOnLoaded;
				}

				UpdateTime();
			}
		}

		void ControlOnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			// The defaults from the control template won't be available
			// right away; we have to wait until after the template has been applied
			_defaultBrush = Control.Foreground;
			_defaultFontFamily = Control.FontFamily;
			UpdateFont();
			UpdateTextColor();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
				UpdateTime();
			else if (e.PropertyName == TimePicker.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == TimePicker.FontAttributesProperty.PropertyName || e.PropertyName == TimePicker.FontFamilyProperty.PropertyName || e.PropertyName == TimePicker.FontSizeProperty.PropertyName)
				UpdateFont();
		}

		void OnControlTimeChanged(object sender, TimePickerValueChangedEventArgs e)
		{
			Element.Time = e.NewTime;
			((IVisualElementController)Element)?.InvalidateMeasure(InvalidationTrigger.SizeRequestChanged);
		}

		void PickerOnForceInvalidate(object sender, EventArgs eventArgs)
		{
			((IVisualElementController)Element)?.InvalidateMeasure(InvalidationTrigger.SizeRequestChanged);
		}

		void UpdateFont()
		{
			if (Control == null)
				return;

			TimePicker timePicker = Element;

			if (timePicker == null)
				return;

			bool timePickerIsDefault = timePicker.FontFamily == null && timePicker.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(TimePicker), true) && timePicker.FontAttributes == FontAttributes.None;

			if (timePickerIsDefault && !_fontApplied)
				return;

			if (timePickerIsDefault)
			{
				// ReSharper disable AccessToStaticMemberViaDerivedType
				Control.ClearValue(ComboBox.FontStyleProperty);
				Control.ClearValue(ComboBox.FontSizeProperty);
				Control.ClearValue(ComboBox.FontFamilyProperty);
				Control.ClearValue(ComboBox.FontWeightProperty);
				Control.ClearValue(ComboBox.FontStretchProperty);
				// ReSharper restore AccessToStaticMemberViaDerivedType
			}
			else
			{
				Control.ApplyFont(timePicker);
			}

			_fontApplied = true;
		}

		void UpdateTime()
		{
			Control.Time = Element.Time;
		}

		void UpdateTextColor()
		{
			Color color = Element.TextColor;
			Control.Foreground = color.IsDefault ? (_defaultBrush ?? color.ToBrush()) : color.ToBrush();
		}
	}
}