using Dapper;
using SellerSystem.DAL.Repository.Interface;
using SellerSystem.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SellerSystem.DAL.Repository
{
    public class AccountRepository: BaseRepository, IAccountRepository
    {
        public void Create(Seller seller)
        {
            SqlMapper.Execute(con, "INSERT INTO seller (UserId, Name, Industry, StoreWebAddress)" +
                "VALUES (@UserId, @Name, @Industry, @StoreWebAddress);", seller);
        }
    }
}
