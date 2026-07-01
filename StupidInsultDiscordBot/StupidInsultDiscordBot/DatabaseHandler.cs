using Microsoft.Data.Sqlite;

namespace StupidInsultDiscordBot
{
    public static class DatabaseHandler
    {
        public static string DB_PATH = $"Data Source={Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName}\\bot.db";

        public static void Initialize()
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS messages (
                    id       INTEGER PRIMARY KEY AUTOINCREMENT,
                    name          TEXT,
                    message       TEXT
                );

                CREATE TABLE IF NOT EXISTS blockedusers (
                    id       INTEGER PRIMARY KEY AUTOINCREMENT,
                    name          TEXT
                );
                """;
            cmd.ExecuteNonQuery();
        }

        public static void LogMessage(string name, string message)
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                INSERT INTO messages (name, message)
                VALUES ($name, $message)
            """;
            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$message", message);
            cmd.ExecuteNonQuery();
        }

        public static List<string> GetUserMessages(string name, int limit = 50)
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                SELECT message FROM messages
                WHERE name = $name
                ORDER BY ROWID DESC
                LIMIT $limit
            """;
            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$limit", limit);
            var reader = cmd.ExecuteReader();
            var messages = new List<string>();
            while (reader.Read() && limit > 0)
            {
                messages.Add(reader["message"].ToString());
                limit--;
            }
            return messages;
        }

        public static void ForgetUserMessages(string name)
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                    DELETE FROM messages
                    WHERE name = $name
                """;
            cmd.Parameters.AddWithValue("$name", name);
            cmd.ExecuteNonQuery();
        }

        public static void BlockUser(string name)
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                    INSERT INTO blockedusers (name)
                    VALUES ($name)
                """;
            cmd.Parameters.AddWithValue("$name", name);
            cmd.ExecuteNonQuery();
        }

        public static bool IsUserBlocked(string name)
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                    SELECT COUNT(*) FROM blockedusers
                    WHERE name = $name
                """;
            cmd.Parameters.AddWithValue("$name", name);
            var count = (long)cmd.ExecuteScalar();
            return count > 0;
        }

        public static void unblockUser(string name)
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                    DELETE FROM blockedusers
                    WHERE name = $name
                """;
            cmd.Parameters.AddWithValue("$name", name);
            cmd.ExecuteNonQuery();
        }
    }
}
