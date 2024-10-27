using Neo4j.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Neo4jUserDal : INeo4jUserDal
{
    private readonly IDriver _driver;

    public Neo4jUserDal(IDriver driver)
    {
        _driver = driver;
    }

    private async Task ExecuteQueryAsync(string query)
    {
        using var session = _driver.AsyncSession();
        await session.RunAsync(query);
    }

    public async Task CreateUserAsync(string userId, string name)
    {
        var query = $"CREATE (n:User {{id: '{userId}', name: '{name}'}})";
        await ExecuteQueryAsync(query);
    }

    public async Task DeleteUserAsync(string userId)
    {
        var query = $"MATCH (n:User {{id: '{userId}'}}) DETACH DELETE n";
        await ExecuteQueryAsync(query);
    }

    public async Task CreateRelationshipAsync(string userId1, string userId2, string relationshipType)
    {
        var query = $"MATCH (a:User {{id: '{userId1}'}}), (b:User {{id: '{userId2}'}}) " +
                    $"CREATE (a)-[:{relationshipType}]->(b)";
        await ExecuteQueryAsync(query);
    }

    public async Task DeleteRelationshipAsync(string userId1, string userId2, string relationshipType)
    {
        var query = $"MATCH (a:User {{id: '{userId1}'}})-[r:{relationshipType}]->(b:User {{id: '{userId2}'}}) DELETE r";
        await ExecuteQueryAsync(query);
    }

    public async Task UpdateUserAsync(string userId, string newFirstName, string newEmail)
    {
        var query = $"MATCH (u:User {{id: '{userId}'}}) SET u.name = '{newFirstName}', u.email = '{newEmail}'";
        await ExecuteQueryAsync(query);
    }

    public async Task<bool> AreUsersConnectedAsync(string userId1, string userId2)
    {
        var query = $"MATCH (a:User {{id: '{userId1}'}})-[r]->(b:User {{id: '{userId2}'}}) RETURN count(r) > 0";
        using var session = _driver.AsyncSession();
        var result = await session.RunAsync(query);
        var records = await result.ToListAsync();
        return records.Any(r => r[0].As<bool>());
    }

    public async Task<int> GetDistanceBetweenUsersAsync(string userId1, string userId2)
    {
        var query = @"
        MATCH (u1:User {id: $userId1})-[:FRIEND|FOLLOW|SUBSCRIBE*]-(u2:User {id: $userId2})
        RETURN length(shortestPath((u1)-[:FRIEND|FOLLOW|SUBSCRIBE*]-(u2))) AS distance";

        using var session = _driver.AsyncSession();
        var result = await session.RunAsync(query, new { userId1, userId2 });
        var records = await result.ToListAsync();

        if (records.Any())
        {
            return records[0]["distance"].As<int>();
        }

        return -1;
    }

    public async Task CreateFriendAsync(string userId1, string userId2)
    {
        string query = $"MATCH (u1:User {{id: '{userId1}'}}), (u2:User {{id: '{userId2}'}}) " +
                       "MERGE (u1)-[:FRIEND]->(u2)";
        await ExecuteQueryAsync(query);
    }

    public async Task DeleteFriendAsync(string userId1, string userId2)
    {
        string query = $"MATCH (u1:User {{id: '{userId1}'}})-[r:FRIEND]->(u2:User {{id: '{userId2}'}}) " +
                       "DELETE r";
        await ExecuteQueryAsync(query);
    }

    public async Task CreateFollowerAsync(string userId1, string userId2)
    {
        string query = $"MATCH (u1:User {{id: '{userId1}'}}), (u2:User {{id: '{userId2}'}}) " +
                       "MERGE (u1)-[:FOLLOWER]->(u2)";
        await ExecuteQueryAsync(query);
    }

    public async Task DeleteFollowerAsync(string userId1, string userId2)
    {
        string query = $"MATCH (u1:User {{id: '{userId1}'}})-[r:FOLLOWER]->(u2:User {{id: '{userId2}'}}) " +
                       "DELETE r";
        await ExecuteQueryAsync(query);
    }

    public async Task CreateSubscriberAsync(string userId1, string userId2)
    {
        string query = $"MATCH (u1:User {{id: '{userId1}'}}), (u2:User {{id: '{userId2}'}}) " +
                       "MERGE (u1)-[:SUBSCRIBER]->(u2)";
        await ExecuteQueryAsync(query);
    }

    public async Task DeleteSubscriberAsync(string userId1, string userId2)
    {
        string query = $"MATCH (u1:User {{id: '{userId1}'}})-[r:SUBSCRIBER]->(u2:User {{id: '{userId2}'}}) " +
                       "DELETE r";
        await ExecuteQueryAsync(query);
    }

    public async Task UpdateUserNameAsync(string userId, string newUserName)
    {
        var query = $"MATCH (u:User {{id: '{userId}'}}) SET u.name = '{newUserName}'";
        await ExecuteQueryAsync(query);
    }
}