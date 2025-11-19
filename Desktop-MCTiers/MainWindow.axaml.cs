using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Desktop_MCTiers;

using static Search;



public partial class MainWindow : Window
{
    private Search? search;
    public MainWindow()
    {
        InitializeComponent();
    }
    

    public async Task SearchForPlayer()
    {
        String name = SearchBox.Text;
        search = new Search(SearchComboBox.SelectedIndex);
        String? uuid = await search.ReturnUuid(name);
        if (uuid is null)
        {
            SearchError.Text = "No player found";
            SearchProgressBar.Value = 0;
            SearchButton.IsEnabled = true;
            SearchBox.IsEnabled = true;
            return;
        };
        SearchProgressBar.Value = 40;
        Console.WriteLine(uuid);
        Dictionary<String, Tier>? d = await search.ReturnTierLists(uuid);
        if (d is null)
        {
            SearchError.Text = "No player found in tierlist";
            SearchProgressBar.Value = 0;
            SearchButton.IsEnabled = true;
            SearchBox.IsEnabled = true;
            return;
        };
        String? headpath = await search.ReturnHead(uuid);
        if (headpath is not null)
        {
            ResultImage.Source = new Bitmap(headpath);
        }
        SearchProgressBar.Value = 80;
        ArrayList list = new ArrayList();
        
        foreach (KeyValuePair<string, Tier> item in d)
        {
            list.Add(item.Key);
        }

        ResultTop.Text = name + " results";
        SearchResultList.ItemsSource = list;
        SearchProgressBar.Value = 100;
        SearchResultTabs.SelectedIndex = 1;
    }
    

    public void ResultExitButtonClick(object sender, RoutedEventArgs e)
    {
        ResultTop.Text = null;
        SearchButton.IsEnabled = true;
        SearchBox.IsEnabled = true;
        SearchProgressBar.Value = 0;
        SearchResultTabs.SelectedIndex = 0;
        search = null;
    }

    public void SearchButtonClick(object sender, RoutedEventArgs e)
    {
        SearchError.Text = "";
        SearchProgressBar.Value = 0;
        SearchForPlayer();
        SearchButton.IsEnabled = false;
        SearchBox.IsEnabled = false;
    }

    private void SearchResultList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SearchResultList.SelectedValue == null) return;
        Tier tier = search.returnTierFromKey(SearchResultList.SelectedValue.ToString());
        if (tier is null) return;
        char peakHoL;
        char HoL;
        if (tier.pos == 1) HoL = 'L';
        else HoL = 'H';
        if(tier.peak_pos == 1) peakHoL = 'L';
        else peakHoL = 'H';
        ResultFirst.Text = "Tier: " + HoL + "T" + tier.tier;
        ResultSecond.Text = "Peak Tier: " + peakHoL + "T" + tier.peak_tier;
        
    }
}