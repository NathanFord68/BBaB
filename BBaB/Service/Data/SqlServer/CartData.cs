using BBaB.Models;
using BBaB.Service.CrudExceptions.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BBaB.Utility.Interfaces;

namespace BBaB.Service.Data
{
    public class CartData : ICrud<CartModel>
    {
        private SqlConnection _connection;
        private IBBaBLogger logger;
        public CartData(SqlConnection connection, IBBaBLogger logger)
        {
            this._connection = connection;
            this.logger = logger;
        }

        /**
         * <summary>Adds an item to the cart of the user.</summary>
         * <remarks>Not intended to generate an entire cart in one call.</remarks>
         */
        public void CreateT(CartModel model)
        {
            this.logger.Info("Entering CartData@CreateT");
            try
            {
                //Create the command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Create the query statement
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"INSERT INTO [bbab].[dbo].[Cart] ([PRINCIPAL_ID], [WEAPON_ID]) VALUES (@pid, @wid)";

                    //Bind Parameters to statement
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@pid", SqlDbType.Int).Value = model._customer._id;
                    command.Parameters.Add("@wid", SqlDbType.Int).Value = model._weaponToUpdate._id;

                    //Prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //Execute the statement
                    this.logger.Info("Executing command NonQuery");
                    command.ExecuteNonQuery();

                    this.logger.Info("Exiting CartData@CreateT");
                }
            }catch(Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing Exception RecordNotCreatedException");
                throw new RecordNotCreatedException("Item not added to cart.", e.InnerException);
            }
        }
        
        /**
         * <summary>Deletes an item from the cart with the item id.</summary>
         */
        public void DeleteT(CartModel model)
        {
            this.logger.Info("Entering CartData@DeleteT");
            try
            {
                //Create the command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Create the query statement
                    this.logger.Info("Generating Sql script");
                    command.CommandText = @"DELETE FROM [bbab].[dbo].[Cart] WHERE [WEAPON_ID] = @wid";

                    //Bind Parameters to statement
                    this.logger.Info("Binding data to sql");
                    command.Parameters.Add("@wid", SqlDbType.Int).Value = model._weaponToUpdate._id;

                    //Prepare the statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    //Execute the statement
                    this.logger.Info("Executing command NonQuery");
                    command.ExecuteNonQuery();
                }

                this.logger.Info("Exiting CartData@DeleteT");
            }
            catch(Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing RecordNotDeletedException");
                throw new RecordNotDeletedException("Item not removed from cart.", e.InnerException);
            }
        }

        /**
         * <exception cref="NotImplementedException"></exception>
         */
        public List<CartModel> ReadAllT()
        {
            this.logger.Warning("Entering CartData@ReadAllT NOT IMPLIMENTED");
            throw new NotImplementedException();
        }

        /**
         * <exception cref="NotImplementedException"></exception>
         */
        public List<CartModel> ReadBetweenT(int low, int high)
        {
            this.logger.Warning("Entering CartData@ReadBetweenT NOT IMPLIMENTED");
            throw new NotImplementedException();
        }
        
        /**
         * <summary>Get the cart based off a principals id</summary>
         */
        public List<WeaponModel> ReadManyTById(int id)
        {
            this.logger.Info("Entering CartData@ReadManyTById");
            try
            {
                List<WeaponModel> cart = new List<WeaponModel>();
                //Create the command
                this.logger.Info("Creating SqlCommand");
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Create the query statement
                    this.logger.Info("Generating sql script");
                    command.CommandText = @"SELECT w.[WEAPON_ID], [MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE] FROM [bbab].[dbo].[Cart] AS c
                                            INNER JOIN [bbab].[dbo].[Weapon] AS w ON c.[WEAPON_ID] = w.[WEAPON_ID]
                                            WHERE [PRINCIPAL_ID] = @pid";

                    //Bind the parameters
                    this.logger.Info("No Account Info Available", "Binding data to sql");
                    command.Parameters.Add("@pid", SqlDbType.Int).Value = id;

                    //prepare statement
                    this.logger.Info("Preparing command");
                    command.Prepare();

                    this.logger.Info("Executing command ExecuteReader");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        this.logger.Info("Checking if data exists");
                        if (reader.HasRows)
                        {
                            this.logger.Info("Reading in data");
                            while (reader.Read())
                            {
                                cart.Add(new WeaponModel(
                                    reader.GetInt32(0),
                                    reader.GetString(1),
                                    reader.GetString(2),
                                    reader.GetString(3),
                                    reader.GetString(4),
                                    reader.GetFloat(5)));
                            }
                        }
                        //close the reader
                        this.logger.Info("Closing the reader");
                        reader.Close();
                    }

                    //Return the cart
                    this.logger.Info("Returning the cart from CartData@ReadManyTById");
                    return cart;
                }
            }catch(Exception e)
            {
                this.logger.Error("Catching Exception", e);
                this.logger.Info("Throwing Exception RecordNotFoundException");
                throw new RecordNotFoundException("There was not any items in your cart.", e.InnerException);
            }
        }

        /**
         * <exception cref="NotImplementedException"></exception>
         */
        public CartModel ReadTByField(CartModel model)
        {
            this.logger.Warning("Entering CartData@ReadTByField NOT IMPLEMENTED");
            throw new NotImplementedException();
        }

        /**
         * <exception cref="NotImplementedException"></exception>
         */
        public CartModel ReadTById(int id)
        {
            this.logger.Warning("Entering CartData@ReadTById NOT IMPLEMENTED");
            throw new NotImplementedException();
        }

        /**
         * <exception cref="NotImplementedException"></exception>
         */
        public void UpdateT(CartModel model)
        {
            this.logger.Warning("Entering CartData@UpdateT NOT IMPLEMENTED");
            throw new NotImplementedException();
        }
    }
}