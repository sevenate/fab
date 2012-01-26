using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fab.Managment.AdminServiceReference;
using Fab.Managment.Core;

namespace Fab.Managment
{
    /// <summary>
    /// Interaction logic for UserDetails.xaml
    /// </summary>
    public partial class UserDetails : UserControl
    {
        public UserDetails()
        {
            InitializeComponent();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            var button = (FrameworkElement)sender;
            button.IsEnabled = false;

            var userDTO = (AdminUserDTO)button.DataContext;

            var adminServiceClient = Helpers.CreateClientProxy(null, null, null);

            adminServiceClient.DeleteUserCompleted += (o, args) =>
            {
                if (args.Error != null)
                {
                    Helpers.ErrorProcessing(args);
                }

                button.IsEnabled = true;
            };
            adminServiceClient.DeleteUserAsync(userDTO.Id);
        }
    }
}
