using BBaB.Models;
using BBaB.Service.Data;
using BBaB.Services.Business;
using BBaB.Services.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BBaB.Service.Business
{
    public class WeaponBusiness : IWeaponBusiness<WeaponModel>
    {

        private IDBConnect<SqlConnection> _connect;

        public WeaponBusiness(IDBConnect<SqlConnection> connect)
        {
            this._connect = connect;
        }

        /**
         * <see cref="IWeaponBusiness{W}"/>
         */
        public void AddWeaponInventory(WeaponModel model)
        {
            //Get a connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                //open connection
                connection.Open();

                //Get the data layer
                ICrud<WeaponModel> weaponData = new WeaponData(connection);

                //Pass the model to be added
                weaponData.CreateT(model);

                //close the connection
                connection.Close();
            }
        }

        /**
         * <see cref="IWeaponBusiness{W}"/>
         */
        public void DeleteWeaponFromInventory(WeaponModel model)
        {
            //Get a connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                //open connection
                connection.Open();

                //Get the data layer
                ICrud<WeaponModel> weaponData = new WeaponData(connection);

                //Pass the model to be deleted
                weaponData.DeleteT(model);

                //close the connection
                connection.Close();
            }
        }

        /**
         * <see cref="IWeaponBusiness{W}"/>
         */
        public void EditWeapon(WeaponModel model)
        {
            //Get a connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                //open connection
                connection.Open();

                //Get the data layer
                ICrud<WeaponModel> weaponData = new WeaponData(connection);

                //Pass the model to be updated
                weaponData.UpdateT(model);

                //close the connection
                connection.Close();
            }
        }

        /**
         * <see cref="IWeaponBusiness{W}"/>
         */
        public List<WeaponModel> GetAllWeapons()
        {
            //Create a default list
            List<WeaponModel> weapons;
            //Get a connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                //open connection
                connection.Open();

                //Get the data layer
                ICrud<WeaponModel> weaponData = new WeaponData(connection);

                //Populate the list
                weapons = weaponData.ReadAllT();

                //close the connection
                connection.Close();
            }

            //Return list
            return weapons;
        }

        /**
         * <inheritdoc/>
         */
        public void AddWeaponToCart(int weaponId, int principalId)
        {
            //Add the id's to be added to the cart
            CartModel cart = new CartModel();
            cart._customer._id = principalId;
            cart._weaponToUpdate._id = weaponId;

            //Get a connection to the database
            using (SqlConnection connection = this._connect.GetConnection())
            {
                //Open the connection
                connection.Open();

                //Get the data layer
                ICrud<CartModel> cartData = new CartData(connection);

                //Pass model to be added to the cart
                cartData.CreateT(cart);

                //Close the connection
                connection.Close();
            }
        }

        /**
         * <inheritdoc/>
         */
        public List<WeaponModel> GetCart(int principalId)
        {
            List<WeaponModel> cartInventory;

            using (SqlConnection connection = this._connect.GetConnection())
            {
                //Open the connection
                connection.Open();

                //Get the data layer
                ICrud<CartModel> cartData = new CartData(connection);

                //Get the inventory
                cartInventory = cartData.ReadTById(principalId)._weapons;

                //Close the connection
                connection.Close();
            }
            //Return the inventory
            return cartInventory;
        }
    }
}