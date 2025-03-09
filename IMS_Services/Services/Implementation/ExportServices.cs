﻿using IMS_Services.Manager;
using Microsoft.Data.SqlClient;
using IMS_Services.Entities;
using System.Data;

namespace IMS_Services.Services.Implementation;

public class ExportServices : ICRUDServices<Export, int>
{

    private static DatabaseConnection connection = DatabaseConnection.Instance;

    public static int Add(Export entity)
    {
        string query = @"
        INSERT INTO tbExport VALUES 
        (@d, @ti, @tc, @h);";

        using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
        {
            cmd.Parameters.AddWithValue("@d", entity.ExportDate);
            cmd.Parameters.AddWithValue("@ti", entity.TotalItem);
            cmd.Parameters.AddWithValue("@tc", entity.TotalCost);
            cmd.Parameters.AddWithValue("@h", entity.HandledBy);
            try
            {
                int effected = cmd.ExecuteNonQuery();
                return effected;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed in adding new export > {ex.Message}");

            }
        }
    }

    public static bool Delete(int id)
    {
        string query = "DELETE FROM tbExport WHERE ExportID = @id;";

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
                throw new Exception($"Failed in deleting export > {ex.Message}");
            }
        }
    }

    public static IEnumerable<Export> GetAll()
    {
        string query = "SELECT * FROM tbExport;";

        using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
        {
            SqlDataReader? reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in getting all Export > {ex.Message}");
            }

            if (reader != null && reader.HasRows == true)
            {
                var queryable = reader.Cast<IDataRecord>().AsQueryable();
                foreach (var record in queryable)
                {
                    yield return record.ToExport();
                }
            }
            reader?.Close();
        }
    }

    public static Export GetById(int id)
    {
        string query = "SELECT * FROM tbExport WHERE ExportID = " + id;
        using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
        {

            SqlDataReader? reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in getting Export with ID, {id} > {ex.Message}");
            }

            Export? result = null;
            if (reader != null && reader.HasRows == true)
            {
                if (reader.Read() == true)
                {
                    result = reader.ToExport();
                }
            }
            reader?.Close();
            return result;
        }
    }

    public static IEnumerable<Export> GetByName(string name)
    {
        throw new NotImplementedException();
    }

    public static bool Update(Export entity)
    {
        string query = @"
        UPDATE tbExport
        SET 
            ExportDate = @d,
            TotalItems = @ti,
            TotalCost = @tc,
            HandledBy = @h
        WHERE 
            ExportID = @id;";

        using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
        {
            cmd.Parameters.AddWithValue("@d", entity.ExportDate);
            cmd.Parameters.AddWithValue("@ti", entity.TotalItem);
            cmd.Parameters.AddWithValue("@tc", entity.TotalCost);
            cmd.Parameters.AddWithValue("@h", entity.HandledBy);
            cmd.Parameters.AddWithValue("@id", entity.ID);

            try
            {
                int effected = cmd.ExecuteNonQuery();
                return effected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed in updating new Export > {ex.Message}");

            }
        }
    }
}
