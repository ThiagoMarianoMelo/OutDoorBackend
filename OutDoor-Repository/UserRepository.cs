﻿using Microsoft.EntityFrameworkCore;
using OutDoor_Models;
using OutDoor_Models.Models;
using OutDoor_Models.Repositorys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OutDoor_Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DbMainContext _mainContext;

        public UserRepository(DbMainContext mainContext)
        {
            _mainContext = mainContext;
        }

        public async Task<UserModel> CreateUser(UserModel user)
        {

            try
            {
                var result = await _mainContext.AddAsync(user);
                _mainContext.SaveChanges();
                return result.Entity;

            }catch(Exception ex)
            {
                throw new RepositoryException("Um erro inesperado ocorreu ao tentar criar um usuário")
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Source = nameof(UserRepository)
                };
            }
        }

        public Task<UserModel> DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel?> GetUserByEmail(string email)
        {
            try
            {
                var varUser = await _mainContext.User.FirstOrDefaultAsync(u => u.Email.Equals(email));
                return varUser;

            }
            catch (Exception ex)
            {
                throw new RepositoryException("A Unexpected Exception ocorrued while trying to find a user by email")
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Source = nameof(UserRepository)
                };
            }
        }

        public async Task<UserModel?> GetById(string id)
        {
            try
            {
                return await _mainContext.User.FirstOrDefaultAsync(u => u.Id == id);

            }
            catch (Exception ex)
            {
                throw new RepositoryException("Um erro inesperado ocorreu ao buscar um usuário")
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Source = nameof(UserRepository)
                };
            }
        }

        public async Task<UserModel> UpdateUser(UserModel user)
        {
            try
            {
                var result = _mainContext.User.Update(user).Entity ;
                await _mainContext.SaveChangesAsync();
                return result;

            }
            catch (Exception ex)
            {
                throw new RepositoryException("Um erro inesperado ocorreu ao editar dados de um usuário")
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Source = nameof(UserRepository)
                };
            }
        }
    }
}
