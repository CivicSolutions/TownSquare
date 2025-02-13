using comApp.communities;
using comApp.posts;
using MySqlConnector;

namespace comApp.db
{
    public class dbConnection
    {
        private MySqlConnection _connection;

        public dbConnection()
        {
            string connectionString = "server=10.0.2.2;port=3306;database=comapp;user=root;password=sml12345";

            _connection = new MySqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            try
            {
                _connection.Open();
                Console.WriteLine("Database connection opened yay.");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                _connection.Close();
                Console.WriteLine("Database connection closed.");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public bool TestConnection()
        {
            try
            {
                _connection.Open();
                _connection.Close();
                Console.WriteLine("Database connection test successful.");
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        // Example method to execute SQL queries
        public void ExecuteQuery(string query)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        //----------------------------------------------------------------------Querys---------------------------------------------------
        public void InsertUser(string name, string email, string bio, string password)
        {
            try
            {
                OpenConnection();

                string query = @"
                    INSERT INTO Users (Name, Email, Bio, Password) 
                    VALUES (@Name, @Email, @Bio, @Password);
                ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Bio", bio);
                    cmd.Parameters.AddWithValue("@Password", password);

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        // Select user from the database by email and password
        public bool SelectUserByEmailAndPassword(string email, string password)
        {
            try
            {
                OpenConnection();

                string query = @"
                    SELECT * FROM Users 
                    WHERE Email = @Email AND Password = @Password;
                ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Check if any rows are returned
                        return reader.HasRows;
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        public int GetUserIdByEmail(string email)
        {
            int userId = -1; // Initialize userId to a default value

            try
            {
                _connection.Open();

                string query = @"
                    SELECT Id FROM Users 
                    WHERE Email = @Email;
                ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userId = reader.GetInt32("Id");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _connection.Close();
            }

            return userId;
        }
        public List<Community> GetAllCommunities()
        {
            List<Community> communities = new List<Community>();

            try
            {
                OpenConnection();

                string query = @"
            SELECT Id, Name, Bio FROM Communities;";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Community community = new Community
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Bio = reader.GetString("Bio")
                        };

                        communities.Add(community);
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            return communities;
        }

        public void InsertMembershipRequest(int userId, int communityId)
        {
            try
            {
                OpenConnection();

                string query = @"
            INSERT INTO User_Community_Request (User_Id, Community_Id) 
            VALUES (@UserId, @CommunityId);";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@CommunityId", communityId);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<Post> GetPosts()
        {
            List<Post> posts = new List<Post>();

            try
            {
                _connection.Open();

                string query = @"
            SELECT p.Id, p.Content, p.User_Id, u.Name AS UserName
            FROM Posts p
            INNER JOIN Users u ON p.User_Id = u.Id;
        ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Post post = new Post
                            {
                                Id = reader.GetInt32("Id"),
                                Content = reader.GetString("Content"),
                                UserId = reader.GetInt32("User_Id"),
                                UserName = reader.GetString("UserName")
                            };

                            posts.Add(post);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _connection.Close();
            }

            return posts;
        }

        public void AddPost(string content, string title)
        {
            try
            {
                _connection.Open();
                using (var cmd1 = new MySqlCommand("SET foreign_key_checks = 0;", _connection))
                {
                    cmd1.ExecuteNonQuery();
                }

                string query = @"
                    INSERT INTO Posts (Content, Title, User_Id, PostTime, isNews, Community_Id)
                    VALUES (@Content, @Title , @UserId, NOW(), 0, 1);
                ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    int userId = GetUserIdFromSession();

                    cmd.Parameters.AddWithValue("@Content", content);
                    cmd.Parameters.AddWithValue("@Title", title);
                    // You need to replace @UserId with the actual user ID of the logged-in user
                    cmd.Parameters.AddWithValue("@UserId", userId); // Example user ID

                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _connection.Close();
            }
        }

        public int GetUserIdFromSession()
        {
            string userIdString = Preferences.Get("UserId", "-1"); // Default value is "-1" if key is not found
            int userId;
            if (int.TryParse(userIdString, out userId))
            {
                return userId;
            }
            else
            {
                // Handle the case where the string cannot be parsed to an integer
                // For example, you might want to log an error or return a default value
                return -1; // Return a default value
            }
        }

        //---------------------------------------------------------------------------------

        public List<PinData> GetPins()
        {
            List<PinData> pins = new List<PinData>();

            try
            {
                _connection.Open();
                string query = "SELECT * FROM Pins";
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PinData pin = new PinData
                        {
                            Id = reader.GetInt32("Id"),
                            UserId = reader.GetInt32("User_Id"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description"),
                            PostTime = reader.GetDateTime("PostTime"),
                            XCoord = reader.GetDouble("X_cord"),
                            YCoord = reader.GetDouble("Y_cord"),
                            CommunityId = reader.GetInt32("Community_Id"),
                            PinType = reader.GetInt32("PinType")
                        };
                        pins.Add(pin);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _connection.Close();
            }

            return pins;
        }

        public void InsertPin(int userId, string title, string description, DateTime postTime, double xCoord, double yCoord, int communityId, int pinType)
        {
            try
            {
                _connection.Open();
                using (var cmd1 = new MySqlCommand("SET foreign_key_checks = 0;", _connection))
                {
                    cmd1.ExecuteNonQuery();
                }
                string query = "INSERT INTO Pins (User_Id, Title, Description, PostTime, X_cord, Y_cord, Community_Id, PinType) " +
                               "VALUES (@UserId, @Title, @Description, @PostTime, @XCoord, @YCoord, @CommunityId, @PinType)";
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@PostTime", postTime);
                cmd.Parameters.AddWithValue("@XCoord", xCoord);
                cmd.Parameters.AddWithValue("@YCoord", yCoord);
                cmd.Parameters.AddWithValue("@CommunityId", communityId);
                cmd.Parameters.AddWithValue("@PinType", pinType);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _connection.Close();
            }
        }



        public class PinData
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime PostTime { get; set; }
            public double XCoord { get; set; }
            public double YCoord { get; set; }
            public int CommunityId { get; set; }
            public int PinType { get; set; }
        }



        //---------------------------------------------------------------------------------

        public User GetUserById(int userId)
        {
            User user = null;

            try
            {
                _connection.Open();

                string query = @"
                    SELECT * FROM Users 
                    WHERE Id = @UserId;
                ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Email = reader.GetString("Email"),
                                Bio = reader.GetString("Bio"),
                                // Add other properties as needed
                            };
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _connection.Close();
            }

            return user;
        }

        public void UpdateUser(int userId, string newUsername, string newBio)
        {
            try
            {
                OpenConnection();

                string query = @"
            UPDATE Users
            SET Name = @NewUsername, Bio = @NewBio
            WHERE Id = @UserId;
        ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@NewUsername", newUsername);
                    cmd.Parameters.AddWithValue("@NewBio", newBio);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                CloseConnection();
            }
        }


        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Bio { get; set; }
            // Add other properties as needed
        }

        public List<Post> GetNormalPosts()
        {
            List<Post> posts = new List<Post>();

            try
            {
                _connection.Open();

                string query = @"
            SELECT p.Id, p.Content, p.User_Id, u.Name AS UserName
            FROM Posts p
            INNER JOIN Users u ON p.User_Id = u.Id WHERE p.isNews = 0;
        ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Post post = new Post
                            {
                                Id = reader.GetInt32("Id"),
                                Content = reader.GetString("Content"),
                                UserId = reader.GetInt32("User_Id"),
                                UserName = reader.GetString("UserName")
                            };

                            posts.Add(post);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _connection.Close();
            }

            return posts;
        }

        public List<Post> GetNewsPosts()
        {
            List<Post> posts = new List<Post>();

            try
            {
                _connection.Open();

                string query = @"
            SELECT p.Id, p.Title, p.Content, p.User_Id, p.PostTime, u.Name AS UserName
            FROM Posts p
            INNER JOIN Users u ON p.User_Id = u.Id
            WHERE p.isNews = 1;
        ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Post post = new Post
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.GetString("Title"),
                                Content = reader.GetString("Content"),
                                UserId = reader.GetInt32("User_Id"),
                                UserName = reader.GetString("UserName"),
                                PostTime = reader.GetDateTime("PostTime") // Add this line to set PostTime
                            };

                            posts.Add(post);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _connection.Close();
            }

            return posts;
        }


        public List<Post> GetUserPosts()
        {
            List<Post> userPosts = new List<Post>();

            try
            {
                OpenConnection();

                string query = @"
            SELECT p.Id, p.Title, p.Content, p.User_Id, p.PostTime, u.Name AS UserName
            FROM Posts p
            INNER JOIN Users u ON p.User_Id = u.Id
            WHERE p.isNews = 0; -- Filter for user posts only
        ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Post post = new Post
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.GetString("Title"),
                                Content = reader.GetString("Content"),
                                UserId = reader.GetInt32("User_Id"),
                                UserName = reader.GetString("UserName"),
                                PostTime = reader.GetDateTime("PostTime") // Add this line to set PostTime
                            };

                            userPosts.Add(post);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }

            return userPosts;
        }


        public List<HelpPosts> GetHelpPosts()
        {
            List<HelpPosts> helpposts = new List<HelpPosts>();

            try
            {
                _connection.Open();

                string query = @"
            SELECT h.Id, h.Title ,h.Description, h.Price, h.Telephone, h.User_Id, u.Name AS UserName
            FROM Help_posts h
            INNER JOIN Users u ON h.User_Id = u.Id
        ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            HelpPosts helppost = new HelpPosts
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.GetString("Title"),
                                Description = reader.GetString("Description"),
                                Price = reader.GetInt32("Price"),
                                Telephone = reader.GetString("Telephone"),
                                UserId = reader.GetInt32("User_Id"),
                                UserName = reader.GetString("UserName")
                            };

                            helpposts.Add(helppost);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _connection.Close();
            }

            return helpposts;
        }

        public void AddHelpPost(string title, string description, int price, string telephone)
        {
            try
            {
                _connection.Open();
                using (var cmd1 = new MySqlCommand("SET foreign_key_checks = 0;", _connection))
                {
                    cmd1.ExecuteNonQuery();
                }

                string query = @"
                    INSERT INTO help_posts (Title, Description, Price, Telephone, HelpPostTime, User_Id)
                    VALUES (@Title, @Description, @Price, @Telephone, NOW(), @UserId);
                ";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    int userId = GetUserIdFromSession();

                    cmd.Parameters.AddWithValue("@Title", title);
                    // You need to replace @UserId with the actual user ID of the logged-in user
                    cmd.Parameters.AddWithValue("@Description", description);

                    cmd.Parameters.AddWithValue("@Price", price);

                    cmd.Parameters.AddWithValue("@Telephone", telephone);

                    cmd.Parameters.AddWithValue("@UserId", userId);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
            finally
            {
                _connection.Close();
            }

        }
        public void DeleteHelpPost(int postId)
        {
            try
            {
                OpenConnection();

                string query = "DELETE FROM Help_Posts WHERE Id = @PostId";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@PostId", postId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

    }
}