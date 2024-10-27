using SocialNetwork_App.DAL.Concrete;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using MongoDB.Bson;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly UserDal _userDal;
    private readonly PostDal _postDal;

    public ObservableCollection<PostDto> Posts { get; set; } = new ObservableCollection<PostDto>();
    public ObservableCollection<UserDto> Friends { get; set; } = new ObservableCollection<UserDto>();

    private string _email;
    private string _password;
    private string _friendName;
    private string _commentText;

    public string FriendName
    {
        get => _friendName;
        set
        {
            _friendName = value;
            OnPropertyChanged(nameof(FriendName));
        }
    }

    public string CommentText
    {
        get => _commentText;
        set
        {
            _commentText = value;
            OnPropertyChanged(nameof(CommentText));
        }
    }

    public ICommand LikeCommand { get; }
    public ICommand CommentCommand { get; }
    public ICommand AddFriendCommand { get; }
    public ICommand RemoveFriendCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MainViewModel(UserDal userDal, PostDal postDal)
    {
        _userDal = userDal;
        _postDal = postDal;

        AddFriendCommand = new RelayCommand(async (param) => await AddFriend(FriendName));
        RemoveFriendCommand = new RelayCommand(async (param) => await RemoveFriend(FriendName));
        LikeCommand = new RelayCommand(async (param) => await LikePost(param as PostDto));

        CommentCommand = new RelayCommand(async (param) =>
        {
            if (param is (ObjectId postId, string commentText))
            {
                await AddComment(postId, commentText);
            }
        }, param => param is (ObjectId, string) && !string.IsNullOrWhiteSpace(((ValueTuple<ObjectId, string>)param).Item2)); // Check if both parameters are present
    }

    public async Task<bool> LoginAsync()
    {
        var user = await _userDal.LoginAsync(Email, Password);
        if (user != null)
        {
            await LoadPostsAsync();
            return true;
        }
        return false;
    }

    private async Task LoadPostsAsync()
    {
        var posts = await _postDal.GetAllPostsAsync();
        Posts.Clear();
        foreach (var post in posts)
        {
            Posts.Add(post);
        }
    }

    public async Task AddFriend(string friendFirstName)
    {
        var allUsers = await _userDal.GetAllUsersAsync();
        var friendToAdd = allUsers.FirstOrDefault(u => u.FirstName.Equals(friendFirstName, StringComparison.OrdinalIgnoreCase));

        if (friendToAdd != null)
        {
            var currentUser = await _userDal.GetUserByEmailAsync(Email);
            if (currentUser != null)
            {
                await _userDal.AddFriendAsync(currentUser.Id, friendToAdd.Id);
                MessageBox.Show($"Друга {friendFirstName} успішно додано.");
            }
            else
            {
                MessageBox.Show("Невірні дані користувача. Спробуйте ще раз.");
            }
        }
        else
        {
            MessageBox.Show("Користувача з таким ім'ям не знайдено.");
        }
    }

    public async Task RemoveFriend(string friendFirstName)
    {
        var friends = await _userDal.GetAllUsersAsync();
        var friendToRemove = friends.FirstOrDefault(u => u.FirstName.Equals(friendFirstName, StringComparison.OrdinalIgnoreCase));

        if (friendToRemove != null)
        {
            var currentUser = await _userDal.GetUserByEmailAsync(Email);
            if (currentUser != null)
            {
                await _userDal.RemoveFriendAsync(currentUser.Id, friendToRemove.Id);
                MessageBox.Show($"Друг {friendFirstName} успішно видалено.");
            }
        }
    }

    public async Task LikePost(PostDto post)
    {
        if (post != null)
        {
            var currentUser = await _userDal.GetUserByEmailAsync(Email);
            if (currentUser != null)
            {
                await _postDal.LikePostAsync(post.Id, currentUser.Id);
                MessageBox.Show("Пост лайкнуто!");
            }
        }
    }

    public async Task AddComment(ObjectId postId, string commentText)
    {
        if (!string.IsNullOrEmpty(commentText))
        {
            var currentUser = await _userDal.GetUserByEmailAsync(Email);
            if (currentUser != null)
            {
                var comment = new CommentDto
                {
                    UserId = currentUser.Id,
                    Text = commentText,
                    CreatedAt = DateTime.UtcNow
                };

                await _postDal.AddCommentAsync(postId, comment);
                CommentText = string.Empty;
            }
            else
            {
                MessageBox.Show("Користувача не знайдено.");
            }
        }
        else
        {
            MessageBox.Show("Коментар не може бути пустим.");
        }
    }
}