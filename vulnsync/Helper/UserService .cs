using System;
using System.Data;
using System.Text;
using Microsoft.Data.Sqlite;
using VulnSync.Models.IDOR;
public class UserService
{
    private string _connectionString;

    public UserService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public User AuthenticateUser(string username, string password)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            string query = "SELECT Id, Username, Email FROM Users WHERE Username = @Username AND Password = @Password";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                var base64EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
                command.Parameters.AddWithValue("@Password", base64EncodedPassword);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2)
                        };
                    }
                    return null;
                }
            }
        }
    }

    public User GetUserDetails(int userId)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT Id, Username, Email, Password, FullName, DateOfBirth, PhoneNumber, Address FROM Users WHERE Id = @UserId";

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2),
                            Password = reader.GetString(3),
                            FullName = reader.GetString(4),
                            DateOfBirth = reader.GetDateTime(5),
                            PhoneNumber = reader.GetString(6),
                            Address = reader.GetString(7)
                        };
                    }
                    return null;
                }
            }
        }
    }
    
    //sql injection vulernable.
    public List<User> Profile(string username)
    {
        List<User> users = new List<User>();  // List to hold all users

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            // The SQL Injection to select all rows from Users table
            string query = $"SELECT * FROM Users WHERE Username = '{username}'"; // SQL Injection vulnerability

            using (var command = new SqliteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())  // Loop through all rows
                    {
                        string base64Password = reader.GetString(3);  // Assuming password is in the 4th column (index 3)

                        // Convert Base64 back to byte array
                        byte[] passwordBytes = Convert.FromBase64String(base64Password);

                        // Convert byte array back to original string using UTF-8
                        string decryptedPassword = Encoding.UTF8.GetString(passwordBytes);

                        // Add each user to the list
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2),
                            Password = decryptedPassword  // Decrypted password for each user
                        });
                    }
                }
            }
        }
        return users;  // Return the entire list of users
    }
    
    public List<User> SecureProfile(string username)
    {
        List<User> users = new List<User>();  // List to hold all users

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            // Secure query using parameterized query to prevent SQL Injection
            string query = "SELECT * FROM Users WHERE Username = @Username";

            using (var command = new SqliteCommand(query, connection))
            {
                // Add parameterized value to prevent SQL injection
                command.Parameters.AddWithValue("@Username", username);

                using (var reader = command.ExecuteReader())
                {
                   
                    while (reader.Read())  // Loop through all rows
                    {
                        string base64Password = reader.GetString(3);  // Assuming password is in the 4th column (index 3)

                        // Convert Base64 back to byte array
                        byte[] passwordBytes = Convert.FromBase64String(base64Password);

                        // Convert byte array back to original string using UTF-8
                        string decryptedPassword = Encoding.UTF8.GetString(passwordBytes);
                        
                        // Add each user to the list (No password decryption)
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2),
                            Password = decryptedPassword  // Store the password directly
                        });
                    }
                }
            }
        }
        return users;
    }


    public bool UpdatePassword(int userId, string newPassword, string confirmPassword, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (newPassword != confirmPassword)
        {
            errorMessage = "Passwords do not match.";
            return false;
        }

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            string query = "UPDATE Users SET Password = @Password WHERE Id = @UserId";

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                var base64EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(newPassword));
                command.Parameters.AddWithValue("@Password", base64EncodedPassword);
                command.ExecuteNonQuery();
            }
        }
        
        return true;
    }

    public void SeedUsers()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            
            var tableCheckQuery = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY, 
                    Username TEXT, 
                    Email TEXT, 
                    Password TEXT, 
                    FullName TEXT, 
                    DateOfBirth TEXT, 
                    PhoneNumber TEXT, 
                    Address TEXT,
                    Token TEXT
                )";
            
            using (var command = new SqliteCommand(tableCheckQuery, connection))
            {
                command.ExecuteNonQuery();
            }
            
            var insertQuery = @"
                INSERT INTO Users (Username, Email, Password, FullName, DateOfBirth, PhoneNumber, Address, Token) 
                SELECT @Username, @Email, @Password, @FullName, @DateOfBirth, @PhoneNumber, @Address, @Token 
                WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username = @Username)";
            
            var users = new[]
            {
                new { Username = "admin", Email="admin@gmail.com", Password = "YWRtaW4xMjM=", FullName = "Admin User", DateOfBirth = "1980-12-31", PhoneNumber = "5555555555", Address = "789 Pine St", Token = "" },
                new { Username = "user", Email="user@gmail.com", Password = "dXNlcjEyMw==", FullName = "John Doe", DateOfBirth = "1992-07-15", PhoneNumber = "5551234567", Address = "321 Maple Ave", Token = "" },
                new { Username = "praveen", Email="praveen@gmail.com", Password = "UGFzc3dvcmQ=", FullName = "Praveen Kumar", DateOfBirth = "1990-01-01", PhoneNumber = "1234567890", Address = "123 Main St", Token = "" },
                new { Username = "demo", Email="demo@gmail.com", Password = "UGFzc3dvcmQ=", FullName = "Demo User", DateOfBirth = "1985-05-10", PhoneNumber = "0987654321", Address = "456 Elm St", Token = "" },
                new { Username = "test", Email="test@gmail.com", Password = "UGFzc3dvcmQ=", FullName = "Test User", DateOfBirth = "1995-09-15", PhoneNumber = "1122334455", Address = "789 Oak St", Token = "" }
            };

            foreach (var user in users)
            {
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                    command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                    command.Parameters.AddWithValue("@Address", user.Address);
                    command.Parameters.AddWithValue("@Token", user.Token);
                    
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public void SeedUserInvites()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var tableCheckQuery = @"
                CREATE TABLE IF NOT EXISTS UserInvites (
                    Id INTEGER PRIMARY KEY, 
                    Username TEXT, 
                    Email TEXT
                )";
            
            using (var command = new SqliteCommand(tableCheckQuery, connection))
            {
                command.ExecuteNonQuery();
            }
            
            var insertQuery = @"
                INSERT INTO UserInvites (Username, Email) 
                SELECT @Username, @Email 
                WHERE NOT EXISTS (SELECT 1 FROM UserInvites WHERE Username = @Username)";
            
            var userInvites = new[]
            {
                new { Username = "user1", Email="user1@gmail.com" },
                new { Username = "user2", Email="user2@gmail.com" },
                new { Username = "user3", Email="user3@gmail.com" }
            };

            foreach (var userInvite in userInvites)
            {
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", userInvite.Username);
                    command.Parameters.AddWithValue("@Email", userInvite.Email);
                    
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
