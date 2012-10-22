using System.Windows;

namespace Fab.Client.Framework.Controls
{
	public partial class ValidationControl
	{
		public ValidationControl()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(string), typeof(ValidationControl), new PropertyMetadata(default(string)));

		public string Message
		{
			get { return (string) GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}

		public static readonly DependencyProperty RefreshTooltipProperty =
			DependencyProperty.Register("RefreshTooltip", typeof(string), typeof(ValidationControl), new PropertyMetadata(default(string)));

		public string RefreshTooltip
		{
			get { return (string)GetValue(RefreshTooltipProperty); }
			set { SetValue(RefreshTooltipProperty, value); }
		}

		public static readonly DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(ValidationStates), typeof(ValidationControl), new PropertyMetadata(ValidationStates.Undefined, StatePropertyChanged));

		public ValidationStates State
		{
			get { return (ValidationStates)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}

		private static void StatePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var validationControl = (ValidationControl) o;
			var newValue = (ValidationStates)e.NewValue;
			
			switch (newValue)
			{
				case ValidationStates.Undefined:
					VisualStateManager.GoToState(validationControl, "UndefinedState", true);
					break;

				case ValidationStates.Checking:
					VisualStateManager.GoToState(validationControl, "CheckingState", true);
					break;

				case ValidationStates.Error:
					VisualStateManager.GoToState(validationControl, "ErrorState", true);
					break;

				case ValidationStates.Ok:
					VisualStateManager.GoToState(validationControl, "OkState", true);
					break;
			}
		}
	}

	public enum ValidationStates
	{
		Undefined,
		Checking,
		Ok,
		Error
	}
}
