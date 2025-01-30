using System.Collections.ObjectModel;
using System.Windows.Input;
using comApp.db; 


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

        private void LoadCommunities()
        {
            
            dbConnection db = new dbConnection();
            Communities = new ObservableCollection<Community>(db.GetAllCommunities());
        }

        private void RequestMembership(int communityId)
        {
            
            int userId = GetUserIdFromSession(); 

            if (userId != -1)
            {
                
                dbConnection db = new dbConnection();
                db.InsertMembershipRequest(userId, communityId);

              
                LoadCommunities();

               
                Application.Current.MainPage.DisplayAlert("Success", "Membership requested successfully", "OK");
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
