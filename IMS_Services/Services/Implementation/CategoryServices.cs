﻿using IMS_Services.Entities;
using IMS_Services.Manager;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IMS_Services.Services.Implementation;

public class CategoryServices : ICRUDServices<Category, byte>
{
    private static DatabaseConnection connection = DatabaseConnection.Instance;

    public static byte Add(Category entity)
    {
        string query = @"
        INSERT INTO tbCategory VALUES 
        (@n, @d);";

        using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
        {
            cmd.Parameters.AddWithValue("@n", entity.Name);
            cmd.Parameters.AddWithValue("@d", entity.Description);
            
            try
            {
                int effected = cmd.ExecuteNonQuery();
                return (byte)effected;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed in adding new Category > {ex.Message}");

            }
        }
    }

    public static bool Delete(byte id)
    {
        string query = "DELETE FROM tbCategory WHERE CategoryID = @id;";

        using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
        {
            cmd.Parameters.AddWithValue("@id", id);
            try
            {
                int effected = cmd.ExecuteNonQuery();
                return effected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed in deleting new Category > {ex.Message}");

            }
        }
    }

    public static IEnumerable<Category> GetAll()
    {
        string query = "SELECT * FROM tbCategory;";

        using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
        {
            SqlDataReader? reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in getting all Categorys > {ex.Message}");
            }

            if (reader != null && reader.HasRows == true)
            {
                var queryable = reader.Cast<IDataRecord>().AsQueryable();
                foreach (var record in queryable)
                {
                    yield return record.ToCategory();
                }
            }
            reader?.Close();

        }
    }

    public static Category GetById(byte id)
    {
        string query = "SELECT * FROM tbCategory WHERE CategoryID = " + id;
        using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
        {
            SqlDataReader? reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in getting Category with ID, {id} > {ex.Message}");
            }

            Category? result = null;
            if (reader != null && reader.HasRows == true)
            {
                if (reader.Read() == true)
                {
                    result = reader.ToCategory();
                }
            }

            reader?.Close();
            return result;

        }
    }

    public static IEnumerable<Category> GetByName(string name)
    {
        string query = "SELECT * FROM tbCategory WHERE CategoryName LIKE '%" + name + "%'";

        using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
        {
            SqlDataReader? reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in getting Category with name, {name} > {ex.Message}");
            }
            if (reader != null && reader.HasRows == true)
            {
                var queryable = reader.Cast<IDataRecord>().AsQueryable();
                foreach (var record in queryable)
                {
                    yield return record.ToCategory();
                }
            }
            reader?.Close();

        }
    }

    public static bool Update(Category entity)
    {
        string query = @"
        UPDATE tbCategory
        SET 
            CategoryName = @n,
            CategoryDesc = @d
           
        WHERE 
            CategoryID = @id;";


        using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
        {
            cmd.Parameters.AddWithValue("@n", entity.Name);
            cmd.Parameters.AddWithValue("@d", entity.Description);
           

            cmd.Parameters.AddWithValue("@id", entity.ID);

            try
            {
                int effected = cmd.ExecuteNonQuery();
                return effected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed in updating new Category > {ex.Message}");

            }
        }
    }
}
