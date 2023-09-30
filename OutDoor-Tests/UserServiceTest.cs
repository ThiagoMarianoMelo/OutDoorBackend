using OutDoor_Models.Models;
using OutDoor_Models.Repositorys;
using OutDoor_Models.Requests.User;
using OutDoor_Models.Services;
using OutDoor_Services;
using OutDoor_Services.UtilServices;
using System.ComponentModel;
using Xunit;

namespace OutDoor_Tests
{
    public class UserServiceTest
    {

        [Fact]
        public async void CreateUserTest()
        {
            //Arrange
            Moq.Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.CreateUser(It.IsAny<UserModel>()).Result).Returns(new UserModel()
            {
                Name = "NameTest",
                Email = "EmailTest@Email.com",
                Password = "PasswordTest123",
                UserType = "client",
                CreatedAt = DateTime.Today
            });
            Moq.Mock<IPostRepository> postRepositoryMock = new Mock<IPostRepository>();
            var userService = new UserService(userRepositoryMock.Object, new CryptographyService(), postRepositoryMock.Object);

            //Act
            var userCreated = await userService.CreateUser(new CreateUserRequest() { 
                Name = "NameTest",
                Email = "EmailTest@Email.com",
                Password = "PasswordTest123",
                UserType = "client"
            });

            //Assert
            Assert.NotNull(userCreated);
            Assert.Equal(DateTime.Today,userCreated.CreatedAt);
        }

        [Fact]
        public async void CreateUserEmailAlreadyInUseTest()
        {
            //Arrange
            Moq.Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetUserByEmail(It.Is<string>(x => x == "EmailInUse@Email.com")).Result).Returns(new UserModel());
            Moq.Mock<IPostRepository> postRepositoryMock = new Mock<IPostRepository>();
            var userService = new UserService(userRepositoryMock.Object, new CryptographyService(), postRepositoryMock.Object);


