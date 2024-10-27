using Social_Network.DAL.Interface;
using Social_Network.DAL.Concrete;
using Neo4j.Driver;
using MongoDB.Driver;
using MongoDB.Bson;
using SocialNetwork_App.DAL.Concrete;

class Program
{
    private static IMongoDatabase database;

    static async Task Main(string[] args)
    {
        var client = new MongoClient("mongodb+srv://victoriia:iraros2005@vlnu.rsmja.mongodb.net/?retryWrites=true&w=majority&appName=VLNU");
        var database = client.GetDatabase("socialntw");

        IUserDal userDal = new UserDal(database);
        IPostDal postDal = new PostDal(database);
        IRelationshipDal relationshipDal = new RelationshipDal(database);

        using (var neo4JConnection = new Neo4JConnection("neo4j+s://a8a68896.databases.neo4j.io", "neo4j", "IooMVGn778QI29Bb7H5TsNgnxL3zsLVNOCTfm6rjWTs"))
        {
            INeo4jUserDal neo4jUserDal = new Neo4jUserDal(neo4JConnection.GetDriver());

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n=== Social Network Application ===");
                Console.WriteLine("1. Add User");
                Console.WriteLine("2. Show All Users");
                Console.WriteLine("3. Update User");
                Console.WriteLine("4. Delete User");
                Console.WriteLine("5. Add Post");
                Console.WriteLine("6. Show All Posts");
                Console.WriteLine("7. Add Relationship");
                Console.WriteLine("8. Delete Relationship");
                Console.WriteLine("9. Check if Users are Connected");
                Console.WriteLine("10. Get Distance Between Users");
                Console.WriteLine("11. Exit");
                Console.Write("Choose an action: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AddUser(userDal, neo4jUserDal);

                        break;
                    case "2":
                        await ShowAllUsers(userDal);
                        break;
                    case "3":
                        await UpdateUser(userDal, neo4jUserDal);
                        break;
                    case "4":
                        await DeleteUser(userDal, neo4jUserDal);
                        break;
                    case "5":
                        await AddPost(postDal);
                        break;
                    case "6":
                        await ShowAllPosts(postDal);
                        break;
                    case "7":
                        await AddRelationship(userDal, neo4jUserDal, relationshipDal);
                        break;
                    case "8":
                        await DeleteRelationship(relationshipDal, neo4jUserDal);
                        break;
                    case "9":
                        await CheckIfUsersConnected(neo4jUserDal);
                        break;
                    case "10":
                        await GetDistanceBetweenUsers(neo4jUserDal);
                        break;
                    case "11":
                        isRunning = false;
                        Console.WriteLine("Program terminated.");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
    static async Task AddUser(IUserDal userDal, INeo4jUserDal neo4jUserDal)
    {
        Console.Write("Enter Email: ");
        string email = Console.ReadLine();

        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        Console.Write("Enter First Name: ");
        string firstName = Console.ReadLine();

        Console.Write("Enter Last Name: ");
        string lastName = Console.ReadLine();

        Console.Write("Enter Interests (comma-separated): ");
        string interestsInput = Console.ReadLine();
        List<string> interests = new List<string>(interestsInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

        UserDto newUser = new UserDto
        {
            Id = ObjectId.GenerateNewId(),
            Email = email,
            Password = password,
            FirstName = firstName,
            LastName = lastName,
            Interests = interests
        };

        await userDal.AddUserAsync(newUser);

        await neo4jUserDal.CreateUserAsync(newUser.Id.ToString(), firstName);

        Console.WriteLine("User added successfully to both MongoDB and Neo4j.");
    }

    static async Task ShowAllUsers(IUserDal userDal)
    {
        var users = await userDal.GetAllUsersAsync();
        if (users.Any())
        {
            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.Id}, First Name: {user.FirstName}, Last Name: {user.LastName}, Email: {user.Email}");
            }
        }
        else
        {
            Console.WriteLine("No users found.");
        }
    }

    static async Task UpdateUser(IUserDal userDal, INeo4jUserDal neo4jUserDal)
    {
        Console.Write("Enter User ID to update: ");
        string userId = Console.ReadLine();

        var user = await userDal.GetUserByIdAsync(new ObjectId(userId));
        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        Console.Write("Enter new first name (leave empty to keep current): ");
        string newFirstName = Console.ReadLine();
        Console.Write("Enter new email (leave empty to keep current): ");
        string newEmail = Console.ReadLine();

        if (!string.IsNullOrEmpty(newFirstName))
        {
            user.FirstName = newFirstName;
        }

        if (!string.IsNullOrEmpty(newEmail))
        {
            user.Email = newEmail;
        }

        await userDal.UpdateUserAsync(user);
        await neo4jUserDal.UpdateUserAsync(user.Id.ToString(), newFirstName, newEmail);

        Console.WriteLine("User updated successfully.");
    }

    static async Task DeleteUser(IUserDal userDal, INeo4jUserDal neo4jUserDal)
    {
        Console.Write("Enter User ID to delete: ");
        string userId = Console.ReadLine();

        var user = await userDal.GetUserByIdAsync(new ObjectId(userId));
        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        await userDal.DeleteUserAsync(new ObjectId(userId));
        await neo4jUserDal.DeleteUserAsync(userId);

        Console.WriteLine("User deleted successfully.");
    }

    static async Task AddPost(IPostDal postDal)
    {
        Console.Write("Enter User ID for the post: ");
        string userId = Console.ReadLine();

        Console.Write("Enter post content: ");
        string content = Console.ReadLine();

        var post = new PostDto
        {
            userId = new ObjectId(userId),
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        await postDal.AddPostAsync(post);
        Console.WriteLine("Post added successfully.");
    }

    static async Task ShowAllPosts(IPostDal postDal)
    {
        var posts = await postDal.GetAllPostsAsync();

        if (posts == null || !posts.Any())
        {
            Console.WriteLine("No posts available.");
            return;
        }

        foreach (var post in posts)
        {
            Console.WriteLine($"Post ID: {post.Id}, User ID: {post.userId}, Content: {post.Content}, Created At: {post.CreatedAt}");
        }
    }

    static async Task AddRelationship(IUserDal userDal, INeo4jUserDal neo4jUserDal, IRelationshipDal relationshipDal)
    {
        Console.Write("Enter User ID 1: ");
        string userId1 = Console.ReadLine();

        Console.Write("Enter User ID 2: ");
        string userId2 = Console.ReadLine();

        Console.WriteLine("Choose relationship type: ");
        Console.WriteLine("1. FRIEND");
        Console.WriteLine("2. FOLLOWER");
        Console.WriteLine("3. SUBSCRIBER");
        string typeChoice = Console.ReadLine();
        string relationshipType = typeChoice switch
        {
            "1" => "FRIEND",
            "2" => "FOLLOWER",
            "3" => "SUBSCRIBER",
            _ => throw new Exception("Invalid choice")
        };

        var relationship = new RelationshipDto
        {
            UserId1 = new ObjectId(userId1),
            UserId2 = new ObjectId(userId2),
            RelationshipType = relationshipType,
            CreatedAt = DateTime.UtcNow
        };

        await relationshipDal.InsertAsync(relationship);
        Console.WriteLine($"Relationship '{relationshipType}' added in MongoDB.");

        await neo4jUserDal.CreateRelationshipAsync(userId1, userId2, relationshipType);
        Console.WriteLine($"Relationship '{relationshipType}' added in Neo4j.");
    }

    static async Task DeleteRelationship(IRelationshipDal relationshipDal, INeo4jUserDal neo4jUserDal)
    {
        Console.Write("Enter User ID 1: ");
        string userId1 = Console.ReadLine();

        Console.Write("Enter User ID 2: ");
        string userId2 = Console.ReadLine();

        Console.WriteLine("Enter Relationship Type (FRIEND, FOLLOWER, SUBSCRIBER): ");
        string relationshipType = Console.ReadLine().ToUpper();

        await relationshipDal.DeleteRelationshipAsync(new ObjectId(userId1), new ObjectId(userId2), relationshipType);
        Console.WriteLine("Relationship deleted from MongoDB.");

        await neo4jUserDal.DeleteRelationshipAsync(userId1, userId2, relationshipType);
        Console.WriteLine($"Relationship '{relationshipType}' deleted from Neo4j.");
    }

    static async Task CheckIfUsersConnected(INeo4jUserDal neo4jUserDal)
    {
        Console.Write("Enter User ID 1: ");
        string userId1 = Console.ReadLine();

        Console.Write("Enter User ID 2: ");
        string userId2 = Console.ReadLine();

        bool isConnected = await neo4jUserDal.AreUsersConnectedAsync(userId1, userId2);
        Console.WriteLine(isConnected ? "Users are connected." : "Users are not connected.");
    }

    static async Task GetDistanceBetweenUsers(INeo4jUserDal neo4jUserDal)
    {
        Console.Write("Enter User ID 1: ");
        string userId1 = Console.ReadLine();

        Console.Write("Enter User ID 2: ");
        string userId2 = Console.ReadLine();

        int distance = await neo4jUserDal.GetDistanceBetweenUsersAsync(userId1, userId2);
        Console.WriteLine($"Distance between users: {distance}");
    }
}
