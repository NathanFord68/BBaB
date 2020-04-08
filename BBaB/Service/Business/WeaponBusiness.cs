﻿using BBaB.Models;
using BBaB.Service.Data;
using BBaB.Services.Business;
using BBaB.Services.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BBaB.Utility.Interfaces;

namespace BBaB.Service.Business
{
    public class WeaponBusiness : IWeaponBusiness<WeaponModel>
    {

        private IDBConnect<SqlConnection> _connect;
        private IBBaBLogger logger;

        public WeaponBusiness(IDBConnect<SqlConnection> connect, IBBaBLogger logger)
        {
            this._connect = connect;
            this.logger = logger;
        }

        /**
         * <see cref="IWeaponBusiness{W}"/>
         */
        public void AddWeaponInventory(WeaponModel model)
        {
            this.logger.Info("No account info available", "Entering WeaponBusiness@AddWeaponInventory");

            //Get a connection
            this.logger.Info("No account info available", "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                //open connection
                this.logger.Info("No account info available", "Opening a connection with the database");
                connection.Open();

                //Get the data layer
                this.logger.Info("No account info available", "Getting data layer WeaponData");
                ICrud<WeaponModel> weaponData = new WeaponData(connection);

                //Pass the model to be added
                this.logger.Info("No account info available", "Passing model to be added to be added to inventory");
                weaponData.CreateT(model);

                //close the connection
                this.logger.Info("No account info available", "Closing the database connection");
                connection.Close();
            }
        }

        /**
         * <see cref="IWeaponBusiness{W}"/>
         */
        public void DeleteWeaponFromInventory(WeaponModel model)
        {
            this.logger.Info("No account info available", "Entering WeaponBusiness@DeleteWeaponFromInventory");

            //Get a connection
            this.logger.Info("No account info available", "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                //open connection
                this.logger.Info("No account info available", "Opening connection to the database");
                connection.Open();

                //Get the data layer
                this.logger.Info("No account info available", "Getting data layer WeaponData");
                ICrud<WeaponModel> weaponData = new WeaponData(connection);

                //Pass the model to be deleted
                this.logger.Info("No account info available", "Passing model to be deleted from inventory");
                weaponData.DeleteT(model);

                //close the connection
                this.logger.Info("No account info available", "Closing connection to the database");
                connection.Close();
            }

            this.logger.Info("No account info available", "Exiting WeaponBusiness@DeleteWeaponFromInventory");
        }

        /**
         * <see cref="IWeaponBusiness{W}"/>
         */
        public void EditWeapon(WeaponModel model)
        {
            this.logger.Info("No account info available", "Entering WeaponBusiness@EditWeapon");

            //Get a connection
            this.logger.Info("No account info available", "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                //open connection
                this.logger.Info("No account info available", "Opening a connection to the database");
                connection.Open();

                //Get the data layer
                this.logger.Info("No account info available", "Getting data layer WeaponData");
                ICrud<WeaponModel> weaponData = new WeaponData(connection);

                //Pass the model to be updated
                this.logger.Info("No account info available", "Passing model to be edited");
                weaponData.UpdateT(model);

                //close the connection
                this.logger.Info("No account info available", "Closing connection to the database");
                connection.Close();
            }
            this.logger.Info("No account info available", "Exiting WeaponBusiness@EditWeapon");
        }

        /**
         * <see cref="IWeaponBusiness{W}"/>
         */
        public List<WeaponModel> GetAllWeapons()
        {
            this.logger.Info("No account info available", "Entering WeaponBusiness@GetAllWeapons");

            //Create a default list
            List<WeaponModel> weapons;

            //Get a connection
            this.logger.Info("No account info available", "Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                //open connection
                this.logger.Info("No account info available", "Opening connection to the database");
                connection.Open();

                //Get the data layer
                this.logger.Info("No account info available", "Getting data layer WeaponData");
                ICrud<WeaponModel> weaponData = new WeaponData(connection);

                //Populate the list
                this.logger.Info("No account info available", "Retrieve the list of weapons");
                weapons = weaponData.ReadAllT();

                //close the connection
                this.logger.Info("No account info available", "Closing connection with the database");
                connection.Close();
            }

            //Return list
            this.logger.Info("No account info available", "Returning weapons list from WeaponBusiness@GetAllWeapons");
            return weapons;
        }

        /**
         * <inheritdoc/>
         */
        public void AddWeaponToCart(int weaponId, int principalId)
        {
            this.logger.Info("No account info available", "Entering WeaponBusiness@AddWeaponToCart");

            //Add the id's to be added to the cart
            CartModel cart = new CartModel();
            cart._customer._id = principalId;
            cart._weaponToUpdate._id = weaponId;

            //Get a connection to the database
            this.logger.Info("No account info available", "Getting connection to database");
            using (SqlConnection connection = this._connect.GetConnection())
            {
                //Open the connection
                this.logger.Info("No account info available", "Opening connection to database");
                connection.Open();

                //Get the data layer
                this.logger.Info("No account info available", "Getting data layer CartData");
                ICrud<CartModel> cartData = new CartData(connection);

                //Pass model to be added to the cart
                this.logger.Info("No account info available", "Passing cart to be created");
                cartData.CreateT(cart);

                //Close the connection
                this.logger.Info("No account info available", "Closing connection to database");
                connection.Close();
            }

            this.logger.Info("No account info available", "Exiting WeaponBusiness@AddWeaponToCart");
        }

        /**
         * <inheritdoc/>
         */
        public List<WeaponModel> GetCart(int principalId)
        {
            this.logger.Info("No account info available", "Entering WeaponBusiness@GetCart");
            List<WeaponModel> cartInventory;

            this.logger.Info("No account info available", "Getting connection to database");
            using (SqlConnection connection = this._connect.GetConnection())
            {
                //Open the connection
                this.logger.Info("No account info available", "Opening connection to database");
                connection.Open();

                //Get the data layer
                this.logger.Info("No account info available", "Getting data layer CartData");
                ICrud<CartModel> cartData = new CartData(connection);

                //Get the inventory
                this.logger.Info("No account info available", "Getting cart data from data layer");
                cartInventory = cartData.ReadTById(principalId)._weapons;

                //Close the connection
                this.logger.Info("No account info available", "Closing connection to database");
                connection.Close();
            }

            //Return the inventory
            this.logger.Info("No account info available", "Returning cart weapons list from WeaponBusiness@GetCart");
            return cartInventory;
        }
    }
}