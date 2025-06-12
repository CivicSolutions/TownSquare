namespace comApp.pins;
using comApp.posts;
using comApp.db;

public partial class CreatePins : ContentPage
{
    private dbConnection _dbConnection;

    public CreatePins()
    {
        InitializeComponent();
        _dbConnection = new dbConnection();
    }

    private async void OnWarningClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WarningPin());
    }

    private async void OnHelpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelpPin());
    }

    private async void CheckUser()
    {
        string userId = App.UserId;

        if (userId is null or "")
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CheckUser();
        NavigationPage.SetHasNavigationBar(this, false);
    }
}