            //Act then Assert
            await Assert.ThrowsAsync <ServiceException>( async () =>  await userService.CreateUser(new CreateUserRequest()
            {
                Name = "NameTest",
                Email = "EmailInUse@Email.com",
                Password = "PasswordTest123",
                UserType = "client"

            }));
        }

        [Fact]
        public async void LoginUserTest()
        {
            //Arrange
            Moq.Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetUserByEmail(It.Is<string>(x => x == "EmailInUse@Email.com")).Result).Returns(new UserModel() { Password = "PasswordTest123" });
            Moq.Mock<IPostRepository> postRepositoryMock = new Mock<IPostRepository>();
            Moq.Mock<ICryptographyService> criptographyServiceMock = new Mock<ICryptographyService>();
            criptographyServiceMock.Setup(x => x.Decrypt(It.Is<string>(x => x == "PasswordTest123"))).Returns("PasswordTest123");
            var userService = new UserService(userRepositoryMock.Object,criptographyServiceMock.Object, postRepositoryMock.Object);

            //Act
            var userLogged = await userService.LoginUser(new LoginRequest()
            {
                Email = "EmailInUse@Email.com",
                Password = "PasswordTest123"
            });

            //Assert
            Assert.NotNull(userLogged);
        }


        [Fact]
        public async void LoginUserNotFoundTest()
        {
            //Arrange
            Moq.Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetUserByEmail(It.Is<string>(x => x == "EmailInUse@Email.com")).Result).Returns((UserModel?) null);
            Moq.Mock<IPostRepository> postRepositoryMock = new Mock<IPostRepository>();
            Moq.Mock<ICryptographyService> criptographyServiceMock = new Mock<ICryptographyService>();
            var userService = new UserService(userRepositoryMock.Object, criptographyServiceMock.Object, postRepositoryMock.Object);

            //Act then Assert
            await Assert.ThrowsAsync<ServiceException>(async () => await userService.LoginUser(new LoginRequest() {
                Email = "EmailInUse@Email.com",
                Password = "Password123"
            }));
        }

        [Fact]
        public async void LoginUserInvalidPasswordTest()
        {
            //Arrange
            Moq.Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetUserByEmail(It.Is<string>(x => x == "EmailInUse@Email.com")).Result).Returns(new UserModel() { Password = "AnotherPassword"});
            Moq.Mock<IPostRepository> postRepositoryMock = new Mock<IPostRepository>();
            Moq.Mock<ICryptographyService> criptographyServiceMock = new Mock<ICryptographyService>();
            criptographyServiceMock.Setup(x => x.Decrypt(It.Is<string>(x => x == "AnotherPassword"))).Returns("AnotherPassword");
            var userService = new UserService(userRepositoryMock.Object, criptographyServiceMock.Object, postRepositoryMock.Object);

            //Act then Assert
            await Assert.ThrowsAsync<ServiceException>(async () => await userService.LoginUser(new LoginRequest()
            {
                Email = "EmailInUse@Email.com",
                Password = "Password123"
            }));
        }

        [Fact]
        public async void EditUserInformationTest()
        {
            //Arrange
            Moq.Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetById(It.Is<string>(x => x == "1")).Result).Returns(new UserModel());
            Moq.Mock<IPostRepository> postRepositoryMock = new Mock<IPostRepository>();
            Moq.Mock<ICryptographyService> criptographyServiceMock = new Mock<ICryptographyService>();
            var userService = new UserService(userRepositoryMock.Object, criptographyServiceMock.Object, postRepositoryMock.Object);

            //Act
            var userEdited = await userService.EditUserData(new EditUserRequest()
            {
                ID = "1",
                Avatar = "EditedAvatar",
                Name = "EditedName",
                Email = "EditedEmail"
            });

            //Assert
            Assert.Equal("EditedAvatar", userEdited.Avatar);
            Assert.Equal("EditedName", userEdited.Name);
            Assert.Equal("EditedEmail", userEdited.Email);
        }

        [Fact]
        public async void EditUserInformationUserNotFoundedTest()
        {
            //Arrange
            Moq.Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.GetById(It.Is<string>(x => x == "1")).Result).Returns( (UserModel?) null);
            Moq.Mock<IPostRepository> postRepositoryMock = new Mock<IPostRepository>();
            Moq.Mock<ICryptographyService> criptographyServiceMock = new Mock<ICryptographyService>();
            var userService = new UserService(userRepositoryMock.Object, criptographyServiceMock.Object, postRepositoryMock.Object);

            //Act then Assert
            await Assert.ThrowsAsync<ServiceException>(async () => {
                await userService.EditUserData(new EditUserRequest()
                {
                    ID = "1",
                    Avatar = "EditedAvatar",
                    Name = "EditedName",
                    Email = "EditedEmail"
                });
            });
            
        }

        [Fact]
        public async void GetPostsFromUserTest()
        {
            //Arrange
            Moq.Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            Moq.Mock<IPostRepository> postRepositoryMock = new Mock<IPostRepository>();
            postRepositoryMock.Setup(x => x.getAllPosts().Result).Returns(new List<PostModel>() { new PostModel() { UserId = "1" } });
            Moq.Mock<ICryptographyService> criptographyServiceMock = new Mock<ICryptographyService>();
            var userService = new UserService(userRepositoryMock.Object, criptographyServiceMock.Object, postRepositoryMock.Object);

            //Act
            var postsFounded = await userService.GetPosts("1");

            //Assert
            Assert.NotNull(postsFounded);
            Assert.Equal<int>(1, postsFounded.Count());

        }

        [Fact]
        public async void GetPostsFromUserWithoutPostsTest()
        {
            //Arrange
            Moq.Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            Moq.Mock<IPostRepository> postRepositoryMock = new Mock<IPostRepository>();
            postRepositoryMock.Setup(x => x.getAllPosts().Result).Returns( (IEnumerable<PostModel>?) null);
            Moq.Mock<ICryptographyService> criptographyServiceMock = new Mock<ICryptographyService>();
            var userService = new UserService(userRepositoryMock.Object, criptographyServiceMock.Object, postRepositoryMock.Object);

            //Act
            var postsFounded = await userService.GetPosts("1");

            //Assert
            Assert.Equal<int>(0, postsFounded.Count());

        }

        [Fact]
        public async void DeleteUserPostTest()
        {
            //Arrange
            Moq.Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            Moq.Mock<IPostRepository> postRepositoryMock = new Mock<IPostRepository>();
            postRepositoryMock.Setup(x => x.deletePost(It.Is<string>(x => x == "1")).Result).Returns("1");
            Moq.Mock<ICryptographyService> criptographyServiceMock = new Mock<ICryptographyService>();
            var userService = new UserService(userRepositoryMock.Object, criptographyServiceMock.Object, postRepositoryMock.Object);

            //Act
            var IdPostDeleted = await userService.DeleteUserPost("1");

            //Assert
            Assert.Equal("1", IdPostDeleted);

        }
    }
}