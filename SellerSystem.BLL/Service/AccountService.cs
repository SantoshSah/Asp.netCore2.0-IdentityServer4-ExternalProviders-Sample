using SellerSystem.BLL.Service.Interface;
using SellerSystem.DAL.Repository;
using SellerSystem.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SellerSystem.BLL.Service
{
   public class AccountService: IAccountService
   {
        public void Create(Seller seller)
        {
            using (var accountRepo = new AccountRepository())
            {
                accountRepo.Create(seller);
            }
        }
   }
}
