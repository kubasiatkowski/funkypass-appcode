using System.Net;
using Microsoft.Azure.Services.AppAuthentication;
using System.Configuration;
using System.Data.SqlClient;
//using System.Web.Configuration;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    //get access token and connect to SQL
    var tokenProvider = new AzureServiceTokenProvider();
    string accessToken = await tokenProvider.GetAccessTokenAsync("https://database.windows.net/");
    log.Info($"accessToken: {accessToken}");
    var connStr = ConfigurationManager.ConnectionStrings["connstring"].ConnectionString;
    log.Info($"connectionString: {connStr}");
    var languages = new List<Language>();

    //get list of available languages
    using (SqlConnection conn = new SqlConnection(connStr))
    {
        conn.AccessToken = accessToken;
        conn.Open();
        var sqlquery = @"SELECT [langcode]
                        ,[langname]
                        ,[maxid]
                        FROM [dbo].[dictionaries]
                        ORDER BY [langcode]";

        SqlCommand cmd = new SqlCommand(sqlquery, conn);
        SqlDataReader reader = cmd.ExecuteReader();
        ;
        while (reader.Read())
        {
            string sqlread = reader.GetString(0);
            log.Info($"{sqlread}");
            Language l = new Language();
            l.language_code = reader.GetString(0);
            l.language_name = reader.GetString(1);
            l.dictionarysize = reader.GetInt32(2);
            languages.Add(l);
        }
    }


    //return the list
    return req.CreateResponse(HttpStatusCode.OK, languages);
}

public class Language
{
    public string language_code;
    public string language_name;
    public int dictionarysize;
}