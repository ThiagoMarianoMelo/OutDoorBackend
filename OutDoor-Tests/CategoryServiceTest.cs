using OutDoor_Models.Models;
using OutDoor_Models.Repositorys;
using OutDoor_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutDoor_Tests
{
    public class CategoryServiceTest
    {

        [Fact]
        public async void GetAllCategoriesTest()
        {
            //Arrange
            Moq.Mock<ICategoryRepository> cateogryRepositoryMock = new Mock<ICategoryRepository>();
            cateogryRepositoryMock.Setup(x => x.GetAll().Result).Returns(new List<CategoryModel>() { new CategoryModel(){
                Id = "123",
                Name = "TestCateogry"
            }});
            var categoryService = new CategoryService(cateogryRepositoryMock.Object);

            //Act
            var categoriesRecives = await categoryService.getAllCategories();

            //Assert
            Assert.NotNull(categoriesRecives);
            Assert.Equal("123", categoriesRecives == null ? "error" : categoriesRecives.First().Id);
            Assert.Equal("TestCateogry", categoriesRecives == null ? "error" : categoriesRecives.First().Name);
            Assert.Equal<int>(1, categoriesRecives == null ? 0 : categoriesRecives.Count());

        }

        [Fact]
        public async void GetCategoryByIdTest()
        {
            //Arrange
            Moq.Mock<ICategoryRepository> cateogryRepositoryMock = new Mock<ICategoryRepository>();
            cateogryRepositoryMock.Setup(x => x.GetById(It.Is<string>(x => x == "123")).Result).Returns(new CategoryModel(){
                Id = "123",
                Name = "TestCateogry"
            });
            var categoryService = new CategoryService(cateogryRepositoryMock.Object);

            //Act
            var categoryRecived = await categoryService.getCategoryById("123");

            //Assert
            Assert.NotNull(categoryRecived);

        }


        [Fact]
        public async void DeleteCategoryByIdTest()
        {
            //Arrange
            Moq.Mock<ICategoryRepository> cateogryRepositoryMock = new Mock<ICategoryRepository>();
            cateogryRepositoryMock.Setup(x => x.DeleteById(It.Is<string>(x => x == "123")).Result).Returns(new CategoryModel()
            {
                Id = "123",
                Name = "TestCateogry"
            });
            var categoryService = new CategoryService(cateogryRepositoryMock.Object);

            //Act
            var categoryDeleted = await categoryService.deleteGategoryById("123");

            //Assert
            Assert.NotNull(categoryDeleted);

        }
    }
}
