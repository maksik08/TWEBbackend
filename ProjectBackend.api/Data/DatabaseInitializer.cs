using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectBackend.api.Models.Domain;
using System.Data;

namespace ProjectBackend.api.Data
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ProjectDbContext _dbContext;
        private readonly AdminSeedOptions _adminSeedOptions;

        public DatabaseInitializer(ProjectDbContext dbContext, IOptions<AdminSeedOptions> adminSeedOptions)
        {
            _dbContext = dbContext;
            _adminSeedOptions = adminSeedOptions.Value;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext.Database.IsRelational())
            {
                await EnsureLegacyMigrationHistoryAsync(cancellationToken);
                await _dbContext.Database.MigrateAsync(cancellationToken);
            }
            else
            {
                await _dbContext.Database.EnsureCreatedAsync(cancellationToken);
            }

            if (string.IsNullOrWhiteSpace(_adminSeedOptions.Email) ||
                string.IsNullOrWhiteSpace(_adminSeedOptions.Username) ||
                string.IsNullOrWhiteSpace(_adminSeedOptions.Password))
            {
                return;
            }

            var adminExists = await _dbContext.Users.AnyAsync(user => user.Role == UserRole.Admin, cancellationToken);
            if (adminExists)
            {
                return;
            }

            var adminUser = new UserDomain
            {
                Email = _adminSeedOptions.Email,
                Username = _adminSeedOptions.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(_adminSeedOptions.Password),
                Role = UserRole.Admin
            };

            await _dbContext.Users.AddAsync(adminUser, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task EnsureLegacyMigrationHistoryAsync(CancellationToken cancellationToken)
        {
            var connection = _dbContext.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;

            if (shouldClose)
            {
                await connection.OpenAsync(cancellationToken);
            }

            try
            {
                await ExecuteSqlAsync(
                    """
                    IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
                    BEGIN
                        CREATE TABLE [__EFMigrationsHistory] (
                            [MigrationId] nvarchar(150) NOT NULL,
                            [ProductVersion] nvarchar(32) NOT NULL,
                            CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                        );
                    END
                    """,
                    cancellationToken);

                await EnsureMigrationHistoryRowAsync(
                    "20260506210828_AddOrders",
                    """
                    OBJECT_ID(N'[Orders]') IS NOT NULL
                    AND OBJECT_ID(N'[OrderItems]') IS NOT NULL
                    """,
                    cancellationToken);

                await EnsureMigrationHistoryRowAsync(
                    "20260506214321_AddUserProfileFields",
                    """
                    COL_LENGTH('Users', 'FirstName') IS NOT NULL
                    AND COL_LENGTH('Users', 'LastName') IS NOT NULL
                    AND COL_LENGTH('Users', 'Phone') IS NOT NULL
                    AND COL_LENGTH('Users', 'Balance') IS NOT NULL
                    """,
                    cancellationToken);

                await EnsureMigrationHistoryRowAsync(
                    "20260507122246_AddRefreshTokens",
                    "OBJECT_ID(N'[RefreshTokens]') IS NOT NULL",
                    cancellationToken);

                await EnsureMigrationHistoryRowAsync(
                    "20260512173820_AddPasswordResetTokens",
                    "OBJECT_ID(N'[PasswordResetTokens]') IS NOT NULL",
                    cancellationToken);

                await EnsureMigrationHistoryRowAsync(
                    "20260512180755_AddContactMessages",
                    "OBJECT_ID(N'[ContactMessages]') IS NOT NULL",
                    cancellationToken);

                await EnsureMigrationHistoryRowAsync(
                    "20260513111115_AddShippingFieldsToOrders",
                    """
                    COL_LENGTH('Orders', 'RecipientName') IS NOT NULL
                    AND COL_LENGTH('Orders', 'Phone') IS NOT NULL
                    AND COL_LENGTH('Orders', 'ShippingAddress') IS NOT NULL
                    AND COL_LENGTH('Orders', 'City') IS NOT NULL
                    AND COL_LENGTH('Orders', 'Comment') IS NOT NULL
                    """,
                    cancellationToken);

                await ExecuteSqlAsync(
                    """
                    IF OBJECT_ID(N'[Products]') IS NOT NULL
                        AND COL_LENGTH('Products', 'StockQuantity') IS NOT NULL
                        AND COL_LENGTH('Products', 'IsPreorder') IS NULL
                    BEGIN
                        ALTER TABLE [Products] ADD [IsPreorder] bit NOT NULL CONSTRAINT [DF_Products_IsPreorder] DEFAULT CAST(0 AS bit);
                    END

                    IF OBJECT_ID(N'[Products]') IS NOT NULL
                        AND COL_LENGTH('Products', 'StockQuantity') IS NOT NULL
                        AND COL_LENGTH('Products', 'RowVersion') IS NULL
                    BEGIN
                        ALTER TABLE [Products] ADD [RowVersion] rowversion NULL;
                    END
                    """,
                    cancellationToken);

                await EnsureMigrationHistoryRowAsync(
                    "20260513121800_AddProductStockAndConcurrency",
                    """
                    COL_LENGTH('Products', 'StockQuantity') IS NOT NULL
                    AND COL_LENGTH('Products', 'IsPreorder') IS NOT NULL
                    AND COL_LENGTH('Products', 'RowVersion') IS NOT NULL
                    """,
                    cancellationToken);

                await EnsureMigrationHistoryRowAsync(
                    "20260516154836_IntegrateManagerInstallerWorkflows",
                    """
                    COL_LENGTH('Products', 'IsVisible') IS NOT NULL
                    AND OBJECT_ID(N'[ActionLogs]') IS NOT NULL
                    AND OBJECT_ID(N'[Notifications]') IS NOT NULL
                    AND OBJECT_ID(N'[ServiceRequests]') IS NOT NULL
                    AND OBJECT_ID(N'[ServiceRequestComments]') IS NOT NULL
                    AND OBJECT_ID(N'[WorkPhotos]') IS NOT NULL
                    """,
                    cancellationToken);
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private Task EnsureMigrationHistoryRowAsync(
            string migrationId,
            string legacyStatePredicate,
            CancellationToken cancellationToken)
        {
            var sql =
                $"""
                 IF {legacyStatePredicate}
                    AND NOT EXISTS (
                        SELECT 1
                        FROM [__EFMigrationsHistory]
                        WHERE [MigrationId] = N'{migrationId}')
                 BEGIN
                    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                    VALUES (N'{migrationId}', N'8.0.26');
                 END
                 """;

            return ExecuteSqlAsync(sql, cancellationToken);
        }

        private async Task ExecuteSqlAsync(string sql, CancellationToken cancellationToken)
        {
            var connection = _dbContext.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
