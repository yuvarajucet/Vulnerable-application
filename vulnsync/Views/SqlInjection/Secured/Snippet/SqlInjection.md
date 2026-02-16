### <u>Sql Injection</u>
##### **Description**
Will see the secured Sql Injection application.


In this application, the user is presented with a Dashboard Page. On this page, there is a button labeled "Profile Details". When the user clicks on this button, the application retrieves and displays the personal details of the current logged-in user. These details could include information like the user's name, email, or other relevant data associated with their account.

```csharp
public string Profile(string username)
{
    using (var connection = new SqliteConnection(_connectionString))
    {
        connection.Open();
        string query = "SELECT * FROM Users WHERE Username = @Username";
        using (var command = new SqliteCommand(query, connection))
        {
            // Add the parameter to the command to safely include the username
            command.Parameters.AddWithValue("@Username", username);
            using (var adapter = new SqliteDataAdapter(command))
            {
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                // Convert DataTable to JSON and return
                return JsonConvert.SerializeObject(dataTable);
            }
        }
    }
}

``````
<br/>

**Mitigation:**  
Use **parameterized queries** to prevent SQL injection by ensuring user input is treated as data rather than part of the SQL command. Avoid string concatenation in queries, as it allows attackers to manipulate SQL statements.
