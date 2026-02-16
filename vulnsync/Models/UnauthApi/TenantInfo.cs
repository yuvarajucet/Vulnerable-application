using System.Text.Json.Serialization;

namespace VulnSync.Models;

public class TenantInfo
{
    [JsonPropertyName("tenantName")]
    public string TenantName { get; set; }

    [JsonPropertyName("superAdminName")]
    public string SuperAdminName { get; set; }

    [JsonPropertyName("tenantId")]
    public string TenantId { get; set; }

    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }

    // New properties for connection details - not sent to frontend
    [JsonPropertyName("connectionString")]
    public string ConnectionString { get; set; }

    [JsonPropertyName("databaseName")]
    public string DatabaseName { get; set; }

    [JsonPropertyName("serverUrl")]
    public string ServerUrl { get; set; }
}

// DTO for frontend communication
public class TenantCreateDto
{
    public string TenantName { get; set; }
    public string SuperAdminName { get; set; }
    public string DatabaseName { get; set; }
    public string ServerUrl { get; set; }
} 