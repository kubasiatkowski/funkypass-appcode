using System.Net;
using Microsoft.Azure.Services.AppAuthentication;
using System.Configuration;
using System.Data.SqlClient;
//using System.Web.Configuration;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

     var tokenProvider = new AzureServiceTokenProvider();
     string accessToken = await tokenProvider.GetAccessTokenAsync("https://database.windows.net/");
     log.Info($"accessToken: {accessToken}");


    var connStr  = ConfigurationManager.ConnectionStrings["connstring"].ConnectionString;
    log.Info($"connectionString: {connStr}");
    var languages = new List<Language>();

    using (SqlConnection conn = new SqlConnection(connStr))
    {
        conn.AccessToken = accessToken;
        conn.Open();
        var sqlquery = @"SELECT [langcode]
                        ,[langname]
                        ,[maxid]
                        FROM [dbo].[dictionaries]";
    
        SqlCommand cmd = new SqlCommand(sqlquery, conn);
        SqlDataReader reader = cmd.ExecuteReader();
        ; 
        while (reader.Read())
        {
            string sqlread = reader.GetString(0);
            log.Info($"{sqlread}");
            //languages.Add(sqlread);
            Language l = new Language();
            l.langcode = reader.GetString("langname");
            l.langname = reader.GetString("langname");
            l.dictionarysize = reader.GetString("maxid");
            languages.Add(l);
        }         
    }


        //Test t = new Test();
        //t.languages = languages;
        return req.CreateResponse(HttpStatusCode.OK, languages); //"Hello " + name)
}


public class Test
{
    public  List<Language> languages;
}

public class Language
{
    public string langcode;
    public string langname;
    public int dictionarysize;
}