using System.Windows;
using System.Windows.Media;

namespace Fab.Client.Framework.Controls
{
	public partial class ValidationOkControl
	{
		public ValidationOkControl()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty OkStateColorProperty =
			DependencyProperty.Register("OkStateColor", typeof(Color), typeof(ValidationOkControl), new PropertyMetadata(Colors.Green));

		public Color OkStateColor
		{
			get { return (Color)GetValue(OkStateColorProperty); }
			set { SetValue(OkStateColorProperty, value); }
		}

		public static readonly DependencyProperty IsOkProperty =
			DependencyProperty.Register("IsOk", typeof (bool), typeof (ValidationOkControl), new PropertyMetadata(default(bool)));

		public bool IsOk
		{
			get { return (bool) GetValue(IsOkProperty); }
			set { SetValue(IsOkProperty, value); }
		}
	}
}
