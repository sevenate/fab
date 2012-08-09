using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Fab.Client.Framework.Controls
{
	public partial class ValidationOKControl : UserControl
	{
		public ValidationOKControl()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty OkStateColorProperty =
			DependencyProperty.Register("OkStateColor", typeof(Color), typeof(ValidationOKControl), new PropertyMetadata(Colors.Green));

		public Color OkStateColor
		{
			get { return (Color)GetValue(OkStateColorProperty); }
			set { SetValue(OkStateColorProperty, value); }
		}

		public static readonly DependencyProperty IsOkProperty =
			DependencyProperty.Register("IsOk", typeof (bool), typeof (ValidationOKControl), new PropertyMetadata(default(bool)));

		public bool IsOk
		{
			get { return (bool) GetValue(IsOkProperty); }
			set { SetValue(IsOkProperty, value); }
		}
	}
}
