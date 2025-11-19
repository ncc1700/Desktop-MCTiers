using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;

namespace Desktop_MCTiers;

using static TopPlayers;


public partial class MainWindow : Window
{
    private readonly ArrayList _list = new ArrayList();
    private Search? _search;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ResetSearchState()
    {
        SearchProgressBar.IsVisible = false;
        ResultPoints.Text = null;
        ResultName.Text = null;
        ResultOverall.Text = null;
        SearchButton.IsEnabled = true;
        SearchBox.IsEnabled = true;
        SearchProgressBar.Value = 0;
        SearchResultTabs.SelectedIndex = 0;
        _search = null;
    }
    private async Task SearchForPlayer()
    {
        if (SearchBox.Text is null)
        {
            ResetSearchState();
            return;
        }
        String name = SearchBox.Text;
        // just in case
        _search = null;
        _search = new Search(SearchComboBox.SelectedIndex);
        
        // we get the uuid from the name
        String? uuid = await _search.ReturnUuid(name);
        if (uuid is null)
        {
            SearchError.Text = "No player found";
            ResetSearchState();
            return;
        }
        SearchProgressBar.Value = 40;
        Console.WriteLine(uuid);
        // we get the tierlist
        Profile? prof = await _search.ReturnTierLists(uuid);
        if (prof is null)
        {
            SearchError.Text = "No player found in tier list";
            ResetSearchState();
            return;
        }
        // we get the head
        String? headPath = await _search.ReturnHead(uuid);
        if (headPath is not null)
        {
            ResultImage.Source = new Bitmap(headPath);
        }
        SearchProgressBar.Value = 80;
        
        // we now arrange the result page and finish everything up
        ArrayList topPlayerList = new ArrayList();
        if (prof.rankings == null)
        {
            SearchError.Text = "invalid rankings";
            ResetSearchState();
            return;
        }
        foreach (KeyValuePair<string, Tier> item in prof.rankings)
        {
            topPlayerList.Add(item.Key);
        }

        ResultName.Text = prof.name + " (" + prof.region + ")";
        ResultPoints.Text = prof.points + " points";
        ResultOverall.Text = "#" + prof.overall + " in the world";
        SearchResultList.ItemsSource = topPlayerList;
        SearchProgressBar.Value = 100;
        SearchResultTabs.SelectedIndex = 1;
    }
    

    public void ResultExitButtonClick(object sender, RoutedEventArgs e)
    {
        ResetSearchState();
    }

    public async void SearchButtonClick(object sender, RoutedEventArgs e)
    {
        
        SearchError.Text = "";
        SearchProgressBar.IsVisible = true;
        SearchProgressBar.Value = 0;
        SearchButton.IsEnabled = false;
        SearchBox.IsEnabled = false;
        await SearchForPlayer();
    }

    private void SearchResultList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SearchResultList.SelectedValue == null) return;
        if(_search is null) return;
        Tier? tier = _search.ReturnTierFromKey(SearchResultList.SelectedValue.ToString());
        if (tier is null) return;
        char peakHoL;
        char hoL;
        if (tier.pos == 1) hoL = 'L';
        else hoL = 'H';
        if(tier.peak_pos == 1) peakHoL = 'L';
        else peakHoL = 'H';
        ResultFirst.Text = "Tier: " + hoL + "T" + tier.tier;
        ResultSecond.Text = "Peak Tier: " + peakHoL + "T" + tier.peak_tier;
        
    }

    private void Theme_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        
    }

    private async void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (Tabs == null) return;
        if (Tabs.SelectedIndex == 1)
        {
            _list.Clear();
            List<PlayerData>? overAllList = await GetTopPlayers(SearchComboBox.SelectedIndex);
            if (overAllList is null)
            {
                _list.Add("This tier list has no listing available, or its API is incompatible");
                TopPlayers.ItemsSource = _list;
            }
            else
            {
                foreach (var item in overAllList)
                {
                    _list.Add(item.name + " (" + item.region + ") - " + item.points + " points");
                }
                TopPlayers.ItemsSource = _list;
            }
        }
        else
        {
            if(TopPlayers.ItemsSource != null) TopPlayers.ItemsSource = null;
        }
    }
}