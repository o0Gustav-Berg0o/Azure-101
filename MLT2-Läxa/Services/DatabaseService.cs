using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MLT2_Läxa.Models;
using Microsoft.Data.SqlClient;
using System.Text;

namespace MLT2_Läxa.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Sparar metadata om en uppladdad fil
        /// </summary>
        public async Task<int> SaveFileRecordAsync(string fileName, string contentType, string blobUrl, long fileSize)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
            INSERT INTO Files (FileName, ContentType, BlobUrl, FileSize)
            OUTPUT INSERTED.Id
            VALUES (@FileName, @ContentType, @BlobUrl, @FileSize)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FileName", fileName);
            command.Parameters.AddWithValue("@ContentType", contentType);
            command.Parameters.AddWithValue("@BlobUrl", blobUrl);
            command.Parameters.AddWithValue("@FileSize", fileSize);

            // Returnerar det nya Id:t
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Hämtar alla filer från databasen
        /// </summary>
        public async Task<List<FileRecord>> GetAllFilesAsync()
        {
            var files = new List<FileRecord>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT Id, FileName, ContentType, BlobUrl, FileSize, UploadedAt FROM Files ORDER BY UploadedAt DESC";

            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                files.Add(new FileRecord
                {
                    Id = reader.GetInt32(0),
                    FileName = reader.GetString(1),
                    ContentType = reader.IsDBNull(2) ? null : reader.GetString(2),
                    BlobUrl = reader.GetString(3),
                    FileSize = reader.GetInt64(4),
                    UploadedAt = reader.GetDateTime(5)
                });
            }

            return files;
        }

        /// <summary>
        /// Hämtar en specifik fil baserat på Id
        /// </summary>
        public async Task<FileRecord?> GetFileByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT Id, FileName, ContentType, BlobUrl, FileSize, UploadedAt FROM Files WHERE Id = @Id";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new FileRecord
                {
                    Id = reader.GetInt32(0),
                    FileName = reader.GetString(1),
                    ContentType = reader.IsDBNull(2) ? null : reader.GetString(2),
                    BlobUrl = reader.GetString(3),
                    FileSize = reader.GetInt64(4),
                    UploadedAt = reader.GetDateTime(5)
                };
            }

            return null;
        }

        /// <summary>
        /// Tar bort en fil-post från databasen
        /// </summary>
        public async Task<bool> DeleteFileRecordAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM Files WHERE Id = @Id";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        /// <summary>
        /// Testar anslutningen till databasen
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }