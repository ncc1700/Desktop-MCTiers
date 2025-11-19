using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static Desktop_MCTiers.Sites;
namespace Desktop_MCTiers;


public class PlayerData
{
    public String? name { get; set; }
    public String? region { get; set; }
    public int? points { get; set; }
}

public class OverAll
{
    public List<String>? rankings { get; set; }
    public Dictionary<string, PlayerData>? players { get; set; }
}

public class ListOfTop
{
    public Dictionary<string, PlayerData>? players { get; set; }
}

public abstract class TopPlayers
{
    public static async Task<List<PlayerData>?> GetTopPlayers(int tierToSearchFrom)
    {
        OverAll? overAll;
        HttpResponseMessage tierListResponse;
        
        String tierSearch = "/api/tier/overall?count=50";
        try
        {
            switch (tierToSearchFrom)
            {
                case 1:
                    tierListResponse = await SubTiers.GetAsync(tierSearch);
                    break;
                case 2:
                    tierListResponse  = await PvpTiers.GetAsync(tierSearch);
                    break;
                default:
                    tierListResponse = await McTiers.GetAsync(tierSearch);
                    break;
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
        String json;
        try
        {
            json = await tierListResponse.Content.ReadAsStringAsync();
            overAll = JsonSerializer.Deserialize<OverAll?>(json);

        }
        catch (Exception e)
        {
            return null;
        }
        if(overAll == null || overAll.players == null || overAll.rankings == null) return null;
        if(overAll.players.Count == 0) return  null;

        List<PlayerData>? topPl = [];
        foreach (String? pdata in overAll.rankings)
        {
            foreach (String? uuid in overAll.players.Keys)
            {
                if (uuid == pdata)
                {
                    topPl.Add(overAll.players[uuid]);
                }
            }
        }
        return topPl;
    }
    
}