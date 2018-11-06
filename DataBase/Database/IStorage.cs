using DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace DataBase.Database
{
	public interface IStorage
	{
		List<User> GetUsersList();
		List<Role> GetRolesList();
		List<Product> GetProductList();
		List<Purchase> GetPurchasesForUser(int id);
		void AddUser(User user);
		void AddRole(Role role);
		void SetRole(Role role, User user);
		Role GetRoleById(int id);
		Role GetRoleByName(string name);
		User GetUserById(int id);

	}
}
