using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Desktop_MCTiers;

public class MojangAPI
{
    public string? id { get; set; }
    public string? name { get; set; }
}

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
    }
    private static HttpClient mojangapi = new()
    {
        BaseAddress = new Uri("https://api.mojang.com"),
    };
    
    private static HttpClient mctiers = new()
    {
        BaseAddress = new Uri("https://mctiers.com"),
    };
    
    
    private static HttpClient pvptiers = new()
    {
        BaseAddress = new Uri("https://pvptiers.com"),
    };
    
    private static HttpClient subtiers = new()
    {
        BaseAddress = new Uri("https://subtiers.com"),
    };

    private async Task<String?> ReturnUuid(String? playerName)
    {
        if (playerName is null) return null;
        HttpResponseMessage mojangUuidSearchResponse;
        
        String mojangUuidSearch = "/users/profiles/minecraft/" + SearchBox.Text;
        try
        {
            mojangUuidSearchResponse = await mojangapi.GetAsync(mojangUuidSearch);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
        SearchProgressBar.Value += 10;
        
        var json = await mojangUuidSearchResponse.Content.ReadAsStringAsync();
        SearchProgressBar.Value += 10;

        Console.WriteLine(json);
        MojangAPI? api = JsonSerializer.Deserialize<MojangAPI>(json);
        return api?.id;
    }

    public async void SearchForPlayer()
    {
        String? uuid = await ReturnUuid(SearchBox.Text);
        if (uuid is null)
        {
            SearchButton.IsEnabled = true;
            SearchBox.IsEnabled = true;
            return;
        };
        Console.WriteLine(uuid);
        SearchButton.IsEnabled = true;
        SearchBox.IsEnabled = true;
    }

    public void SearchButtonClick(object sender, RoutedEventArgs e)
    {
        SearchProgressBar.Value = 0;
        SearchForPlayer();
        SearchButton.IsEnabled = false;
        SearchBox.IsEnabled = false;
    }
}