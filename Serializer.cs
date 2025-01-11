using Arch.Persistence;
using Arch.Core;
using System;
using Microsoft.Data.Sqlite;

namespace Atlantis;

class Serializer
{
    private ArchJsonSerializer jsonSerializer;
    private string connectionString;

    public Serializer()
    {
        this.jsonSerializer = new ArchJsonSerializer();
        this.connectionString = "Data Source=atlantis.db";

        using (var connection = new SqliteConnection(this.connectionString))
        {
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS GameState (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StateJson TEXT NOT NULL,
                    SaveDate TEXT NOT NULL
                );";
            var command = new SqliteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    public string Serialize(World world)
    {
        var state = this.jsonSerializer.ToJson(world);
        return state;
    }

    public void SaveGameState(World world)
    {
        string jsonifiedWorld = this.Serialize(world);
        string saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        using (var connection = new SqliteConnection(this.connectionString))
        {
            connection.Open();
            string insertQuery = @"
                INSERT INTO GameState (StateJson, SaveDate)
                VALUES ($stateJson, $saveDate);";
            var command = new SqliteCommand(insertQuery, connection);
            command.Parameters.AddWithValue("$stateJson", jsonifiedWorld);
            command.Parameters.AddWithValue("$saveDate", saveDate);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public World Deserialize(string state)
    {
        var world = this.jsonSerializer.FromJson(state);
        return world;
    }

    public World LoadGameState()
    {
        using (var connection = new SqliteConnection(this.connectionString))
        {
            connection.Open();
            string selectQuery = @"
                SELECT StateJson
                FROM GameState
                ORDER BY SaveDate DESC
                LIMIT 1;";
            var command = new SqliteCommand(selectQuery, connection);
            var reader = command.ExecuteReader();
            reader.Read();
            string state = reader.GetString(0);
            connection.Close();
            return this.Deserialize(state);
        }
    }
}