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
            var response = await db.GetAllCommunities();

            if (response == null || !response.IsSuccess)
            {
                string message = response == null
                    ? "Failed to load communities."
                    : response.StatusCode == 401
                        ? "You are not authorized to view communities."
                        : $"Failed to load communities: {response.ErrorMessage}";
                await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
                Communities = new ObservableCollection<Community>();
                return;
            }

            var communityList = JsonConvert.DeserializeObject<List<Community>>(response.Content);
            Communities = new ObservableCollection<Community>(communityList ?? new List<Community>());
        }

        private async void RequestMembership(int communityId)
        {
            int userId = GetUserIdFromSession();

            if (userId != -1)
            {
                dbConnection db = new dbConnection();
                var response = await db.RequestMembership(userId, communityId);

                if (response == null || !response.IsSuccess)
                {
                    string message = response == null
                        ? "Failed to request membership."
                        : response.StatusCode == 401
                            ? "You are not authorized to request membership."
                            : $"Failed to request membership: {response.ErrorMessage}";
                    await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
                }
                else
                {
                    LoadCommunities();
                    await Application.Current.MainPage.DisplayAlert("Success", "Membership requested successfully", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "User ID not available", "OK");
            }
        }

        private int GetUserIdFromSession()
        {
            int userId = Preferences.Get("UserId", -1);
            return userId;
        }
    }
}
