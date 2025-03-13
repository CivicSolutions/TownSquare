using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using comApp;
using Microsoft.Maui.Controls;
using comApp.signUp;
using comApp.db;
using comApp.login;
using NUnit.Framework.Legacy;

namespace TestProject1
{
    [TestFixture]
    public class SignupViewModelTests
    {
        private Mock<INavigation> _mockNavigation;
        private Mock<dbConnection> _mockDbConnection;
        private Mock<Page> _mockPage;
        private SignupViewModel _viewModel;


        [SetUp]
        public void SetUp()
        {
            _mockNavigation = new Mock<INavigation>();
            _mockDbConnection = new Mock<dbConnection>();
            _mockPage = new Mock<Page>(); // Mock the Page object

            ClassicAssert.IsNotNull(_mockNavigation);
            ClassicAssert.IsNotNull(_mockDbConnection);
            ClassicAssert.NotNull(_mockPage);

            // DO NOT use Application.Current
            _viewModel = new SignupViewModel(_mockNavigation.Object, _mockPage.Object);
        }




        [Test]
        public async Task Signup_ShouldDisplayError_WhenFieldsAreEmpty()
        {
            // Arrange
            _viewModel.Name = "";
            _viewModel.Email = "";
            _viewModel.Password = "";
            _viewModel.Bio = "";

            // Act
            await _viewModel.Signup(); // Make sure this is awaited

            // Assert
            _mockPage.Verify(p => p.DisplayAlert("Error", "Please fill in all fields.", "OK"), Times.Once);
        }


        [Test]
        public async Task Signup_ShouldDisplayError_WhenPasswordsDoNotMatch()
        {
            // Arrange: Set the fields
            _viewModel.Name = "John Doe";
            _viewModel.Email = "john@example.com";
            _viewModel.Password = "password123";
            _viewModel.ConfirmPassword = "differentpassword";

            // Act: Execute the Signup command
            await _viewModel.Signup();

            // Assert: Check that the password mismatch error alert is shown
            _mockPage.Verify(p => p.DisplayAlert("Error", "Passwords do not match.", "OK"), Times.Once);
        }

        [Test]
        public async Task Signup_ShouldNavigateToLogin_WhenRegistrationIsSuccessful()
        {
            // Arrange: Set up mock database response
            _mockDbConnection.Setup(db => db.RegisterUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                             .ReturnsAsync("Success");

            _viewModel.Name = "John Doe";
            _viewModel.Email = "john@example.com";
            _viewModel.Password = "password123";
            _viewModel.Bio = "This is a bio";
            _viewModel.ConfirmPassword = "password123";

            // Act: Execute the Signup command
            await _viewModel.Signup();

            // Assert: Check that the success message is shown
            _mockPage.Verify(p => p.DisplayAlert("Success", "You have registered successfully.", "OK"), Times.Once);

            // Assert: Check that navigation to the Login page occurs
            _mockNavigation.Verify(nav => nav.PushAsync(It.IsAny<Login>()), Times.Once);
        }

        [Test]
        public async Task Signup_ShouldDisplayError_WhenRegistrationFails()
        {
            // Arrange: Set up mock database response for failure
            _mockDbConnection.Setup(db => db.RegisterUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                             .ReturnsAsync("Error: Database failure");

            _viewModel.Name = "John Doe";
            _viewModel.Email = "john@example.com";
            _viewModel.Password = "password123";
            _viewModel.Bio = "This is a bio";
            _viewModel.ConfirmPassword = "password123";

            // Act: Execute the Signup command
            await _viewModel.Signup();

            // Assert: Check that the error alert is shown
            _mockPage.Verify(p => p.DisplayAlert("Error", "Error: Database failure", "OK"), Times.Once);
        }
    }
}