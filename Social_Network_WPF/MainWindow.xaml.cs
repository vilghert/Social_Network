using System.Windows;
using System.Windows.Controls;
using MongoDB.Driver;
using SocialNetwork_App.DAL.Concrete;

namespace Social_Network
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            var mongoClient = new MongoClient("mongodb+srv://victoriia:iraros2005@vlnu.rsmja.mongodb.net/?retryWrites=true&w=majority&appName=VLNU");
            var database = mongoClient.GetDatabase("socialntw");

            var userDal = new UserDal(database);
            var postDal = new PostDal(database);

            _viewModel = new MainViewModel(userDal, postDal);
            DataContext = _viewModel;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var success = await _viewModel.LoginAsync();
            if (success)
            {
                var mainPage = new MainPage(_viewModel);
                mainPage.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid credentials. Please try again.");
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            _viewModel.Password = passwordBox.Password;
        }
    }
}