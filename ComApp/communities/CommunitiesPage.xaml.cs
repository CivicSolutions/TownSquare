namespace comApp.communities
{
    public partial class CommunitiesPage : ContentPage
    {
        public CommunitiesPage()
        {
            InitializeComponent();
            BindingContext = new CommunityViewModel();
        }
    }
}
