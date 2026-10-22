using System.Configuration;
using System.Data.SqlClient;

/// <summary>
/// One place that knows how to open a connection to DojoDB. Every page
/// uses this instead of building its own connection string, so there is
/// exactly one place to change if the connection string ever moves.
/// Requires a "DojoDbConnection" entry in Web.config -- see the
/// connectionStrings snippet provided alongside this file.
/// </summary>
public static class DojoDb
{
    public static SqlConnection GetConnection()
    {
        string connStr = ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;
        return new SqlConnection(connStr);
    }
}