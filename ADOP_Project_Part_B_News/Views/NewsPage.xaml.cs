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
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Title = $"Headlines for {category}";

        MainThread.BeginInvokeOnMainThread(async () => { await LoadNewsCategory(); });
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {
        await LoadNewsCategory();
    }

    private async Task LoadNewsCategory()
    {
        News news = await service.GetNewsAsync(category);

        GroupedNews.ItemsSource = news.Articles;
    }
}
