using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;

namespace TasteKid
{
  public class Info
  {
    public string Name { get; set; }
    public string Type { get; set; }
    public string wTeaser { get; set; }
    public string wUrl { get; set; }
    public string yTitle { get; set; }
    public string yUrl { get; set; }
    public string yID { get; set; }
  }

  public class Result
  {
    public string Name { get; set; }
    public string Type { get; set; }
    public string wTeaser { get; set; }
    public string wUrl { get; set; }
    public string yTitle { get; set; }
    public string yUrl { get; set; }
    public string yID { get; set; }
  }

  public class Similar
  {
    public List<Info> Info { get; set; }
    public List<Result> Results { get; set; }
  }

  public class TasteKidResponse
  {
    public Similar Similar { get; set; }
  }

  public enum TasteKidSearchFilter
  {
    music, movies, shows, books, authors, games
  }

  public class TasteKidQueryItem
  {
    public string ItemType { get; private set; }
    public string Query { get; private set; }

    public TasteKidQueryItem(string itemType, string query)
    {
      this.ItemType = itemType;
      this.Query = query;
    }

    public TasteKidQueryItem(string query)
    {
      this.ItemType = "";
      this.Query = query;
    }

    public string GetValueForUrlQuery()
    {
      if (string.IsNullOrEmpty(this.ItemType))
        return this.Query;
      else
        return this.ItemType + ":" + this.Query;
    }
  }

  public class TasteKidRequestParams
  {
    public List<TasteKidQueryItem> QueryItems { get; private set; }
    public HashSet<TasteKidSearchFilter> Filters { get; private set; }
    public bool Verbose { get; set; }
    
    /// <summary>
    /// TasteKid F Param, part of AppID
    /// </summary>
    public string F_Param { get; set; }

    /// <summary>
    /// TasteKid K_Param, part of AppID
    /// </summary>
    public string K_Param { get; set; }

    public TasteKidRequestParams()
    {
      InitProperties();
      CreateFilter(true);
    }

    public TasteKidRequestParams(bool emptyResultFilter)
    {
      InitProperties();
      CreateFilter(!emptyResultFilter);
    }

    public TasteKidRequestParams(HashSet<TasteKidSearchFilter> filter)
    {
      InitProperties();
      this.Filters = filter;
    }

    private void CreateFilter(bool fillWithItems)
    {
      this.Filters = new HashSet<TasteKidSearchFilter>();
      if (fillWithItems)
      {
        this.Filters.Add(TasteKidSearchFilter.authors);
        this.Filters.Add(TasteKidSearchFilter.books);
        this.Filters.Add(TasteKidSearchFilter.games);
        this.Filters.Add(TasteKidSearchFilter.movies);
        this.Filters.Add(TasteKidSearchFilter.music);
        this.Filters.Add(TasteKidSearchFilter.shows);
      }
    }

    private void InitProperties()
    {
      this.Verbose = false;
      this.QueryItems = new List<TasteKidQueryItem>();
      this.F_Param = "";
      this.K_Param = "";
    }

    #region QueryMethods
    public void AddQuery(string query)
    {
      this.QueryItems.Add(new TasteKidQueryItem(query));
    }

    public void AddBandQuery(string query)
    {
      this.QueryItems.Add(new TasteKidQueryItem("band", query));
    }

    public void AddMovieQuery(string query)
    {
      this.QueryItems.Add(new TasteKidQueryItem("movie", query));
    }

    public void AddShowQuery(string query)
    {
      this.QueryItems.Add(new TasteKidQueryItem("show", query));
    }

    public void AddBookQuery(string query)
    {
      this.QueryItems.Add(new TasteKidQueryItem("book", query));
    }

    public void AddAuthorQuery(string query)
    {
      this.QueryItems.Add(new TasteKidQueryItem("author", query));
    }

    public void AddGameQuery(string query)
    {
      this.QueryItems.Add(new TasteKidQueryItem("game", query));
    }
    #endregion
  }

  public static class TasteKidRequest
  {
    public static TasteKidResponse Search(string query)
    {
      var reqParams = new TasteKidRequestParams();
      reqParams.AddQuery(query);
      return Search(reqParams);
    }

    public static TasteKidResponse Search(string query, TasteKidSearchFilter filter)
    {
      var reqParams = new TasteKidRequestParams(true);
      reqParams.AddQuery(query);
      reqParams.Filters.Add(filter);
      return Search(reqParams);
    }

    public static TasteKidResponse Search(string query, HashSet<TasteKidSearchFilter> filterSet)
    {
      var reqParams = new TasteKidRequestParams(filterSet);
      reqParams.AddQuery(query);
      return Search(reqParams);
    }

    public static TasteKidResponse Search(TasteKidRequestParams reqParams)
    {
      WebClient wc = new WebClient();
      string resp = wc.DownloadString(GenerateUri(reqParams));
      return JsonDeserialize<TasteKidResponse>(resp);
    }

    private static string GenerateUri(TasteKidRequestParams reqParams)
    {
      var url = "http://www.tastekid.com/ask/ws?";

      /// get queries
      string query = "";
      foreach (var item in reqParams.QueryItems)
      {
        if (!string.IsNullOrEmpty(query))
          query += ",";
        query += item.GetValueForUrlQuery();
      }

      /// result filter goes into query, if all items are in filter we don't have to set it
      int allFiltersCount = Enum.GetNames(typeof(TasteKidSearchFilter)).ToList().Count;
      if (reqParams.Filters.Count < allFiltersCount)
      {
        foreach (var flt in reqParams.Filters)
        {
          query += "//" + flt.ToString();
        }
      }

      url += "q=" + WebUtility.UrlEncode(query);

      /// app id 
      if (!string.IsNullOrEmpty(reqParams.K_Param) && !string.IsNullOrEmpty(reqParams.F_Param))
      {
        url += "&f=" + reqParams.F_Param;
        url += "&k=" + reqParams.K_Param;
      }

      url += "&verbose=" + (reqParams.Verbose ? "1" : "0");
      url += "&format=JSON";
      return url;
    }

    private static T JsonDeserialize<T>(string jsonString)
    {
      DataContractJsonSerializer ser =  new DataContractJsonSerializer(typeof(T));
      MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
      T obj = (T)ser.ReadObject(ms);
      return obj;
    }
  }
}
