### <u>Sql Injection</u>
##### **Description**
Will see the vulernable Sql Injection application.


In this application, the user is presented with a Dashboard Page. On this page, there is a button labeled "Profile Details". When the user clicks on this button, the application retrieves and displays the personal details of the current logged-in user. These details could include information like the user's name, email, or other relevant data associated with their account.

```csharp
public string Profile(string username)
{
    using (var connection = new SqliteConnection(_connectionString))
    {
        connection.Open();
        // Vulnerable SQL query: Directly embedding the user input in the query
        string query = $"SELECT * FROM Users WHERE Username = '{username}'";
        using (var command = new SqliteCommand(query, connection))
        {
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

**Attack Technique:**  
An attacker can use an interception tool, like Burp Suite, to capture the request made when clicking the "Profile Details" button. By injecting malicious SQL code into the request, the attacker can manipulate the database query. This allows unauthorized access to sensitive data, such as user details or even the entire database. SQL injection exploits unvalidated inputs to execute arbitrary SQL commands and extract confidential information.

