using SellerSystem.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SellerSystem.DAL.Repository.Interface
{
    public interface IAccountRepository
    {
        void Create(Seller seller);
    }
}
