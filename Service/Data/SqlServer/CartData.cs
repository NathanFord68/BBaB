using BBaB.Models;
using BBaB.Service.CrudExceptions.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BBaB.Service.Data
{
    public class CartData : ICrud<CartModel>
    {
        private SqlConnection _connection;
        public CartData(SqlConnection connection)
        {
            this._connection = connection;
        }

        /**
         * <summary>Adds an item to the cart of the user.</summary>
         * <remarks>Not intended to generate an entire cart in one call.</remarks>
         */
        public void CreateT(CartModel model)
        {
            try
            {
                //Create the command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Create the query statement
                    command.CommandText = @"INSERT INTO [bbab].[dbo].[Cart] ([PRINCIPAL_ID], [WEAPON_ID]) VALUES (@pid, @wid)";

                    //Bind Parameters to statement
                    command.Parameters.Add("@pid", SqlDbType.Int).Value = model._customer._id;
                    command.Parameters.Add("@wid", SqlDbType.Int).Value = model._weaponToUpdate._id;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the statement
                    command.ExecuteNonQuery();
                }
            }catch(Exception e)
            {
                throw new RecordNotCreatedException("Item not added to cart.", e.InnerException);
            }
        }
        
        /**
         * <summary>Deletes an item from the cart with the item id.</summary>
         */
        public void DeleteT(CartModel model)
        {
            try
            {
                //Create the command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Create the query statement
                    command.CommandText = @"DELETE FROM [bbab].[dbo].[Cart] WHERE [WEAPON_ID] = @wid";

                    //Bind Parameters to statement
                    command.Parameters.Add("@wid", SqlDbType.Int).Value = model._weaponToUpdate._id;

                    //Prepare the statement
                    command.Prepare();

                    //Execute the statement
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception e)
            {
                throw new RecordNotCreatedException("Item not removed from cart.", e.InnerException);
            }
        }

        /**
         * <exception cref="NotImplementedException"></exception>
         */
        public List<CartModel> ReadAllT()
        {
            throw new NotImplementedException();
        }

        /**
         * <exception cref="NotImplementedException"></exception>
         */
        public List<CartModel> ReadBetweenT(int low, int high)
        {
            throw new NotImplementedException();
        }
        
        /**
         * <summary>Get the cart based off a principals id</summary>
         */
        public List<WeaponModel> ReadManyTById(int id)
        {
            try
            {
                List<WeaponModel> cart = new List<WeaponModel>();
                //Create the command
                using (SqlCommand command = _connection.CreateCommand())
                {
                    //Create the query statement
                    command.CommandText = @"SELECT w.[WEAPON_ID], [MAKE], [MODEL], [CALIBER], [SERIAL_NUMBER], [PRICE] FROM [bbab].[dbo].[Cart] AS c
                                            INNER JOIN [bbab].[dbo].[Weapon] AS w ON c.[WEAPON_ID] = w.[WEAPON_ID]
                                            WHERE [PRINCIPAL_ID] = @pid";

                    //Bind the parameters
                    command.Parameters.Add("@pid", SqlDbType.Int).Value = id;

                    //prepare statement
                    command.Prepare();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
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
                        reader.Close();
                    }

                    //Return the cart
                    return cart;
                }
            }catch(Exception e)
            {
                throw new RecordNotFoundException("There was not any items in your cart.", e.InnerException);
            }
        }

        /**
         * <exception cref="NotImplementedException"></exception>
         */
        public CartModel ReadTByField(CartModel model)
        {
            throw new NotImplementedException();
        }

        /**
         * <exception cref="NotImplementedException"></exception>
         */
        public CartModel ReadTById(int id)
        {
            throw new NotImplementedException();
        }

        /**
         * <exception cref="NotImplementedException"></exception>
         */
        public void UpdateT(CartModel model)
        {
            throw new NotImplementedException();
        }
    }
}