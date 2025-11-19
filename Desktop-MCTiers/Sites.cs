using System;
using System.Net.Http;

namespace Desktop_MCTiers;

public class Sites
{
    public static readonly HttpClient MojangApi = new()
    {
        BaseAddress = new Uri("https://api.mojang.com"),
    };
    
    public static readonly HttpClient McTiers = new()
    {
        BaseAddress = new Uri("https://mctiers.com"),
    };
    
    
    public static readonly HttpClient PvpTiers = new()
    {
        BaseAddress = new Uri("https://pvptiers.com"),
    };
    
    public static readonly HttpClient SubTiers = new()
    {
        BaseAddress = new Uri("https://subtiers.net"),
    };
    public static readonly HttpClient McHeads = new()
    {
        BaseAddress = new Uri("https://www.mc-heads.net"),
    };
}