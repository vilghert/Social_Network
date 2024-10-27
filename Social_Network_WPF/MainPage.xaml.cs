using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Social_Network
{
    public partial class MainPage : Window
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private async void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.FriendName))
            {
                try
                {
                    await _viewModel.AddFriend(_viewModel.FriendName);
                    MessageBox.Show($"Друга з іменем {_viewModel.FriendName} додано.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Введіть ім'я друга для додавання.");
            }
        }

        private async void RemoveFriendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.FriendName))
            {
                try
                {
                    await _viewModel.RemoveFriend(_viewModel.FriendName);
                    MessageBox.Show($"Друга з іменем {_viewModel.FriendName} видалено.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Введіть ім'я друга для видалення.");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var watermarkTextBlock = FindVisualChild<TextBlock>(textBox);

            if (watermarkTextBlock != null)
            {
                watermarkTextBlock.Visibility = string.IsNullOrEmpty(textBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private async void CommentTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Get the context object (post)
                var textBox = sender as TextBox;
                var post = (PostDto)textBox.DataContext; // Ensure post is of the correct type

                // Call the comment command
                if (post != null && !string.IsNullOrWhiteSpace(textBox.Text))
                {
                    await _viewModel.AddComment(post.Id, textBox.Text);
                    MessageBox.Show("Коментар успішно додано!");
                    textBox.Clear();
                }
                e.Handled = true; // Prevent further processing
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
    }
}
