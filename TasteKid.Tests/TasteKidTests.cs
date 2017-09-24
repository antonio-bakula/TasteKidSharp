using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace TasteKid.Tests
{
  [TestClass]
  public class TasteKidTests
  {

    [TestMethod]
    public void TestBasicQuery()
    {
      var resp = TasteKidRequest.Search("daft punk");
      Assert.IsNotNull(resp);
      Assert.IsNotNull(resp.Similar.Info.FirstOrDefault(ii => ii.Name == "Daft Punk" && ii.Type == "music"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "Justice" && rr.Type == "music"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "The Chemical Brothers" && rr.Type == "music"));
    }

    [TestMethod]
    public void TestQueryItems()
    {
      var reqParams = new TasteKidRequestParams();
      reqParams.AddQuery("daft punk");
      reqParams.AddQuery("pulp fiction");
      var resp = TasteKidRequest.Search(reqParams);
      Assert.IsNotNull(resp);
      Assert.IsNotNull(resp.Similar.Info.FirstOrDefault(ii => ii.Name == "Daft Punk" && ii.Type == "music"));
      Assert.IsNotNull(resp.Similar.Info.FirstOrDefault(ii => ii.Name == "Pulp Fiction" && ii.Type == "movie"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "Justice" && rr.Type == "music"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "The Chemical Brothers" && rr.Type == "music"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "Jackie Brown" && rr.Type == "movie"));
    }

    [TestMethod]
    public void TestQueryMusicItems()
    {
      var resp = TasteKidRequest.Search("daft punk", TasteKidSearchFilter.music);
      Assert.IsNotNull(resp);
      Assert.IsNotNull(resp.Similar.Info.FirstOrDefault(ii => ii.Name == "Daft Punk" && ii.Type == "music"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "Justice" && rr.Type == "music"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "The Chemical Brothers" && rr.Type == "music"));
    }

    [TestMethod]
    public void TestBandsQuery()
    {
      var reqParams = new TasteKidRequestParams();
      reqParams.AddBandQuery("the beatles");
      var resp = TasteKidRequest.Search(reqParams);
      Assert.IsNotNull(resp);
      Assert.IsNotNull(resp.Similar.Info.FirstOrDefault(ii => ii.Name == "The Beatles" && ii.Type == "music"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "The Rolling Stones" && rr.Type == "music"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "A Hard Day's Night" && rr.Type == "movie"));
    }

    [TestMethod]
    public void TestQueryForMusicOnly()
    {
      var resp = TasteKidRequest.Search("the beatles", TasteKidSearchFilter.music);
      Assert.IsNotNull(resp);
      Assert.IsNotNull(resp.Similar.Info.FirstOrDefault(ii => ii.Name == "The Beatles" && ii.Type == "music"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "The Rolling Stones" && rr.Type == "music"));
      Assert.IsNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "A Hard Day's Night" && rr.Type == "movie"));
    }

    [TestMethod]
    public void TestQueryWithAppIdentification()
    {
      var reqParams = new TasteKidRequestParams();
      reqParams.AddBandQuery("the beatles");
      /// put here your app ID data here, don't wont to share mine with the world :)
      reqParams.K_Param = "";
      reqParams.F_Param = "";
      var resp = TasteKidRequest.Search(reqParams);
      Assert.IsNotNull(resp);
      Assert.IsNotNull(resp.Similar.Info.FirstOrDefault(ii => ii.Name == "The Beatles" && ii.Type == "music"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "The Rolling Stones" && rr.Type == "music"));
      Assert.IsNotNull(resp.Similar.Results.FirstOrDefault(rr => rr.Name == "A Hard Day's Night" && rr.Type == "movie"));
    }

  }
}
