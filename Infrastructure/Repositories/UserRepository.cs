using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using TaskCircle.AuthentcationApi.Infrastructure.Repositories.Iterfaces;
using TaskCircle.AuthentcationApi.Infrastructure.Setting;
using TaskCircle.AuthentcationApi.Models;

namespace TaskCircle.AuthentcationApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{

    private readonly ConnectionSetting? _connection;

    public UserRepository(IOptions<ConnectionSetting> connection)
    {
        _connection = connection.Value;
    }

    public async Task<User> Register(User Entity)
    {
        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("CALL registerUser(@email, @password, @salt)", connect))
            {
                cmd.Parameters.AddWithValue("email", Entity.Email);
                cmd.Parameters.AddWithValue("password", Entity.PasswordHash);
                cmd.Parameters.AddWithValue("salt", Entity.PasswordSalt);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        return Entity;
    }

    public async Task<User> RefreshToken(User Entity)
    {
        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();

            using (var cmd = new NpgsqlCommand("CALL refreshUserToken(@id, @refreshToken, @tokenCreated, @tokenExpires)", connect))
            {
                cmd.Parameters.AddWithValue("id", Entity.IdUser);
                cmd.Parameters.AddWithValue("refreshToken", Entity.RefreshToken);
                cmd.Parameters.AddWithValue("tokenCreated", Entity.TokenCreated);
                cmd.Parameters.AddWithValue("tokenExpires", Entity.TokenExpires);

                await cmd.ExecuteNonQueryAsync();
            }
        }
        return Entity;
    }

    public async Task<User> Delete(int id)
    {
        User user = await GetUserById(id);

        if (user != null)
        {
            using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
            {
                await connect.OpenAsync();
                using (var cmd = new NpgsqlCommand("CALL deleteUser(@id)", connect))
                {
                    cmd.Parameters.AddWithValue("id", id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        return user;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        User user = null;

        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();

            using (var cmd = new NpgsqlCommand("SELECT * FROM getUserByEmail(@email)", connect))
            {
                cmd.Parameters.AddWithValue("email", email);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            IdUser = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            PasswordHash = (byte[])reader[2],
                            PasswordSalt = (byte[])reader[3]
                        };
                    }
                }
            }
        }

        return user;
    }

    public async Task<User> GetUserById(int id)
    {
        User user = null;

        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("SELECT * FROM getUserById(@id)", connect))
            {
                cmd.Parameters.AddWithValue("id", id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            IdUser = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            PasswordHash = (byte[])reader[2],
                            PasswordSalt = (byte[])reader[3],
                            RefreshToken = reader.GetString(4),
                            TokenCreated = reader.GetDateTime(5),
                            TokenExpires = reader.GetDateTime(6)
                        };
                    }
                }
            }
        }

        return user;
    }

    public async Task<User> Update(User Entity)
    {
        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("CALL updateEmail(@id, @email)", connect))
            {
                cmd.Parameters.AddWithValue("id", Entity.IdUser);
                cmd.Parameters.AddWithValue("email", Entity.Email);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        return Entity;
    }

    public async Task<User> UpdatePassword(User Entity)
    {
        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("CALL changePassword(@id, @password, @salt)", connect))
            {
                cmd.Parameters.AddWithValue("id", Entity.IdUser);
                cmd.Parameters.AddWithValue("password", Entity.PasswordHash);
                cmd.Parameters.AddWithValue("salt", Entity.PasswordSalt);

                await cmd.ExecuteNonQueryAsync();
            }

        }
        return Entity;
    }

    public async Task RemoveRefreshToken(User Entity)
    {
        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connect;
                cmd.CommandText = @"UPDATE ""USER"" SET RefreshToken = NULL WHERE Id = @id";
                cmd.Parameters.AddWithValue("id", Entity.IdUser);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}