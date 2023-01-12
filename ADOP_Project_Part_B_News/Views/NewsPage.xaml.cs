using ADOP_Project_Part_B_News.Models;
using ADOP_Project_Part_B_News.Services;
namespace ADOP_Project_Part_B_News.Views;

public partial class NewsPage : ContentPage
{
    NewsService service;
    NewsCategory category;

	public NewsPage(NewsCategory category)
	{
        InitializeComponent();

        this.category = category;
        service= new NewsService();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Title = $"{category}";

        try 
        {
        await LoadNewsCategory();
        }
        catch (Exception ex)
        {
            await DisplayAlert("An error occured!", $"Error: {ex}", "OK");
        }
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            await LoadNewsCategory();
        }
        catch (Exception ex)
        {
            await DisplayAlert("An error occured!", $"Error: {ex}", "OK");
        }
    }
    private async void ListViewItemTapped(object sender, ItemTappedEventArgs e)
    {
        NewsItem item = (NewsItem)e.Item;
        try
        {
            await Navigation.PushAsync(new ArticleView(item.Url));
        }
        catch (Exception ex)
        {
            await DisplayAlert("An error occured!", $"Error: {ex}", "OK");
        }
        ((ListView)sender).SelectedItem = null;
    }

    private async Task LoadNewsCategory()
    {
        News news = await service.GetNewsAsync(category);

        GroupedNews.ItemsSource = news.Articles;
    }
}