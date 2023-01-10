using ADOP_Project_Part_B_News.Models;
using ADOP_Project_Part_B_News.Services;

namespace ADOP_Project_Part_B_News.Views;

public partial class NewsPage : ContentPage
{
    NewsService service;
    NewsCategory category;
    News item;

	public NewsPage(NewsCategory category)
	{
        InitializeComponent();

        this.category = category;
        service= new NewsService();
        this.BindingContext = item;
    }
    protected override async void OnAppearing()
    {
        //base.OnAppearing();
        Title = $"Headlines for {category}";

        await LoadNewsCategory();
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
