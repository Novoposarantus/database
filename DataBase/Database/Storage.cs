using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using DataBase.Models;
using MySql.Data.MySqlClient;

namespace DataBase.Database
{
	public class Storage : IStorage
	{
		const string connectionString = "Server = localhost; Database = myDb; Uid = root; Pwd = Novo;";

		readonly MySqlConnection mySqlConnection;
		public Storage()
		{
			this.mySqlConnection = new MySqlConnection(connectionString);
		}
		public void AddRole(Role role)
		{
			if ( role == null )
			{
				return;
			}
			Execute($"insert into {roleTable} set {roleName} = '{role.Name}'");
		}

		public void AddUser(User user)
		{
			if (user == null )
			{
				return;
			}
			Execute($"insert into {userTabel} set {userLogin} = '{user.Login}',"
												 + $"{userPassword} = '{user.Password}'"
												 + $"{userRoleId} = {user.Role.Id}");
		}

		public Role GetRoleById(int id)
		{
			if( id < 0 )
			{
				return null;
			}
			return getRoles(listRolesQuery + $"where {roleId} = {id}")[0];
		}

		public Role GetRoleByName(string name)
		{
			return getRoles(listRolesQuery + $"where {roleName} = '{name}'")[0];
		}

		public List<Role> GetRolesList()
		{
			return getRoles(listRolesQuery);
		}

		public User GetUserById(int id)
		{
			if (id < 0 )
			{
				return null;
			}
			return getUsers(listUserQuery + $"where {userId} = {id}")[0];
		}

		public List<User> GetUsersList()
		{
			return getUsers(listUserQuery);
		}

		public void SetRole(Role role, User user)
		{
			if (role == null || user == null )
			{
				return;
			}
			Execute($"update {userTabel} set {roleId} = {role.Id} where {userId} = {user.Id}");
		}

		public List<Product> GetProductList()
		{
			var products = new List<Product>();
			using( var connection = new MySqlConnection(connectionString) )
			{
				connection.Open();
				using( MySqlCommand command = connection.CreateCommand() )
				{
					command.CommandText = string.Format($"select * from {productTable}");
					using( MySqlDataReader reader = command.ExecuteReader() )
					{
						while( reader.Read() )
						{
							products.Add(new Product()
							{
								Id = (int)reader[productId],
								Name = (string)reader[productName],
								Description = (string)reader[productDescription],
								Price = (decimal)reader[productPrice]
							});
						}
					}
				}
			}
			return products;
		}

		public List<Purchase> GetPurchasesForUser(int id)
		{
			var purchases = new List<Purchase>();
			using( var connection = new MySqlConnection(connectionString) )
			{
				connection.Open();
				using( MySqlCommand command = connection.CreateCommand() )
				{
					command.CommandText = string.Format(listPurchasesQuery + $"where {userId} = {id}");
					using( MySqlDataReader reader = command.ExecuteReader() )
					{
						while( reader.Read() )
						{
							var purchase = purchases.FirstOrDefault(p => p.Id == (int)reader[purchaseId]);
							if (purchase == null )
							{
								purchases.Add(new Purchase()
								{
									Id = (int)reader[purchaseId],
									Date = (DateTime)reader[purchaseDate],
									Products = new List<Product>()
									{
										new Product()
										{
											Id = (int)reader[productId],
											Name = (string)reader[productName],
											Description = (string)reader[productDescription],
											Price = (decimal)reader[productPrice]
										}
									}
								});
							}
							else
							{
								purchase.Products.Add(new Product()
								{
									Id = (int)reader[productId],
									Name = (string)reader[productName],
									Description = (string)reader[productDescription],
									Price = (decimal)reader[productPrice]
								});
							}
						}
					}
				}
			}
			return purchases;
		}

