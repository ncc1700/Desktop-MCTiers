using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static Desktop_MCTiers.Sites;
namespace Desktop_MCTiers;

public class MojangApi
{
    public string? id { get; set; }
    public string? name { get; set; }
}

public class Tier
{
    public int tier { get; set; }
    public int pos { get; set; }
    public int peak_tier { get; set; }
    public int peak_pos { get; set; }
    public long attained { get; set; }
    public bool retired { get; set; }
}
public class TierList
{
    public Dictionary<string, Tier>? root;
}

public class Badges
{
    public string? title { get; set; }
    public string? desc { get; set; }
}

public class Profile
{
    public string? uuid { get; set; }
    public string? name { get; set; }
    public string? region { get; set; }
    public int? points { get; set; }
    public int? overall {get; set;}
    public Dictionary<string, Tier>? rankings { get; set; }
    public List<Badges>? badges { get; set; }
}

public class Search
{
    private Profile? _api;
    private int _tierToSearchFrom;
    
    
    public Search(int tierToSearchFrom)
    {
        _tierToSearchFrom = tierToSearchFrom;
    }

    public async Task<String?> ReturnUuid(String? playerName)
    {
        if (playerName is null) return null;
        HttpResponseMessage mojangUuidSearchResponse;
        
        String mojangUuidSearch = "/users/profiles/minecraft/" + playerName;
        try
        {
            mojangUuidSearchResponse = await Sites.MojangApi.GetAsync(mojangUuidSearch);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
        String json;
        try
        {
            json = await mojangUuidSearchResponse.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            return null;
        }
        Console.WriteLine(json);
        MojangApi? api = JsonSerializer.Deserialize<MojangApi>(json);
        return api?.id;
    }
    
    public async Task<String?> ReturnHead(String uuid)
    {
        HttpResponseMessage headSearchResponse;
        
        String headSearch = "/head/" + uuid;
        try
        {
            headSearchResponse = await McHeads.GetAsync(headSearch);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
        byte[] result;
        try
        {
            result = await headSearchResponse.Content.ReadAsByteArrayAsync();
        }
        catch (Exception e)
        {
            return null;
        }
        Console.WriteLine(result);
        String path = "head.png";
        try
        {
            File.WriteAllBytes(path, result);
        }
        catch
        {
            return null;
        }
        return path;
    }
    
    public async Task<Profile?> ReturnTierLists(String uuid)
    {
        HttpResponseMessage tierListResponse;
        
        String tierSearch = "/api/profile/" + uuid;
        try
        {
            switch (_tierToSearchFrom)
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
        try
        {
            String json = await tierListResponse.Content.ReadAsStringAsync();
            _api = JsonSerializer.Deserialize<Profile?>(json);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
        Console.WriteLine(_api.region);
        return _api;
    }

    public Tier? ReturnTierFromKey(string? key)
    {
        foreach (KeyValuePair<string, Tier> entry in _api.rankings)
        {  
            if(entry.Key == key) return entry.Value;
        }
        return null;
    }
}