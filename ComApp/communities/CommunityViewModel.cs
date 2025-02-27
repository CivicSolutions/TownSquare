using System.Collections.ObjectModel;
using System.Windows.Input;
using comApp.db;
using Newtonsoft.Json;

namespace comApp.communities
{
    public class CommunityViewModel
    {
        private ObservableCollection<Community> _communities;
        public ObservableCollection<Community> Communities
        {
            get { return _communities; }
            set { _communities = value; }
        }

        public ICommand RequestMembershipCommand { get; private set; }

        public CommunityViewModel()
        {
            LoadCommunities();
            RequestMembershipCommand = new Command<int>(RequestMembership);
        }

        private async void LoadCommunities()
        {
            dbConnection db = new dbConnection();
            string response = await db.GetAllCommunities();
            // Assuming you need to parse response into a collection of Community objects
            var communityList = JsonConvert.DeserializeObject<List<Community>>(response);
            Communities = new ObservableCollection<Community>(communityList);
        }

        private async void RequestMembership(int communityId)
        {
            int userId = GetUserIdFromSession();

            if (userId != -1)
            {
                dbConnection db = new dbConnection();
                var response = await db.RequestMembership(userId, communityId);

                // Check the response for success
                if (response.Contains("Error"))
                {
                    Application.Current.MainPage.DisplayAlert("Error", response, "OK");
                }
                else
                {
                    LoadCommunities();
                    Application.Current.MainPage.DisplayAlert("Success", "Membership requested successfully", "OK");
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Error", "User ID not available", "OK");
            }
        }

        private int GetUserIdFromSession()
        {
            int userId = Preferences.Get("UserId", -1);
            return userId;
        }
    }
}