		private void setUser(ref List<User> users, MySqlDataReader reader)
		{
			var user = users.FirstOrDefault(u => u.Id == (int)reader[userId]);
			if (user == null )
			{
				users.Add(new User()
				{
					Id = (int)reader[userId],
					Login = (string)reader[userLogin],
					Password = (string)reader[userPassword],
					Role = new Role()
					{
						Id = (int)reader[roleId],
						Name = (string)reader[roleName2]
					},
					Purchases = new List<Purchase>
					{
						new Purchase()
						{
							Id = (int)reader[purchaseId],
							Date = (DateTime)reader[purchaseDate],
							Products = new List<Product>()
							{
								new Product()
								{
									Id = (int)reader[productId],
									Name = (string)reader[productName2],
									Description = (string)reader[productDescription],
									Price = (decimal)reader[productPrice]
								}
							}
						}
					}
				});
			}
			else
			{
				var purchase = user.Purchases.FirstOrDefault(p => p.Id == (int)reader[purchaseId]);
				if (purchase == null )
				{
					user.Purchases.Add(new Purchase()
					{
						Id = (int)reader[purchaseId],
						Date = (DateTime)reader[purchaseDate],
						Products = new List<Product>()
						{
							new Product()
							{
								Id = (int)reader[productId],
								Name = (string)reader[productName2],
								Description = (string)reader[productDescription],
								Price = (decimal)reader[productPrice]
							}
						}
					});
				}
				else
				{
					purchase.Products.Add(new Product()
					{
						Id = (int)reader[productId],
						Name = (string)reader[productName2],
						Description = (string)reader[productDescription],
						Price = (decimal)reader[productPrice]
					});
				}
			}
		}
		private List<User> getUsers(string query)
		{
			var users = new List<User>();
			using( var connection = new MySqlConnection(connectionString) )
			{
				connection.Open();
				using( MySqlCommand command = connection.CreateCommand() )
				{
					command.CommandText = string.Format(query);
					using( MySqlDataReader reader = command.ExecuteReader() )
					{
						while( reader.Read() )
						{
							setUser(ref users, reader);
						}
					}
				}
			}
			return users;
		}
		private List<Role> getRoles(string query)
		{
			var roles = new List<Role>();
			using( var connection = new MySqlConnection(connectionString) )
			{
				connection.Open();
				using( MySqlCommand command = connection.CreateCommand() )
				{
					command.CommandText = string.Format(query);
					using( MySqlDataReader reader = command.ExecuteReader() )
					{
						while( reader.Read() )
						{
							roles.Add(new Role()
							{
								Id = (int)reader[roleId],
								Name = (string)reader[roleName]
							});
						}
					}
				}
			}
			return roles;
		}
		private void Execute(string commandText)
		{
			using( var connection = new MySqlConnection(connectionString) )
			{
				connection.Open();
				using( MySqlCommand command = connection.CreateCommand() )
				{
					command.CommandText = string.Format(commandText);
					command.ExecuteNonQuery();
				}
			}
		}


		#region queries
		readonly string listUserQuery = "select u.user_id, login,password,r.role_id ,r.name as 'roleName',p.purchase_id,p.date,p2.product_id,p2.name as 'productName',description,price from users "
											+ "join roles r on users.role_id = r.role_id "
											+ "join userPurchase u on users.user_id = u.user_id "
											+ "join purchases p on u.purchase_id = p.purchase_id "
											+ "join purchaseProduct Product on p.purchase_id = Product.purchase_id "
											+ "join products p2 on Product.product_id = p2.product_id";
			//$"select u.{userId}, {userLogin},{userPassword},r.{roleId} ,r.{roleName} as '{roleName2}',p.{purchaseId},"
			//							 + $"p.{purchaseDate},p2.{productId},p2.{productName} as '{productName2}',{productDescription},{productPrice} from {userTabel}"
			//							 + $"join {roleTable} r on {userTabel}.{roleId} = r.{roleId}"
			//							 + $"join {userPurchaseTable} u on {userTabel}.{userId} = u.{userId}"
			//							 + $"join {purchaseTable} p on u.{purchaseId} = p.{purchaseId}"
			//							 + $"join {purchaseProductTable} Product on p.{purchaseId} = Product.{purchaseId}"
			//							 + $"join {productTable} p2 on Product.{productId} = p2.{productId}";
		readonly string listPurchasesQuery = $"select {userId},p.{purchaseId},{purchaseDate},p2.{productId},{productName},{productDescription},{productPrice} from {userPurchaseTable} "
											+ $"join {purchaseTable} p on {userPurchaseTable}.{purchaseId} = p.{purchaseId} "
											+ $"join {purchaseProductTable} Product on p.{purchaseId} = Product.{purchaseId} "
											+ $"join {productTable} p2 on Product.{productId} = p2.{productId} ";
		readonly string listRolesQuery = $"select * from {roleTable} ";
		#endregion

		#region tables
		const string userTabel = "users";
		const string roleTable = "roles";
		const string purchaseTable = "purchases";
		const string productTable = "products";
		const string purchaseProductTable = "perchaseproduct";
		const string userPurchaseTable = "userpurchase";
		#endregion

		#region columns
		const string userId = "user_id";
		const string userLogin = "login";
		const string userPassword = "password";
		const string userRoleId = "role_id";
		const string roleId = "role_id";
		const string roleName = "name";
		const string roleName2 = "roleName";
		const string purchaseId = "purchase_id";
		const string purchaseDate = "date";
		const string productId = "product_id";
		const string productName = "name";
		const string productName2 = "productName";
		const string productDescription = "description";
		const string productPrice = "price";
		#endregion
	}
}
