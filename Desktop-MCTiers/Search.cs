using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Desktop_MCTiers;
public class MojangAPI
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
    public int attained { get; set; }
    public bool retired { get; set; }
}
public class TierList
{
    public Dictionary<string, Tier>? root;
}
public class Search
{
    private Dictionary<string, Tier>? api;
    private int _tierToSearchFrom;
    private static HttpClient _mojangApi = new()
    {
        BaseAddress = new Uri("https://api.mojang.com"),
    };
    
    private static HttpClient _mctiers = new()
    {
        BaseAddress = new Uri("https://mctiers.com"),
    };
    
    
    private static HttpClient _pvptiers = new()
    {
        BaseAddress = new Uri("https://pvptiers.com"),
    };
    
    private static HttpClient _subtiers = new()
    {
        BaseAddress = new Uri("https://subtiers.net"),
    };
    private static HttpClient _mcheads = new()
    {
        BaseAddress = new Uri("https://www.mc-heads.net"),
    };
    
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
            mojangUuidSearchResponse = await _mojangApi.GetAsync(mojangUuidSearch);
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
        MojangAPI? api = JsonSerializer.Deserialize<MojangAPI>(json);
        return api?.id;
    }
    
    public async Task<String?> ReturnHead(String uuid)
    {
        HttpResponseMessage headSearchResponse;
        
        String headSearch = "/head/" + uuid;
        try
        {
            headSearchResponse = await _mcheads.GetAsync(headSearch);
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
    
    public async Task<Dictionary<string, Tier>?> ReturnTierLists(String uuid)
    {
        HttpResponseMessage tierListResponse;
        
        String mojangUuidSearch = "/api/rankings/" + uuid;
        try
        {
            switch (_tierToSearchFrom)
            {
                case 1:
                    tierListResponse = await _subtiers.GetAsync(mojangUuidSearch);
                    break;
                case 2:
                    tierListResponse  = await _pvptiers.GetAsync(mojangUuidSearch);
                    break;
                default:
                    tierListResponse = await _mctiers.GetAsync(mojangUuidSearch);
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
            api = JsonSerializer.Deserialize<Dictionary<string, Tier>>(json);

        }
        catch (Exception e)
        {
            return null;
        }
        return api;
    }

    public Tier? returnTierFromKey(String key)
    {
        foreach (KeyValuePair<string, Tier> entry in api)
        {  
            if(entry.Key == key) return entry.Value;
        }
        return null;
    }
}