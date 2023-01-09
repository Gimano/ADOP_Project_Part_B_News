using ADOP_Project_Part_B_News.Models;
using ADOP_Project_Part_B_News.Views;

namespace ADOP_Project_Part_B_News;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        foreach (NewsCategory item in Enum.GetValues(typeof(NewsCategory)))
        {
            var sc = new ShellContent
            {
                Title = item.ToString(),
                Route = item.ToString().ToLower(),
                ContentTemplate = new DataTemplate(() => new NewsPage(item))
            };
            this.Items.Add(sc);
        }
    }
}
